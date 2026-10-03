using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace INSY7315_Prototype.Controllers
{
    [Authorize(Roles = "client")]
    public class ClientController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Email = User.FindFirst(ClaimTypes.Email)?.Value;
            ViewBag.Role = User.FindFirst(ClaimTypes.Role)?.Value;

            return View();
        }
    }
}
