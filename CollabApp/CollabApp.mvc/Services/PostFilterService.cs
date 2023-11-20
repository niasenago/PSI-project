
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Services
{
    public class PostFilterService
    {
        private readonly ApplicationDbContext _context;

        public PostFilterService(ApplicationDbContext context)
        {
            _context = context;
        }    
        
        public List<Post> FilterPosts(string searchTerm, string authorName, DateTime? startDate, DateTime? endDate, int boardId)
        {


            // Retrieve all posts from the repository.
            var allPosts = _context.Posts.Where(p => p.BoardId == boardId).ToList();
            IQueryable<Post> filteredPosts = _context.Posts.Where(p => p.BoardId == boardId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = $"%{searchTerm}%"; // Add wildcards for 'LIKE' comparison
                filteredPosts = filteredPosts.Where(post =>
                    EF.Functions.Like(post.Title, searchTerm) || EF.Functions.ILike(post.Description, searchTerm)
                );
            }
            if (!string.IsNullOrEmpty(authorName))
            {
                //filteredPosts = filteredPosts.Where(post => post.Author == authorName);
                filteredPosts = filteredPosts.Where(post => post.Author.Username == authorName);
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