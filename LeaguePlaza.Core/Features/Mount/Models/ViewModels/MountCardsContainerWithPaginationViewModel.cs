using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Pagination.Models;

namespace LeaguePlaza.Core.Features.Mount.Models.ViewModels
{
    public class MountCardsContainerWithPaginationViewModel
    {
        public IEnumerable<MountDto> Mounts { get; set; } = new List<MountDto>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
