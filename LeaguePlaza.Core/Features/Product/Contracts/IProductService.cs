using LeaguePlaza.Core.Features.Product.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Product.Contracts
{
    public interface IProductService
    {
        Task<ProductsViewModel> CreateAvailableProductsViewModelAsync();
    }
}
