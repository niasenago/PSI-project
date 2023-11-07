// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

connection.on("ReceiveNotification", function (message) {
    // Handle the notification, e.g., display a toast or update UI
    showToast(message);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function showToast(message) {
    alert(message);
}
