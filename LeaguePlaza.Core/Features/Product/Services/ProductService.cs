using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Core.Features.Product.Contracts;
using LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Product.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

namespace LeaguePlaza.Core.Features.Product.Services
{
    public class ProductService(IRepository repository) : IProductService
    {
        private readonly IRepository _repository = repository;

        public async Task<ProductsViewModel> CreateAvailableProductsViewModelAsync()
        {
            IEnumerable<ProductEntity> availableProducts = await _repository.FindSpecificCountOrderedReadOnlyAsync<ProductEntity, string>(ProductConstants.PageOne, ProductConstants.CountForPagination, false, p => p.Name, p => p.IsInStock);
            int totalResults = await _repository.GetCountAsync<ProductEntity>(p => p.IsInStock);

            return new ProductsViewModel()
            {
                Products = availableProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(p.Description) ? ProductConstants.NoDescriptionAvailable : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsInStock = p.IsInStock,
                    ProductType = p.ProductType.ToString(),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = ProductConstants.PageOne,
                    TotalPages = (int)Math.Ceiling(totalResults / 6d),
                },
            };
        }

        public async Task<ViewProductViewModel> CreateViewProductViewModelAsync(int id)
        {
            var product = await _repository.FindByIdAsync<ProductEntity>(id) ?? new();
            IEnumerable<ProductEntity> recommendedProducts = await _repository.FindSpecificCountReadOnlyAsync<ProductEntity>(ProductConstants.RecommendedProductsCount, p => p.Id != id && p.ProductType == product.ProductType);

            return new ViewProductViewModel()
            {
                Product = new ProductDto()
                {
                    Id = id,
                    Name = product.Name,
                    Description = string.IsNullOrWhiteSpace(product.Description) ? ProductConstants.NoDescriptionAvailable : product.Description,
                    Price = product.Price,
                    ImageUrl = product.ImageUrl,
                    IsInStock= product.IsInStock,
                    ProductType = product.ProductType.ToString(),
                },
                RecommendedProducts = recommendedProducts.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(p.Description) ? ProductConstants.NoDescriptionAvailable : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsInStock = p.IsInStock,
                    ProductType = p.ProductType.ToString(),
                }),
            };
        }
    }
}
