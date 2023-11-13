using CollabApp.mvc.Utilities;
using Microsoft.EntityFrameworkCore.Storage;
using System.ComponentModel.DataAnnotations;

namespace CollabApp.mvc.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }

        public User Author { get; set; }
        // public DateTime DatePosted { get; set; } = DateTime.Now;

        // public Comment(User author, string description)
        // public string Author { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow; 
        // the virtual keyword allows EF Core to automatically manage the relationship between Post and Comment
        public virtual Post Post {get; set;}
        public int PostId {get;set;}
        public int Rating { get; set; }

        public Comment(User author, string description, int postId)

        {
            // Initialize the ID when creating a new Post object.
            this.Id = GenerateUniqueId();
            Author = author;
            Description = description;
            PostId = postId;

        }
        private int GenerateUniqueId()
        {
            return IdGenerator.GeneratePostId();
        }
    }
}
