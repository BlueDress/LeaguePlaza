using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Core.Features.Quest.Contracts;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.RequestData;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Enums;
using LeaguePlaza.Infrastructure.Data.Repository;
using LeaguePlaza.Infrastructure.Dropbox.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Linq.Expressions;

namespace LeaguePlaza.Core.Features.Quest.Services
{
    public class QuestService(IRepository repository, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, IDropboxService dropboxService) : IQuestService
    {
        private const string ImageUploadPath = "/quests/{0}/{1}/{2}";

        private readonly Dictionary<string, string> DefaultQuestTypeImageUrls = new()
        {
            { "1", "https://www.dropbox.com/scl/fi/zxqv1fy2io88ytcdi3iqa/monster-hunt-default.jpg?rlkey=vkl9dt9q96af2qlv8gx5etsdy&st=03rctf0o&raw=1" },
            { "2", "https://www.dropbox.com/scl/fi/ns7u5n9zhqw9q3i5g6gsq/gathering-default.jpg?rlkey=zbrno8iqnhxdqgmm2xkg8moyh&st=gm6ja4j6&raw=1" },
            { "3", "https://www.dropbox.com/scl/fi/977mmg7o6fxpr3e4i5k4p/escort-default.jpg?rlkey=fyekeazwrh373cyxqtu6kjxeg&st=2y5oj0ms&raw=1" },
        };

        private readonly IRepository _repository = repository;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IDropboxService _dropboxService = dropboxService;

        public async Task<QuestsViewModel> CreateAvailableQuestsViewModelAsync()
        {
            IEnumerable<QuestEntity> availableQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync<QuestEntity, DateTime>(QuestConstants.PageOne, QuestConstants.CountForPagination, true, q => q.Created, q => q.Status == QuestStatus.Posted);
            int totalResults = await _repository.GetCountAsync<QuestEntity>(q => q.Status == QuestStatus.Posted);

            return new QuestsViewModel()
            {
                Quests = availableQuests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? QuestConstants.NoDescriptionAvailable : q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    Status = q.Status.ToString(),
                    CreatorId = q.CreatorId,
                    AdventurerId = q.AdventurerId,
                    ImageUrl = q.ImageName,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = QuestConstants.PageOne,
                    TotalPages = (int)Math.Ceiling(totalResults / 6d),
                },
            };
        }

