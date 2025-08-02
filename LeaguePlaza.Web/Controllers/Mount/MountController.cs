using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;
using LeaguePlaza.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Mount
{
    [Authorize(Roles = Adventurer)]
    public class MountController(IMountService mountService, ILogger<MountController> logger) : BaseController
    {
        private readonly IMountService _mountService = mountService;
        private readonly ILogger<MountController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                MountsViewModel mountsViewModel = await _mountService.CreateMountsViewModelAsync();

                return View(mountsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(Index));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new MountsViewModel());
            }
        }

        public async Task<IActionResult> ViewMount(int id)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new ViewMountViewModel());
                }

                ViewMountViewModel viewMountViewModel = await _mountService.CreateViewMountViewModelAsync(id, currentUserId);

                return View(viewMountViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ViewMount));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new ViewMountViewModel());
            }
        }

        public async Task<IActionResult> MountRentHistory()
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new MountRentHistoryViewModel());
                }

                MountRentHistoryViewModel mountRentHistoryViewModel = await _mountService.CreateMountRentHistoryViewModelAsync(currentUserId);

                return View(mountRentHistoryViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(MountRentHistory));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new MountRentHistoryViewModel());
            }
        }
    }
}
