using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Mount.Models.RequestData;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

using static LeaguePlaza.Common.Constants.PaginationConstants;

namespace LeaguePlaza.Core.Features.Mount.Contracts
{
    public interface IMountService
    {
        Task<MountsViewModel> CreateMountsViewModelAsync();

        Task<ViewMountViewModel> CreateViewMountViewModelAsync(int id);

        Task<MountRentHistoryViewModel> CreateMountRentHistoryViewModelAsync(int pageNumber = PageOne);

        Task<MountsViewModel> CreateMountsViewModelAsync(FilterAndSortMountsRequestData filterAndSortMountsRequestData);

        Task<MountRentalResultDto> RentMountAsync(RentMountDto rentMountDto);

        Task<string> AddOrUpadeMountRatingAsync(RateMountDto rateMountDto);

        Task CancelMountRentAsync(int id);

        Task CreateMountsAsync(CreateMountDto createMountDto);

        Task UpdateMountAsync(UpdateMountDto updateMountDto);

        Task DeleteMountAsync(int id);
    }
}
