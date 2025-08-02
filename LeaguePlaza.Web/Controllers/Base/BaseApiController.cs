using Microsoft.AspNetCore.Mvc;

namespace LeaguePlaza.Web.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseApiController : BaseController
    {
    }
}
