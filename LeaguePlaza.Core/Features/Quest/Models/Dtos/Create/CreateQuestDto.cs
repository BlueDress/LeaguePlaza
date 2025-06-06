using LeaguePlaza.Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LeaguePlaza.Core.Features.Quest.Models.Dtos.Create
{
    public class CreateQuestDto
    {
        [Required]
        [MinLength(1)]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [DecimalModelBinder]
        public decimal RewardAmount { get; set; }

        [Required]
        public string Type { get; set; } = string.Empty;

        [MaxFileSize(5 * 1024 * 1024)]
        [ValidateImageFileSignature]
        public IFormFile? Image { get; set; }
    }
}
