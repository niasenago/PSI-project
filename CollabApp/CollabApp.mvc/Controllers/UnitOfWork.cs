using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;

namespace CollabApp.mvc.Controllers
{
    public class UnitOfWork : IUnitOfWork
    {
        public IPostRepository postRepository {get; private set;}
        private readonly ApplicationDbContext dbContext;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            postRepository = new PostRepository(dbContext);
        }

        public async Task CompleteAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}