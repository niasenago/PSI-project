
using CollabApp.mvc.Context;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Models;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Repo;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public LoginController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

            var user = new User(username, "temp-password");
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetInt32("UserId", user.Id);

            await _unitOfWork.UserRepository.AddEntity(user);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("Index", "Home");
        }

        private async Task IsValidUserAsync(string username)
        {
            username.IsValidUsername();
        }
    }
}
