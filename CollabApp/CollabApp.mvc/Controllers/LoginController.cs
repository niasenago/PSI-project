
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
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var user = await ValidateCredentialsAsync(username, password);
                if (user == null)
                {
                    ViewBag.ErrorMessage = "Invalid username or password.";
                    return View();
                }

                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetInt32("UserId", user.Id);

                return RedirectToAction("Index", "Home");
            }
            catch (ValidationException err)
            {
                ViewBag.ErrorMessage = err.Message;
                return View();
            }
        }

        private async Task<User?> ValidateCredentialsAsync(string username, string password)
        {
            // Retrieve the user from the database based on the provided username
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            if (user == null || !ValidatePassword(password, user.PasswordHash, user.Salt))
            {
                return null; // Invalid credentials
            }

            return user;
        }

        private bool ValidatePassword(string enteredPassword, string storedPasswordHash, string salt)
        {
            // Validate the entered password against the stored hash and salt
            var enteredPasswordHash = PasswordHasher.HashPassword(enteredPassword, salt);
            return enteredPasswordHash == storedPasswordHash;
        }

        private async Task IsValidUserAsync(string username)
        {
            username.IsValidUsername();
        }
    }
}
