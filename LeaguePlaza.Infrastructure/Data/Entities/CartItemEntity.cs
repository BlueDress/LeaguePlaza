using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class CartItemEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public ProductEntity Product { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }

        public CartEntity Cart { get; set; } = null!;
    }
}
