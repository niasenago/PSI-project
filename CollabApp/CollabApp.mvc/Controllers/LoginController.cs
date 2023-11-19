using CollabApp.mvc.Context;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Models;
using CollabApp.mvc.Exceptions;
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
            try {
                await IsValidUserAsync(username);

            }
            catch(ValidationException err)
            {
                ViewBag.ErrorMessage = err.Message;
                return View();
            }

            username = username.Trim();

            var user = new User(username);
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        private async Task IsValidUserAsync(string username)
        {
            username.IsValidUsername();
        }
    }
}
