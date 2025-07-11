using LeaguePlaza.Core.Features.Home.Contracts;
using LeaguePlaza.Core.Features.Home.Models.ViewModels;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

using static LeaguePlaza.Common.Constants.QuestConstants;
using static LeaguePlaza.Common.Constants.MountConstants;
using static LeaguePlaza.Common.Constants.ProductConstants;
using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Home.Services
{
    public class HomeService(IRepository repository) : IHomeService
    {
        private readonly IRepository _repository = repository;

        public async Task<HomePageViewModel> CreateHomePageViewModelAsync()
        {
            IEnumerable<QuestEntity> latestQuests = await _repository.FindSpecificCountOrderedReadOnlyAsync<QuestEntity, DateTime>(PageOne, QuestsForHomePage, true, q => q.Created, q => q.AdventurerId == null);
            IEnumerable<MountEntity> latestMounts = await _repository.FindSpecificCountOrderedReadOnlyAsync<MountEntity, DateTime>(PageOne, MountsForHomePage, true, m => m.Created, m => true);
            IEnumerable<ProductEntity> cheapestProducts = await _repository.FindSpecificCountOrderedReadOnlyAsync<ProductEntity, decimal>(PageOne, ProductsForHomePage, false, p => p.Price, p => p.IsInStock);

            return new HomePageViewModel()
            {
                LatestQuests = latestQuests.Select(q => new QuestDto()
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = string.IsNullOrWhiteSpace(q.Description) ? NoQuestDescriptionAvailable : q.Description,
                    Created = q.Created,
                    RewardAmount = q.RewardAmount,
                    Type = q.Type.ToString(),
                    ImageUrl = q.ImageName,
                }),
                LatestMounts = latestMounts.Select(m => new MountDto()
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = string.IsNullOrWhiteSpace(m.Description) ? NoMountDescriptionAvailable : m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                }),
                CheapestProducts = cheapestProducts.Select(p => new ProductDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = string.IsNullOrWhiteSpace(p.Description) ? NoProductDescriptionAvailable : p.Description,
                    Price = p.Price,
                    ImageUrl = p.ImageUrl,
                    ProductType = p.ProductType.ToString(),
                }),
            };
        }
    }
}
