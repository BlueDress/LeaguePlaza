using LeaguePlaza.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Order
{
    [Authorize(Roles = UserRoleConstants.Adventurer)]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
