using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;

namespace CollabApp.mvc.Repo
{
    //idk do we need IDisposable
    public class UnitOfWork : IUnitOfWork
    {
        public IPostRepository postRepository {get; private set;}
        public IBoardRepository boardRepository {get; private set;}
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            postRepository = new PostRepository(dbContext);
            boardRepository = new BoardRepository(dbContext);
            
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}