using System.ComponentModel.DataAnnotations;

namespace CollabApp.mvc.Models
{
    public class Board
    {
        [Key]
        public int BoardId { get; set; }
        
        [Required]

        public string BoardName { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.UtcNow; 


    }
}