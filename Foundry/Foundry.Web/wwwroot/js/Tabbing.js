var oTable;
var canViewTransactioCheckBool;
$(document).ready(function () {
    $("#accordion").find('li').removeClass('active');
    $("#accordion #liTransactionIcon").addClass('active');
    var id = $("#ActivelinkedId").val();
    var canViewTransactionCheck = $("#ActiveCanViewTransaction").val();

    if (canViewTransactionCheck == "" || canViewTransactionCheck == "False" || canViewTransactionCheck == "false") {
        canViewTransactioCheckBool = false;
    }
    else if (canViewTransactionCheck == "True" || canViewTransactionCheck == "true") {
        canViewTransactioCheckBool = true;
    }

    var stringElement = "tab_" + id;
    $("#" + stringElement).addClass('active');
    $(".nameTab").click(function (e) {
        $(".nameTab").removeClass('active');
        var idName = $(this).attr('id').replace('tab_', '');
        $("#ActivelinkedId").val(idName);
        $("#hdnDateMonth").val($('.aCurrentPeriod').attr('id'));
        $("#hdnPlan").val('');

        var canviewtransaction = $(this).attr('data-CanViewTransaction').replace('tab_', '').toString();
        if (canviewtransaction == "" || canviewtransaction == "False" || canviewtransaction == "false") {
            canViewTransactioCheckBool = false;
        }
        else if (canviewtransaction == "True" || canviewtransaction == "true") {
            canViewTransactioCheckBool = true;
        }

        var stringElement = "tab_" + idName;
        $("#" + stringElement).addClass('active');
        GetDataTableContent();
        return false;
    });
    oTable = $('#tblTransactions').DataTable({
        "destroy": true,
        "bPaginate": false,
        "bServerSide": false,
        "searching": false,
        "ordering": false,
        "bStateSave": true,
        "info": false,
        "dom": 'lBfrtip',
        "oLanguage": {
            "sEmptyTable": "No Transactions available."
        },

        "columnDefs": [{
            "targets": 5,
            "searchable": true
        }],
        "columns": [
            {
                "data": "MerchantFullName",
                "name": "0",
                "autoWidth": true
            },
            {
                "data": "Period",
                "name": "1",
                "autoWidth": true
            },
            {
                "data": "Amount",
                "name": "2",
                "autoWidth": true
            },
            {
                "data": "Date",
                "name": "3",
                "autoWidth": true
            },
            {
                "data": "Time",
                "name": "4",
                "autoWidth": true
            },
            {
                "data": "PlanName",
                "name": "5",
                "autoWidth": true
            }],
        initComplete: function () {
            $('.dt-buttons').hide();
            $('#aExportUsers').on('click', function () {
                var id = $("#ActivelinkedId").val();
                var dateMonth = $("#hdnDateMonth").val();
                var dataSetPlan = $("#hdnPlan").val();
                $.ajax({
                    type: "POST",
                    url: "/Benefactor/BenefactorTransactionExportExcel/?id=" + id + "&dateMonth=" + dateMonth + "&dataSetPlan=" + dataSetPlan,
                    cache: false,
                    success: function (data) {
                        //get the file name for download
                        if (data.fileName !== "") {
                            //use window.location.href for redirect to download action for download the file
                            window.location.href = '/Benefactor/Download/?fileName=' + data.fileName;
                        }
                    },
                    error: function (data) {
                        swal({
                            title: "There is some issue in processing!",
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
                filename: 'Organisations',
                exportOptions: {
                    modifier: {
                        search: 'applied',
                        order: 'applied'
                    }
                }
            }
        ]//,

    });

    $("#hdnDateMonth").val($(".aCurrentPeriod").attr('id'));

    GetDataTableContent();



    $("#Filters  a.categoryFilter").click(function () {
        $("#hdnPlan").val(this.title);
        GetDataTableContent();
        $("#plan-panel-dropdown.collapse").collapse('hide');
    });

});

var GetDataTableContent = function () {
    var id = $("#ActivelinkedId").val();
    var dateMonth = $("#hdnDateMonth").val();
    var dataSetPlan = $("#hdnPlan").val();
    var CanView = canViewTransactioCheckBool;

    if (CanView) {

        $.ajax({
            type: "GET",
            url: "/Benefactor/GetTransactionsData/",
            data: { 'id': id, 'dateMonth': dateMonth, 'planAccount': dataSetPlan },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                oTable.clear().draw();
                oTable.rows.add(data.data); // Add new data
                oTable.columns.adjust().draw();
                if (data.data.length <= 1) {
                    $("#dvHeight").addClass("distance");
                }
                else { $("#dvHeight").removeClass("distance"); }
            },
            error: function () {
            }
        });
    }
    else {

        $('#tblTransactions .dataTables_empty').text('You don\'t have permission to view the transaction.');
        $("#aExportUsers").hide();
    }


};

var CheckForDate = function (id) {
    $("#hdnDateMonth").val(id);
    GetDataTableContent();
    $("#transactions-panel-dropdown.collapse").collapse('hide');
};

