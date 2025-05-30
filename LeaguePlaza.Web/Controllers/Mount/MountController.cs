using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Mount
{
    public class MountController(IMountService mountService, ILogger<MountController> logger) : Controller
    {
        private readonly IMountService _mountService = mountService;
        private readonly ILogger<MountController> _logger = logger;

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        public async Task<IActionResult> Index()
        {
            try
            {
                MountsViewModel mountsViewModel = await _mountService.CreateMountsViewModelAsync();

                return View(mountsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(Index));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new MountsViewModel());
            }
        }

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        public async Task<IActionResult> ViewMount(int id)
        {
            try
            {
                ViewMountViewModel viewMountViewModel = await _mountService.CreateViewMountViewModelAsync(id);

                return View(viewMountViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(ViewMount));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new ViewMountViewModel());
            }
        }

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        public async Task<IActionResult> MountRentHistory()
        {
            try
            {
                MountRentHistoryViewModel mountRentHistoryViewModel = await _mountService.CreateMountRentHistoryViewModelAsync();

                return View(mountRentHistoryViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(MountRentHistory));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new MountRentHistoryViewModel());
            }
        }
    }
}
