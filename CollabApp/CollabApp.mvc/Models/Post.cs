using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public virtual ICollection<Comment> Comments { get; set; } //relationship with comment class. 1 to many
        //it needs to be virtual because by using virtual navigation properties, EF Core can automatically manage relationships between entities .
        
        public virtual Board Board {get; set;}
        public int BoardId {get;set;}


        [NotMapped]
        public IFormFile? Photo { get; set; }
        [NotMapped]
        public string? SignedUrl { get; set; }
        [NotMapped]
        public string? fileType { get; set; }
        public string? SavedFileName { get; set; }
        public string? SavedUrl {get; set;}
        
        public Post()
        {
            this.Id = IdGenerator.GeneratePostId();
            Comments = new HashSet<Comment>(); //hashset List
        } 
    }
}
