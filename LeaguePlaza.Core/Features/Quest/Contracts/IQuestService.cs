using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Quest.Contracts
{
    public interface IQuestService
    {
        Task<AvailableQuestsViewModel> CreateAvailableQuestsViewModelAsync();

        Task<UserQuestsViewModel> CreateUserQuestsViewModelAsync();

        Task<ViewQuestViewModel> CreateViewQuestViewModelAsync(int id);

        Task<QuestDto> CreateQuestAsync(CreateQuestDto createQuestDto);

        Task<QuestDto> UpdateQuestAsync(UpdateQuestDataDto updateQuestDto);

        Task AcceptQuest(int id);

        Task RemoveQuestAsync(int id);

        Task CompleteQuestAsync(int id);

        Task AbandonQuestAsync(int id);
    }
}
