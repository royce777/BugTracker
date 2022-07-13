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
    
    //dismiss popover on click outside of it !
    $('html').on('click', function (e) {
        var target;
        if ($(e.target).is('button')) {
            target = e.target;
        }
        else {
            target = e.target.parentElement;
        }
        if (typeof $(target).data('bs-original-title') == 'undefined' &&
            !($(target).parents().is('.popover') || $(target).hasClass('popover'))) {
            $('[data-bs-toggle="popover"]').popover('hide');
        }
    });

    $('body').append(`<div id="notification-content" class="d-none">
                        <a href="/Notification/GetAllUserNotifications" id="allNotificationsLink" class="btn btn-link mt-0"> See All </a>
                    </div>`);

    function getNotifications() {
        if ($('.notification-list').length === 0) {
            $('#notification-content').append('<ul class="list-group notification-list"></ul>');
        }
        $.ajax({
            url: "/Notification/GetNotifications",
            method: "GET",
            success: function (result) {
                if (result.count != 0) {
                    if ($('.icon-button__badge').length === 0) {
                        $('#notificationButton').append('<span class="icon-button__badge"></span>');
                    }
                    else {
                        $(".icon-button__badge").show('slow');
                    }
                    $(".icon-button__badge").html(result.count);
                }
                else {
                    $(".icon-button__badge").html();
                    $(".icon-button__badge").hide('slow');
                }
                var notifications = result.userNotifications;
                notifications.forEach(element => {
                    var id = element.notification.id.toString();
                    if ($('ul.list-group li').length > 0) {
                        if ($('ul.list-group li:last-child').attr('id') < id) {
                            $('<li>', {
                                id: id
                            }).text(element.notification.text).addClass('list-group-item').addClass('notification-text').appendTo('ul.list-group');
                        }
                    }
                    else {
                        $('<li>', {
                            id: id
                        }).text(element.notification.text).addClass('list-group-item').addClass('notification-text').appendTo('ul.list-group');

                    }
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
                $(target).remove();
            },
            error: function (error) {
                console.log(error);
            }
        })
    }

    getNotifications();

    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

    connection.on('getNotifications', () => {
        getNotifications();
    });
    connection.start();

});


