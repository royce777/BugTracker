$((function () {
    var url;
    var redirectUrl;
    var target;
    var itemName;

    $('body').append(`<div class="modal" id="deleteModal">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title">Warning</h5>
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close">
                                    <span aria-hidden="true"></span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <p class="delete-modal-body"></p>
                                <p>This action is irreversible!</p>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-danger" id="confirm-delete">Delete</button>
                                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                            </div>
                        </div>
                    </div>
                </div>`);

    
    //Delete Action
    $(".delete").on('click', (e) => {
        e.preventDefault();
        target = e.target;
        if (e.target.classList.contains('bi')){
            target = target.parentElement; 
        }
        var Id = $(target).data('id');
        var controller = $(target).data('controller');
        var action = $(target).data('action');
        var bodyMessage = $(target).data('body-message');
        itemName = $(target).data('item-name');

        url = "/" + controller + "/" + action + "?Id=" + Id;
        $(".delete-modal-body").text(bodyMessage);
        $("#deleteModal").modal('show');
    });

    $("#confirm-delete").on('click', () => {
        $.get(url)
            .done((result) => {
                toastr.success(itemName +' successfully deleted !');
                return $(target).closest('tr').hide('slow');
            })
            .fail((error) => {
                if (redirectUrl)
                    window.location.href = redirectUrl;
            }).always(() => {
                $("#deleteModal").modal('hide');
            });
    });

}()));