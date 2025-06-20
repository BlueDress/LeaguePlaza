using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Order
{
    [Authorize(Roles = UserRoleConstants.Adventurer)]
    public class OrderController(IOrderService orderService, ILogger<OrderController> logger) : Controller
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<OrderController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                OrderHistoryViewModel orderHistoryViewModel = await _orderService.CreateOrderHistoryViewModelAsync();

                return View(orderHistoryViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(Index));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new OrderHistoryViewModel());
            }
        }
    }
}
