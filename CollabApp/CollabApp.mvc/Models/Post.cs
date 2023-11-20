
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollabApp.mvc.Utilities;

namespace CollabApp.mvc.Models
{
    public class Post : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string? Description { get; set; }
        public virtual User Author { get; set; }
        public int AuthorId { get; set; }
        public DateTime DatePosted { get; set; } = DateTime.UtcNow; 
        public virtual ICollection<Comment> Comments { get; set; } //relationship with comment class. 1 to many
        //it needs to be virtual because by using virtual navigation properties, EF Core can automatically manage relationships between entities .
        
        public virtual Board Board {get; set;}
        public int BoardId {get;set;}
        
        public virtual ICollection<Attachment> Attachments { get; set; }
        [NotMapped]
        public List<IFormFile> MediaFiles { get; set; }
        
        public Post()
        {
            Id = IdGenerator.GeneratePostId();
            Comments = new HashSet<Comment>();
            Attachments = new HashSet<Attachment>();
        } 
    }
}
