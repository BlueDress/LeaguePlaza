using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Quest.Models.ViewModels
{
    public class QuestsViewModel
    {
        public IEnumerable<QuestDto> Quests { get; set; } = new List<QuestDto>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
