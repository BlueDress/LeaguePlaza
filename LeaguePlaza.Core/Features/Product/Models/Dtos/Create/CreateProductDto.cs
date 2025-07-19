using LeaguePlaza.Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

using static LeaguePlaza.Common.Constants.ProductConstants;

namespace LeaguePlaza.Core.Features.Product.Models.Dtos.Create
{
    public class CreateProductDto
    {
        [Required]
        [MinLength(ProductNameMinLength)]
        [MaxLength(ProductNameMaxLength)]
        public string Name { get; set; } = null!;

        [MaxLength(ProductDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [DecimalModelBinder]
        public decimal Price { get; set; }

        [MaxFileSize(ProductFileMaxSize)]
        [ValidateImageFileSignature]
        public IFormFile? Image { get; set; }

        [Required]
        public string ProductType { get; set; } = null!;
    }
}
