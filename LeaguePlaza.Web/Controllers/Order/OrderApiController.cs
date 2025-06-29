using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Order.Contracts;
using LeaguePlaza.Core.Features.Order.Models.Dtos.Create;
using LeaguePlaza.Core.Features.Order.Models.Dtos.ReadOnly;
using LeaguePlaza.Core.Features.Order.Models.ViewModels;
using LeaguePlaza.Core.Features.Quest.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoleConstants.Adventurer)]
    public class OrderApiController(IOrderService orderService, ILogger<OrderController> logger) : Controller
    {
        private const string OrderHistoryContainerWithPagination = "~/Views/Order/Partials/_OrderHistoryContainerWithPagination.cshtml";
        private const string OrderInformation = "~/Views/Order/Partials/_OrderInformation.cshtml";

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
                _logger.LogError(ErrorConstants.FailedAt, nameof(GetPageResults));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

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
                _logger.LogError(ErrorConstants.FailedAt, nameof(AddToCart));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

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
                _logger.LogError(ErrorConstants.FailedAt, nameof(ShowOrderInformation));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }
    }
}
