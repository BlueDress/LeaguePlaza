using LeaguePlaza.Core.Features.Admin.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Admin.Contracts
{
    public interface IAdminService
    {
        Task<MountAdminViewModel> CreateMountAdminViewModelAsync();
    }
}
