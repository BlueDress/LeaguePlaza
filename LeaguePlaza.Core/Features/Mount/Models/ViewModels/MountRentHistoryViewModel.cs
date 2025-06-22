using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Pagination.Models;

namespace LeaguePlaza.Core.Features.Mount.Models.ViewModels
{
    public class MountRentHistoryViewModel
    {
        public IEnumerable<MountRentalDto> MountRentals { get; set; } = new HashSet<MountRentalDto>();

        public PaginationViewModel Pagination { get; set; } = new PaginationViewModel();
    }
}
