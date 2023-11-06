using CollabApp.mvc.Controllers;
using System.Diagnostics;

namespace CollabApp.mvc.Services
{
    public class NotificationService
    {
        public void SubscribeToNewPostEvent(PostController postController)
        {
            postController.NewPostAdded += HandleNewPostAdded;
        }

        private void HandleNewPostAdded(object sender, PostEventArgs e)
        {
            Debug.WriteLine($"New post added: {e.AddedPost.Title}");
        }
    }
}
