using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Mount
{
    public class MountController(IMountService mountService) : Controller
    {
        private IMountService _mountService = mountService;

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        public async Task<IActionResult> Index()
        {
            MountsViewModel mountsViewModel = await _mountService.CreateMountsViewModelAsync();

            return View(mountsViewModel);
        }

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        public async Task<IActionResult> ViewMount(int id)
        {
            ViewMountViewModel viewMountViewModel = await _mountService.CreateViewMountViewModelAsync(id);

            return View(viewMountViewModel);
        }
    }
}
