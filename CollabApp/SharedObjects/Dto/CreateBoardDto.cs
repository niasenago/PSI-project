using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace CollabApp.SharedObjects.Dto
{
    public class CreateBoardDto
    {
        [Required]
        public string BoardName { get; set; }
        public string BoardDescription { get; set; }
    }
}