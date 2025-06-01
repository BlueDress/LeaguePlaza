using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.Create
{
    public class CreateMountDto
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public decimal RentPrice { get; set; }

        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        [Required]
        public string MountType { get; set; } = null!;
    }
}
