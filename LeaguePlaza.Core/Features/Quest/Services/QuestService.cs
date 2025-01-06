using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
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

        public async Task<AvailableQuestsViewModel> CreateAvailableQuestsViewModelAsync()
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

        public async Task<UserQuestsViewModel> CreateUserQuestsViewModelAsync()
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

        public async Task<ViewQuestViewModel> CreateViewQuestViewModelAsync(int id)
        {
            QuestEntity quest = await _repository.FindByIdAsync<QuestEntity>(id);

            return new ViewQuestViewModel()
            {
                QuestDto = new QuestDto()
                {
                    Id = id,
                    Title = quest.Title,
                    Description = quest.Description,
                    Created = quest.Created,
                    RewardAmount = quest.RewardAmount,
                    Type = quest.Type.ToString(),
                    Status = quest.Status.ToString(),
                    CreatorId = quest.CreatorId,
                    AdventurerId = quest.AdventurerId,
                },
            };
        }

        public async Task<QuestDto> CreateQuestAsync(CreateQuestDto createQuestDto)
        {
            ApplicationUser currentUser = (await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!))!;

            var newQuest = new QuestEntity()
            {
                Title = createQuestDto.Title,
                Description = createQuestDto.Description,
                Created = DateTime.Now,
                RewardAmount = createQuestDto.RewardAmount,
                Type = (QuestType)Enum.Parse(typeof(QuestType), createQuestDto.Type),
                Status = QuestStatus.Posted,
                Creator = currentUser,
            };

            await _repository.AddAsync(newQuest);
            await _repository.SaveChangesAsync();

            return new QuestDto
            {
                Id = newQuest.Id,
                Title = newQuest.Title,
                Description = newQuest.Description,
                Created = DateTime.Now,
                RewardAmount = newQuest.RewardAmount,
                Type = newQuest.Type.ToString(),
                Status = newQuest.Status.ToString(),
                CreatorId = currentUser.Id,
            };
        }
    }
}
