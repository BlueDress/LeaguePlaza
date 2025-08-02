using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using LeaguePlaza.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.OrderConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Order
{
    [Authorize(Roles = Adventurer)]
    public class OrderApiController(IOrderService orderService, ILogger<OrderController> logger) : BaseApiController
    {
        private readonly IOrderService _orderService = orderService;
        private readonly ILogger<OrderController> _logger = logger;

        [HttpGet("getcartitemscount")]
        public async Task<int> GetCartItemsCount()
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return 0;
                }

                return await _orderService.GetCartItemsCountAsync(currentUserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(GetCartItemsCount));
                _logger.LogError(ErrorMessage, ex.Message);

                return 0;
            }
        }

        [HttpGet("getpageresults")]
        public async Task<IActionResult> GetPageResults([FromQuery] int pageNumber)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return View(new OrderHistoryViewModel());
                }

                OrderHistoryViewModel orderHistoryViewModel = await _orderService.CreateOrderHistoryViewModelAsync(currentUserId, pageNumber);

                return PartialView(OrderHistoryContainerWithPagination, orderHistoryViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(GetPageResults));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new OrderHistoryViewModel());
            }
        }

        [HttpPost("addtocart")]
        public async Task<IActionResult> AddToCart([FromBody] CreateCartItemDto createCartItemDto)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return BadRequest();
                }

                AddToCartResultDto addToCartResult = await _orderService.AddToCartAsync(createCartItemDto, currentUserId);

                return Ok(addToCartResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(AddToCart));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpGet("showorderinformation")]
        public async Task<IActionResult> ShowOrderInformation()
        {
            try
            {
                return PartialView(OrderInformation);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ShowOrderInformation));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpGet("showcartitems")]
        public async Task<IActionResult> ShowCartItems()
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return BadRequest();
                }

                CartViewModel cartViewModel = await _orderService.CreateViewCartViewModelAsync(currentUserId);

                return PartialView(CartItems, cartViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ShowCartItems));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpGet("showsubmitorder")]
        public async Task<IActionResult> ShowSubmitOrder([FromQuery] OrderInformationDto orderInformationDto)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return BadRequest();
                }

                CartViewModel cartViewModel = await _orderService.CreateViewCartViewModelAsync(currentUserId, orderInformationDto);

                return PartialView(SubmitOrder, cartViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ShowSubmitOrder));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpPost("createorder")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderInformationDto orderInformationDto)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return BadRequest();
                }

                bool orderCreatedSuccessfully = await _orderService.CreateOrderAsync(orderInformationDto, currentUserId);

                if (orderCreatedSuccessfully)
                {
                    return PartialView(OrderSuccessful);
                }

                return PartialView(OrderFailed);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(CreateOrder));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpPost("removecartitem")]
        public async Task<IActionResult> RemoveCartItem([FromBody] int cartItemId)
        {
            try
            {
                string? currentUserId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    return BadRequest();
                }

                await _orderService.RemoveCartItemAsync(cartItemId);
                CartViewModel cartViewModel = await _orderService.CreateViewCartViewModelAsync(currentUserId);

                return PartialView(CartItems, cartViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(RemoveCartItem));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }
    }
}
