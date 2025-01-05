using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers
{
    public class QuestController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
