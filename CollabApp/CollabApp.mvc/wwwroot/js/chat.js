"use strict";

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
            connection.invoke("SendMessage", username, message).catch(function (err) {
                return console.error(err.toString());
            });
        })
        .catch(error => console.error(error.toString()));

    event.preventDefault();
    document.getElementById("messageInput").value = "";
});

document.getElementById("enterButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var groupName = document.getElementById("groupInput").value;
            connection.invoke("AddToGroup", groupName, username).catch(function (err) {
                return console.error(err.toString());
            });
        })
        .catch(error => console.error(error.toString()));

    event.preventDefault();
});

document.getElementById("exitButton").addEventListener("click", function (event) {
    fetch('/LoginController/GetUsername')
        .then(response => response.text())
        .then(username => {
            var groupName = document.getElementById("groupInput").value;
            connection.invoke("RemoveFromGroup", groupName, username).catch(function (err) {
                return console.error(err.toString());
            });
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
        
            connection.invoke("SendMessageGroup", groupName, username, message).catch(function (err) {
                return console.error(err.toString());
            });
        })
        .catch(error => console.error(error.toString()));
        
    event.preventDefault();
    document.getElementById("messageInput").value = "";
});