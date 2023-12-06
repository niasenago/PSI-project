using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace CollabApp.mvc.Dto
{
    public class CreateBoardDto
    {
        [Required]
        public string BoardName { get; set; }
    }
}