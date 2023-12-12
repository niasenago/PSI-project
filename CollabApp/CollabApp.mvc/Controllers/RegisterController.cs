using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.mvc.Controllers
{
    [Route("[controller]")]
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterController(ILogger<RegisterController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    TempData["RegisterErrorMessage"] = "The password and confirmation password do not match.";
                    return RedirectToAction("Login", "Login");
                }
                if (!IsPasswordValid(model.Password))
                {
                    ModelState.AddModelError("Password", "Password must contain both uppercase and lowercase letters, numbers, and be at least 8 characters long.");
                    return View("Register", model);
                }
                if (!IsPasswordValid(model.Password))
                {
                    ModelState.AddModelError("Password", "Password must contain both uppercase and lowercase letters, numbers, and be at least 8 characters long.");
                    return View("Register", model);
                }

                // Check if the username is already taken using the repository method
                if (await _unitOfWork.UserRepository.IsUsernameTakenAsync(model.Username))
                {
                    TempData["RegisterErrorMessage"] = "The username is already taken. Please choose a different one.";
                    return RedirectToAction("Login", "Login");
                }
                if (ProfanityHandler.HasProfanity(model.Username))
                {
                    TempData["RegisterErrorMessage"] = "Username contains profanities.";
                    return RedirectToAction("Login", "Login");
                }
            }

                // Save the user to the database using your repository or service
            var user = new User(model.Username, model.Password);
            await _unitOfWork.UserRepository.AddEntity(user);
            await _unitOfWork.CompleteAsync();

            // Redirect to a success page or login page
            return RedirectToAction("Login", "Login");

        }
        private bool IsPasswordValid(string password)
        {
            // Password must contain both uppercase and lowercase letters, numbers, and be at least 8 characters long
            return
                password.Any(char.IsUpper) &&
                password.Any(char.IsLower) &&
                password.Any(char.IsDigit) &&
                password.Length >= 8;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}