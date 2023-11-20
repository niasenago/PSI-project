
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CollabApp.mvc.Utilities;

namespace CollabApp.mvc.Models
{
    public class Attachment : IBaseEntity
    {
        [Key]
        public int Id { get; set; }
        [NotMapped]
        public string? SignedUrl { get; set; }
        [NotMapped]
        public string? FileType { get; set; }
        public string? FileName { get; set; }
        public string? Url { get; set; }
        public virtual Post Post { get; set; }
        public int PostId { get; set; }

        public Attachment(string fileName, string url, int postId)
        {
            Id = IdGenerator.GeneratePostId();
            FileName = fileName;
            Url = url;
            PostId = postId;
        }
    }
}