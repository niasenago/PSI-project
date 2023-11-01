using CollabApp.mvc.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CollabApp.mvc.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow; 

        public Comment(string author, string description)
        {
            // Initialize the ID when creating a new Post object.
            this.Id = GenerateUniqueId();
            Author = author;
            Description = description;

        }
        private int GenerateUniqueId()
        {
            return IdGenerator.GeneratePostId();
        }
    }
}
