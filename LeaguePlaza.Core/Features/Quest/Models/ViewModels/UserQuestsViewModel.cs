using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Quest.Models.ViewModels
{
    public class UserQuestsViewModel
    {
        public IEnumerable<QuestDto> UserQuests { get; set; } = new List<QuestDto>();
    }
}
