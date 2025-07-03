using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Pagination.Models;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

using static LeaguePlaza.Common.Constants.MountConstants;
using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Admin.Services
{
    public class AdminService(IRepository repository) : IAdminService
    {
        private readonly IRepository _repository = repository;

        public async Task<MountAdminViewModel> CreateMountAdminViewModelAsync(int pageNumber = PageOne)
        {
            IEnumerable<MountEntity> mounts = await _repository.FindSpecificCountOrderedReadOnlyAsync<MountEntity, int>(pageNumber, AdminCountForPagination, false, m => m.Id, m => true);
            int totalResults = await _repository.GetCountAsync<MountEntity>(m => true);

            return new MountAdminViewModel()
            {
                Mounts = mounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = string.IsNullOrWhiteSpace(m.Description) ? NoMountDescriptionAvailable : m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                }),
                Pagination = new PaginationViewModel()
                {
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalResults / AdminCountForPagination),
                },
            };
        }
    }
}
