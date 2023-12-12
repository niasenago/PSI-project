using CollabApp.mvc.Exceptions;
using CollabApp.mvc.Models;
using CollabApp.API.Dto;
using Microsoft.EntityFrameworkCore;
using CollabApp.mvc.Utilities;

using CollabApp.mvc.Repo;
using CollabApp.mvc.Validation;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await ValidateCredentialsAsync(loginDto.Username, loginDto.Password);
                if (user == null)
                {
                    return BadRequest("Invalid username or password.");
                }

                return Ok(user);
            }
            catch (ValidationException err)
            {
                return BadRequest(err.Message);
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
    }
}