using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Pagination.Models;

namespace LeaguePlaza.Core.Features.Mount.Models.ViewModels
{
    public class MountsViewModel
    {
        public IEnumerable<MountDto> Mounts { get; set; } = new HashSet<MountDto>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
