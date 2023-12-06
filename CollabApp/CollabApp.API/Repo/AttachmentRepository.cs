
using CollabApp.API.Models;
using CollabApp.API.Context;

namespace CollabApp.API.Repo
{
    public interface IAttachmentRepository : IGenericRepository<Attachment>
    {
    }
    public class AttachmentRepository : GenericRepository<Attachment>, IAttachmentRepository
    {
        //GetAllAsync, GetAsync,DeleteEntity, DeleteEntitiesByExpression  are implemented in GenericRepository
        public AttachmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public override async Task<bool> AddEntity(Attachment entity)
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
        public override async Task<bool> UpdateEntity(Attachment entity)
        {
            try
            {
                //TODO deep cloning methods (https://stackoverflow.com/questions/129389/how-do-you-do-a-deep-copy-of-an-object-in-net)
                var existData = await DbSet.FindAsync(entity.Id).AsTask();
                if(existData != null)
                {
                    existData.Id = entity.Id;
                    existData.FileName = entity.FileName;
                    existData.FileType = entity.FileType;
                    existData.Url = entity.Url;
                    existData.SignedUrl = entity.SignedUrl;

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