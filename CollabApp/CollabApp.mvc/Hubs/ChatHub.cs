﻿using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Services;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IDBAccess<Message> _db;

        public ChatHub(IDBAccess<Message> db)
        {
            _db = db;
        }

        public async Task SendMessage(string user, string message)
        {
            ValidationError error = message.IsValidMessage();

            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);

            // Instantiate the MessageController
            MessageController messageController = new MessageController(_db);

            // Call the AddMessage method
            Message newMessage = new Message { Sender = user, Content = message};
            messageController.AddMessage(newMessage);

            await Clients.All.SendAsync(method:"ReceiveMessage", user, message, formattedDateTime);
        } 
        public async Task AddToGroup(string groupName, string user)
        {
            ValidationError result = groupName.IsValidGroupName();
            
            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync(method:"ReceiveMessage",
                user, $"has joined the group {groupName}.", formattedDateTime);
        }

        public async Task RemoveFromGroup(string groupName, string user)
        {
            ValidationError result = groupName.IsValidGroupName();

            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync(method: "ReceiveMessage",
                user, $"has left the group {groupName}.", formattedDateTime);
        }

        public async Task SendMessageGroup(string groupName, string user, string message)
        {
            ValidationError result = groupName.IsValidGroupName();

            result = message.IsValidMessage();

            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);

            // Instantiate the MessageController
            MessageController messageController = new MessageController(_db);

            // Call the AddMessage method
            Message newMessage = new Message { Sender = user, Content = message, Group = groupName };
            messageController.AddMessage(newMessage);

            await Clients.Group(groupName).SendAsync(method: "ReceiveMessage", user, message, formattedDateTime);
        }
    }
}
