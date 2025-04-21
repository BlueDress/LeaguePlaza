using LeaguePlaza.Infrastructure.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LeaguePlaza.Infrastructure.Data.Entities
{
    public class QuestEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public DateTime Created { get; set; }

        [Required]
        [Column(TypeName = "decimal(12,2)")]
        public decimal RewardAmount { get; set; }

        [Required]
        public QuestType Type { get; set; }

        [Required]
        public QuestStatus Status { get; set; }

        [Required]
        [MaxLength(200)]
        public string ImageName { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(Creator))]
        public string CreatorId { get; set; } = null!;

        public ApplicationUser Creator { get; set; } = null!;

        [ForeignKey(nameof(Adventurer))]
        public string? AdventurerId { get; set; }

        public ApplicationUser? Adventurer { get; set; }
    }
}
