using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Mount.Models.ViewModels
{
    public class ViewMountViewModel
    {
        public MountDto Mount { get; set; } = new MountDto();

        public IEnumerable<MountDto> RecommendedMounts { get; set; } = new List<MountDto>();
    }
}
