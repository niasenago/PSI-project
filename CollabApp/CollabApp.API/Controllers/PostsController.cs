using CollabApp.API.Models;
using CollabApp.API.Repo;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollabApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PostsController(ILogger<PostsController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Post>>> GetPosts()
        {
            Console.WriteLine("API: GetPosts works");
            var posts = await _unitOfWork.PostRepository.GetAllAsync();
            return Ok(posts);
        }
    }
}
