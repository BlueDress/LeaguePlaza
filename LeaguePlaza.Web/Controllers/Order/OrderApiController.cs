using LeaguePlaza.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Order
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = UserRoleConstants.Adventurer)]
    public class OrderApiController : Controller
    {
        
    }
}
