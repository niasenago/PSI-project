using System.Collections;
using System.ComponentModel.DataAnnotations;
using CollabApp.mvc.Utilities;

namespace CollabApp.mvc.Models
{
    public class Post 
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;

        public Post()
        {
            // Initialize the ID when creating a new Post object.
            this.Id = GenerateUniqueId();
        }
        private int GenerateUniqueId()
        {
            return PostIdGenerator.GenerateRandomId();
        }
            
    }
}
