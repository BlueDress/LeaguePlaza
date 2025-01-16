using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Mount.Contracts
{
    public interface IMountService
    {
        Task<MountsViewModel> CreateMountViewModelAsync();
    }
}
