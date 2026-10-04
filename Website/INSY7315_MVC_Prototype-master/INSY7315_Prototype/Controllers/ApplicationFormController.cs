using INSY7315_Prototype.Models;
using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{
    public class ApplicationFormController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ApplicationForm());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ApplicationForm model)
        {
            if (!ModelState.IsValid)
                return View(model);

            TempData["Success"] = "Your application has been submitted.";
            return RedirectToAction("ThankYou");
        }

        [HttpGet]
        public IActionResult ThankYou()
        {
            return View(); 
        }
    }
}
