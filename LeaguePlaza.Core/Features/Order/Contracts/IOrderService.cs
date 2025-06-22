using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Core.Features.Order.Contracts
{
    public interface IOrderService
    {
        Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync();

        Task<CartViewModel> CreateViewCartViewModelAsync();

        Task<int> GetCartItemsCountAsync();
    }
}
