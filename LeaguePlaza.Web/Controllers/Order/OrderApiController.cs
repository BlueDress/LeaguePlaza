using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Adventurer)]
    public class OrderApiController(IOrderService orderService, ILogger<OrderController> logger) : Controller
    {
        private const string OrderHistoryContainerWithPagination = "~/Views/Order/Partials/_OrderHistoryContainerWithPagination.cshtml";
        private const string OrderInformation = "~/Views/Order/Partials/_OrderInformation.cshtml";
        private const string CartItems = "~/Views/Order/Partials/_CartItems.cshtml";
        private const string SubmitOrder = "~/Views/Order/Partials/_SubmitOrder.cshtml";
        private const string OrderSuccessful = "~/Views/Order/Partials/_OrderSuccessful.cshtml";

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
                OrderHistoryViewModel orderHistoryViewModel = await _orderService.CreateOrderHistoryViewModelAsync(pageNumber);

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
                AddToCartResultDto addToCartResult = await _orderService.AddToCartAsync(createCartItemDto);

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
                CartViewModel cartViewModel = await _orderService.CreateViewCartViewModelAsync();

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
                CartViewModel cartViewModel = await _orderService.CreateViewCartViewModelAsync(orderInformationDto);

                return PartialView(SubmitOrder, cartViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ShowSubmitOrder));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }

        [HttpGet("showordersuccessful")]
        public async Task<IActionResult> ShowOrderSuccessful()
        {
            try
            {
                return PartialView(OrderSuccessful);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ShowOrderSuccessful));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }
    }
}
