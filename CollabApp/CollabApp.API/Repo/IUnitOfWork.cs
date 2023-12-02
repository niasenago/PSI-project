
namespace CollabApp.API.Repo
{
    public interface IUnitOfWork
    {
        IPostRepository PostRepository {get;}
        IBoardRepository BoardRepository {get;}
        ICommentRepository CommentRepository {get;}
        IAttachmentRepository AttachmentRepository {get;}
        IUserRepository UserRepository {get;}
        Task CompleteAsync();
    }
}