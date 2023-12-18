
using CollabApp.mvc.Validation;
using CollabApp.mvc.Models;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Repo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using CollabApp.mvc.Services;
using CollabApp.SharedObjects.Dto;

namespace CollabApp.mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpServiceClient _httpServiceClient;

        public LoginController(IHttpServiceClient httpServiceClient)
        {
            _httpServiceClient = httpServiceClient;
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
            try
            {
                var loginDto = new LoginDto
                {
                    Username = username,
                    Password = password
                };

                var responseContent = await _httpServiceClient.PostAsync("api/Auth/login", JsonConvert.SerializeObject(loginDto));

                var user = JsonConvert.DeserializeObject<User>(responseContent);
                return user;
            }
            catch
            {
                return null; // Invalid credentials or other API error
            }
        }
    }
}
