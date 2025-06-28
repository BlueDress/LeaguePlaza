namespace LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = null!;

        public string ProductImageUrl { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
