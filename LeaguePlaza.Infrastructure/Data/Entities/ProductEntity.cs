using LeaguePlaza.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using static LeaguePlaza.Common.Constants.ProductConstants;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(ProductDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(ProductImageUrlMaxLength)]
        public string ImageUrl { get; set; } = null!;

        public bool IsInStock { get; set; }

        [Required]
        public ProductType ProductType { get; set; }

        public ICollection<CartItemEntity> CartItems { get; set; } = new HashSet<CartItemEntity>();

        public ICollection<OrderItemEntity> OrderItems { get; set; } = new HashSet<OrderItemEntity>();
    }
}
