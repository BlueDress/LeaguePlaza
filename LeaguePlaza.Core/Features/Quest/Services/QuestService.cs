using Dropbox.Api;
using Dropbox.Api.Files;

using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace LeaguePlaza.Core.Features.Quest.Services
{
    public class QuestService(IRepository repository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager) : IQuestService
    {
        // TODO: Add constants
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
            var userQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync<QuestEntity, object>(1, 6, true, q => q.Created, q => q.CreatorId == currentUser.Id || q.AdventurerId == currentUser.Id);
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

            string imageUrl = string.Empty;

            if (createQuestDto.Image != null)
            {
                using var dbx = new DropboxClient("");
                using var memoryStream = new MemoryStream();

                await createQuestDto.Image.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var uploadResponse = await dbx.Files.UploadAsync(path: "/quests/" + createQuestDto.Image.FileName, mode: WriteMode.Overwrite.Instance, body: memoryStream);
                var sharedLink = await dbx.Sharing.CreateSharedLinkWithSettingsAsync("/quests/" + createQuestDto.Image.FileName);
                imageUrl = sharedLink.Url;
            }

            var newQuest = new QuestEntity()
            {
                Title = createQuestDto.Title,
                Description = createQuestDto.Description,
                Created = DateTime.Now,
                RewardAmount = createQuestDto.RewardAmount,
                Type = (QuestType)Enum.Parse(typeof(QuestType), createQuestDto.Type),
                Status = QuestStatus.Posted,
                Creator = currentUser,
                ImageName = imageUrl,
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

        // TODO: Add bool parameter to method signature for user filtering
        public async Task<QuestCardsContainerWithPaginationViewModel> CreateQuestCardsContainerWithPaginationViewModelAsync(FilterAndSortQuestsRequestData filterAndSortQuestsRequestData)
        {
            // TODO: Refactor expression build and extract it in method
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            Expression<Func<QuestEntity, bool>> userFilterExpression = filterAndSortQuestsRequestData.FilterOnlyUserQuests
                ? q => q.CreatorId == currentUser.Id || q.AdventurerId == currentUser.Id
                : q => true;

            Expression<Func<QuestEntity, object>> sortExpression = filterAndSortQuestsRequestData.SortBy == "Reward" ? q => q.RewardAmount : q => q.Created;

            Expression<Func<QuestEntity, bool>> searchExpression = string.IsNullOrWhiteSpace(filterAndSortQuestsRequestData.SearchTerm)
                ? q => true
                : q => q.Title.Contains(filterAndSortQuestsRequestData.SearchTerm) || (q.Description != null && q.Description.Contains(filterAndSortQuestsRequestData.SearchTerm));

            string[] statusFilters = filterAndSortQuestsRequestData.StatusFilters?.Split(',') ?? [];

            // TODO: Replace Enum Parse with Try Parse and extract method
            Expression<Func<QuestEntity, bool>> statusFiltersExpression = statusFilters.Length != 0
                ? q => statusFilters.Select(f => (QuestStatus)Enum.Parse(typeof(QuestStatus), f)).Contains(q.Status)
                : q => true;

            string[] typeFilters = filterAndSortQuestsRequestData.TypeFilters?.Split(',') ?? [];

            Expression<Func<QuestEntity, bool>> typeFiltersExpression = typeFilters.Length != 0
                ? q => typeFilters.Select(f => (QuestType)Enum.Parse(typeof(QuestType), f)).Contains(q.Type)
                : q => true;

            var parameter = Expression.Parameter(typeof(QuestEntity), "q");
            var combinedFilterExpression = Expression.Lambda<Func<QuestEntity, bool>>(
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Invoke(statusFiltersExpression, parameter),
                        Expression.Invoke(typeFiltersExpression, parameter)),
                    Expression.AndAlso(
                        Expression.Invoke(searchExpression, parameter),
                        Expression.Invoke(userFilterExpression, parameter))),
                parameter);

            var totalFilteredAndSortedQuestsCount = await _repository.GetCountAsync(combinedFilterExpression);

            var pageToShow = Math.Min(totalFilteredAndSortedQuestsCount, filterAndSortQuestsRequestData.CurrentPage);

            var filteredAndSortedQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync(pageToShow, 6, filterAndSortQuestsRequestData.OrderIsDescending, sortExpression, combinedFilterExpression);

            return new QuestCardsContainerWithPaginationViewModel()
            {
                //TODO : Extract mapping into method
                Quests = filteredAndSortedQuests.Select(q => new QuestDto
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
                    CurrentPage = pageToShow,
                    TotalPages = (int)Math.Ceiling(totalFilteredAndSortedQuestsCount / 6d),
                },
            };
        }
    }
}
