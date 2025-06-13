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
            IEnumerable<ProductEntity> availableProducts = await _repository.FindSpecificCountOrderedReadOnlyAsync<ProductEntity, string>(1, 6, false, p => p.Name, p => p.IsInStock);
            int totalResults = await _repository.GetCountAsync<ProductEntity>(p => p.IsInStock);

            return new ProductsViewModel()
            {
                Products = availableProducts.Select(p => new ProductDto
                {
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? "No description available" : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    IsInStock = p.IsInStock,
                    ProductType = p.ProductType.ToString(),
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = 1,
                    TotalPages = (int)Math.Ceiling(totalResults / 6d),
                },
            };
        }
    }
}
