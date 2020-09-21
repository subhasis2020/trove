$(document).ready(function () {
    var orgProgramListColumnDefs = [{ "targets": [0], "visible": false, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": true, "orderable": true }, { "targets": [2], "visible": true, "searchable": true, "orderable": true }, { "targets": [3], "visible": true, "searchable": false, "orderable": true }, { "targets": [4], "visible": true, "searchable": true, "orderable": true }, { "targets": [5], "visible": true, "searchable": true, "orderable": true }, { "targets": [6], "searchable": false, "orderable": false }];
    var urlForOrgProgramList = "/Organisation/LoadAllOrgProgram";
    $("#tblOrgnisationProgram").DataTable({
        "processing": true, // for show progress bar
        "serverSide": true, // for process server side
        "filter": true, // this is for disable filter (search box)
        "orderMulti": false, // for disable multiple column at once
        "pageLength": 10,
        "order": [[4, "desc"]],
        "dom": 'Bfrtip',
        "oLanguage": {
            "sEmptyTable": "No data available."
        },
        "ajax": {
            "url": urlForOrgProgramList,
            "data": { id: $("#hdnOrganisationId").val() },
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": orgProgramListColumnDefs,
        "columns": [
            { "data": "ProgramId", "id": "ProgramId", "name": "ProgramId", "autoWidth": true },
            {
                "data": "ProgramName", "name": "ProgramName", "autoWidth": true,
                "render": function (data, type, full) {
                    return '<a href=/Program/Index/' + full.strProgramId + "?poId=" + $("#hdnOrganisationId").val() + "&ppN=" + full.EncProgName + "&poN=" + ($("#hdnOrganisationName").val() !== '' ? $("#hdnOrganisationName").val() : '') +'>' + full.ProgramName + '</a>';
                }
            },
            { "data": "ProgramCodeId", "name": "ProgramCodeId", "autoWidth": true },
            {
                "data": "AccountListCount", "name": "AccountListCount", "autoWidth": true,
                "render": function (data, type, full) {
                    return '<a href=/Program/Index/' + full.strProgramId + "?poId=" + $("#hdnOrganisationId").val() + "&ppN=" + full.EncProgName + "&poN=" + ($("#hdnOrganisationName").val() !== '' ? $("#hdnOrganisationName").val() : '') + "&sal=" + ($("#hdnOrganisationName").val() !== '' ? $("#hdnOrganisationName").val() : '') + '>' + full.AccountListCount + '</a>';
                }
            },            
            { "name": "DateAdded", "autoWidth": true, "render": function (data, type, full, mets) { return moment(full.DateAdded).format('MMMM - Do - YYYY'); } }, { "data": "ProgramType", "name": "ProgramType", "autoWidth": true },
            {
                "render": function (data, type, full, mets) {
                    var linkDataContentEditDelete = "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                        "<div class='img-dots'></div></div>" +
                        "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                        "<ul><li><a href='/Program/Index/" + full.strProgramId + "?poId=" + $("#hdnOrganisationId").val() + "&ppN=" + full.EncProgName + "&poN=" + ($("#hdnOrganisationName").val() !== '' ? $("#hdnOrganisationName").val() : '') + "'>Edit</a></li>";
                    if (userRlN.toLowerCase() === "super admin") {
                    //    linkDataContentEditDelete += "<li><a href='#' data-org='" + full.ProgramName + "' onclick=DeleteData(this,'" + full.ProgramId + "','" + $("#hdnOrganisationId").val() + "');>Delete</a></li></ul></div></div></div>"; //   Commented for sodexho   
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
                var organisationId = $("#hdnOrganisationId").val(); 
                $.ajax({
                    type: "POST",
                    url: "/Organisation/OrganisationProgramExportExcel/?searchValue=" + searchValue + "&id=" + organisationId,
                    cache: false,
                    success: function (data) {/*get the file name for download*/ if (data.fileName !== "") { /* use window.location.href for redirect to download action for download the file*/                            window.location.href = '/Organisation/Download/?fileName=' + data.fileName;
                        }
                    },
                    error: function (data) {swal({title: "There is some issue in processing!",icon: "error"});
                    }
                });

            });
        },
        buttons: [{ extend: 'excel', text: '', exportOptions: { format: { body: function (data, row, column, node) { /* Strip $ from salary column to make it numeric */  return column === 2 ? moment(data, 'MMMM - Do - YYYY').format('MM/DD/YYYY') : data; }}, columns: [1, 2, 3, 4], modifier: {search: 'applied', order: 'applied' }},  title: 'Organization Program List'}]
    });

});

function DeleteData(e, programId, organisationId, prgName) {
    swal({
        title: "Are you sure you want to delete " + $(e).attr("data-org") + "?",
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
            Delete(programId, organisationId);
        }
    });
}

function Delete(programId, organisationId) {
    $("#dvLoadingGif").show();
    $.ajax({
        type: "POST",
        url: "/Organisation/DeleteOrganisationProgram/",
        data: { 'programId': programId, 'organisationId': organisationId },
        dataType: "json",
        success: function (data) { $("#dvLoadingGif").hide(); if (data.data > 0 && data.success) { swal({ title: "Organization program has been deleted successfully!", icon: "success" }); var oTable = $('#tblOrgnisationProgram').DataTable(); oTable.draw(); } else { swal({ title: "Currently unable to process the request! Please try again later.", icon: "error" }); } },
        error: function () { swal({ title: "Currently unable to process the request! Please try again later.", icon: "error" });
        }
    });


}
