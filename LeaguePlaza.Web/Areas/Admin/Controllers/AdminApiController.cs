using LeaguePlaza.Core.Features.Admin.Contracts;
using LeaguePlaza.Core.Features.Admin.Models.ViewModels;
using LeaguePlaza.Core.Features.Mount.Contracts;
using LeaguePlaza.Core.Features.Mount.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.AdminConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = LeagueMaster)]
    public class AdminApiController(IAdminService adminService, IMountService mountService, IOrderService orderService) : Controller
    {
        public readonly IAdminService _adminService = adminService;
        private readonly IMountService _mountService = mountService;
        private readonly IOrderService _orderService = orderService;

        [HttpPost("createmount")]
        public async Task<IActionResult> CreateMount([FromForm] CreateMountDto createMountDto)
        {
            await _mountService.CreateMountsAsync(createMountDto);
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync();

            return PartialView(MountAdminCardsContainerWithPagination, mountAdminViewModel);
        }

        [HttpPut("updatemount")]
        public async Task<IActionResult> UpdateMount([FromForm] UpdateMountDto updateMountDto)
        {
            await _mountService.UpdateMountAsync(updateMountDto);
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync();

            return PartialView(MountAdminCardsContainerWithPagination, mountAdminViewModel);
        }

        [HttpDelete("deletemount")]
        public async Task<IActionResult> DeleteMount([FromBody] DeleteMountDto deleteMountDto)
        {
            await _mountService.DeleteMountAsync(deleteMountDto.Id);
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync();

            return PartialView(MountAdminCardsContainerWithPagination, mountAdminViewModel);
        }

        [HttpGet("getpageresults")]
        public async Task<IActionResult> GetPageResults([FromQuery] int pageNumber)
        {
            MountAdminViewModel mountAdminViewModel = await _adminService.CreateMountAdminViewModelAsync(pageNumber);

            return PartialView(MountAdminCardsContainerWithPagination, mountAdminViewModel);
        }

        [HttpPost("updateorderstatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDto updateOrderStatusDto)
        {
            await _orderService.UpdateOrderStatusAsync(updateOrderStatusDto);
            OrderAdminViewModel orderAdminViewModel = await _adminService.CreateOrderAdminViewModelAsync();

            return PartialView(OrderAdminCardsContainerWithPagination, orderAdminViewModel);
        }

        [HttpGet("getorderpageresults")]
        public async Task<IActionResult> GetOrderPageResults([FromQuery] int pageNumber)
        {
            OrderAdminViewModel orderAdminViewModel = await _adminService.CreateOrderAdminViewModelAsync(pageNumber);

            return PartialView(OrderAdminCardsContainerWithPagination, orderAdminViewModel);
        }
    }
}
