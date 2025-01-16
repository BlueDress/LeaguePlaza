using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.ViewModels;

using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Mount
{
    public class MountController(IMountService mountService) : Controller
    {
        private IMountService _mountService = mountService;

        public async Task<IActionResult> Index()
        {
            MountsViewModel mountViewModel = await _mountService.CreateMountViewModelAsync();

            return View(mountViewModel);
        }
    }
}
