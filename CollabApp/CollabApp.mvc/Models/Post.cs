using System.ComponentModel.DataAnnotations;

namespace CollabApp.mvc.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Author { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.Now;

    }
}
