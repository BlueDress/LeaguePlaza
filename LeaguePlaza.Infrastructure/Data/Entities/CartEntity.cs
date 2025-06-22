using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class CartEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserID { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        public ICollection<CartItemEntity> CartItems { get; set; } = new HashSet<CartItemEntity>();
    }
}
