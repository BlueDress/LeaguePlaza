using LeaguePlaza.Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

using static LeaguePlaza.Common.Constants.MountConstants;

namespace LeaguePlaza.Core.Features.Mount.Models.Dtos.Create
{
    public class CreateMountDto
    {
        [Required]
        [MinLength(MountNameMinLength)]
        [MaxLength(MountNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(MountDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [DecimalModelBinder]
        public decimal RentPrice { get; set; }

        [MaxFileSize(MountFileMaxSize)]
        [ValidateImageFileSignature]
        public IFormFile? Image { get; set; }

        [Required]
        public string MountType { get; set; } = null!;
    }
}
