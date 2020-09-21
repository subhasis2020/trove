
$(document).ready(function () {
    $("#tblPrograms").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        "order": [[3, "desc"]],
        "dom": 'Bfrtip',
        "oLanguage": {
            "sEmptyTable": "No data available."
        },

        "ajax": {
            "url": "/Program/LoadAllProgram",
            "type": "POST",
            "datatype": "json"
        },

        "columnDefs":
            [{
                "targets": [0],
                "visible": false,
                "searchable": false,
                "orderable": false
            },
            {
                "targets": [1],
                "visible": true,
                "searchable": true,
                "orderable": true
                                },
            {
                "targets": [2],
                "visible": true,
                "searchable": true,
                "orderable": true
            },
            {
                "targets": [3],
                "visible": true,
                "searchable": false,
                "orderable": true
            },
            {
                "targets": [4],
                "visible": true,
                "searchable": true,
                "orderable": true
            },
            {
                "targets": [5],
                "visible": true,
                "searchable": true,
                "orderable": true
            },
            {
                "targets": [6],
                "searchable": false,
                "orderable": false
            }
            ],

        "columns": [
            { "data": "ProgramId", "id": "ProgramId", "name": "ProgramId", "autoWidth": true },
            {
                "data": "ProgramName", "name": "ProgramName", "autoWidth": true,
                "render": function (data, type, full) {
                    return '<a href=/Program/Index/' + full.strProgramId + "?poId=" + full.OrganisationEncId + "&ppN=" + full.EncProgName + "&poN=" + full.EncOrganisationName + '>' + full.ProgramName + '</a>';
   
                }

            },
            { "data": "ProgramCodeId", "name": "ProgramCodeId", "autoWidth": true },
            {
                "name": "DateAdded",
                "autoWidth": true,
                "render": function (data, type, full, mets) { return moment(full.DateAdded).format('MMMM - Do - YYYY'); }
            },
            { "data": "ProgramType", "name": "ProgramType", "autoWidth": true },
            { "data": "OrganisationSubTitle", "name": "OrganisationSubTitle", "autoWidth": true },
            {
                "render": function (data, type, full, mets) {

                    var linkDataContentEditDelete = "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                        "<div class='img-dots'></div></div>" +
                        "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                        "<ul>";
                    if (userRlN.toLowerCase() == "merchant full") {
                        linkDataContentEditDelete += "<li><a href='/Merchant/Create/" + OrgMerEncId + "?poId=" + full.OrganisationEncId + "&ppId=" + full.strProgramId.toString() + "&ppN=" + full.EncProgName + "'>Merchant Detail</a></li>";
                    }
                    else {
                        linkDataContentEditDelete += "<li><a  href='/Program/Index/" + full.strProgramId + "?poId=" + full.OrganisationEncId + "&ppN=" + full.EncProgName + "&poN=" + full.EncOrganisationName + "'>Edit Program</a></li>";
                    }
                    if (userRlN.toLowerCase() == "super admin") {

                   //linkDataContentEditDelete += "<li><a href='#' data-org='" + full.ProgramName + "' onclick=DeleteData(this,'" + full.strProgramId + "','" + full.OrganisationEncId + "','" + full.OrgPrgLinkCount + "','" + full.JPOS_IssuerId + "');>Delete Program</a></li></ul></div></div></div>";   //   Commented for sodexho   
                    }
                    else {
                        linkDataContentEditDelete += "</ul></div></div></div>";
                    }
                    return linkDataContentEditDelete;
                }
            }],
        initComplete: function () {
            $('.dt-buttons').hide();
            $('#aExport').on('click', function () {
                var searchValue = $('.dataTables_filter input').val();

                $.ajax({
                    type: "POST",
                    url: "/Program/OrganisationProgramExportExcel/?searchValue=" + searchValue,
                    cache: false,
                    success: function (data) {
                        //get the file name for download
                        if (data.fileName !== "") {
                            //use window.location.href for redirect to download action for download the file
                            window.location.href = 'Download/?fileName=' + data.fileName;
                        }
                    },
                    error: function (data) {
                        swal({
                            title: "Something went wrong.",
                            icon: "error"
                        });
                    }
                });
            });
        },
        buttons: [
            {
                extend: 'excel',
                text: '',
                exportOptions: {
                    format: {
                        body: function (data, row, column, node) {
                            // Strip $ from salary column to make it numeric
                            return column === 2 ?
                                moment(data, 'MMMM - Do - YYYY').format('MM/DD/YYYY') : data;
                        }
                    },
                    columns: [1, 2, 3, 4, 5],
                    modifier: {
                        search: 'applied',
                        order: 'applied'
                    }
                },
                title: 'Programs List'
            }
        ]
    });
});

function DeleteData(e, programId, organisationId, OrgPrgLinkCount, JPOS_IssuerId) {
    var swaltitle;
    if (parseInt(OrgPrgLinkCount) > 0) {
        swaltitle = $(e).attr("data-org") + " is linked with organizations. Are you sure you want to delete " + $(e).attr("data-org") + "?";
    } else { swaltitle = "Are you sure you want to delete " + $(e).attr("data-org") + "?"; }
    swal({
        title: swaltitle,
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
            Delete(programId, organisationId, JPOS_IssuerId);
        }
    });
}

function Delete(programId, organisationId, JPOS_IssuerId) {
    $("#dvLoadingGif").show();
    $.ajax({
        type: "POST",
        url: "/Program/DeleteProgram/",
        data: { 'Id': programId, 'JPOS_IssuerId': JPOS_IssuerId },
        dataType: "json",
        success: function (data) {
            $("#dvLoadingGif").hide();
            if (data.data > 0 && data.success) {
                swal({
                    title: "Program has been deleted successfully!",
                    icon: "success"
                });
                var oTable = $('#tblPrograms').DataTable();
                oTable.draw();
            } else {
                swal({
                    title: "Currently unable to process the request! Please try again later.",
                    icon: "error"
                });
            }
        },
        error: function () {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
        }
    });
}
