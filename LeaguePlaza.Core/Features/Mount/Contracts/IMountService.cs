using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Mount.Models.RequestData;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

namespace LeaguePlaza.Core.Features.Mount.Contracts
{
    public interface IMountService
    {
        Task<MountsViewModel> CreateMountsViewModelAsync();

        Task<ViewMountViewModel> CreateViewMountViewModelAsync(int id);

        Task<MountRentHistoryViewModel> CreateMountRentHistoryViewModelAsync();

        Task<MountCardsContainerWithPaginationViewModel> CreateMountCardsContainerWithPaginationViewModelAsync(FilterAndSortMountsRequestData filterAndSortMountsRequestData);

        Task<string> RentMountAsync(RentMountRequestData rentMountRequestData);

        Task<string> AddOrUpadeMountRatingAsync(RateMountRequestData rateMountRequestData);

        Task CancelMountRentAsync(int id);

        Task CreateMountsAsync(CreateMountDto createMountDto);

        Task UpdateMountAsync(UpdateMountDto updateMountDto);

        Task DeleteMountAsync(int id);
    }
}
