var canViewTransactioCheckBool;
$(document).ready(function () {
    var canViewTransactionCheck = $("#ActiveCanViewTransaction").val();

    if (canViewTransactionCheck == "" || canViewTransactionCheck == "False" || canViewTransactionCheck == "false") {
        canViewTransactioCheckBool = false;
    }
    else if (canViewTransactionCheck == "True" || canViewTransactionCheck == "true") {
        canViewTransactioCheckBool = true;
    }
    if ($("#IsAutoReload").prop("checked") == true && $("#CheckDroppedAmountdd").val() != '' && $("#CheckDroppedAmountdd").val() != undefined) {
        var valueDropped = $("#CheckDroppedAmountdd").val();
      //  alert(valueDropped);
        var spanClass = "choose_autoreload." + valueDropped;
        $("." + spanClass).parent();
        $("." + spanClass).parent().addClass("active");
        $("#dvAutoReloadOptions").css('pointer-events', '');
    }
    else { $("#dvAutoReloadOptions").css('pointer-events', 'none'); }
    var id = $("#ReloadUserId").val();
    var stringElement = "tab_" + id;
    $("#" + stringElement).addClass('active');
    $(document).on('click', '.reloadVal', function () {
        var value = $(this).html().replace('$', '');
        $("#ReloadAmount").val(value);
    });
    $(document).on('click', '.choose_autoreload', function () {
        var value = $(this).html().replace('$', '');
        $("#CheckDroppedAmountdd").val(value);
    });

    $(document).on('change', '#IsAutoReload', function () {
        if (this.checked) {
            $("#dvAutoReloadOptions").css('pointer-events', '');
        }
        else {
            $("#dvAutoReloadOptions").css('pointer-events', 'none');
            $(".autoLabel").removeClass('active');
            $("#AutoReloadAmount").val('');
        }
    });

   
    $(document).on('click', '#btnSetAutoReload', function () {
      
        if ($("#IsAutoReload").prop("checked") && $("#AutoReloadAmount").val() == '') {
            $("#spnErrorAutoReload").show();
            $("#AutoReloadAmount").focus();
            return false;
        }
        else { $("#spnErrorAutoReload").hide(); }


        if ($("#IsAutoReload").prop("checked") == true) {
            if ($("#CheckDroppedAmountdd").val() === '') {
                $("#spnErrorDroppedAmount").show();
                $("#CheckDroppedAmountdd").focus();
                return false;
            }
            else { $("#spnErrorDroppedAmount").hide(); }
            if ($("#AutoReloadAmount").val() === '') {
                $("#spnErrorAutoReload").show();
                $("#AutoReloadAmount").focus();
                return false;
            } else { $("#spnErrorAutoReload").hide(); }
        }
        if ($("#ddlcardslectionforrule").val() == '') {
            $("#spnErrorCardSelectionforrule").show();
            return false;
        }
        else {
            $("#spnErrorCardSelectionforrule").hide();
            var BankIdRule = ""; var CardIdRule = "";

            var ddl1 = $("#ddlcardslectionforrule option:selected").text();
          
            if (ddl1.includes('Verified')) {
                $("#IsBankPay").val("1");
                BankIdRule = $("#ddlcardslectionforrule").val();
               
            }
            else {
                $("#IsBankPay").val("0");
                CardIdRule = $("#ddlcardslectionforrule").val();
               

            }



        }
       
        $("#dvLoadingGif").show();
        var model = {
            
             AutoReloadAmount: $("#AutoReloadAmount").val(),
            CheckDroppedAmount: $("#CheckDroppedAmountdd").val(),
            IsAutoReload: $("#IsAutoReload").is(":checked") ? true : false,
            ReloadAmount: $("#ReloadAmount").val(),
            ProgramId: $("#ProgramId").val(),
            ReloadRequestId: $("#ReloadRequestId").val(),
           // ReloadUserId: $("#ReloadUserId").val(),
            ReloadUserId: $("#ddlLinkedUserId").val(),
            AccountReloadSrNo: BankIdRule,
            CardToken: CardIdRule
            

            
        };
        $.post("/benefactor/setreloadrules",
            model,
            function (data) {
                $("#dvLoadingGif").hide();
                var titlemessage = '';
               // if (data.status) {

                    titlemessage = 'The auto-reload rule has been set. It will trigger automatically once the balance drops to the threshold amount.';

                    swal({
                        title: titlemessage,
                        icon: "success"
                    }, function () {
                       // window.location.href = window.location.href;
                    });
                //}
            });

        $("#dvLoadingGif").hide();
        $('#addcardpopupforrule').modal('hide');

    });

    //Cancel reload rule
    $(document).on('click', '#btnCancelAutoReload', function () {
      //  debugger;
        var model = {
            ReloadUserId: $("#ReloadUserId").val()
          //  BenefactorUserId: "3140"
        };
        $.post("/Benefactor/PostCancelSubscriptionRule",
            model,
            function (data) {
                $("#dvLoadingGif").hide();
               // if (data.status) {

                    titlemessage = 'The auto-reload rule has been cancelled ';

                    swal({
                        title: titlemessage,
                        icon: "success"
                    }, function () {
                        window.location.href = window.location.href;
                    });
               // }
            });

        $("#dvLoadingGif").hide();

    });
    //
   
   
    //new
    $(document).on('click', '#btnReloadAmount,#btnAddSecurityCode', function () {
        // if ($("#frmReloadAmountform").validate() && $("#frmReloadAmountform").valid()) {
        //if ($("#ProgramAccountIdSelected").val() == '') {
        //    $("#spnErrorProgramAccountSelect").show();
        //    $("#ProgramAccountIdSelected").focus();
        //    return false;
        //}
        //else { $("#spnErrorProgramAccountSelect").hide(); }
     // debugger;

        if ($("#ddlLinkedUserId").val() == '') {
             $("#spnErrorlinkedUserSelect").show();
            $("#ddlLinkedUserId").focus();
            return false;
        }
         else { $("#spnErrorlinkedUserSelect").hide(); }
        
        if ($("#ReloadAmount").val() == '') {
           
            $("#spnErrorReloadAmount").show();
            $("#ReloadAmount").focus();
            return false;
        }
        else {
            var Amounttoadd = $("#spnmaxvalue").text();
            var amttoadd = parseFloat(Amounttoadd);
            var reloadamt = parseFloat($("#ReloadAmount").val());
           
            if (reloadamt > amttoadd) {
                $("#spnErrorMaxReloadAmount").show();
                return false;
            }
            else {
                $("#spnErrorMaxReloadAmount").hide();
            }
            $("#spnErrorReloadAmount").hide();
        }
        if ($("#ddlcardslection").val() == '') {
            $("#spnErrorCardSelection").show();
            $("#ddlcardslection").focus();
            return false;
        }
        else { $("#spnErrorCardSelection").hide(); }

        //if ($("#IsAutoReload").prop("checked") && $("#AutoReloadAmount").val() == '') {
        //    $("#spnErrorAutoReload").show();
        //    $("#AutoReloadAmount").focus();
        //    return false;
        //}
        //else { $("#spnErrorAutoReload").hide(); }
   
        var ii = null;
        // if ($("#IsBankPay").val() != '') {
        if ($("#ddlcardslection").val() != '') {
           // debugger;
            var ddl = $("#ddlcardslection option:selected").text();
            var BankId = ""; var CardId = "";
            if (ddl.includes('Verified')) {
                $("#IsBankPay").val("1");
                BankId = $("#ddlcardslection").val();
            }
            else {
                $("#IsBankPay").val("0");              

                var ipg = $("#txtIpg").val();


               // alert(ipg);
                CardId = $("#ddlcardslection").val();
                //if (ipg =="") {
                //  //  alert("yes");
                //    if ($("#CCCodeCVV").val() == '') {
                //        $('#SecurityCodeModal').modal('show');
                //        if ($(this).attr('id') == 'btnAddSecurityCode')

                //            $("#spnErrorCardCode").show();
                //        return false;
                //    }
                //    else {
                //        $("#spnErrorCardCode").hide();
                //        $('#SecurityCodeModal').modal('hide');
                //    }
                //}
                //else {
                //   // alert("no");
                //}
            }
            $("#dvLoadingGif").show();
            //set reload rule
            //var model = {
            //  //  AutoReloadAmount: $("#AutoReloadAmount").val(),
            //    AutoReloadAmount: $("#ReloadAmount").val(), 
            //    CheckDroppedAmount: $("#CheckDroppedAmountdd").val(),
            //    IsAutoReload: $("#IsAutoReload").is(":checked") ? true : false,
            //    ReloadAmount: $("#ReloadAmount").val(),
            //    ProgramId: $("#ProgramId").val(),
            //    ReloadRequestId: $("#ReloadRequestId").val(),
            //    ReloadUserId: $("#ReloadUserId").val(),
            //    AccountReloadSrNo: BankId,
            //    CardToken: CardId
            //};
            //$.post("/benefactor/setreloadrules",
            //    model,
            //    function (data) {
            //      //  $("#dvLoadingGif").hide();
            //        var titlemessage = '';
            //        // if (data.status) {
            //      //  titlemessage = '';
            //       // titlemessage = 'The auto-reload rule has been set. It will trigger automatically once the balance drops to the threshold amount.';
            //        swal({
            //          //  title: titlemessage,
            //          //  icon: "success"
            //        }, function () {
            //            // window.location.href = window.location.href;
            //        });
            //        //}
            //    });

            //
          //  debugger;
            var userid = $("#ddlLinkedUserId").val();

            if (userid == undefined) {
                userid = $("#ReloadUserId").val();
            }
           
            var model = {
               // AutoReloadAmount: $("#AutoReloadAmount").val(),
                AutoReloadAmount: $("#ReloadAmount").val(),
                CheckDroppedAmount: $("#CheckDroppedAmountdd").val(),
                IsAutoReload: $("#IsAutoReload").is(":checked") ? true : false,
                ReloadAmount: $("#ReloadAmount").val(),
                ProgramId: $("#ProgramId").val(),
                ReloadRequestId: $("#ReloadRequestId").val(),
                ReloadUserId: userid,
              //  ReloadUserId: $("#ddlLinkedUserId").val(),                
                AccountReloadSrNo: BankId,
                IsPaymentViaBank: $("#IsBankPay").val() == "1" ? true : false,
                //CardToken: $("#TokenPG").val(),
                CardToken: CardId,
             //   clientTokenPG: $("#ClientTokenPG").val(),
               clientTokenPG: CardId,
                CCCode: $("#CCCodeCVV").val(),
                //IsCardDetailToSave: $("#SaveCDChkBox").val(),
                IsCardDetailToSave: 1,
                //ProgramAccountIdSelected: $("#ProgramAccountIdSelected").val(),
                ProgramAccountIdSelected: 38, //for sodexhoprogramaccount
                IsCardSelectionFromDropdown: $("#IsCardSelectionFromDropdown").val() == "1" ? true : false,
                IsNewCardTransaction: $("#IsNewCardTransaction").val() == "1" ? true : false
            };
            $.post("/Benefactor/ReloadAmountRequest",
                model,
                function (data) {
                    $("#dvLoadingGif").hide();
                    var titleMessage = '';
                    if (data.Status) {





                        if ($("#IsBankPay").val() == "1") {
                            if ($("#hdnUserType").val() !== '' && $("#hdnUserType").val() === 'basic user') {
                                // titleMessage = 'Your payment request has been received. Payment will be reflected in your account soon.';
                                titleMessage = 'Your Transaction is Approved. Your Balance will update momentarily.';
                             
                            }
                            else {
                               // titleMessage = 'Payment request for the selected user has been received. Payment will be reflected in user\'s account soon.';
                            } titleMessage = 'Your transaction is being processed.When your transaction clears, we will send account holder a notification.';
                        }
                        else {
                            if ($("#hdnUserType").val() !== '' && $("#hdnUserType").val() === 'basic user') {
                                // titleMessage = 'Balance is successfully added in your account.';
                                titleMessage = 'Your Transaction is Approved. Your Balance will update momentarily.';

                            }
                            else {
                                //  titleMessage = 'Balance is successfully added in user\'s account.';
                                titleMessage = 'Your transaction is being processed.When your transaction clears, we will send account holder a notification.';

                            }
                        }
                        swal({
                            title: titleMessage,
                            icon: "success"
                        }, function () {
                            window.location.href = window.location.href;
                        });

                        $("#dvLoadingGif").hide();
                    }
                    else {
                        //titleMessage = 'There is some issue in processing the request. Please try again later.';
                        if (data.responseCode == '51') {
                            titleMessage = 'We’re sorry but your card was declined. Please use an alternative credit card and try submitting again.';
                        }
                        else if (data.responseCode == '54') {
                            titleMessage = 'We’re sorry but your card was declined due to the card being expired.';
                        }
                        else if (data.responseCode == '91') {
                            titleMessage = 'We’re sorry there was a network error. Please try again.';
                        }
                        else if (data.responseCode == '94') {
                            titleMessage = 'We’re sorry but your transaction was blocked by duplicate transaction protection';
                        }
                        else if (data.responseCode == '14') {
                            titleMessage = 'We’re sorry but your card was declined due to an invalid card number.  Please use an alternative credit card and try submitting again.';
                        }

                        else if (data.responseCode == '5005') {
                            titleMessage = 'We’re sorry but your transaction was blocked by our fraud prevention service';
                        }
                        else if (data.responseCode == 'auto-refunded') {
                            titleMessage = 'There was a problem loading funds to your account. Your payment has been reversed. Please try again or contact support';
                        }
                        else {
                            var msg = data.messageSignature;
                            // alert(msg);
                            titleMessage = 'Oops ! Something went wrong. Payment can not be processed now. Please try again later.Technical Error: ' + msg;
                        }
                        swal({
                            title: titleMessage,
                            icon: "error"
                        });
                        $("#dvLoadingGif").hide(); return false;
                    }
                });
        }
        else {
            swal({
                title: 'Please select the payment bank or card.',
                icon: "error"
            });
            return false;
        }
        return false;
    });

    //$(document).on('click', '#btnReloadAmount,#btnAddSecurityCode', function () {
    //    // if ($("#frmReloadAmountform").validate() && $("#frmReloadAmountform").valid()) {
    //    if ($("#ProgramAccountIdSelected").val() == '') {
    //        $("#spnErrorProgramAccountSelect").show();
    //        $("#ProgramAccountIdSelected").focus();
    //        return false;
    //    }
    //    else { $("#spnErrorProgramAccountSelect").hide(); }
    //    if ($("#ReloadAmount").val() == '') {
    //        $("#spnErrorReloadAmount").show();
    //        $("#ReloadAmount").focus();
    //        return false;
    //    }
    //    else {
    //        $("#spnErrorReloadAmount").hide();
    //    }
    //    if ($("#ddlcardslection").val() == '') {
    //        $("#spnErrorCardSelection").show();
    //        $("#ddlcardslection").focus();
    //        return false;
    //    }
    //    else { $("#spnErrorCardSelection").hide(); }
        
    //    if ($("#IsAutoReload").prop("checked") && $("#AutoReloadAmount").val() == '') {
    //        $("#spnErrorAutoReload").show();
    //        $("#AutoReloadAmount").focus();
    //        return false;
    //    }
    //    else { $("#spnErrorAutoReload").hide(); }
    //    if ($("#IsBankPay").val() != '') {
    //  //  if ($("#ddlcardslection").val() != '') {
    //        debugger;
           
    //        if ($("#IsBankPay").val() == '1') {
    //            if ($("#AccountReloadSrNo").val() == '') {
    //                $("#spnErrorBankSelect").show();
    //                $("#BankIdSelected").focus();
    //                return false;
    //            }
    //            else {
    //                alert("ss");
    //                $("#spnErrorBankSelect").hide();
    //                if ($("#SelectedBankStatus").val() === "0") {
    //                    swal({
    //                        title: 'Please select verified bank.',
    //                        icon: "error"
    //                    });
    //                    return false;
    //                }
    //            }
    //            if ($("#IsAutoReload").prop("checked") == true) {
    //                if ($("#CheckDroppedAmount").val() === '') {
    //                    $("#spnErrorDroppedAmount").show();
    //                    $("#CheckDroppedAmount").focus();
    //                    return false;
    //                }
    //                else { $("#spnErrorDroppedAmount").hide(); }
    //                if ($("#AutoReloadAmount").val() === '') {
    //                    $("#spnErrorAutoReload").show();
    //                    $("#AutoReloadAmount").focus();
    //                    return false;
    //                } else { $("#spnErrorAutoReload").hide(); }
    //            }
    //        }
    //        else {
    //            if ($("#ClientTokenPG").val() != '') {
    //                $("#spnErrorCardSelect").hide();
    //                if ($("#CCCodeCVV").val() == '' && $("#IsNewCardTransaction").val() == "1") {
    //                    $('#SecurityCodeModal').modal('show');
    //                    if ($(this).attr('id') == 'btnAddSecurityCode')
    //                        alert("ss1");
    //                        $("#spnErrorCardCode").show();
    //                    return false;
    //                }
    //                else {
    //                    $("#spnErrorCardCode").hide();
    //                    $('#SecurityCodeModal').modal('hide');
    //                }
    //            }
    //            else {
    //                $("#spnErrorCardSelect").show();
    //                $("#CardIdSelected").focus();
    //                return false;
    //            }
    //        }
    //        $("#dvLoadingGif").show();
    //        var model = {   
    //            AutoReloadAmount: $("#AutoReloadAmount").val(),
    //            CheckDroppedAmount: $("#CheckDroppedAmount").val(),
    //            IsAutoReload: $("#IsAutoReload").is(":checked") ? true : false,
    //            ReloadAmount: $("#ReloadAmount").val(),
    //            ProgramId: $("#ProgramId").val(),
    //            ReloadRequestId: $("#ReloadRequestId").val(),
    //            ReloadUserId: $("#ReloadUserId").val(),
    //            AccountReloadSrNo: $("#AccountReloadSrNo").val(),
    //            IsPaymentViaBank: $("#IsBankPay").val() == "1" ? true : false,
    //            CardToken: $("#TokenPG").val(),
    //            clientTokenPG: $("#ClientTokenPG").val(),
    //            CCCode: $("#CCCodeCVV").val(),
    //            IsCardDetailToSave: $("#SaveCDChkBox").val(),
    //            ProgramAccountIdSelected: $("#ProgramAccountIdSelected").val(),
    //            IsCardSelectionFromDropdown: $("#IsCardSelectionFromDropdown").val() == "1" ? true : false,
    //            IsNewCardTransaction: $("#IsNewCardTransaction").val() == "1" ? true : false
    //        };
    //        $.post("/Benefactor/ReloadAmountRequest",
    //            model,
    //            function (data) {
    //                $("#dvLoadingGif").hide();
    //                var titleMessage = '';
    //                if (data.Status) {
    //                    if ($("#IsBankPay").val() == "1") {
    //                        if ($("#hdnUserType").val() !== '' && $("#hdnUserType").val() === 'basic user') {
    //                            titleMessage = 'Your payment request has been received. Payment will be reflected in your account soon.';
    //                        }
    //                        else {
    //                            titleMessage = 'Payment request for the selected user has been received. Payment will be reflected in user\'s account soon.';
    //                        }
    //                    }
    //                    else {
    //                        if ($("#hdnUserType").val() !== '' && $("#hdnUserType").val() === 'basic user') {
    //                           // titleMessage = 'Balance is successfully added in your account.';
    //                            titleMessage = 'Your Transactions is being processed.When your Transaction clears, we will send a Notification.';

    //                        }
    //                        else {
    //                          //  titleMessage = 'Balance is successfully added in user\'s account.';
    //                            titleMessage = 'Your Transactions is being processed.When your Transaction clears, we will send the account holder a Notification.';
                                
    //                        }
    //                    }
    //                    swal({
    //                        title: titleMessage,
    //                        icon: "success"
    //                    }, function () {
    //                        window.location.href = window.location.href;
    //                    });

    //                    $("#dvLoadingGif").hide();
    //                }
    //                else {
    //                    titleMessage = 'There is some issue in processing the request. Please try again later.';

    //                    swal({
    //                        title: titleMessage,
    //                        icon: "error"
    //                    });
    //                    $("#dvLoadingGif").hide(); return false;
    //                }
    //            });
    //    }
    //    else {
    //        swal({
    //            title: 'Please select the payment bank or card.',
    //            icon: "error"
    //        });
    //        return false;
    //    }
    //    return false;
    //});
    $(document).on('click', '.nameTab', function () {
        $("#dvLoadingGif").show();
        $(".nameTab").removeClass('active');
        var idName = $(this).attr('id').replace('tab_', '');
        var canviewtransaction = $(this).attr('data-CanViewTransaction').replace('tab_', '').toString();
        if (canviewtransaction == "" || canviewtransaction == "False" || canviewtransaction == "false") {
            canViewTransactioCheckBool = false;
        }
        else if (canviewtransaction == "True" || canviewtransaction == "true") {
            canViewTransactioCheckBool = true;
        }
        $("#ActivelinkedId").val(idName);
        $("#ReloadUserId").val(idName);
        var stringElement = "tab_" + idName;
        $("#" + stringElement).addClass('active');
        GetUserProgramId();
        $.ajax({
            method: 'GET',
            url: '/Benefactor/GetBalanceReloadViewComponent',
            data: { id: $("#ReloadUserId").val(), reloadRequestId: 0, programId: $("#ProgramId").val(), LoggedId: 0 }
        }).done(function (data, statusText, xhdr) {
            $("#spnRefreshBalanceReload").html(data);
            $.validator.unobtrusive.parse($("#spnRefreshBalanceReload"));
            CheckCurrentBalanceUser(canViewTransactioCheckBool);
            CheckReloadRuleUser();
            RefreshFormContent();
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        });

        return false;
    });

    CheckReloadRuleUser();
    GetUserProgramId();
    CheckCurrentBalanceUser(canViewTransactioCheckBool);

    $(document).on('click', '.clsAddBankAcc', function () {
        $('#exampleModal').modal('hide');
        $('#addbankpopup').modal('show');
      //  $('#dvAddBankAccount').show();
        $('#AccountNumber').focus();
        $('#frmAddBankform')[0].reset();
    });
    $(document).on('click', '#btnaddcardpopuprule', function () {
       
        $('#addcardpopupforrule').modal('show');
      
    });

    $(document).on('click', '#btnAddPaymentViaCard_rule', function () {
        var bankCount = $("#hdnExistingCard").val();
        if (bankCount != '' && parseInt(bankCount) > 0) {
            $('#dvChooseCardforSubscriptionrule').show();
            $("#IsCardSelectionFromDropdown").val("1");
            $("#IsNewCardTransaction").val("0");
            $("#spnErrorCardForRule").hide();
            $("#btnSetAutoReload").show();
            $('#spnErrorBankAccountForRule').hide();
        }
        else {
            alert("ss");
            $("#spnErrorCardForRule").show();
            $("#btnSetAutoReload").hide();
            $("#IsCardSelectionFromDropdown").val("0");
            $("#IsNewCardTransaction").val("1");
            //if ($("#hdnUserType").val() == "basic user") {
            //    $("#dvAddPaymentCard").show();
            //}
            //else {
            //    $('#PaymentCardGateway').modal();
            //}
        }
        $("#company").val('');
        $("#isCardDetailSave").prop("checked", false);
        $('#dvChooseBankforsubscriptionrule').hide();
       // $('#dvAddBankAccount').hide();
       // $('#frmAddBankform')[0].reset();
        $("#IsBankPay").val("0");

    });
    $(document).on('click', '#btnclose', function () {
        $('#myModal').hide();
    });
    
    $(document).on('click', '#btnpayviabank_rule', function () {
        var bankCount = $("#hdnExistingBank").val();
        if (bankCount != '' && parseInt(bankCount) > 0) {

            $('#dvChooseBankforsubscriptionrule').show();
            $('#spnErrorBankAccountForRule').hide();
            $("#btnSetAutoReload").show();
            $("#spnErrorCardForRule").hide();
        }
        else {
            $('#spnErrorBankAccountForRule').show();
            $("#btnSetAutoReload").hide();
        }
        $('#AccountNumber').focus();
     //   $('#frmAddBankform')[0].reset();
        /* For hiding Card selection*/
        $("#dvChooseCardforSubscriptionrule").hide();
        $("#company").val('');
        $("#isCardDetailSave").prop("checked", false);
        $("#IsBankPay").val("1");
        hideCardContent();
    });
    $(document).on('click', '.payViaBank', function () {
        var bankCount = $("#hdnExistingBank").val();
        if (bankCount != '' && parseInt(bankCount) > 0) {
            $('#dvChooseBank').show();
        }
        else {
            $('#dvAddBankAccount').show();
        }
        $('#AccountNumber').focus();
        $('#frmAddBankform')[0].reset();
        /* For hiding Card selection*/
        $("#dvChooseCard").hide();
        $("#company").val('');
        $("#isCardDetailSave").prop("checked", false);
        $("#IsBankPay").val("1");
        hideCardContent();
    });

    $(document).on('click', '.payviacard', function () {
        //debugger;
        var bankCount = $("#hdnExistingCard").val();
        if (bankCount != '' && parseInt(bankCount) > 0) {
      //      $('#dvChooseCard').show();
            $("#IsCardSelectionFromDropdown").val("1");
            $("#IsNewCardTransaction").val("0");
        }
        else {
         //   alert("hi");
            $("#IsCardSelectionFromDropdown").val("0");
            $("#IsNewCardTransaction").val("1");
            if ($("#hdnUserType").val() == "basic user") {
                $("#dvAddPaymentCard").show();
            }
            else {
               // alert("dd");
         //       $('#dvChooseCard').show();
                $('#PaymentCardGateway').modal('show');
            }
        }
        $("#company").val('');
        $("#isCardDetailSave").prop("checked", true);
        $('#dvChooseBank').hide();
        $('#dvAddBankAccount').hide();
        $('#frmAddBankform')[0].reset();
        $("#IsBankPay").val("0");

    });

    $(document).on('click', '.openPaymentGateway', function () {
        if ($("#hdnUserType").val() == "basic user") {
            $('#exampleModal').modal('hide');
            $('#PaymentCardGateway1').modal(); 
            $("#dvAddPaymentCard").show();
        }
        else {
          //  alert("sss");
           $('#exampleModal').modal('hide');
            $('#PaymentCardGateway').modal();
        }
        $("#company").val('');
   //     $("#isCardDetailSave").prop("checked", false);
        $("#isCardDetailSave").prop("checked", true);
        $('#dvChooseBank').hide();
        $('#dvAddBankAccount').hide();
        $('#frmAddBankform')[0].reset();
    });

    $(document).on('click', '#btnCancelAddBank', function () {
        $('#frmAddBankform')[0].reset();
        $('#dvAddBankAccount').hide();
    });

    $(document).on('change', '#CardIdSelected', function () {
        $("#ClientTokenPG").val($(this).val());
        hideCardContent();
        $('#btnReloadAmount').show();
        $("#IsCardSelectionFromDropdown").val("1");
        $("#IsNewCardTransaction").val("0");
        $("#SaveCDChkBox").val('1');
    });

    $(document).on('click', '#btnAddBankAccount', function () {
        if ($("#frmAddBankform").validate() && $("#frmAddBankform").valid()) {
            var ipAddress = '';
            $.getJSON("https://api.ipify.org/?format=json", function (e) {
                ipAddress = e.ip
            });
            $("#dvLoadingGif").show();
            var BankDetailModel = {
                AccountNumber: $("#AccountNumber").val().trim(),
                AccountTitle: $("#AccountTitle").val().trim(),
                AccountNickName: $("#AccountNickName").val().trim(),
                BankName: $("#BankName").val().trim(),
                RoutingNumber: $("#RoutingNumber").val().trim(),
                Comments: $("#Comments").val().trim(),
                AccountType: $("#AccountType").val().trim(),
                //ACHType: $("#ACHType").val().trim(),
                ClientIPAddress: ipAddress,
                RequesteeUserId: $("#ReloadUserId").val().trim(),
                //CardDetailIdI2C: $("#hdnCardDetilId").val().trim(),
                IsCardDetailToSave: 1,
                ProgramAccountIdSelected: $("#ProgramAccountIdSelected").val()
            };
            $.post("/Benefactor/PostBankDetailInformation",
                BankDetailModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" && data.data === "1") {
                        swal({
                            title: data.dataMessage,
                            icon: "success"
                        });
                        $.ajax({
                            url: "/Benefactor/GetCardDropdownList1",
                            type: 'GET',
                            dataType: 'json', // added data type
                            data: { id: $("#ReloadUserId").val() }
                        }).done(function (data, statusText, xhdr) {


                            var ddlCard = $("[id*=ddlcardslection]");
                            ddlCard.empty().append('<option value=""> -- Select payment method-- </option>');
                            $.each(data.data, function () {
                                ddlCard.append($("<option></option>").val(this['Value']).html(this['Text']));
                            });

                        

                         //   $("#ddlcardslection").val(clientToken);
                            $("#spnRefreshBankAccount").html(data);
                            $.validator.unobtrusive.parse($("#spnRefreshBankAccount"));

                        }).fail(function (xhdr, statusText, errorText) {
                            swal({
                                title: statusText,
                                icon: "error"
                            });

                        });

                        $("#dvLoadingGif").hide();

                    }
                    else {
                        swal({
                            title: data.dataMessage,
                            icon: "error"

                        });
                        $("#dvLoadingGif").hide();
                    }
                });
            $("#addbankpopup").modal('hide');
          //  window.location.href = window.location.href;
        }
        return false;
    });

    $(document).on('change', '#BankIdSelected', function () {
        var bankValueSelected = $('option:selected', this).val();
        if (bankValueSelected !== '') {
            $.get("/Benefactor/GetBankAccountDropdownList", { accountSerialNo: bankValueSelected }, function (data) {
                if (data.data != null) {
                    $("#SelectedBankStatus").val(data.data.Status ? "1" : "0");
                    if (data.data.Status == false) {
                        $("#dvVerifyBankAccount").show();
                        $('#btnReloadAmount').hide();
                    }
                    else {
                        $("#dvVerifyBankAccount").hide();
                        $('#btnReloadAmount').show();
                    }
                    $("#spnErrorBankSelect").hide();
                }
            });
            $("#AccountReloadSrNo").val(bankValueSelected);
        }
        else {
            $("#dvVerifyBankAccount").hide();
            $("#AccountReloadSrNo").val('');
            $("#spnErrorBankSelect").show();
            $("#SelectedBankStatus").val('');
        }
    });

    $(document).on('click', '#btnVerifyBankAcc', function () {
        
        //    if ($("#frmVerifyBankform").validate() && $("#frmVerifyBankform").valid()) {
    
        var bankIdSelection = $("#ddlcardslection").val();
        var bankIdSelectionforrule = $("#ddlcardslectionforrule").val();
        var bankId = ""; var bank_name = "";
        if (bankIdSelection != '') {
            bankId = bankIdSelection;
            bank_name = $("#ddlcardslection option:selected").text();
        }
        else {
            bankId = bankIdSelectionforrule;
            bank_name = $("#ddlcardslectionforrule option:selected").text();
        }
      //  debugger;
        $.post("/Benefactor/VerifyBankAccount", { accountSrNo: bankId, bankName: bank_name.split('(')[0].trim(), amountOne: $("#AmountOne").val(), amountTwo: $("#AmountTwo").val() }, function (data) {
            if (data !== null) {
                if (data.Status) {
                    swal({
                        title: data.Message,
                        icon: "success"
                    });
                    $.ajax({
                        //method: 'GET',
                        //url: '/Benefactor/GetBankListViewComponent',
                        //data: { id: $("#ReloadUserId").val(), reloadRequestId: $("#ReloadRequestId").val(), programId: $("#ProgramId").val() }
                        url: "/Benefactor/GetCardDropdownList1",
                        type: 'GET',
                        dataType: 'json', // added data type
                        data: { id: $("#ReloadUserId").val() }
                    }).done(function (data, statusText, xhdr) {
                        var ddlCard = $("[id*=ddlcardslection]");
                        ddlCard.empty().append('<option  value=""> -- Select payment method-- </option>');
                        $.each(data.data, function () {
                            ddlCard.append($("<option></option>").val(this['Value']).html(this['Text']));
                        });
                        $("#spnRefreshBankAccount").html(data);
                        $.validator.unobtrusive.parse($("#spnRefreshBankAccount"));
                        $("#BankIdSelected").val(bankIdSelection);
                        $("#AccountReloadSrNo").val(bankIdSelection);
                        $("#SelectedBankStatus").val("1");
                        $('#btnReloadAmount').show();
                        return false;
                    }).fail(function (xhdr, statusText, errorText) {
                        swal({
                            title: statusText,
                            icon: "error"
                        });
                        $('#btnReloadAmount').hide();
                        return false;
                    });
                    return false;
                }
                else {
                    swal({
                        title: data.data.Message,
                        icon: "error"
                    });
                    return false;
                }
            }
            return false;
        });
        //  }
        $("#addverifybankpopup").modal('hide');
        $("#ddlcardslection").val('');
        $("#ddlcardslectionforrule").val('');
        return false;
       
    });
});

