using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoleConstants.LeagueMaster)]
    public class AdminApiController(IAdminService adminService, IMountService mountService) : Controller
    {
        public readonly IAdminService _adminService = adminService;
        private readonly IMountService _mountService = mountService;

        [HttpPost("createmount")]
        public async Task<IActionResult> CreateMount([FromForm] CreateMountDto createMountDto)
        {
            await _mountService.CreateMountsAsync(createMountDto);
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync();

            return View(mountAdminViewModel);
        }

        [HttpPut("updatemount")]
        public async Task<IActionResult> UpdateMount([FromForm] UpdateMountDto updateMountDto)
        {
            await _mountService.UpdateMountAsync(updateMountDto);
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync();

            return View(mountAdminViewModel);
        }

        [HttpPut("deletemount")]
        public async Task<IActionResult> DeleteMount([FromForm] DeleteMountDto deleteMountDto)
        {
            await _mountService.DeleteMountAsync(deleteMountDto);
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync();

            return View(mountAdminViewModel);
        }
    }
}
