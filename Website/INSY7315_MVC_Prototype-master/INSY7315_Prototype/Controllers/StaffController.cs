using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{
    public class StaffController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
