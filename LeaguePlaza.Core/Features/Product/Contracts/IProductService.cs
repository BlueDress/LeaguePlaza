using LeaguePlaza.Core.Features.Product.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Product.Models.RequestData;
using LeaguePlaza.Core.Features.Product.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Product.Contracts
{
    public interface IProductService
    {
        Task<ProductsViewModel> CreateAvailableProductsViewModelAsync();

        Task<ViewProductViewModel> CreateViewProductViewModelAsync(int id);

        Task<ProductsViewModel> CreateProductCardsContainerWithPaginationViewModelAsync(FilterAndSortProductsRequestData filterAndSortProductsRequestData);

        Task CreateProductAsync(CreateProductDto createProductDto);

        Task UpdateProductAsync(UpdateProductDto updateProductDto);

        Task DeleteProductAsync(int id);
    }
}
