using CollabApp.mvc.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CollabApp.mvc.Models
{
    public class User : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string Salt { get; set; }

        public User()
        {
            this.Id = GenerateUniqueId();
        }

        public User(string username, string password)
        {
            // Initialize the ID when creating a new object.
            this.Id = GenerateUniqueId();
            if (username == null)
            {
                this.Username = "Anonymous";
            }
            else
                this.Username = username;

            // Hash and salt the password before storing it
            this.Salt = PasswordHasher.GenerateSalt();
            this.PasswordHash = PasswordHasher.HashPassword(password, this.Salt);
        }
        private int GenerateUniqueId()
        {
            return IdGenerator.GenerateUserId();
        }
    }
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

}