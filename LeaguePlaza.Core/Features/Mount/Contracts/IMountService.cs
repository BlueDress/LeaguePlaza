using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Mount.Models.RequestData;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Mount.Contracts
{
    public interface IMountService
    {
        Task<MountsViewModel> CreateMountsViewModelAsync();

        Task<ViewMountViewModel> CreateViewMountViewModelAsync(int id);

        Task<MountRentHistoryViewModel> CreateMountRentHistoryViewModelAsync(int pageNumber = MountConstants.PageOne);

        Task<MountCardsContainerWithPaginationViewModel> CreateMountCardsContainerWithPaginationViewModelAsync(FilterAndSortMountsRequestData filterAndSortMountsRequestData);

        Task<string> RentMountAsync(RentMountDto rentMountDto);

        Task<string> AddOrUpadeMountRatingAsync(RateMountDto rateMountDto);

        Task CancelMountRentAsync(int id);

        Task CreateMountsAsync(CreateMountDto createMountDto);

        Task UpdateMountAsync(UpdateMountDto updateMountDto);

        Task DeleteMountAsync(int id);
    }
}
