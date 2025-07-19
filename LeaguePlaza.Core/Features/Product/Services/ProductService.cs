using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Core.Features.Product.Contracts;
using LeaguePlaza.Core.Features.Product.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Product.Models.RequestData;
using LeaguePlaza.Core.Features.Product.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using System.Linq.Expressions;

using static LeaguePlaza.Common.Constants.PaginationConstants;
using static LeaguePlaza.Common.Constants.ProductConstants;

namespace LeaguePlaza.Core.Features.Product.Services
{
    public class ProductService(IRepository repository, IDropboxService dropboxService) : IProductService
    {
        private readonly Dictionary<string, string> DefaultProductTypeImageUrls = new()
        {
            { "0", HealingDefaultImageUrl },
            { "1", EnhancementDefaultImageUrl },
            { "2", ImpairmentDefaultImageUrl },
        };

        private readonly IRepository _repository = repository;
        private readonly IDropboxService _dropboxService = dropboxService;

        public async Task<ProductsViewModel> CreateAvailableProductsViewModelAsync()
        {
            IEnumerable<ProductEntity> availableProducts = await _repository.FindSpecificCountOrderedReadOnlyAsync<ProductEntity, string>(PageOne, ProductsPerPage, false, p => p.Name, p => p.IsInStock);
            int totalResults = await _repository.GetCountAsync<ProductEntity>(p => p.IsInStock);

            return new ProductsViewModel()
            {
                Products = availableProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(p.Description) ? NoProductDescriptionAvailable : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsInStock = p.IsInStock,
                    ProductType = p.ProductType.ToString(),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = PageOne,
                    TotalPages = (int)Math.Ceiling((double)totalResults / ProductsPerPage),
                },
            };
        }

        public async Task<ViewProductViewModel> CreateViewProductViewModelAsync(int id)
        {
            var product = await _repository.FindByIdAsync<ProductEntity>(id) ?? new();
            IEnumerable<ProductEntity> recommendedProducts = await _repository.FindSpecificCountReadOnlyAsync<ProductEntity>(RecommendedProductsCount, p => p.Id != id && p.ProductType == product.ProductType);

            return new ViewProductViewModel()
            {
                Product = new ProductDto()
                {
                    Id = id,
                    Name = product.Name,
                    Description = string.IsNullOrWhiteSpace(product.Description) ? NoProductDescriptionAvailable : product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    IsInStock = product.IsInStock,
                    ProductType = product.ProductType.ToString(),
                },
                RecommendedProducts = recommendedProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(p.Description) ? NoProductDescriptionAvailable : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsInStock = p.IsInStock,
                    ProductType = p.ProductType.ToString(),
                }),
            };
        }

        public async Task<ProductsViewModel> CreateProductCardsContainerWithPaginationViewModelAsync(FilterAndSortProductsRequestData filterAndSortProductsRequestData)
        {
            Expression<Func<ProductEntity, bool>> searchExpression = string.IsNullOrWhiteSpace(filterAndSortProductsRequestData.SearchTerm)
                ? p => true
                : p => p.Name.Contains(filterAndSortProductsRequestData.SearchTerm) || (p.Description != null && p.Description.Contains(filterAndSortProductsRequestData.SearchTerm));

            string[] typeFilters = filterAndSortProductsRequestData.TypeFilters?.Split(',') ?? [];

            Expression<Func<ProductEntity, bool>> typeFiltersExpression = typeFilters.Length != 0
                ? p => typeFilters.Select(f => (ProductType)Enum.Parse(typeof(ProductType), f)).Contains(p.ProductType)
                : p => true;

            ParameterExpression parameter = Expression.Parameter(typeof(ProductEntity), "p");
            Expression<Func<ProductEntity, bool>> combinedFilterExpression = Expression.Lambda<Func<ProductEntity, bool>>(
                Expression.AndAlso(
                        Expression.Invoke(searchExpression, parameter),
                        Expression.Invoke(typeFiltersExpression, parameter)),
                parameter);

            int totalFilteredAndSortedProductsCount = await _repository.GetCountAsync(combinedFilterExpression);

            if (totalFilteredAndSortedProductsCount == 0)
            {
                return new ProductsViewModel();
            }

            int pageToShow = Math.Min((int)Math.Ceiling((double)totalFilteredAndSortedProductsCount / ProductsPerPage), filterAndSortProductsRequestData.CurrentPage);

            Expression<Func<ProductEntity, object>> sortExpression = filterAndSortProductsRequestData.SortBy == "Price" ? p => p.Price : p => p.Name;

            IEnumerable<ProductEntity> filteredAndSortedProducts = await _repository.FindSpecificCountOrderedReadOnlyAsync(pageToShow, ProductsPerPage, filterAndSortProductsRequestData.OrderIsDescending, sortExpression, combinedFilterExpression);

            return new ProductsViewModel()
            {
                Products = filteredAndSortedProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(p.Description) ? NoProductDescriptionAvailable : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsInStock = p.IsInStock,
                    ProductType = p.ProductType.ToString(),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = pageToShow,
                    TotalPages = (int)Math.Ceiling((double)totalFilteredAndSortedProductsCount / ProductsPerPage),
                },
            };
        }

        public async Task CreateProductAsync(CreateProductDto createProductDto)
        {
            var dateCreated = DateTime.Now;

            string imageUrl = string.Empty;

            if (createProductDto.Image != null)
            {
                string accessToken = await _dropboxService.GetAccessToken();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    string uploadPath = string.Format(ImageUploadPath, createProductDto.Name, dateCreated.ToLongTimeString(), createProductDto.Image.FileName);
                    imageUrl = await _dropboxService.UploadImage(createProductDto.Image, uploadPath, accessToken);
                }
            }

            imageUrl = string.IsNullOrEmpty(imageUrl) ? DefaultProductTypeImageUrls[createProductDto.ProductType] : imageUrl;

            var newProduct = new ProductEntity()
            {
                Name = createProductDto.Name,
                Description = createProductDto.Description,
                Price = createProductDto.Price,
                ProductType = (ProductType)Enum.Parse(typeof(ProductType), createProductDto.ProductType),
                ImageUrl = imageUrl,
                IsInStock = true,
            };

            await _repository.AddAsync(newProduct);
            await _repository.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(UpdateProductDto updateProductDto)
        {
            var productToUpdate = await _repository.FindByIdAsync<ProductEntity>(updateProductDto.Id);

            if (productToUpdate != null)
            {
                productToUpdate.Name = updateProductDto.Name;
                productToUpdate.Description = updateProductDto.Description;
                productToUpdate.Price = updateProductDto.Price;
                productToUpdate.ProductType = (ProductType)Enum.Parse(typeof(ProductType), updateProductDto.ProductType);

                if (updateProductDto.Image != null)
                {
                    string accessToken = await _dropboxService.GetAccessToken();

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        string uploadPath = string.Format(ImageUploadPath, updateProductDto.Name, DateTime.Now.ToLongTimeString(), updateProductDto.Image.FileName);
                        string imageUrl = await _dropboxService.UploadImage(updateProductDto.Image, uploadPath, accessToken);

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            productToUpdate.ImageUrl = imageUrl;
                        }
                    }
                }

                _repository.Update(productToUpdate);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var productToRemove = await _repository.FindByIdAsync<ProductEntity>(id);

            if (productToRemove != null)
            {
                _repository.Remove(productToRemove);
                await _repository.SaveChangesAsync();
            }
        }
    }
}
