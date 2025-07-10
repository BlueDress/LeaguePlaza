using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = LeagueMaster)]
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

        public async Task<IActionResult> ProductAdmin()
        {
            ProductAdminViewModel productAdminViewModel = await _adminService.CreateProductAdminViewModelAsync();

            return View(productAdminViewModel);
        }

        public async Task<IActionResult> OrderAdmin()
        {
            OrderAdminViewModel orderAdminViewModel = await _adminService.CreateOrderAdminViewModelAsync();

            return View(orderAdminViewModel);
        }
    }
}
