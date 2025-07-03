using LeaguePlaza.Core.Features.Admin.Models.ViewModels;

using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Admin.Contracts
{
    public interface IAdminService
    {
        Task<MountAdminViewModel> CreateMountAdminViewModelAsync(int pageNumber = PageOne);
    }
}
