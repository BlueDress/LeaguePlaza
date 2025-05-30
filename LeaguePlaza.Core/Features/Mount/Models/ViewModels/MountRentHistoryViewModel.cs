using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Mount.Models.ViewModels
{
    public class MountRentHistoryViewModel
    {
        public IEnumerable<MountRentalDto> MountRentals { get; set; } = new List<MountRentalDto>();
    }
}
