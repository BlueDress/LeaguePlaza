using System.Diagnostics;
using LeaguePlaza.Common.Constants;
using LeaguePlaza.Core.Features.Home.Contracts;
using LeaguePlaza.Core.Features.Home.Models.ViewModels;
using LeaguePlaza.Web.Models;

using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Home
{
    public class HomeController(IHomeService homeService, ILogger<HomeController> logger) : Controller
    {
        private readonly IHomeService _homeService = homeService;
        private readonly ILogger<HomeController> _logger = logger;

        public async Task<IActionResult> Index()
        {
            try
            {
                HomePageViewModel homePageViewModel = await _homeService.CreateHomePageViewModelAsync();

                return View(homePageViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ErrorConstants.FailedAt, nameof(Index));
                _logger.LogError(ErrorConstants.ErrorMessage, ex.Message);

                return View(new HomePageViewModel());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
