using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Product.Contracts;
using LeaguePlaza.Core.Features.Product.Models.RequestData;
using LeaguePlaza.Core.Features.Product.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApiController(IProductService productService, ILogger<ProductController> logger) : Controller
    {
        private const string ProductCardsContainerWithPagination = "~/Views/Product/Partials/_ProductCardsContainerWithPagination.cshtml";

        private readonly IProductService _productService = productService;
        private readonly ILogger<ProductController> _logger = logger;

        [Authorize(Roles = UserRoleConstants.Adventurer)]
        [HttpGet("filterandsortproducts")]
        public async Task<IActionResult> FilterAndSortProducts([FromQuery] FilterAndSortProductsRequestData filterAndSortProductsRequestData)
        {
            try
            {
                ProductsViewModel productsViewModel = await _productService.CreateProductCardsContainerWithPaginationViewModelAsync(filterAndSortProductsRequestData);

                return PartialView(ProductCardsContainerWithPagination, productsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(FilterAndSortProducts));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return BadRequest();
            }
        }
    }
}
