using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Repo
{
    public class PostRepository : GenericRepository<Post>,  IPostRepository
    {
        public PostRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public override Task<List<Post>> GetAllAsync()
        {
            return base.GetAllAsync();
        }

        public override async Task<Post> GetAsync(int id)
        {
            return await DbSet.FirstOrDefaultAsync(item => item.Id == id);
        }

        public override async Task<bool> AddEntity(Post entity)
        {
            try 
            {
                await DbSet.AddAsync(entity);
                return true;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public override async Task<bool> UpdateEntity(Post entity)
        {
            try
            {
                var existData = await DbSet.FirstOrDefaultAsync(item => item.Id == entity.Id);
                if(existData != null)
                {
                    existData.Id = entity.Id;
                    existData.Title = entity.Title;
                    entity.Description = entity.Description;
                    existData.Author = entity.Author;
                    existData.BoardId = entity.BoardId;
                    existData.SavedFileName = entity.SavedFileName;
                    existData.Photo = entity.Photo;
                    existData.SignedUrl = entity.SignedUrl;
                    existData.SavedUrl = entity.SavedUrl;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}