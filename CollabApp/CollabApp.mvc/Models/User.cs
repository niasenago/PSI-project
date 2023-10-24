using CollabApp.mvc.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CollabApp.mvc.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Username { get; set; }

        public User()
        {
            this.Id = GenerateUniqueId();
        }

        public User(string? username)
        {
            // Initialize the ID when creating a new object.
            this.Id = GenerateUniqueId();
            Username = username;

        }
        private int GenerateUniqueId()
        {
            return IdGenerator.GenerateUserId();
        }
    }
}