var CheckCurrentBalanceUser = function (canViewTransactioCheckBool) {
    var id = $("#ReloadUserId").val();
    if (canViewTransactioCheckBool) {
        $.ajax({
            type: "GET",
            url: "/Benefactor/GetCurrentBalance/",
            data: { 'id': id, 'programId': $("#ProgramId").val() },
            dataType: "json",
            contentType: 'application/json; charset=utf-8',
            success: function (data) {
                
                if (data.data.length > 0) {
                    var balance = data.data;
                    $("#ulCurrentBalance").html('');
                    $.each(balance, function (index, value) {
                        var contentLi = '';
                        if (balance[index].DataKeyType == 1)
                            contentLi = balance[index].DataValue + ' Meal Passes';
                        else if (balance[index].DataKeyType == 2) contentLi = balance[index].DataValue + ' Flex Points';
                        else if (balance[index].DataKeyType == 3) contentLi = "$" + balance[index].DataValue + ' Campus Cash';
                        else
                            contentLi = "$" + balance[index].DataValue + ' Campus Cash';
                        $("#ulCurrentBalance").append('<li>' + contentLi + '</li>');
                    });

                }
                //var number = data.data.toString().length < 2 ? "0" + data.data.toFixed(2) : data.data.toFixed(2);
                //$("#currentAmount").html("$" + number);
            },
            error: function () {
            }
        });
    } else {
        $("#ulCurrentBalance").html('');
        $("#ulCurrentBalance").append('<li>Balance is hidden due to its privacy</li>');
    }
};

