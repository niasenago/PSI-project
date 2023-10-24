using System.Linq;
using CollabApp.mvc.Models;

namespace CollabApp.mvc.Services
{
    //logic for filtering posts
    public class PostFilterService
    {
        private readonly IDBAccess<Post> _postRepository;

        public PostFilterService(IDBAccess<Post> postRepository)
        {
            _postRepository = postRepository;
        }       
        
        public List<Post> FilterPosts(string searchTerm, string authorName, DateTime? startDate, DateTime? endDate)
        {


            // Retrieve all posts from the repository.
            var allPosts = _postRepository.GetAllItems();
            var filteredPosts = allPosts;
            
            if (!string.IsNullOrEmpty(searchTerm))
            {   
                // Filter by matching the search term in the Title or Description.
                filteredPosts = filteredPosts.Where(post =>
                    post.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    post.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList(); 
                //LINQ to Objects
            }
            if(!string.IsNullOrEmpty(authorName))
            {
                filteredPosts = filteredPosts.Where(post => post.Author?.Username == authorName).ToList();
                //LINQ to Objects
            }

            if (startDate != DateTime.MinValue)
            {
                filteredPosts = filteredPosts.Where(post => post.DatePosted >= startDate.Value).ToList();
                //LINQ to Objects
            }

            if (endDate != DateTime.MinValue)
            {
                filteredPosts = filteredPosts.Where(post => post.DatePosted <= endDate.Value).ToList();
                //LINQ to Objects
            }
            return filteredPosts;
        }

        /*TODO: public List<Post> GetPopularPosts(List<Post> posts, int minAmountOfComments)*/
    }
}