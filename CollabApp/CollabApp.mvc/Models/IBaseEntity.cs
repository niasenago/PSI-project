using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CollabApp.mvc.Models
{
    public interface IBaseEntity
    {
        int Id { get; set; }
    }
}