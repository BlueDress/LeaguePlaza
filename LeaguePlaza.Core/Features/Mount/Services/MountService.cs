using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;
using LeaguePlaza.Infrastructure.Data.Entities;
using LeaguePlaza.Infrastructure.Data.Repository;

namespace LeaguePlaza.Core.Features.Mount.Services
{
    public class MountService(IRepository repository) : IMountService
    {
        private IRepository _repository = repository;

        public async Task<MountsViewModel> CreateMountViewModelAsync()
        {
            var mounts = await _repository.GetAllReadOnlyAsync<MountEntity>();

            return new MountsViewModel()
            {
                Mounts = mounts.Select(m => new MountDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    RentPrice = m.RentPrice,
                    ImageUrl = m.ImageUrl,
                    Type = m.MountType.ToString(),
                    Rating = m.Rating,
                })
            };
        }
    }
}
