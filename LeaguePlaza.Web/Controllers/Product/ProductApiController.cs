using LeaguePlaza.Core.Features.Product.Contracts;
using LeaguePlaza.Core.Features.Product.Models.RequestData;
using LeaguePlaza.Core.Features.Product.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ProductConstants;
using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Product
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApiController(IProductService productService, ILogger<ProductController> logger) : Controller
    {
        private readonly IProductService _productService = productService;
        private readonly ILogger<ProductController> _logger = logger;

        [Authorize(Roles = Adventurer)]
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
                _logger.LogError(FailedAt, nameof(FilterAndSortProducts));
                _logger.LogError(ErrorMessage, ex.Message);

                return BadRequest();
            }
        }
    }
}
