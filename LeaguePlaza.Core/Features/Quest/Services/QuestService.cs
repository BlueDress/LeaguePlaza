using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace LeaguePlaza.Core.Features.Quest.Services
{
    public class QuestService(IRepository repository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) : IQuestService
    {
        private readonly IRepository _repository = repository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<AvailableQuestsViewModel> CreateAvailableQuestsViewModel()
        {
            var availableQuests = await _repository.FindAllReadOnlyAsync<QuestEntity>(q => q.Adventurer == null);

            return new AvailableQuestsViewModel()
            {
                AvailableQuests = availableQuests.Select(q => new QuestDto
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

        public async Task<UserQuestsViewModel> CreateUserQuestsViewModel()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            var userQuests = await _repository.FindAllReadOnlyAsync<QuestEntity>(q => q.CreatorId == currentUser?.Id || q.AdventurerId == currentUser?.Id);

            return new UserQuestsViewModel()
            {
                 UserQuests = userQuests.Select(q => new QuestDto
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
