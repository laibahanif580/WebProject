// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


$(document).ready(function() {

    // Use event delegation in case forms are dynamic
    $(document).on('submit', '.make-editor-form', function(e) {
        e.preventDefault(); // stop normal form submission

        var form = $(this);
        var userId = form.data('user-id');
        var token = form.find('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: '/Admin/MakeEditor',  // absolute path
            method: 'POST',
            data: {
                id: userId,
                __RequestVerificationToken: token
            },
            success: function(response) {
                if (response.success) {
                    // Replace button with Editor badge
                    form.replaceWith('<span class="badge bg-success editor-badge" data-user-id="' + userId + '">Editor</span>');
                } else {
                    alert(response.message);
                }
            },
            error: function() {
                alert('Error upgrading user to editor.');
            }
        });

    });

});


// The SignalR client script should be loaded first
// <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>

$(function() {
    var $announcementList = $("#announcementList");

    // Connect to the Hub
    var connection = new signalR.HubConnectionBuilder()
        .withUrl("/announcementhub") // endpoint defined in Program.cs
        .build();

    // Listen for new announcements
    connection.on("ReceiveAnnouncement", function(announcement) {
        var $div = $("<div>")
            .addClass("alert alert-info mb-3")
            .html(
                "<strong>" + announcement.title + "</strong><br>" +
                announcement.message + "<br>" +
                "<small class='text-muted'>Posted on: " + new Date(announcement.createdAt).toLocaleString() + "</small>"
            );

        // Add new announcement at the top
        $announcementList.prepend($div);
    });

    // Start the connection
    connection.start()
        .then(function() { console.log("Connected to AnnouncementHub"); })
        .catch(function(err) { console.error(err); });
});
