using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Admin.Models.ViewModels
{
    public class MountAdminViewModel
    {
        public MountCardsContainerWithPaginationViewModel ViewModel { get; set; } = new MountCardsContainerWithPaginationViewModel();
    }
}