        public async Task<QuestsViewModel> CreateUserQuestsViewModelAsync()
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser != null)
            {
                IEnumerable<QuestEntity> userQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync<QuestEntity, object>(QuestConstants.PageOne, QuestConstants.CountForPagination, true, q => q.Created, q => q.CreatorId == currentUser.Id || q.AdventurerId == currentUser.Id);
                int totalResults = await _repository.GetCountAsync<QuestEntity>(q => q.CreatorId == currentUser.Id || q.AdventurerId == currentUser.Id);

                return new QuestsViewModel()
                {
                    Quests = userQuests.Select(q => new QuestDto
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Description = string.IsNullOrWhiteSpace(q.Description) ? QuestConstants.NoDescriptionAvailable : q.Description,
                        Created = q.Created,
                        RewardAmount = q.RewardAmount,
                        Type = q.Type.ToString(),
                        Status = q.Status.ToString(),
                        CreatorId = q.CreatorId,
                        AdventurerId = q.AdventurerId,
                        ShowExtraButtons = true,
                        ImageUrl = q.ImageName,
                    }),
                    Pagination = new PaginationViewModel()
                    {
                        CurrentPage = QuestConstants.PageOne,
                        TotalPages = (int)Math.Ceiling(totalResults / 6d),
                    },
                };
            }
            else
            {
                return new QuestsViewModel();
            }
        }

        public async Task<ViewQuestViewModel> CreateViewQuestViewModelAsync(int id)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser != null)
            {
                var quest = await _repository.FindByIdAsync<QuestEntity>(id) ?? new();
                IEnumerable<QuestEntity> recommendedQuests = await _repository.FindSpecificCountReadOnlyAsync<QuestEntity>(QuestConstants.RecommendedQuestsCount, q => q.Id != id && q.Type == quest.Type && q.Status == QuestStatus.Posted);

                return new ViewQuestViewModel()
                {
                    Quest = new QuestDto()
                    {
                        Id = id,
                        Title = quest.Title,
                        Description = string.IsNullOrWhiteSpace(quest.Description) ? QuestConstants.NoDescriptionAvailable : quest.Description,
                        Created = quest.Created,
                        RewardAmount = quest.RewardAmount,
                        Type = quest.Type.ToString(),
                        Status = quest.Status.ToString(),
                        CreatorId = quest.CreatorId,
                        AdventurerId = quest.AdventurerId,
                        ImageUrl = quest.ImageName,
                    },
                    RecommendedQuests = recommendedQuests.Select(q => new QuestDto
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Description = string.IsNullOrWhiteSpace(q.Description) ? QuestConstants.NoDescriptionAvailable : q.Description,
                        Created = q.Created,
                        RewardAmount = q.RewardAmount,
                        Type = q.Type.ToString(),
                        Status = q.Status.ToString(),
                        CreatorId = q.CreatorId,
                        AdventurerId = q.AdventurerId,
                        ImageUrl = q.ImageName,
                    }),
                    CurrentUserId = currentUser.Id,
                };
            }
            else
            {
                return new ViewQuestViewModel();
            }
        }

        public async Task CreateQuestAsync(CreateQuestDto createQuestDto)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            if (currentUser != null)
            {
                var dateCreated = DateTime.Now;

                string imageUrl = string.Empty;

                if (createQuestDto.Image != null)
                {
                    string accessToken = await _dropboxService.GetAccessToken();

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        string uploadPath = string.Format(ImageUploadPath, createQuestDto.Title, dateCreated.ToLongTimeString(), createQuestDto.Image.FileName);
                        imageUrl = await _dropboxService.UploadImage(createQuestDto.Image, uploadPath, accessToken);
                    }
                }

                imageUrl = string.IsNullOrEmpty(imageUrl) ? DefaultQuestTypeImageUrls[createQuestDto.Type] : imageUrl;

                var newQuest = new QuestEntity()
                {
                    Title = createQuestDto.Title,
                    Description = createQuestDto.Description,
                    Created = dateCreated,
                    RewardAmount = createQuestDto.RewardAmount,
                    Type = (QuestType)Enum.Parse(typeof(QuestType), createQuestDto.Type),
                    Status = QuestStatus.Posted,
                    Creator = currentUser,
                    ImageName = imageUrl,
                };

                await _repository.AddAsync(newQuest);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task UpdateQuestAsync(UpdateQuestDataDto updateQuestDto)
        {
            var questToUpdate = await _repository.FindByIdAsync<QuestEntity>(updateQuestDto.Id);

            if (questToUpdate != null)
            {
                questToUpdate.Title = updateQuestDto.Title;
                questToUpdate.Description = updateQuestDto.Description;
                questToUpdate.RewardAmount = updateQuestDto.RewardAmount;
                questToUpdate.Type = (QuestType)Enum.Parse(typeof(QuestType), updateQuestDto.Type);

                if (updateQuestDto.Image != null)
                {
                    string accessToken = await _dropboxService.GetAccessToken();

                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        string uploadPath = string.Format(ImageUploadPath, updateQuestDto.Title, questToUpdate.Created.ToLongTimeString(), updateQuestDto.Image.FileName);
                        string imageUrl = await _dropboxService.UploadImage(updateQuestDto.Image, uploadPath, accessToken);

                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            questToUpdate.ImageName = imageUrl;
                        }
                    }
                }

                _repository.Update(questToUpdate);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task AcceptQuestAsync(int id)
        {
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);
            var questToAccept = await _repository.FindByIdAsync<QuestEntity>(id);

            if (currentUser != null && questToAccept != null)
            {
                questToAccept.Status = QuestStatus.Accepted;
                questToAccept.AdventurerId = currentUser.Id;

                _repository.Update(questToAccept);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task RemoveQuestAsync(int id)
        {
            var questToRemove = await _repository.FindByIdAsync<QuestEntity>(id);

            if (questToRemove != null)
            {
                _repository.Remove(questToRemove);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task CompleteQuestAsync(int id)
        {
            var questToComplete = await _repository.FindByIdAsync<QuestEntity>(id);

            if (questToComplete != null)
            {
                questToComplete.Status = QuestStatus.Completed;

                _repository.Update(questToComplete);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task AbandonQuestAsync(int id)
        {
            var questToAbandon = await _repository.FindByIdAsync<QuestEntity>(id);

            if (questToAbandon != null)
            {
                questToAbandon.Status = QuestStatus.Posted;
                questToAbandon.AdventurerId = null;

                _repository.Update(questToAbandon);
                await _repository.SaveChangesAsync();
            }
        }

        public async Task<QuestsViewModel> CreateQuestCardsContainerWithPaginationViewModelAsync(FilterAndSortQuestsRequestData filterAndSortQuestsRequestData)
        {
            // TODO: Refactor expression build and extract it in method
            ApplicationUser? currentUser = await _userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User!);

            Expression<Func<QuestEntity, bool>> userFilterExpression = filterAndSortQuestsRequestData.PageIsMyQuests && currentUser != null
                ? q => q.CreatorId == currentUser.Id || q.AdventurerId == currentUser.Id
                : q => true;

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

            ParameterExpression parameter = Expression.Parameter(typeof(QuestEntity), "q");
            Expression<Func<QuestEntity, bool>> combinedFilterExpression = Expression.Lambda<Func<QuestEntity, bool>>(
                Expression.AndAlso(
                    Expression.AndAlso(
                        Expression.Invoke(userFilterExpression, parameter),
                        Expression.Invoke(searchExpression, parameter)),
                    Expression.AndAlso(
                        Expression.Invoke(statusFiltersExpression, parameter),
                        Expression.Invoke(typeFiltersExpression, parameter))),
                parameter);

            int totalFilteredAndSortedQuestsCount = await _repository.GetCountAsync(combinedFilterExpression);

            if (totalFilteredAndSortedQuestsCount == 0)
            {
                return new QuestsViewModel();
            }

            int pageToShow = Math.Min((int)Math.Ceiling((double)totalFilteredAndSortedQuestsCount / ProductConstants.CountForPagination), filterAndSortQuestsRequestData.CurrentPage);

            Expression<Func<QuestEntity, object>> sortExpression = filterAndSortQuestsRequestData.SortBy == "Reward" ? q => q.RewardAmount : q => q.Created;

            IEnumerable<QuestEntity> filteredAndSortedQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync(pageToShow, QuestConstants.CountForPagination, filterAndSortQuestsRequestData.OrderIsDescending, sortExpression, combinedFilterExpression);

            return new QuestsViewModel()
            {
                Quests = filteredAndSortedQuests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? QuestConstants.NoDescriptionAvailable : q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    Status = q.Status.ToString(),
                    CreatorId = q.CreatorId,
                    AdventurerId = q.AdventurerId,
                    ShowExtraButtons = filterAndSortQuestsRequestData.PageIsMyQuests,
                    ImageUrl = q.ImageName,
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
