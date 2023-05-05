using dat154_oblig4_reservations.Data;
using Microsoft.AspNetCore.Identity.UI.V4.Pages.Account.Internal;
using Microsoft.AspNetCore.Mvc;

namespace dat154_oblig4_reservations.Areas.Auth.Controllers
{
    [Area("Auth")]
    public class AccountController : Controller
    {
        private readonly LoginManager _loginManager;

        public AccountController(LoginManager loginManager)
        {
            _loginManager = loginManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnurl, string? error)
        {
            ViewData["ReturnUrl"] = returnurl;
            ViewData["Error"] = error;
            return View("~/Areas/Auth/Views/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnurl)
        {
            returnurl = returnurl ?? "/";

            bool success = await _loginManager.TryLogin(HttpContext.Session, username, password);
            if (success) return LocalRedirect(returnurl);

            return RedirectToAction(nameof(Login), new { returnurl, error = "WrongCredentials" });
        }

        [HttpGet]
        public IActionResult Register(string? returnurl, string? error, string? errorusername)
        {
            ViewData["ReturnUrl"] = returnurl;
            ViewData["ErrorUsername"] = errorusername;
            ViewData["Error"] = error;
            return View("~/Areas/Auth/Views/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string name, string? returnurl)
        {
            returnurl = returnurl ?? "/";

            var result = await _loginManager.Register(HttpContext.Session, username, password, name);
            if (result == LoginManager.RegisterResult.Success) return LocalRedirect(returnurl);

            return RedirectToAction(nameof(Register), new { returnurl, error = result.ToString(), errorusername = username });
        }

        [HttpPost]
        public IActionResult Logout(string? returnurl)
        {
            returnurl = returnurl ?? "/";

            _loginManager.Logout(HttpContext.Session);

            return LocalRedirect(returnurl);
        }
    }
}