using Microsoft.AspNetCore.Mvc;

namespace APIINSY7315.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FirebaseAuthorize]
    public class MeController : ControllerBase
    {
        // GET api/Me - web and mobile call this after login to learn who they are and which screens to show.
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            uid = HttpContext.GetUserId(),
            email = HttpContext.GetEmail(),
            role = HttpContext.GetRole()
        });
    }
}
