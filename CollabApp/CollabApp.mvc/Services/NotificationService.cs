using CollabApp.mvc.Controllers;
using CollabApp.mvc.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Diagnostics;

namespace CollabApp.mvc.Services
{
    public class NotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public void SubscribeToNewPostEvent(PostController postController)
        {
            postController.NewPostAdded += HandleNewPostAdded;
        }

        private void HandleNewPostAdded(object? sender, PostEventArgs e)
        {
            //Debug.WriteLine($"New post added: {e.AddedPost.Title}");
            _hubContext.Clients.All.SendAsync("ReceiveNotification", $"New post added: {e.AddedPost.Title}");
        }
    }
}
