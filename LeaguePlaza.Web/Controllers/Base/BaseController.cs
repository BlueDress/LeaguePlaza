using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeaguePlaza.Web.Controllers.Base
{
    public abstract class BaseController : Controller
    {
        protected string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
