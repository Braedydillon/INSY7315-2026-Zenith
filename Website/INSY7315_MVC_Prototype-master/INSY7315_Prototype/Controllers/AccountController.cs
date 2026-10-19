
using INSY7315_Prototype.ViewModels;
using INSY7315_Prototype.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace INSY7315_Prototype.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthApiService _authApiService;

        public AccountController(AuthApiService authApiService)
        {
            _authApiService = authApiService;
        }

        // LOGIN - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectByRole(
                    User.FindFirst(ClaimTypes.Role)?.Value,
                    returnUrl
                );
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // LOGIN - POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var loginResult = await _authApiService.LoginAsync(model);

            if (loginResult == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password."
                );

                return View(model);
            }

            var role = loginResult.Role.ToLowerInvariant();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, loginResult.Uid),
                new Claim(ClaimTypes.Email, loginResult.Email),
                new Claim(ClaimTypes.Name, loginResult.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim("IdToken", loginResult.IdToken)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = false
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                authProperties
            );

            return RedirectByRole(role, returnUrl);
        }

        // REGISTER - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectByRole(
                    User.FindFirst(ClaimTypes.Role)?.Value,
                    returnUrl
                );
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // REGISTER - POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var registerResult =
                await _authApiService.RegisterAsync(model);

            if (registerResult == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Registration failed. The email may already be registered."
                );

                return View(model);
            }

            var role = registerResult.Role.ToLowerInvariant();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, registerResult.Uid),
                new Claim(ClaimTypes.Email, registerResult.Email),
                new Claim(ClaimTypes.Name, registerResult.Email),
                new Claim(ClaimTypes.Role, role),
                new Claim("IdToken", registerResult.IdToken)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = false
                }
            );

            return RedirectByRole(role, returnUrl);
        }

        // SAFE ROLE-BASED REDIRECT
        private IActionResult RedirectByRole(
            string? role,
            string? returnUrl = null)
        {
            switch (role?.ToLowerInvariant())
            {
                case "client":
                    if (!string.IsNullOrWhiteSpace(returnUrl)
                        && Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Client");

                case "staff":
                    return RedirectToAction("Index", "Staff");

                case "admin":
                    return RedirectToAction("Index", "Manager");

                default:
                    return RedirectToAction("Index", "Home");
            }
        }

        // ACCESS DENIED
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // LOGOUT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            return RedirectToAction("Login", "Account");
        }
    }
}
