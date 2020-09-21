$(document).on('click', '.adelete', function () {
    var id = $(this).attr('value');
    swal({
        title: "Are you sure you want to delete user?",
        type: "warning",
        showCancelButton: true,
        cancelButtonClass: 'btn btn-danger',
        confirmButtonClass: 'btn btn-success',
        confirmButtonText: 'Ok',
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    }, function (result) {
        if (result === true) {
            DeleteAccount(id);

        }
    }
    );
});
var DeleteAccount = function (id) {
    try {
        $.ajax({
            type: "POST",
            url: "/Benefactor/DeleteBenefactorUser/",
            data: { 'userId': id },
            dataType: "html",
            success: function (data) {
                $('._linkedaccountpartial').html(data);
                swal({
                    title: "Account deleted successfully!",
                    icon: "success"

                });

            },
            error: function (xhr, status, error) {
                swal({
                    title: "There is some issue in processing!",
                    icon: "error"

                });
            }
        });
    }
    catch (a) {
        swal({
            title: a,
            icon: "error"

        });
    }
};