using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using LeaguePlaza.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Order
{
    [Authorize(Roles = Adventurer)]
    public class OrderController(IOrderService orderService, ILogger<OrderController> logger) : BaseController
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<OrderController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new OrderHistoryViewModel());
                }

                OrderHistoryViewModel orderHistoryViewModel = await _orderService.CreateOrderHistoryViewModelAsync(currentUserId);

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
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new CartViewModel());
                }

                CartViewModel cartViewModel = await _orderService.CreateViewCartViewModelAsync(currentUserId);

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
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new OrderViewModel());
                }

                OrderViewModel orderViewModel = await _orderService.CreateOrderViewModelAsync(id, currentUserId);

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
