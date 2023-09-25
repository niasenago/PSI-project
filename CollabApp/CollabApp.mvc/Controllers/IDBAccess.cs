using CollabApp.mvc.Models;


namespace CollabApp.mvc.Controllers {

    public interface IDBAccess
    {
        void AddPost(Post post);
        Post GetPostById(int id);
        List<Post> GetAllPosts();
    }    
}    