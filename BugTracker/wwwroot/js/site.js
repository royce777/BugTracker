// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
    function getNotifications() {
        $.ajax({
            url: "/Notification/GetNotifications",
            method: "GET",
            success: function (result) {
                $(".icon-button__badge").html(result.count);
                console.log(result);
            },
            error: function (error) {
                alert(error);
            }
        });
    }
