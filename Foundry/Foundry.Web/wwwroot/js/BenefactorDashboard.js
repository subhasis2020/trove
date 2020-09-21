$(document).ready(function () {

    var linked = $('#hdnlinked').attr('value');
    if (linked === "0") {
        swal({
            title: "No account linked.",
            type: "warning",
            confirmButtonText: 'Ok',
        });
    }
});