var CheckReloadRuleUser = function () {

    var id = $("#ReloadUserId").val();

  
    $.ajax({
        type: "GET",
        url: "/Benefactor/GetReloadRuleOfUser/",
        data: { 'id': id },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
          //  alert("s");
            if (data.data !== null) {
                var selectedId = "";
                var rulebankid = data.data.i2cBankAccountId;
               
                var rulecardid = data.data.CardId;
               
                if (rulebankid == null) {
                    selectedId = rulecardid;
                }
                else {
                    selectedId = rulebankid;
                }
                if (data.data.isAutoReloadAmount) {
                    $("#lblpaymentmethod").text('Payment Method');       
                    $("#btnCancelAutoReload").css('display', 'block');
                    $("#IsAutoReload").prop('checked', 'checked');
                    $("#ReloadAmount").val(data.data.reloadAmount);
                    $("#ddlcardslection").val(selectedId);
                    $("#AutoReloadAmount").val(data.data.reloadAmount);
                    $("#CheckDroppedAmount").val(data.data.userDroppedAmount);
                    var lblClass = ".labelAutoReload" + data.data.userDroppedAmount;
                    $(lblClass).addClass("active1");
                    $("#dvAutoReloadOptions").css('pointer-events', '');
                    $('#CheckDroppedAmountdd').val(data.data.userDroppedAmount);
                    $('#CheckDroppedAmountdd').attr("disabled", false);
                    $('#AutoReloadAmount').attr("disabled", false);
                    document.querySelector('#btnReloadAmount').innerHTML = 'Pay and Save Auto Reload Rule';
                    $('#btnaddcardpopuprule').show();
                    

                    // check ipg for selected card

                    $.ajax({

                        url: '/Benefactor/CheckIpgTransactionId',
                        type: 'GET',
                        dataType: 'json',
                        data: { clientToken: rulecardid },
                        success: function (data) {
                            var id = $("#txtIpg").val(data);

                        },
                        error: function () {

                        }
                    });
                  
                  
               
    //
                }
                else {
                    $("#lblpaymentmethod").text('My Card');
                    $('#CheckDroppedAmountdd').attr("disabled", true);
                    $('#AutoReloadAmount').attr("disabled", true);
                    document.querySelector('#btnReloadAmount').innerHTML = 'Add Money';
                    $("#btnCancelAutoReload").css('display', 'none');
                    $('#btnaddcardpopuprule').hide();
                }
            }
            else {
              //  alert("s");
                $("#btnCancelAutoReload").css('display', 'none');
            }
        },
        error: function () {
        }
    });

   
};

