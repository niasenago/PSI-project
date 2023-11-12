var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.on("ReceiveNotification", function (message) {
    showNotification(message, "info");
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function showNotification(message, type) {
    var notificationContainer = document.getElementById("notification-container");
    var notificationElement = document.createElement("div");
    notificationElement.classList.add("notification", type);

    // Create close button
    var closeButton = document.createElement("span");
    closeButton.classList.add("close");
    closeButton.textContent = "×";
    closeButton.onclick = function () {
        notificationElement.remove();
    };
    notificationElement.appendChild(closeButton);

    // Optional: Add icon here if needed

    // Add message text
    var textElement = document.createElement("span");
    textElement.textContent = message;
    notificationElement.appendChild(textElement);

    notificationContainer.appendChild(notificationElement);

    // Automatically remove the notification after a certain duration (e.g., 5 seconds)
    setTimeout(function () {
        notificationElement.remove();
    }, 5000);
}
