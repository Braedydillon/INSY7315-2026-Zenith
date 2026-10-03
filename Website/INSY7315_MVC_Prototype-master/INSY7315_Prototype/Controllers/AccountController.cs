using INSY7315_Prototype.ViewModels;
using Microsoft.AspNetCore.Mvc;
using INSY7315_Prototype.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace INSY7315_Prototype.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authApiService;

        public AccountController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            //Check ViewModel validation first
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            //Send email + password to API
            var loginResult = await _authApiService.LoginAsync(model);

            //API rejects the login
            if (loginResult == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");

                return View(model);
            }

            //If Login is Successful 
            //Claims are simply a pieece of information of each user so the MVC knows the user logged in
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginResult.Uid),
                new Claim(ClaimTypes.Email, loginResult.Email),
                new Claim(ClaimTypes.Name, loginResult.Email),
                new Claim(ClaimTypes.Role, loginResult.Role),
                new Claim("IdToken", loginResult.IdToken)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);


            //Once user is logged in return them to their page based on role
            switch (loginResult.Role.ToLower())
            {
                case "client":
                    return RedirectToAction("Index", "Client");

                case "management":
                    return RedirectToAction("Index", "Staff");

                case "admin":
                    return RedirectToAction("Index", "Manager");

                default:
                    return RedirectToAction("Index", "Home");
            }
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registerResult = await _authApiService.RegisterAsync(model);

            if (registerResult == null)
            {
                ModelState.AddModelError(string.Empty, "Registration failed. The email may already be registered");

                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, registerResult.Uid),
                new Claim(ClaimTypes.Email, registerResult.Email),
                new Claim(ClaimTypes.Name, registerResult.Email),
                new Claim(ClaimTypes.Role, registerResult.Role),
                new Claim("IdToken", registerResult.IdToken)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Client");
        }


        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync( CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("login", "Account");
        }

    }
}
