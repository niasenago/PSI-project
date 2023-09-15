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
    }
}
