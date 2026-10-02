using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{
    [Authorize(Roles = "management")]
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
