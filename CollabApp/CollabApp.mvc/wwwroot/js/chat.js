"use strict";

// Call the loadMessages function when the page loads
document.addEventListener('DOMContentLoaded', function () {
    loadMessages();
});

var currentGroup = null; // Initialize to null, indicating user is not in any group initially

// Function to load messages from the server
function loadMessages() {
    fetch('/MessageController/Messages')
        .then(response => response.json())
        .then(messages => {
            var messagesList = document.getElementById("messagesList");

            messagesList.innerHTML = ""; // Clear the existing messages

            messages
                .filter(message => {
                    if (currentGroup) {
                        return message.group === currentGroup; // Filter messages by group if user is in a group
                    } else {
                        return !message.group; // Show messages not associated with any group if user is not in a group
                    }
                })
                .forEach(message => {
                    var li = document.createElement("li");
                    var formattedDateTime = formatDateTime(message.sentAt);
                    li.textContent = `${message.sender}: ${message.content} (sent at ${formattedDateTime})`;
                    messagesList.appendChild(li);
                });
        })
        .catch(error => console.error(error.toString()));
}

// temporary method to format date and time
function formatDateTime(dateString) {
    var date = new Date(dateString);
    var year = date.getFullYear();
    var month = String(date.getMonth() + 1).padStart(2, '0'); // Add leading zero if needed
    var day = String(date.getDate()).padStart(2, '0'); // Add leading zero if needed
    var hours = String(date.getHours()).padStart(2, '0'); // Add leading zero if needed
    var minutes = String(date.getMinutes()).padStart(2, '0'); // Add leading zero if needed

    return `${year}-${month}-${day} ${hours}:${minutes}`;
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

                // Set the currentGroup here
                currentGroup = groupName;

                // Load messages after setting the current group
                loadMessages();
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

                // Reset the currentGroup back to null
                currentGroup = null;

                // Load messages after resetting the current group
                loadMessages();
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