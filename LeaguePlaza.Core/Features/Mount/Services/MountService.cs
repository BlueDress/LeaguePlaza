using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

namespace LeaguePlaza.Core.Features.Mount.Services
{
    public class MountService(IRepository repository) : IMountService
    {
        private readonly IRepository _repository = repository;

        public async Task<MountsViewModel> CreateMountsViewModelAsync()
        {
            IEnumerable<MountEntity> mounts = await _repository.FindSpecificCountOrderedReadOnlyAsync<MountEntity, string>(QuestConstants.PageOne, QuestConstants.CountForPagination, false, m => m.Name, m => true);
            int totalResults = await _repository.GetCountAsync<MountEntity>(m => true);

            var mountCardsContainerWithPaginationViewModel = new MountCardsContainerWithPaginationViewModel()
            {
                Mounts = mounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = string.IsNullOrWhiteSpace(m.Description) ? MountConstants.NoDescriptionAvailable : m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = 1,
                    TotalPages = (int)Math.Ceiling(totalResults / 6d),
                },
            };

            return new MountsViewModel()
            {
                ViewModel = mountCardsContainerWithPaginationViewModel,
            };
        }

        public async Task<ViewMountViewModel> CreateViewMountViewModelAsync(int id)
        {
            var mount = await _repository.FindByIdAsync<MountEntity>(id);
            var recommendedMounts = await _repository.FindSpecificCountReadOnlyAsync<MountEntity>(3, m => m.Id != id && m.MountType == mount.MountType);

            return new ViewMountViewModel()
            {
                Mount = new MountDto()
                {
                    Id = id,
                    Name = mount.Name,
                    Description = mount.Description,
                    RentPrice = mount.RentPrice,
                    ImageUrl = mount.ImageUrl,
                    Type = mount.MountType.ToString(),
                },
                RecommendedMounts = recommendedMounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                }),
            };
        }
    }
}
