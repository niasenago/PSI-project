using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [Route("LoginController/GetUsername")]
        public string? GetUsername()
        {
            return HttpContext.Session.GetString("Username");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username)
        {
            ValidationError error = await IsValidUserAsync(username);
            if(!error.HasError())
            {
                username = username.Trim();

                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = error.ErrorMessage;
                return View();
            }
        }

        private async Task<ValidationError> IsValidUserAsync(string username)
        {
            return username.IsValidUsername();
        }
    }
}
