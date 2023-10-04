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
            if(await IsValidUserAsync(username))
            {
                HttpContext.Session.SetString("Username", username);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username. Please try again.";
                return View();
            }
        }

        private async Task<bool> IsValidUserAsync(string username)
        {
            return true;
        }
    }
}