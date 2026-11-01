
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{
    public class LoanApplicationController : Controller
    {
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Application()
        {
            return RedirectToAction("Apply", "Loan");
        }
    }
}
