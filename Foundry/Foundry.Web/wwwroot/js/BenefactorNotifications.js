var refreshComponent;

$(document).ready(function () {
    if ($("#hdnRole").val() === "benefactor") {
        GetBenefactorsNotifications();
    }
    $(document).on('click', '.clsReloadRequest', function () {
        var userId = $(this).attr('data-id');
        var reloadRequestId = $(this).attr('data-reloadRequestId');
        var programId = $(this).attr('data-programId');
        window.location.href = '/Benefactor/ReloadRequest?id=' + userId + '&reloadRequestId=' + reloadRequestId + '&programId=' + programId;
    });

    $(document).on('click', '.clsDeleteInvite', function () {
        var id = $(this).attr('id').replace('aDeleteInvite_', '');
        var programId = $(this).attr('data-ProgramId');
        swalAlertBenefactor("Are you sure you want to decline the invitation?", false, id, programId);
    });


    $(document).on('click', '.clsAddInvite', function () {
        var id = $(this).attr('id').replace('aAcceptInvite_', '');
        var programId = $(this).attr('data-ProgramId');
        swalAlertBenefactor("Are you sure you want to accept the invitation?", true, id, programId);
    });


});


var swalAlertBenefactor = function (swalTitle, IsForAcceptInvitation, id, programId) {
    swal({
        title: swalTitle,
        type: "warning",
        showCancelButton: true,
        cancelButtonClass: 'btn btn-danger',
        confirmButtonClass: 'btn btn-success',
        confirmButtonText: 'Ok',
        cancelButtonText: "Cancel",
        closeOnConfirm: true,
        closeOnCancel: true
    }, function (result) {
        if (result) {
            if (IsForAcceptInvitation)
                AcceptInvitation(id, programId);
            else
                DeleteInvitation(id, programId);
        }
    }
    );
};

var GetBenefactorsNotifications = function () {
    $.ajax({
        type: "GET",
        url: "/Benefactor/GetNotifications/",
        dataType: "json",
        success: function (data) {

            var result = data.data;
            var i;
            $("#spnNotifications").empty();
            if (result.length > 0) {

                var MainData = '<div id="notifiation-panel" class="collapse notification"><h2>NOTIFICATIONS</h2><a class="close" href="" data-toggle="collapse" data-target="#notifiation-panel"><img src="/images/icon-close.png"></a><div class="notification-listing content-scroll">';  // 
                MainData += '<ul id="ulNotificationsContentShow">';
                for (i = 0; i < result.length; ++i) {
                    var li = '<li><div class="profile-link"><div class="profile-img">';
                    li += '<span class="avatar" style="background-image: url(' + result[i].ImagePath + ')"></span></div >';
                    li += '<div class="profile-dis">' + result[i].Message + '</div></div>';
                    var contentButtons = '<div class="profile-btns">';
                    if (result[i].IsInvitation) {
                        contentButtons += '<div class="row"><div class="col-sm-6 col-6"><a href="javascript:void(0);" id="aDeleteInvite_' + result[i].UserId + '" data-ProgramId="' + result[i].ProgramId + '" class="btn btn-secondary btn-sm clsDeleteInvite">Decline</a> </div>';
                        contentButtons += '<div class="col-sm-6 col-6"><a href="javascript:void(0);" id="aAcceptInvite_' + result[i].UserId + '" data-ProgramId="' + result[i].ProgramId + '" class="btn btn-primary btn-sm  clsAddInvite">Accept</a></div></div>';
                    }
                    else {
                        contentButtons += '<div class="col-sm-6 col-6"><a href="javascript:void(0);" id="aReloadRequest_' + result[i].UserId + '" data-id="' + result[i].UserId + '" data-programId="' + result[i].ProgramId + '" data-reloadRequestId="' + result[i].ReloadRequestId + '" class="btn btn-primary btn-sm  clsReloadRequest">Reload Balance</a></div>';
                    }
                    contentButtons += '</div></li>';
                    li += contentButtons;
                    MainData += li;
                }
                MainData += '</ul></div></div>';
                $("#spnNotifications").append(MainData);
                $("#spnNotificationsCount").html(result.length);
                $("#spnNotificationsCount").show();
                $(".content-scroll").mCustomScrollbar({ scrollButtons: { enable: true } });
            } else {
                $("#spnNotificationsCount").hide();
            }

        },
        error: function () {
            swal({
                title: "There is some issue in processing!",
                icon: "error"

            });
        }
    });


};


var DeleteInvitation = function (id, programId) {

    $.ajax({
        type: "POST",
        url: "/Benefactor/DeleteInvitation/",
        data: { 'id': id, 'programId': programId },
        dataType: "json",
        success: function (data) {

            GetBenefactorsNotifications();
            swal({
                title: "Invitation has been declined successfully!",
                icon: "success"

            });

        },
        error: function () {
            swal({
                title: "There is some issue in processing!",
                icon: "error"

            });
        }
    });
};

var AcceptInvitation = function (id, programId) {

    $.ajax({
        type: "POST",
        url: "/Benefactor/AcceptInvitation/",
        data: { 'id': id, 'programId': programId },
        dataType: "json",
        success: function (data) {

            GetBenefactorsNotifications();
            swal({
                title: "Invitation has been accepted successfully!",
                icon: "success"

            });

        },
        error: function () {
            swal({
                title: "There is some issue in processing!",
                icon: "error"

            });
        }
    });
};