using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Quest.Models.ViewModels
{
    public class AvailableQuestsViewModel
    {
        public IEnumerable<QuestDto> AvailableQuests { get; set; } = new List<QuestDto>();
    }
}
