using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
        [HttpGet]
        public IActionResult DisplayRegisterForm()
        {
            return View("Register");
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Password != model.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The password and confirmation password do not match.");
                    return View("DisplayRegisterForm", model);
                }
                
                // Save the user to the database using your repository or service
                var user = new User(model.Username, model.Password);
                await _unitOfWork.UserRepository.AddEntity(user);
                await _unitOfWork.CompleteAsync();

                // Redirect to a success page or login page
                return RedirectToAction("Login", "Login");
            }

            // If the model is not valid, return to the registration page with validation errors
            return View("Register", model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}