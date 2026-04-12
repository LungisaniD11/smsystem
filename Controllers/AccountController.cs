using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace FutureTech.StudentManagement.Web.Controllers;

public sealed class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult LoginWithGoogle(string? returnUrl = null)
    {
        var redirectUrl = Url.Action(nameof(OAuthCallback), new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, "Google");
    }

    [HttpGet]
    public IActionResult OAuthCallback(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            TempData["Error"] = "Authentication failed. Please try again.";
            return RedirectToAction(nameof(Login));
        }

        if (!User.IsInRole("Admin"))
        {
            TempData["Error"] = "Your account is authenticated but not authorized as an administrator.";
            return RedirectToAction(nameof(AccessDenied));
        }

        return LocalRedirect(string.IsNullOrWhiteSpace(returnUrl) ? Url.Action("Index", "Students")! : returnUrl);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
