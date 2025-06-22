namespace LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly
{
    public class CartItemDto
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public string ProductName { get; set; } = null!;

        public decimal ProductPrice { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
