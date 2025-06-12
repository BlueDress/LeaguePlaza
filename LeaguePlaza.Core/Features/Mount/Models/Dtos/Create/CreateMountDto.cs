using LeaguePlaza.Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.Create
{
    public class CreateMountDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [DecimalModelBinder]
        public decimal RentPrice { get; set; }

        [MaxFileSize(5 * 1024 * 1024)]
        [ValidateImageFileSignature]
        public IFormFile? Image { get; set; }

        [Required]
        public string MountType { get; set; } = null!;
    }
}
