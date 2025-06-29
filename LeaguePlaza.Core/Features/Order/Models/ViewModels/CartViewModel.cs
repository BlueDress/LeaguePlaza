using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Order.Models.ViewModels
{
    public class CartViewModel
    {
        public int CartId { get; set; }

        public IEnumerable<CartItemDto> CartItems { get; set; } = new HashSet<CartItemDto>();
    }
}
