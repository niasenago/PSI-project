
using CollabApp.mvc.Context;
using CollabApp.mvc.Models;
using CollabApp.mvc.Repo;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Services
{
    public class PostFilterService
    {
        public PostFilterService()
        {
            
        }
        private readonly IUnitOfWork _unitOfWork;

        public PostFilterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        public async Task<List<Post>> FilterPostsAsync(string searchTerm, string authorName, DateTime? startDate, DateTime? endDate, int boardId, bool includePosts, bool includeQuestions)
        {


            // Retrieve all posts from the repository.
            var filteredPosts = await _unitOfWork.PostRepository.GetAllAsync();
            filteredPosts = filteredPosts.Where(p => p.BoardId == boardId).ToList();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                filteredPosts = filteredPosts.Where(post =>
                    post.Title.ToLower().Contains(searchTerm) ||
                    post.Description.ToLower().Contains(searchTerm)
                ).ToList();
            }
            if (!string.IsNullOrEmpty(authorName))
            {
                var authorNameToLower = authorName.ToLower();
                var postsList = new List<Post>(); 
                foreach(var post in filteredPosts)
                {
                    var author = await _unitOfWork.UserRepository.GetAsync(post.AuthorId);
                    if (author != null && author.Username.ToLower() == authorNameToLower)
                    {
                        postsList.Add(post);
                    }
                }
                filteredPosts = postsList;
            }

            if (startDate.HasValue)
            {   
                filteredPosts = filteredPosts.Where(post => post.DatePosted >= startDate).ToList();
            }

            if (endDate.HasValue)
            {
                filteredPosts = filteredPosts.Where(post => post.DatePosted <= endDate).ToList();
            }


            if (includePosts && !includeQuestions)
            {
                filteredPosts = filteredPosts.Where(post => !post.IsQuestion).ToList();
            }
            else if (includeQuestions && !includePosts)
            {
                filteredPosts = filteredPosts.Where(post => post.IsQuestion).ToList();
            }
            // Else, include both posts and questions.

            return filteredPosts;
        }

        /*TODO: public List<Post> GetPopularPosts(List<Post> posts, int minAmountOfComments)*/
    }
}