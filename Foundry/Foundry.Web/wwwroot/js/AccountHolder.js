var inviteAllAccountHolder = function () {
    var encIdArray = [];
    $('.clsInvite').each(function () {
        var encId = $(this).attr('data-encId');
        encIdArray.push(encId);
    });
    if (encIdArray.length > 0) {

        $("#dvLoadingGif").show();
        var url = '/Program/InviteAccountHolder';

        $.post(url, { encIds: encIdArray }, function (data) {
            if (data) {
                swal({
                    title: "Account holder(s) has been invited successfully!",
                    icon: "success"

                });
                var oTable = $('#tblAccountHolder').DataTable();
                oTable.draw();
            }
            else {
                swal({
                    title: "Currently unable to process the request! Please try again later.",
                    icon: "error"

                });
            }
            $("#dvLoadingGif").hide();
        });
    }
    return false;
};

function inviteAccountHolderUser(e) {

    var url = '/Program/InviteAccountHolder';
    var encIdArray = [];
    encIdArray.push($(e).attr('data-encId'));
    $("#dvLoadingGif").show();
    $.post(url, { encIds: encIdArray }, function (data) {
        if (data) {
            swal({
                title: "Account holder has been invited successfully!",
                icon: "success"

            });
           var oTable = $('#tblAccountHolder').DataTable();
            oTable.draw();
        }
        else {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
        }
        $("#dvLoadingGif").hide();
    });
}
