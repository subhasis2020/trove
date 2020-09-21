$(document).ready(function () {
    var primaryorgId = $('#hdnprimaryOrgId').attr('value');
    var primaryprogId = $('#hdnprimaryProgId').attr('value');
    var primaryprogName = $('#hdnprimaryProgName').attr('value');
    var merchantTable = "#merchant";
    $(merchantTable).DataTable({
        "processing": true, "serverSide": true, "bfilter": true, "orderMulti": false, "pageLength": 10, "order": [[5, "desc"]], "dom": 'Brtip', "oLanguage": { "sEmptyTable": "No data available." }, "ajax": { "url": "/Merchant/LoadData", "data": { ppId: primaryprogId }, "type": "POST", "datatype": "json" },
        "columnDefs": [{ "targets": [0], "visible": false, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": true, "orderable": true }, { "targets": [2], "visible": true, "searchable": true, "orderable": true }, { "targets": [3], "visible": true, "searchable": false, "orderable": false }, { "targets": [4], "visible": true, "searchable": false, "orderable": true }, { "targets": [5], "searchable": false, "orderable": true }, { "targets": [6], "searchable": false, "orderable": false }, { "targets": [7], "searchable": false, "orderable": false }, { "targets": [8], "searchable": false, "orderable": false }],
        "columns": [{ "data": "Id", "id": "Id", "name": "Id", "autoWidth": true, className: "text-center" }, { "data": "MerchantId", "name": "MerchantId", "autoWidth": true, className: "text-center" },
            { "data": "MerchantName", "name": "MerchantName", "autoWidth": true, className: "text-center" },
            { "data": "Location", "name": "Location", "autoWidth": true, className: "text-center" }, { "className": "text-center", "name": "LastTransaction", "autoWidth": true, "render": function (data, type, full, mets) { return full.LastTransaction !== '' && full.LastTransaction !== null ? moment(full.LastTransaction).format('DD MMMM YYYY') : ""; } },
            {
                "name": "DateAdded",
                "autoWidth": true,
                "render": function (data, type, full, mets) { return moment(full.DateAdded).format('DD MMMM YYYY'); }
            },
            {
                "render": function (data, type, full, mets) {
                    return $("<div>").html(full.AccountType).text().replace("lt;/br>", "");
                }
            },
            {
                "className": "text-center",
                "render": function (data, type, full, mets) {
                    if (full.Activity === 0) {
                        return "<div class='status-point alert-point'></div>";
                    }
                    if (full.Activity === 1) {
                        return "<div class='status-point orange-point'></div>";
                    }
                    if (full.Activity === 2) {
                        return "<div class='status-point green-point'></div>";
                    }
                }
            },
            {
                "render": function (data, type, full, mets) {
                    return "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                        "<div class='img-dots'></div></div>" +
                        "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                        "<ul><li><a href='/Merchant/Create/" + full.Id + "?poId=" + primaryorgId.toString() + "&ppId=" + primaryprogId.toString() + "&ppN=" + primaryprogName + "'>Edit</a></li><li><a href='#' onclick=Clone('" + full.Id + "')>Clone</a></li><li><a href='#' onclick=DeleteData('" + full.Id + "');>Delete</a></li></ul>" +
                        "</div></div></div>";
                }
            }
        ],
        initComplete: function () {
            $('.dataTables_filter').show();
            var $buttons = $('.dt-buttons').hide();
            $('#aExport').on('click', function () {
                var btnClass = '.buttons-excel';
                $buttons.find(btnClass).click();
            });
        },
        buttons: [
            {
                extend: 'excel',
                text: '',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5],
                    modifier: {
                        search: 'applied',
                        order: 'applied'
                    }
                },
                title: 'Merchant List'
            }
        ]//,
    });
});
function DeleteData(id) {
    swal({
        title: "Are you sure you want to delete the merchant?",
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
            Delete(id);
        }
    });
}
function Delete(id) {
    var url = '/Merchant/Delete';
    $.post(url, { ID: id }, function (data) {
        if (data) {
            swal({
                title: "Merchant has been deleted successfully!",
                icon: "success"

            });
            var oTable = $('#merchant').DataTable();
            oTable.draw();
        }
        else {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
        }
    });
}
function Clone(id) {
    var url = '/Merchant/CloneMerchant';
    $.post(url, { merchantId: id }, function (data) {
        if (data) {
            swal({
                title: "Merchant has been cloned successfully!",
                icon: "success"

            });
            var oTable = $('#merchant').DataTable();
            oTable.draw();
        }
        else {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
        }
    });
}  