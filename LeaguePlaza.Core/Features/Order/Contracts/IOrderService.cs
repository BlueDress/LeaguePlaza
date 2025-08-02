using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;

using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Order.Contracts
{
    public interface IOrderService
    {
        Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync(string currentUserId, int pageNumber = PageOne);

        Task<CartViewModel> CreateViewCartViewModelAsync(string currentUserId, OrderInformationDto? orderInformationDto = null);

        Task<int> GetCartItemsCountAsync(string currentUserId);

        Task<OrderViewModel> CreateOrderViewModelAsync(int orderId, string currentUserId);

        Task<AddToCartResultDto> AddToCartAsync(CreateCartItemDto createCartItemDto, string currentUserId);

        Task<bool> CreateOrderAsync(OrderInformationDto orderInformationDto, string currentUserId);

        Task RemoveCartItemAsync(int cartItemId);

        Task UpdateOrderStatusAsync(UpdateOrderStatusDto updateOrderStatusDto);
    }
}
