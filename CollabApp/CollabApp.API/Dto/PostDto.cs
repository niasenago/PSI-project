using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace CollabApp.API.Dto
{
    public class PostDto
    {
        [Required]
        public string PostTitle { get; set; }
        public int AuthorId { get; set; }
        public int BoardId { get; set; }
    }
}