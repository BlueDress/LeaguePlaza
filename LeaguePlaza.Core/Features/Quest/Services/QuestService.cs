using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

namespace LeaguePlaza.Core.Features.Quest.Services
{
    public class QuestService(IRepository repository) : IQuestService
    {
        private readonly IRepository _repository = repository;

        public async Task<AllAvailableQuestsViewModel> CreateAllAvailableQuestsViewModel()
        {
            var allAvailableQuests = await _repository.FindAllReadOnlyAsync<QuestEntity>(q => q.Adventurer == null);

            return new AllAvailableQuestsViewModel()
            {
                AllAvailableQuests = allAvailableQuests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    Status = q.Status.ToString(),
                    CreatorId = q.CreatorId,
                    AdventurerId = q.AdventurerId,
                })
            };
        }
    }
}
