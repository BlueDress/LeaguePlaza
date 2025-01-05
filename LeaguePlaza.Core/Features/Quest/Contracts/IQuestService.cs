using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Quest.Contracts
{
    public interface IQuestService
    {
        Task<AvailableQuestsViewModel> CreateAvailableQuestsViewModel();

        Task<UserQuestsViewModel> CreateUserQuestsViewModel();
    }
}
