using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class OrderItemEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal Price { get; set; }

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public ProductEntity Product { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        public OrderEntity Order { get; set; } = null!;
    }
}
