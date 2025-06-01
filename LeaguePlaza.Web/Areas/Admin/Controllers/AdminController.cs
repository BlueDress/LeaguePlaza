using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = UserRoleConstants.LeagueMaster)]
    public class AdminController(IAdminService adminService) : Controller
    {
        private readonly IAdminService _adminService = adminService;

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> MountAdmin()
        {
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync();

            return View(mountAdminViewModel);
        }
    }
}
