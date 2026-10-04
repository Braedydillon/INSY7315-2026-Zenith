using Microsoft.AspNetCore.Mvc;

namespace APIINSY7315.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [FirebaseAuthorize]
    public class MeController : ControllerBase
    {
        
        [HttpGet]
        public IActionResult Get() => Ok(new
        {
            uid = HttpContext.GetUserId(),
            email = HttpContext.GetEmail(),
            role = HttpContext.GetRole()
        });
    }
}
