
using LeaguePlaza.Core.Features.Home.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Home.Contracts
{
    public interface IHomeService
    {
        Task<HomePageViewModel> CreateHomePageViewModelAsync();
    }
}
