using System.Linq;
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Services
{
    //logic for filtering posts
    public class PostFilterService
    {
        private readonly ApplicationDbContext _context;

        public PostFilterService(ApplicationDbContext context)
        {
            _context = context;
        }    
        
        public List<Post> FilterPosts(string searchTerm, string authorName, DateTime? startDate, DateTime? endDate)
        {


            // Retrieve all posts from the repository.
            var allPosts = _context.Posts;
            //var filteredPosts = allPosts;
            IQueryable<Post> filteredPosts = _context.Posts;
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = $"%{searchTerm}%"; // Add wildcards for 'LIKE' comparison
                filteredPosts = filteredPosts.Where(post =>
                    EF.Functions.Like(post.Title, searchTerm) || EF.Functions.Like(post.Description, searchTerm)
                );
            }
            if (!string.IsNullOrEmpty(authorName))
            {
                filteredPosts = filteredPosts.Where(post => post.Author == authorName);
            }

            if (startDate != DateTime.MinValue)
            {   
                filteredPosts = filteredPosts.Where(post => post.DatePosted >= startDate);
            }

            if (endDate != DateTime.MinValue)
            {
                filteredPosts = filteredPosts.Where(post => post.DatePosted <= endDate);
            }
            return filteredPosts.ToList();
        }

        /*TODO: public List<Post> GetPopularPosts(List<Post> posts, int minAmountOfComments)*/
    }
}