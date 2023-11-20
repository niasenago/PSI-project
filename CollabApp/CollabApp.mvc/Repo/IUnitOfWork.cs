
namespace CollabApp.mvc.Repo
{
    public interface IUnitOfWork
    {
        IPostRepository PostRepository {get;}
        IBoardRepository BoardRepository {get;}
        ICommentRepository CommentRepository {get;}
        IAttachmentRepository AttachmentRepository {get;}
        Task CompleteAsync();
    }
}