using CollabApp.mvc.Utilities;
using System.ComponentModel.DataAnnotations;

namespace CollabApp.API.Models
{
    public class Comment : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }

        // Add a foreign key property
        public virtual User Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow;

        // the virtual keyword allows EF Core to automatically manage the relationship between Post and Comment
        public virtual Post Post { get; set; }

        public int PostId { get; set; }
        public int Rating { get; set; }

        public Comment(int authorId, string description, int postId)
        {
            // Initialize the ID when creating a new Comment object.
            this.Id = GenerateUniqueId();
            AuthorId = authorId; // Set the foreign key property
            Description = description;
            PostId = postId;
        }

        private int GenerateUniqueId()
        {
            return IdGenerator.GeneratePostId();
        }
    }
}
