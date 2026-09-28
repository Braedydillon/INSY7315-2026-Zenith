using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{
    public class LoanApplicationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
