"use strict";

// Call the loadMessages function when the page loads
document.addEventListener('DOMContentLoaded', function () {
    loadMessages();
});

// Function to load messages from the server
function loadMessages() {
    fetch('/MessageController/Messages')
        .then(response => response.json())
        .then(messages => {
            var messagesList = document.getElementById("messagesList");
            messages.forEach(message => {
                var li = document.createElement("li");
                li.textContent = `${message.sender}: ${message.content} (sent at ${message.sentAt})`;
                messagesList.appendChild(li);
            });
        })
        .catch(error => console.error(error.toString()));
}

function isValidMessage(str) {
    return str.trim().length > 0;
}

function isValidGroupName(str) {
    return str.trim().length > 0;
}

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;


connection.on("ReceiveMessage", function (user, message, sentAt) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user}: ${message} (sent at ${sentAt})`;
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var message = document.getElementById("messageInput").value;
            if(isValidMessage(message)) 
            {
                connection.invoke("SendMessage", username, message).catch(function (err) {
                    return console.error(err.toString());
                });
                document.getElementById("messageInput").value = "";
            }
        })
        .catch(error => console.error(error.toString()));

    event.preventDefault();
});

document.getElementById("enterButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var groupName = document.getElementById("groupInput").value;
            if(isValidGroupName(groupName)) 
            {
                connection.invoke("AddToGroup", groupName, username).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        })
        .catch(error => console.error(error.toString()));

    event.preventDefault();
});

document.getElementById("exitButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var groupName = document.getElementById("groupInput").value;
            if(isValidGroupName(groupName)) 
            {
                connection.invoke("RemoveFromGroup", groupName, username).catch(function (err) {
                    return console.error(err.toString());
                });
            }
        })
        .catch(error => console.error(error.toString()));
        
    event.preventDefault();
});

document.getElementById("sendGroupButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var groupName = document.getElementById("groupInput").value;
            var message = document.getElementById("messageInput").value;
            if(isValidGroupName(groupName) && isValidMessage(message)) 
            {
                connection.invoke("SendMessageGroup", groupName, username, message).catch(function (err) {
                    return console.error(err.toString());
                });
                document.getElementById("messageInput").value = "";
            }
        })
        .catch(error => console.error(error.toString()));
        
    event.preventDefault();
});