using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class MountRatingEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Rating { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        public ApplicationUser User { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Mount))]
        public int MountId { get; set; }

        public MountEntity Mount { get; set; } = null!;
    }
}
