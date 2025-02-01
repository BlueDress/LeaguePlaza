using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Quest.Models.ViewModels
{
    public class ViewQuestViewModel
    {
        public QuestDto Quest { get; set; } = new QuestDto();

        public IEnumerable<QuestDto> RecommendedQuests { get; set; } = new List<QuestDto>();

        public string CurrentUserId { get; set; } = null!;
    }
}
