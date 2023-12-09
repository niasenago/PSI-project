
using CollabApp.mvc.Context;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Models;
using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Repo;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CollabApp.mvc.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
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
            // Use HttpClient to call the AuthController's Login action in the API
            var apiClient = _httpClientFactory.CreateClient("Api");

            var loginDto = new
            {
                Username = username,
                Password = password
            };

            var response = await apiClient.PostAsJsonAsync("api/Auth/login", loginDto);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<User>(content);
                return user;
            }
            else
            {
                return null; // Invalid credentials or other API error
            }
        }
    }
}
