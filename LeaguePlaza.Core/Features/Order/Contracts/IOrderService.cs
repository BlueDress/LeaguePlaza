using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;

using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Order.Contracts
{
    public interface IOrderService
    {
        Task<OrderHistoryViewModel> CreateOrderHistoryViewModelAsync(int pageNumber = PageOne);

        Task<CartViewModel> CreateViewCartViewModelAsync();

        Task<int> GetCartItemsCountAsync();

        Task<OrderViewModel> CreateOrderViewModelAsync(int orderId);

        Task<AddToCartResultDto> AddToCartAsync(CreateCartItemDto createCartItemDto);
    }
}
