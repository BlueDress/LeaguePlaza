using LeaguePlaza.Core.Features.Order.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Order.Contracts
{
    public interface IOrderService
    {
        Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync();

        Task<CartViewModel> CreateViewCartViewModelAsync();
    }
}
