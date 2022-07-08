// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $('[data-bs-toggle="popover"]').popover({
        placement: 'bottom',
        content: function () {
            return $("#notification-content").html();
        },
        html: true
    });

    $('body').append(`<div id="notification-content" class="d-none"></div>`);

    function getNotifications() {
        $('#notification-content').append('<ul class="list-group"></ul>');
        $.ajax({
            url: "/Notification/GetNotifications",
            method: "GET",
            success: function (result) {
                if (result.count != 0) {
                    $(".icon-button__badge").html(result.count);
                }
                else {
                    $(".icon-button__badge").html();
                    $(".icon-button__badge").hide('slow');
                }
                var notifications = result.userNotifications;
                notifications.forEach(element => {
                    $("ul.list-group").append($('<li>').text(element.notification.text).addClass('list-group-item').addClass('notification-text'));
                    $("li.notification-text").attr("id", element.notification.id.toString());
                });
            },
            error: function (error) {
                alert(error);
            }
        });
    }

    $(document).on('click', 'li.notification-text', function (e) {
        var target = e.target;
        var id = $(target).attr('id');
        readNotification(id, target);
    })

    function readNotification(id, target) {
        $.ajax({
            url: "/Notification/ReadNotification",
            method: "GET",
            data: { notificationId: id },
            success: function (result) {
                getNotifications();
                $(target).fadeOut('slow');
            },
            error: function (error) {
                console.log(error);
            }
        })
    }

    getNotifications();

});


