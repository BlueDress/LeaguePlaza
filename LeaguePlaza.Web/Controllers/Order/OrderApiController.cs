using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Order.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoleConstants.Adventurer)]
    public class OrderApiController(IOrderService orderService, ILogger<OrderController> logger) : Controller
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<OrderController> _logger = logger;

        [HttpGet("getcartitemscount")]
        public async Task<int> GetCartItemsCount()
        {
            try
            {
                return await _orderService.GetCartItemsCountAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(GetCartItemsCount));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return 0;
            }
        }
    }
}
