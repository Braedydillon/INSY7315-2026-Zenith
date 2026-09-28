using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{
    public class LoanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
