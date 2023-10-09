using System.Collections;

namespace CollabApp.mvc.Models
{
    public class PostCollection : IEnumerable<Post> 
    {
        private List<Post> posts = new List<Post>();

        // Implement the GetEnumerator() method from IEnumerable<Post>
        public IEnumerator<Post> GetEnumerator()
        {
            return posts.GetEnumerator();
        }

        // Implement the non-generic IEnumerable.GetEnumerator() method
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void AddPost(Post post)
        {
            posts.Add(post);
        }

        public void RemovePost(Post post)
        {
            posts.Remove(post);
        }

        public int CountPosts()
        {
            return posts.Count;
        } 
    }
}