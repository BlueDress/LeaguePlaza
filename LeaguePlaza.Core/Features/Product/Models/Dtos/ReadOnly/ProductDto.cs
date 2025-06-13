namespace LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly
{
    public class ProductDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public string ImageUrl { get; set; } = null!;

        public bool IsInStock { get; set; }

        public string ProductType { get; set; } = null!;
    }
}
