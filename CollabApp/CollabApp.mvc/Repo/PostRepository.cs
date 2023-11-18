using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Repo
{
    public interface IPostRepository : IGenericRepository<Post>
    {
    }
    public class PostRepository : GenericRepository<Post>,  IPostRepository
    {
        public PostRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        // GetAllAsync GetAsync are implemented in GenericRepository

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
                //TODO deep cloning methods (https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net)
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