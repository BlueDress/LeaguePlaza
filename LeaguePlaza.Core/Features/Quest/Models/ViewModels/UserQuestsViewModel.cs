using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Quest.Models.ViewModels
{
    public class UserQuestsViewModel
    {
        public QuestCardsContainerWithPaginationViewModel ViewModel { get; set; } = new QuestCardsContainerWithPaginationViewModel();
    }
}
