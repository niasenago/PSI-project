using CollabApp.mvc.Controllers;
using CollabApp.mvc.Models;
using CollabApp.mvc.Validation;
using CollabApp.mvc.Exceptions;
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

        public async Task<bool> SendMessage(string user, string message)
        {
            try {
                message.IsValidMessage();
                UserValidator.UserExists(user);
            }
            catch(ValidationException err)
            {
                await Clients.Caller.SendAsync(method:"ReceiveErrorMessage", "messageError", err.Message);
                return false;                
            }

            message = ProfanityHandler.CensorProfanities(message);

            string formattedDateTime = DateTime.Now.ToString(format:"g", provider:CultureInfo.CurrentCulture);

            // Instantiate the MessageController
            MessageController messageController = new MessageController(_db);

            // Call the AddMessage method
            Message newMessage = new Message { Sender = new User(user), Content = message}; // need to add check if user exists
            messageController.AddMessage(newMessage);

            await Clients.All.SendAsync(method:"ReceiveMessage", user, message, formattedDateTime);
        
            return true;
        } 
        public async Task<bool> AddToGroup(string groupName, string user)
        {
            try {
                groupName.IsValidGroupName();
            }
            catch(ValidationException err)
            {
                await Clients.Caller.SendAsync(method:"ReceiveErrorMessage", "groupError", err.Message);
                return false;                
            }

            string formattedDateTime = DateTime.Now.ToString("g", CultureInfo.CurrentCulture);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync(method:"ReceiveMessage",
                user, $"has joined the group {groupName}.", formattedDateTime);

            return true;
        }

        public async Task<bool> RemoveFromGroup(string groupName, string user)
        {
            try {
                groupName.IsValidGroupName();
            }
            catch(ValidationException err)
            {
                await Clients.Caller.SendAsync(method:"ReceiveErrorMessage", "groupError", err.Message);
                return false;                
            }

            string formattedDateTime = DateTime.Now.ToString(format: "g", provider: CultureInfo.CurrentCulture);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);

            await Clients.Group(groupName).SendAsync(method: "ReceiveMessage",
                user, $"has left the group {groupName}.", formattedDateTime);

            return true;
        }

        public async Task<bool> SendMessageGroup(string groupName, string user, string message)
        {
            try {
                groupName.IsValidGroupName();
            }
            catch(ValidationException err)
            {
                await Clients.Caller.SendAsync(method:"ReceiveErrorMessage", "groupError", err.Message);
                return false;                
            }

            try {
                message.IsValidMessage();
            }
            catch(ValidationException err)
            {
                await Clients.Caller.SendAsync(method:"ReceiveErrorMessage", "messageError", err.Message);
                return false;                
            }

            message = ProfanityHandler.CensorProfanities(message);

            string formattedDateTime = DateTime.Now.ToString( provider: CultureInfo.CurrentCulture, format: "g");

            // Instantiate the MessageController
            MessageController messageController = new MessageController(_db);

            // Call the AddMessage method
            Message newMessage = new Message { Sender = new User(user), Content = message, Group = groupName }; // need to add check if user exists
            messageController.AddMessage(newMessage);

            await Clients.Group(groupName).SendAsync(method: "ReceiveMessage", user, message, formattedDateTime);
        
            return true;
        }
    }
}
