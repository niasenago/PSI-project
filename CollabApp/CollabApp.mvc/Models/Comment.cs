using System.ComponentModel.DataAnnotations;

namespace CollabApp.mvc.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Text { get; set; }
        public string Author { get; set; }
        public DateTime DatePosted { get; set; }
    }
}
