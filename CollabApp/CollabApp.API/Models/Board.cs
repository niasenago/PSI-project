using System.ComponentModel.DataAnnotations;

namespace CollabApp.API.Models
{
    public class Board : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string BoardName { get; set; }
        public DateTime CreationDateTime { get; set; } = DateTime.UtcNow; 
        public virtual ICollection<Post> Posts { get; set; }
    }
}