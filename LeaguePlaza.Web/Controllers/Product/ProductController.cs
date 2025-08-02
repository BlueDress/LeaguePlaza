using LeaguePlaza.Core.Features.Product.Contracts;
using LeaguePlaza.Core.Features.Product.Models.ViewModels;
using LeaguePlaza.Web.Controllers.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using static LeaguePlaza.Common.Constants.ErrorConstants;
using static LeaguePlaza.Common.Constants.UserRoleConstants;

namespace LeaguePlaza.Web.Controllers.Product
{
    [Authorize(Roles = Adventurer)]
    public class ProductController(IProductService productService, ILogger<ProductController> logger) : BaseController
    {
        private readonly IProductService _productService = productService;
        private readonly ILogger<ProductController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                ProductsViewModel productsViewModel = await _productService.CreateAvailableProductsViewModelAsync();

                return View(productsViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(Index));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new ProductsViewModel());
            }
        }

        public async Task<IActionResult> ViewProduct(int id)
        {
            try
            {
                ViewProductViewModel viewProductViewModel = await _productService.CreateViewProductViewModelAsync(id);

                return View(viewProductViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(FailedAt, nameof(ViewProduct));
                _logger.LogError(ErrorMessage, ex.Message);

                return View(new ViewProductViewModel());
            }
        }
    }
}
