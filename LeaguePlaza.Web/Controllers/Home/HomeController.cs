using System.Diagnostics;

using LeaguePlaza.Core.Features.Home.Contracts;
using LeaguePlaza.Core.Features.Home.Models.ViewModels;
using LeaguePlaza.Web.Models;

using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Home
{
    public class HomeController(ILogger<HomeController> logger, IHomeService homeService) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly IHomeService _homeService = homeService;

        public async Task<IActionResult> Index()
        {
            HomePageViewModel homePageViewModel = await _homeService.CreateHomePageViewModelAsync();

            return View(homePageViewModel);
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
