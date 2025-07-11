using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Product.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Quest.Models.Dtos.ReadOnly;

namespace LeaguePlaza.Core.Features.Home.Models.ViewModels
{
    public class HomePageViewModel
    {
        public IEnumerable<QuestDto> LatestQuests { get; set; } = new HashSet<QuestDto>();

        public IEnumerable<MountDto> LatestMounts { get; set; } = new HashSet<MountDto>();

        public IEnumerable<ProductDto> CheapestProducts { get; set; } = new HashSet<ProductDto>();
    }
}
