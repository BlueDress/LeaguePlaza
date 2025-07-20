using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Error
{
    public class ErrorController : Controller
    {
        public IActionResult Error500()
        {
            return View();
        }

        public IActionResult Error404()
        {
            return View();
        }
    }
}
