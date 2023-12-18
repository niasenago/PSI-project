using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;

namespace CollabApp.SharedObjects.Dto
{
    public class PostDto
    {
        [Required]
        public string PostTitle { get; set; }
        [Required]
        public int AuthorId { get; set; }
        [Required]
        public int BoardId { get; set; }

        public string? Description { get; set; }
    }
}