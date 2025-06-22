using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Home.Models.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<QuestDto> LatestQuests { get; set; } = new HashSet<QuestDto>();
    }
}
