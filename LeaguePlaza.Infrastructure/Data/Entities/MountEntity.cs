using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using LeaguePlaza.Infrastructure.Data.Enums;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class MountEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal RentPrice { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        public MountType MountType { get; set; }

        [Column(TypeName = "float")]
        public double Rating { get; set; }

        public ICollection<MountRentalEntity> MountRentals { get; set; } = new HashSet<MountRentalEntity>();

        public ICollection<MountRatingEntity> MountRatings { get; set;} = new HashSet<MountRatingEntity>();
    }
}
