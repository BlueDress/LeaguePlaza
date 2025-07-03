using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Home.Contracts;
using LeaguePlaza.Core.Features.Home.Models.ViewModels;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

using static LeaguePlaza.Common.Constants.QuestConstants;
using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Home.Services
{
    public class HomeService(IRepository repository) : IHomeService
    {
        private readonly IRepository _repository = repository;

        public async Task<HomePageViewModel> CreateHomePageViewModelAsync()
        {
            var latestQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync<QuestEntity, DateTime>(PageOne, QuestsForHomePage, true, q => q.Created, q => q.AdventurerId == null);

            return new HomePageViewModel()
            {
                LatestQuests = latestQuests.Select(q => new QuestDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? NoQuestDescriptionAvailable : q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    ImageUrl = q.ImageName,
                }),
            };
        }
    }
}
