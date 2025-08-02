using LeaguePlaza.Core.Features.Product.Contracts;
using LeaguePlaza.Core.Features.Product.Models.RequestData;
using LeaguePlaza.Core.Features.Product.Models.ViewModels;
using LeaguePlaza.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.ProductConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Product
{
    [Authorize(Roles = Adventurer)]
    public class ProductApiController(IProductService productService, ILogger<ProductController> logger) : BaseApiController
    {
        private readonly IProductService _productService = productService;
        private readonly ILogger<ProductController> _logger = logger;

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
