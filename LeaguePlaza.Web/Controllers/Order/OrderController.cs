using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Order
{
    [Authorize(Roles = Adventurer)]
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
                _logger.LogError(FailedAt, nameof(Index));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new OrderHistoryViewModel());
            }
        }

        public async Task<IActionResult> ViewCart()
        {
            try
            {
                CartViewModel cartViewModel = await _orderService.CreateViewCartViewModelAsync();

                return View(cartViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ViewCart));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new CartViewModel());
            }
        }

        public async Task<IActionResult> ViewOrder(int id)
        {
            try
            {
                OrderViewModel orderViewModel = await _orderService.CreateOrderViewModelAsync(id);

                return View(orderViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ViewOrder));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new OrderViewModel());
            }
        }
    }
}
