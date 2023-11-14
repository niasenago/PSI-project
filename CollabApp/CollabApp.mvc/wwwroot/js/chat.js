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

connection.on("ReceiveErrorMessage", function (type, errorMessage) {
    var errorBox = document.getElementById(type);
    errorBox.style.display = "block";
    errorBox.textContent = `${errorMessage}`;

    if(errorBox.timeoutId)
        clearTimeout(errorBox.timeoutId);

    errorBox.timeoutId = setTimeout(function () {
        errorBox.style.display = "none";
    }, 2000);
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

            connection.invoke("SendMessage", username, message).then(function (res) {
                if(res === true) {
                    document.getElementById("messageInput").value = "";
                }
            })
            .catch(error => console.err(error.toString()));
        })
        .catch(error => console.error(error.toString()));

    event.preventDefault();
});

document.getElementById("enterButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var groupName = document.getElementById("groupInput").value;

            connection.invoke("AddToGroup", groupName, username).then(function (res) {
                if(res === true) {
                    currentGroup = groupName; // Set the currentGroup here
                    loadMessages(); // Load messages after setting the current group
                }
            }).catch(error => console.err(error.toString()));
        })
        .catch(error => console.error(error.toString()));

    event.preventDefault();
});

document.getElementById("exitButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var groupName = document.getElementById("groupInput").value;

            connection.invoke("RemoveFromGroup", groupName, username).then(function (res) {
                if (res === true) {
                    currentGroup = null; // Reset the currentGroup back to null
                    loadMessages(); // Load messages after resetting the current group
                }

            }).catch(error => console.err(error.toString()));
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

            connection.invoke("SendMessageGroup", groupName, username, message).then(function (res) {
            if(res === true) {
                document.getElementById("messageInput").value = "";
            }
                
            }).catch(error => console.err(error.toString()));
        })
        .catch(error => console.error(error.toString()));
        
    event.preventDefault();
});