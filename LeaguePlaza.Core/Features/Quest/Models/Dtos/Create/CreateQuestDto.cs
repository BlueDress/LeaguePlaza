using LeaguePlaza.Infrastructure.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

using static LeaguePlaza.Common.Constants.QuestConstants;

namespace LeaguePlaza.Core.Features.Quest.Models.Dtos.Create
{
    public class CreateQuestDto
    {
        [Required]
        [MinLength(QuestTitleMinLength)]
        [MaxLength(QuestTitleMaxLength)]
        public string Title { get; set; } = null!;

        [MaxLength(QuestDescriptionMaxLength)]
        public string? Description { get; set; }

        [Required]
        [DecimalModelBinder]
        public decimal RewardAmount { get; set; }

        [Required]
        public string Type { get; set; } = null!;

        [MaxFileSize(QuestFileMaxSize)]
        [ValidateImageFileSignature]
        public IFormFile? Image { get; set; }
    }
}
