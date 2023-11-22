using CollabApp.mvc.Controllers;
using CollabApp.mvc.Hubs;
using CollabApp.mvc.Models;
using Microsoft.AspNetCore.SignalR;

namespace CollabApp.mvc.Services
{
    public class NotificationService
    {
        public NotificationService()
        {
            
        }
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void SubscribeToNewPostEvent(PostController postController)
        {
            postController.NewPostAdded += HandleNewPostAdded;
        }

        private void HandleNewPostAdded(object? sender, Post post)
        {
            // FIXME: send notification to all users except the sender
            _hubContext.Clients.All.SendAsync("ReceiveNotification", $"New post added: {post.Title}");
        }
    }
}
