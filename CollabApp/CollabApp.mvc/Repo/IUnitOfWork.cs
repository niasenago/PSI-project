using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Repo;

namespace CollabApp.mvc.Repo
{
    public interface IUnitOfWork
    {
        IPostRepository postRepository {get;}
        IBoardRepository boardRepository {get;}
        Task CompleteAsync();
    }
}