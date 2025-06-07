using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

namespace LeaguePlaza.Core.Features.Admin.Services
{
    public class AdminService(IRepository repository) : IAdminService
    {
        private readonly IRepository _repository = repository;

        public async Task<MountAdminViewModel> CreateMountAdminViewModelAsync(int pageNumber = AdminConstants.PageOne)
        {
            IEnumerable<MountEntity> mounts = await _repository.FindSpecificCountOrderedReadOnlyAsync<MountEntity, int>(pageNumber, AdminConstants.CountForPagination, false, m => m.Id, m => true);
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
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling(totalResults / 10d),
                },
            };

            return new MountAdminViewModel()
            {
                ViewModel = mountCardsContainerWithPaginationViewModel,
            };
        }
    }
}
