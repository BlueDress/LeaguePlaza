using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Order.Contracts
{
    public interface IOrderService
    {
        Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync(int pageNumber = OrderConstants.PageOne);

        Task<CartViewModel> CreateViewCartViewModelAsync();

        Task<int> GetCartItemsCountAsync();

        Task<OrderViewModel> CreateOrderViewModelAsync(int orderId);

        Task<AddToCartResultDto> AddToCartAsync(CreateCartItemDto createCartItemDto);
    }
}
