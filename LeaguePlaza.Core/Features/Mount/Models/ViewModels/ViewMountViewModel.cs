using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Mount.Models.ViewModels
{
    public class ViewMountViewModel
    {
        public int UserRating { get; set; }

        public MountDto Mount { get; set; } = new MountDto();

        public IEnumerable<MountDto> RecommendedMounts { get; set; } = new HashSet<MountDto>();
    }
}
