using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Repo;

namespace CollabApp.mvc.Controllers
{
    public interface IUnitOfWork
    {
        IPostRepository postRepository {get;}
        Task CompleteAsync();
    }
}