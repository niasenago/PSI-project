using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;

namespace CollabApp.mvc.Repo
{
    public class PostRepository : GenericRepository<Post>,  IPostRepository
    {
        public PostRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public Task<bool> AddEntity(Post entity)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Post>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

        public Task<List<Post>> GetAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateEntity(Post entity)
        {
            throw new NotImplementedException();
        }
    }
}