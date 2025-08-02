using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Quest.Contracts
{
    public interface IQuestService
    {
        Task<QuestsViewModel> CreateAvailableQuestsViewModelAsync();

        Task<QuestsViewModel> CreateUserQuestsViewModelAsync(string currentUserId);

        Task<ViewQuestViewModel> CreateViewQuestViewModelAsync(int id, string currentUserId);

        Task CreateQuestAsync(CreateQuestDto createQuestDto, string currentUserId);

        Task UpdateQuestAsync(UpdateQuestDataDto updateQuestDto);

        Task AcceptQuestAsync(int id, string currentUserId);

        Task RemoveQuestAsync(int id);

        Task CompleteQuestAsync(int id);

        Task AbandonQuestAsync(int id);

        Task<QuestsViewModel> CreateQuestCardsContainerWithPaginationViewModelAsync(FilterAndSortQuestsRequestData filterAndSortQuestsRequestData, string? currentUserId);
    }
}
