using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Mount
{
    public class MountController : Controller
    {
        public async Task<ActionResult> Index()
        {
            return View();
        }
    }
}
