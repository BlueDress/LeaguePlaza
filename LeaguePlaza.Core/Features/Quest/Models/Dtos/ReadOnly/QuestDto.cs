namespace LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly
{
    public class QuestDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime Created { get; set; }

        public decimal RewardAmount { get; set; }

        public string Type { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string CreatorId { get; set; } = null!;

        public string? AdventurerId { get; set; }

        public bool ShowExtraButtons { get; set; }

        public string ImageUrl { get; set; } = null!;
    }
}
