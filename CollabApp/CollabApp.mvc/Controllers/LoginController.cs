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
            try {
                await IsValidUserAsync(username);
            }
            catch(Exception err)
            {
                ViewBag.ErrorMessage = err.Message;
                return View();
            }

            username = username.Trim();

            HttpContext.Session.SetString("Username", username);
            return RedirectToAction("Index", "Home");
        }

        private async Task IsValidUserAsync(string username)
        {
            username.IsValidUsername();
        }
    }
}
