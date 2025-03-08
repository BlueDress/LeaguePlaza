using LeaguePlaza.Core.Features.Pagination.Models;
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
            var availableQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync<QuestEntity, DateTime>(1, 6, true, q => q.Created, q => q.Status == QuestStatus.Posted);
            var totalResults = await _repository.GetCountAsync<QuestEntity>(q => true);

            var QuestCardsContainerWithPaginationViewModel = new QuestCardsContainerWithPaginationViewModel()
            {
                Quests = availableQuests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? "No description available" : q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    Status = q.Status.ToString(),
                    CreatorId = q.CreatorId,
                    AdventurerId = q.AdventurerId,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = 1,
                    TotalPages = (int)Math.Ceiling(totalResults / 6d),
                },
            };

            return new AvailableQuestsViewModel()
            {
                ViewModel = QuestCardsContainerWithPaginationViewModel,
            };
        }

        public async Task<UserQuestsViewModel> CreateUserQuestsViewModelAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);
            var userQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync<QuestEntity, DateTime>(1, 6, true, q => q.Created, q => q.CreatorId == currentUser.Id || q.AdventurerId == currentUser.Id);
            var totalResults = await _repository.GetCountAsync<QuestEntity>(q => q.CreatorId == currentUser.Id || q.AdventurerId == currentUser.Id);

            var QuestCardsContainerWithPaginationViewModel = new QuestCardsContainerWithPaginationViewModel()
            {
                Quests = userQuests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? "No description available" : q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    Status = q.Status.ToString(),
                    CreatorId = q.CreatorId,
                    AdventurerId = q.AdventurerId,
                    ShowExtraButtons = true,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = 1,
                    TotalPages = (int)Math.Ceiling(totalResults / 6d),
                },
            };

            return new UserQuestsViewModel()
            {
                ViewModel = QuestCardsContainerWithPaginationViewModel,
            };
        }

        public async Task<ViewQuestViewModel> CreateViewQuestViewModelAsync(int id)
        {
            QuestEntity quest = await _repository.FindByIdAsync<QuestEntity>(id);
            IEnumerable<QuestEntity> recommendedQuests = await _repository.FindAllReadOnlyAsync<QuestEntity>(q => q.Id != id && q.Type == quest.Type && q.Status == QuestStatus.Posted);

            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            return new ViewQuestViewModel()
            {
                Quest = new QuestDto()
                {
                    Id = id,
                    Title = quest.Title,
                    Description = string.IsNullOrWhiteSpace(quest.Description) ? "No description available" : quest.Description,
                    Created = quest.Created,
                    RewardAmount = quest.RewardAmount,
                    Type = quest.Type.ToString(),
                    Status = quest.Status.ToString(),
                    CreatorId = quest.CreatorId,
                    AdventurerId = quest.AdventurerId,
                },
                RecommendedQuests = recommendedQuests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? "No description available" : q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    Status = q.Status.ToString(),
                    CreatorId = q.CreatorId,
                    AdventurerId = q.AdventurerId,
                }),
                CurrentUserId = currentUser.Id,
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

        public async Task<QuestDto> UpdateQuestAsync(UpdateQuestDataDto updateQuestDto)
        {
            var questToUpdate = await _repository.FindByIdAsync<QuestEntity>(updateQuestDto.Id);

            questToUpdate.Title = updateQuestDto.Title;
            questToUpdate.Description = updateQuestDto.Description;
            questToUpdate.RewardAmount = updateQuestDto.RewardAmount;
            questToUpdate.Type = (QuestType)Enum.Parse(typeof(QuestType), updateQuestDto.Type);

            _repository.Update(questToUpdate);
            await _repository.SaveChangesAsync();

            return new QuestDto
            {
                Id = questToUpdate.Id,
                Title = questToUpdate.Title,
                Description = questToUpdate.Description,
                Created = questToUpdate.Created,
                RewardAmount = questToUpdate.RewardAmount,
                Type = questToUpdate.Type.ToString(),
                Status = questToUpdate.Status.ToString(),
                CreatorId = questToUpdate.CreatorId,
            };
        }

        public async Task AcceptQuest(int id)
        {
            ApplicationUser currentUser = (await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!))!;

            var questToAccept = await _repository.FindByIdAsync<QuestEntity>(id);

            questToAccept.Status = QuestStatus.Accepted;
            questToAccept.AdventurerId = currentUser.Id;

            _repository.Update(questToAccept);
            await _repository.SaveChangesAsync();
        }

        public async Task RemoveQuestAsync(int id)
        {
            var questToRemove = await _repository.FindByIdAsync<QuestEntity>(id);

            _repository.Remove(questToRemove);
            await _repository.SaveChangesAsync();
        }

        public async Task CompleteQuestAsync(int id)
        {
            var questToComplete = await _repository.FindByIdAsync<QuestEntity>(id);

            questToComplete.Status = QuestStatus.Completed;

            _repository.Update(questToComplete);
            await _repository.SaveChangesAsync();
        }

        public async Task AbandonQuestAsync(int id)
        {
            var questToAbandon = await _repository.FindByIdAsync<QuestEntity>(id);

            questToAbandon.Status = QuestStatus.Posted;
            questToAbandon.AdventurerId = null;

            _repository.Update(questToAbandon);
            await _repository.SaveChangesAsync();
        }
    }
}
