using LeaguePlaza.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class ProductEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = null!;

        public bool IsInStock { get; set; }

        [Required]
        public ProductType ProductType { get; set; }

        public ICollection<CartItemEntity> CartItems { get; set; } = new HashSet<CartItemEntity>();

        public ICollection<OrderItemEntity> OrderItems { get; set; } = new HashSet<OrderItemEntity>();
    }
}
