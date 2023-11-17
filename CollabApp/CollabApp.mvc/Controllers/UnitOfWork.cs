using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;

namespace CollabApp.mvc.Controllers
{
    //idk yet whe we need IDisposable
    public class UnitOfWork : IUnitOfWork, IDisposable
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

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            Console.WriteLine(" dispose works");
            if (!this.disposed)
            {
                if (disposing)
                {
                    dbContext.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Console.WriteLine("empty dispose works");
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}