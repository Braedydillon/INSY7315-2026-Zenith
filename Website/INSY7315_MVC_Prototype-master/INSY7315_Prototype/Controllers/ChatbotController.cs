using INSY7315_Prototype.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace INSY7315_Prototype.Controllers
{  // staff and managers are refused, not just hidden
    public class ChatbotController : Controller
    {
        public IActionResult Index() => View();

        [HttpGet]
        public IActionResult Ask(string? q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length > 300)
                return BadRequest();

            return Json(ChatbotResponder.Reply(q));
        }
    }
}