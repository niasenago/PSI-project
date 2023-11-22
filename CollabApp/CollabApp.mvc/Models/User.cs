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

        public User()
        {
            this.Id = GenerateUniqueId();
        }

        public User(string username)
        {
            // Initialize the ID when creating a new object.
            this.Id = GenerateUniqueId();
            if (username == null)
            {
                this.Username = "Anonymous";
            }
            else
                this.Username = username;
        }
        private int GenerateUniqueId()
        {
            return IdGenerator.GenerateUserId();
        }
    }
}