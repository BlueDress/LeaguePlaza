using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Mount.Models.ViewModels
{
    public class MountsViewModel
    {
        public IEnumerable<MountDto> Mounts { get; set; } = new List<MountDto>();
    }
}
