// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    var res; // li notification items stored here !
    var notificationsLink = '<a href="/Notification/GetAllUserNotifications" id="allNotificationsLink" class="btn btn-link mt-0"> See All </a>';
    var finalResult;

    var setPopoverContent = () => {
        finalResult = '';
        finalResult += notificationsLink;
        finalResult += '<ul class="list-group notification-list">';
        finalResult += res;
        finalResult += '</ul>';
        return finalResult;
    };

    $('[data-bs-toggle="popover"]').popover({
        placement: 'bottom',
        content: function () {
            return setPopoverContent();
        },
        html: true
    }).on('shown.bs.popover', function () {
        var contentEl = $(".popover-body");
        contentEl.html(setPopoverContent());
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


    function getNotifications() {

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
                res = '';
                notifications.forEach(element => {
                    var id = element.notification.id;
                    var temp = $('<div>', { id: 'tempLi' });
                    $('<li>', {
                        id: id
                    }).text(element.notification.text).addClass('list-group-item').addClass('notification-text').appendTo(temp);
                    res = res + temp.html();
                })
                var contentEl = $(".popover-body");
                contentEl.html(setPopoverContent());
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


