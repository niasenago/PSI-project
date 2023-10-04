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
        
        public List<Post> FilterPosts(string searchTerm, DateTime? startDate, DateTime? endDate)
        {
            // Retrieve all posts from the repository.
            var allPosts = _postRepository.GetAllItems();
            var filteredPosts = allPosts.ToList(); // Create a new List<Post> from IEnumerable<Post>.

            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Filter by matching the search term in the Title or Description.
                filteredPosts = filteredPosts.Where(post =>
                    post.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    post.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                ).ToList(); // Create a new List<Post> from IEnumerable<Post>.
            }

            if (startDate.HasValue)
            {
                filteredPosts = filteredPosts.Where(post => post.DatePosted >= startDate.Value).ToList();
            }

            if (endDate.HasValue)
            {
                filteredPosts = filteredPosts.Where(post => post.DatePosted <= endDate.Value).ToList();
            }

            return filteredPosts;
        }

        public List<Post> SearchPostsByAuthor(string authorName)
        {
            var allPosts = _postRepository.GetAllItems();
            var filteredPosts = allPosts.ToList();
            filteredPosts = filteredPosts.Where(post => post.Author == authorName).ToList();
            return filteredPosts;
        }

    }
}