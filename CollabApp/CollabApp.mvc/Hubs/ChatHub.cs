using Microsoft.AspNetCore.SignalR;
using System;
using System.Globalization;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            await Clients.All.SendAsync("ReceiveMessage", user, message, formattedDateTime);
        } 
        public async Task AddToGroup(string groupName, string user)
        {
            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage",
                user, $"has joined the group {groupName}.", formattedDateTime);
        }

        public async Task RemoveFromGroup(string groupName, string user)
        {
            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync("ReceiveMessage",
                user, $"has left the group {groupName}.", formattedDateTime);
        }

        public async Task SendMessageGroup(string groupName, string user, string message)
        {
            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message, formattedDateTime);
        }
    }
}
