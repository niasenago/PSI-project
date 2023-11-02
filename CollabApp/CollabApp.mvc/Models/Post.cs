using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Emit;
using CollabApp.mvc.Controllers;
using CollabApp.mvc.Utilities;

namespace CollabApp.mvc.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Author { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow; 
        public virtual ICollection<Comment> Comments { get; set; } //relationship with comment class. One to many

        public Post()
        {
            this.Id = IdGenerator.GeneratePostId();
            Comments = new HashSet<Comment>(); //hashset List
        } 
    }
}