var GetUserProgramId = function () {

    var id = $("#ReloadUserId").val();
    $.ajax({
        type: "GET",
        url: "/Benefactor/GetUserProgramByUserId/",
        data: { 'id': id },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.data !== null && data.data > 0) {
                $("#ProgramId").val(data.data);
            }
        },
        error: function () {
        }
    });
};

var RefreshFormContent = function () {
    $('.input').val('');
    $('.autoLabel,.amount_label').removeClass('active');
    $('#IsAutoReload').prop('checked', false);
};
var hideCardContent = function () {
    $('#frmPaymentFD')[0].reset();
   // $('#frmPaymentFD')[0]
    $("#dvAddPaymentCard input[type='tel']").val('');
    $("#dvAddPaymentCard").hide();

};

$(document).on('change', '#ddlLinkedUserId', function () {
    $("#dvLoadingGif").show();
    $.ajax({
        method: 'GET',
        url: '/Benefactor/GetLinkedUserDetails',
        data: { id: $("#ddlLinkedUserId").val() }
    }).done(function (data, statusText, xhdr) {
       // debugger;
        
        $("#bitepaybalance").val(data.balance);
        $("#dvLoadingGif").hide();
        if (data.data !== null) {
            var selectedId = "";
            var rulebankid = data.data.i2cBankAccountId;

            var rulecardid = data.data.CardId;

            if (rulebankid == null) {
                selectedId = rulecardid;
            }
            else {
                selectedId = rulebankid;
            }
            if (data.data.isAutoReloadAmount) {
                $("#lblpaymentmethod").text('Payment Method');
                $("#btnCancelAutoReload").css('display', 'block');
                $("#IsAutoReload").prop('checked', 'checked');
                $("#ReloadAmount").val(data.data.reloadAmount);
                $("#ddlcardslection").val(selectedId);
                $("#AutoReloadAmount").val(data.data.reloadAmount);
                $("#CheckDroppedAmount").val(data.data.userDroppedAmount);
                var lblClass = ".labelAutoReload" + data.data.userDroppedAmount;
                $(lblClass).addClass("active1");
                $("#dvAutoReloadOptions").css('pointer-events', '');
                $('#CheckDroppedAmountdd').val(data.data.userDroppedAmount);
                $('#CheckDroppedAmountdd').attr("disabled", false);
                $('#AutoReloadAmount').attr("disabled", false);
                document.querySelector('#btnReloadAmount').innerHTML = 'Pay and Save Auto Reload Rule';
                $('#btnaddcardpopuprule').show();
                // check ipg for selected card

                $.ajax({

                    url: '/Benefactor/CheckIpgTransactionId',
                    type: 'GET',
                    dataType: 'json',
                    data: { clientToken: rulecardid },
                    success: function (data) {
                        var id = $("#txtIpg").val(data);

                    },
                    error: function () {

                    }
                });
            }
            else {
                $("#lblpaymentmethod").text('My Card');
                $('#CheckDroppedAmountdd').attr("disabled", true);
                $('#AutoReloadAmount').attr("disabled", true);
                document.querySelector('#btnReloadAmount').innerHTML = 'Add Money';
                $("#btnCancelAutoReload").css('display', 'none');
                $('#btnaddcardpopuprule').hide();
                $("#ddlcardslection").val('');
                $("#IsAutoReload").prop('checked', false);
                $("#ReloadAmount").val('');
                $('#CheckDroppedAmountdd').val('10');
            }
           
        }
       
        else {

            $("#btnCancelAutoReload").css('display', 'none');
        }
        })
   
  
});



