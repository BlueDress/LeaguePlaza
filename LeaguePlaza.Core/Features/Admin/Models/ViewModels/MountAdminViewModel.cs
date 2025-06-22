using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Pagination.Models;

namespace LeaguePlaza.Core.Features.Admin.Models.ViewModels
{
    public class MountAdminViewModel
    {
        public IEnumerable<MountDto> Mounts { get; set; } = new HashSet<MountDto>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
