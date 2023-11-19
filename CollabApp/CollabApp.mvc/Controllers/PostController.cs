using CollabApp.mvc.Context;
using CollabApp.mvc.Repo;

﻿using System.Linq.Expressions;

using CollabApp.mvc.Models;
using CollabApp.mvc.Services;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollabApp.mvc.Controllers
{
    public class PostController : Controller
    {

        private readonly PostFilterService _postFilterService;
        private readonly ICloudStorageService _cloudStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly NotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;
        
        public event EventHandler<Post>? NewPostAdded;

        public PostController( PostFilterService postFilterService, IHttpContextAccessor httpContextAccessor, ICloudStorageService cloudStorageService, NotificationService notificationService, IUnitOfWork unitOfWork)
        {
            _postFilterService = postFilterService;
            _httpContextAccessor = httpContextAccessor;
            _cloudStorageService = cloudStorageService;
            _notificationService = notificationService;
            _notificationService.SubscribeToNewPostEvent(this);
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> PostsAsync(int? boardId) //get boardId from route
        {
            if (boardId == null)
            {
                //!CHANGE THIS
                boardId = 0;
                // Handle the case when no board is selected
                //return RedirectToAction("Index");
            }
            var posts = await _unitOfWork.postRepository.GetAllAsync();

            posts = posts
                 .Where(p => p.BoardId == boardId)
                 .ToList();

            return View(posts);
        }

        public async Task<IActionResult> PostViewAsync(int Id)
        {
            //var post = _context.Posts.FirstOrDefault(p => p.Id == Id);
            var post = await _unitOfWork.postRepository.GetAsync(Id);
            if (post == null)
            {
                return NotFound();
            }
            var comments = new List<Comment>();
            comments = await _unitOfWork.commentRepository.GetAllAsync();

            comments = comments
                .Where(item => item.PostId == Id)
                .ToList();

            ViewData["Comments"] = comments;

            await GenerateSignedUrl(post);   
                     

            return View(post);
        }

        private async Task GenerateSignedUrl(Post post)
        {
            //Get signed URL only when Saved file Name is available
            if(!string.IsNullOrEmpty(post.SavedFileName))
            {
                post.SignedUrl = await _cloudStorageService.GetSignedUrlAsync(post.SavedFileName);
                post.fileType = await _cloudStorageService.GetFileType(post.SavedFileName);
            }
        }
        /*
        The DisplayForm method is used for displaying the form to create a new post
        */

        [HttpPost]
        public IActionResult DisplayForm([FromQuery]int? boardId)
        {
            var post = new Post();
            if (boardId == null)
            {
                //!CHANGE THIS
                boardId = 0;
                Console.WriteLine("In DisplayForm method boardId value null");
                // Handle the case when no board is selected
                // return RedirectToAction("Index");
            }
            post.BoardId = (int)boardId;
            Console.WriteLine("In DisplayForm method boardId value" + post.BoardId);
            return View("Index", post); // Return the Index view with the Post model
        }
            
        /*
        The Index method (POST) is used for handling the submission of the form, 
        processing the form data, and creating a new post.
        */

        [HttpPost]
        public async Task<IActionResult> Index([Bind("Author, BoardId, Title, Description, Photo, SavedUrl, SavedFileName")]  Post post) //add post
        {

            try {
                UserValidator.UserExists(post.Author);
                post.Title.IsValidTitle();
                post.Description.IsValidDescription();
            }
            catch(ValidationException err)

            {
                ViewBag.ErrorMessage = err.Message;
                return View();
            }
            catch(InvalidUserException err)
            {
                ViewBag.ErrorMessage = err.Message;
                return RedirectToAction("Login", "Login");
            }

            if(post.Photo != null)
            {
                post.SavedFileName = GenerateFileNameToSave(post.Photo.FileName);
                post.SavedUrl = await _cloudStorageService.UploadFileAsync(post.Photo, post.SavedFileName);
            }

            post.Description = ProfanityHandler.CensorProfanities(post.Description);

            var data = await _unitOfWork.postRepository.AddEntity(post);
            await _unitOfWork.CompleteAsync();
            
            OnNewPostAdded(post);
            
            return RedirectToAction("Posts", new { boardId = post.BoardId});
        }

        private string? GenerateFileNameToSave(string incomingFileName)
        {
            var fileName = Path.GetFileNameWithoutExtension(incomingFileName);
            var extension = Path.GetExtension(incomingFileName);
            return $"{fileName}-{DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss")}{extension}";
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int Id, string Author, string commentDescription)
        {
            var post = await _unitOfWork.postRepository.GetAsync(Id);

            if (post == null)
            {
                // Handle the case where the post with the given ID is not found.
                return NotFound();
            }

            try {
                commentDescription.IsValidDescription();
                UserValidator.UserExists(Author);
            }
            catch(ValidationException err) 
            {
                ViewBag.ErrorMessage = err.Message;
                return RedirectToAction("PostView", post);    
            }
            catch(InvalidUserException err)
            {
                ViewBag.ErrorMessage = err.Message;
                return RedirectToAction("Login", "Login");
            }

            commentDescription = ProfanityHandler.CensorProfanities(commentDescription);
            //if everything is alright
            var comment = new Comment(Author, commentDescription, Id);

            var data = await _unitOfWork.commentRepository.AddEntity(comment);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction("PostView", new { id = Id }); // Redirect to the post view page.
        }
        
        [HttpPost]
        public IActionResult FilterPosts(string searchTerm = "", string authorName = "", DateTime from = default, DateTime to = default)
        {
            var filteredPosts = _postFilterService.FilterPosts(searchTerm, authorName, from, to);
            return View("Posts", filteredPosts);
        }
        [HttpPost]
        public async Task<IActionResult> SortPosts(SortingOption sortBy)
        {
           
            var allPosts = await _unitOfWork.postRepository.GetAllAsync();
            var sortedPosts = allPosts;

            switch (sortBy)
            {
                case SortingOption.DescComments:
                    sortedPosts.Sort(new CompareOnlyCommentAmount());
                    sortedPosts.Reverse();
                    break;
                case SortingOption.AscComments:
                    sortedPosts.Sort(new CompareOnlyCommentAmount());
                    break;
                case SortingOption.DescDate:
                    sortedPosts.Sort(new CompareOnlyDates());
                    sortedPosts.Reverse();
                    break;
                case SortingOption.AscDate:
                    sortedPosts.Sort(new CompareOnlyDates());
                    break;
            }
            return View("Posts", sortedPosts);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRating(int postId, int commentId, RatingOption rating)
        {
            var post = await _unitOfWork.postRepository.GetAsync(postId);
            if(null == post)
            {
                return NotFound();
            }
            var comment = await _unitOfWork.commentRepository.GetAsync(commentId);
            if(null == comment)
            {
                return NotFound();
            }

            comment.Rating += (rating == RatingOption.Upvote) ? 1 : -1;
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("PostView", post);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _unitOfWork.postRepository.GetAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("Username");

            if (currentUser != post.Author)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this post.";
                return RedirectToAction("PostView", new { id });
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Post updatedPost)
        {
            //var existingPost = _context.Posts.FirstOrDefault(p => p.Id == id);
            var existingPost = await _unitOfWork.postRepository.GetAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("Username");

            if (currentUser != existingPost.Author)
            {
                TempData["ErrorMessage"] = "You are not authorized to edit this post.";
                return RedirectToAction("PostView", new { id });
            }

            try {
                updatedPost.Title.IsValidTitle();
            }
            catch(ValidationException err) 
            {
                ViewBag.ErrorMessage = err.Message;
                return View("Edit", existingPost);   
            }

            existingPost.Title = updatedPost.Title;
            existingPost.Description = updatedPost.Description;

            //_context.SaveChanges();
            await _unitOfWork.CompleteAsync();


            return RedirectToAction("PostView", new { id });
        }
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var post = await _unitOfWork.postRepository.GetAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("Username");

            if (currentUser != post.Author)
            {
                TempData["ErrorMessage"] = "You are not authorized to delete this post.";
                return RedirectToAction("PostView", new { id });
            }

            //remove all comments associated with the post before deletion

            //_context.Comments.RemoveRange(_context.Comments.Where(c => c.PostId == id));
            //_context.Posts.Remove(post);
            //_context.SaveChanges();
            await _unitOfWork.commentRepository.DeleteEntitiesByExpression(c => c.PostId == id);
            await _unitOfWork.postRepository.DeleteEntity(post);
            await _unitOfWork.CompleteAsync();
            

            return RedirectToAction("Posts");
        }

        protected virtual void OnNewPostAdded(Post post)
        {
            NewPostAdded?.Invoke(this, post);
        }
    }
}
