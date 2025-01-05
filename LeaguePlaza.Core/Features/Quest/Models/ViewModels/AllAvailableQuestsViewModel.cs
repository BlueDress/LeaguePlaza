using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Quest.Models.ViewModels
{
    public class AllAvailableQuestsViewModel
    {
        public IEnumerable<QuestDto> AllAvailableQuests { get; set; } = new List<QuestDto>();
    }
}
