using CollabApp.API.Dto;
using CollabApp.API.Exceptions;
using CollabApp.API.Models;
using CollabApp.API.Repo;
using CollabApp.API.Validation;
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

        [HttpPost]
        public async Task<ActionResult<Post>> CreatePost([FromBody] PostDto postDto)
        {
            Console.WriteLine("API: CreatePost works");
            try
            {
                postDto.PostTitle.IsValidTitle();
            }
            catch (ValidationException err)
            {
                return BadRequest(err.Message);
            }

            var post = new Post { Title = postDto.PostTitle, AuthorId = postDto.AuthorId, BoardId = postDto.BoardId};
            bool isAdded = await _unitOfWork.PostRepository.AddEntity(post);
            await _unitOfWork.CompleteAsync();

            if (isAdded)
            {
                Post createdPost = await _unitOfWork.PostRepository.GetAsync(post.Id);

                return CreatedAtAction(nameof(GetPost), new { id = createdPost.Id }, createdPost);
            }
            else
            {
                return BadRequest("Post could not be created");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Post>> GetPost(int id)
        {
            Console.WriteLine("API: GetPost works");
            var post = await _unitOfWork.PostRepository.GetAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePost(int id, PostDto postDto)
        {
            Console.WriteLine("API: UpdatePost works");
            try
            {
                postDto.PostTitle.IsValidTitle();
            }
            catch (ValidationException err)
            {
                return BadRequest(err.Message);
            }

            var post = await _unitOfWork.PostRepository.GetAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            post.Title = postDto.PostTitle;
            /*post.AuthorId = postDto.AuthorId;
            post.BoardId = postDto.BoardId;
            Depends on how we want to handle this, if we want to allow the user to change the author or board of a post
             */ 
            post.Description = postDto.Description;

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
    }
}
