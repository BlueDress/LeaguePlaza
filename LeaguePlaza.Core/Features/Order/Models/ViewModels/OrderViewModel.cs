using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Order.Models.ViewModels
{
    public class OrderViewModel
    {
        public OrderDto Order { get; set; } = null!;

        public IEnumerable<OrderItemDto> OrderItems { get; set; } = new HashSet<OrderItemDto>();
    }
}
