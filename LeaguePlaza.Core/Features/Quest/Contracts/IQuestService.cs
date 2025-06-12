using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Quest.Contracts
{
    public interface IQuestService
    {
        Task<QuestsViewModel> CreateAvailableQuestsViewModelAsync();

        Task<QuestsViewModel> CreateUserQuestsViewModelAsync();

        Task<ViewQuestViewModel> CreateViewQuestViewModelAsync(int id);

        Task CreateQuestAsync(CreateQuestDto createQuestDto);

        Task UpdateQuestAsync(UpdateQuestDataDto updateQuestDto);

        Task AcceptQuestAsync(int id);

        Task RemoveQuestAsync(int id);

        Task CompleteQuestAsync(int id);

        Task AbandonQuestAsync(int id);

        Task<QuestsViewModel> CreateQuestCardsContainerWithPaginationViewModelAsync(FilterAndSortQuestsRequestData filterAndSortQuestsRequestData);
    }
}
