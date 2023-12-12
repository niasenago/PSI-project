
using CollabApp.mvc.Models;
using CollabApp.mvc.Context;

namespace CollabApp.mvc.Repo
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