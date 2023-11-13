using CollabApp.mvc.Context;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Models;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }
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

                var user = new User(username);
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
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
