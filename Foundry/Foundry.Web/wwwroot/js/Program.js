var programAccount = [];
var selectedMerchant = [];
var merchantRules = [];
var oModalVersionTable;
var tab;
$(document).ready(function () {
    $(".hrefMerchant").click(function (e) {
        if (($("#hdnProgramId").val() === "" || $("#hdnProgramId").val() === "3000")) {
            swal({
                title: "Please add program detail before moving further.",
                icon: "error"
            });
            return false;
        }
    });
    $(document).on("click", ".add-accountholder", function () {
        var userId = $(this).attr('data-id');
        GetTabViewComponent("5", userId);
    });
    $(document).on("click", ".edit-accountholder", function () {
        var userId = $(this).attr('data-id');
        GetTabViewComponent("5", userId);
    });

    $(document).on("click", "#rdct-import", (function (e) {
        $("#dvLoadingGif").show();
        $.ajax({
            method: 'GET',
            url: '/Program/UploadAccountHolderList',
        }).done(function (data, statusText, xhdr) {

            $("#spnProgramTab").html(data);
            $('.nav-tabs a[href="#program-info"]').tab('show');

            $("#dvLoadingGif").hide();
            $("#program-info").addClass("show");
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: statusText,
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }));
    GetTabViewComponent("1", "", "", "", "");

    $(".progtab").click(function (e) {
        if (($("#hdnProgramId").val() === "" || $("#hdnProgramId").val() === "3000") && $(this).attr('id') !== 'tab_1') {
            swal({
                title: "Please add program detail before moving further.",
                icon: "error"
            });
        }
        else {
            $(".progtab").removeClass('active');
            var idName = $(this).attr('id').replace('tab_', '');
            var id = $(this).attr('data-id') !== undefined ? $(this).attr('data-id') : '';
            GetTabViewComponent(idName, id);
            var stringElement = "tab_" + idName;
            $(".nav-link").removeClass('active');
            $("#" + stringElement).addClass('active');
            $(".nav-tabs .nav-item.dropdown, .nav-tabs .dropdown-menu").removeClass("show");
        }
        return false;
    });

    $(document).on("click", ".add-plan", function () {
        var planid = $(this).attr('data-id');
        GetTabViewComponent("3", planid);
    });
    $(document).on("click", ".edit-plan", function () {
        var planid = $(this).attr('data-id');
        GetTabViewComponent("3", planid);
    });


    $(document).on("click", ".btn-add-plan", function () {
        var retfalse = 0;
        if (!CheckForStartEndDate()) {
            retfalse = 1;
        }
        if ($("#form").validate() && $("#form").valid()) {
            if (retfalse === 1) {
                $('#EndDate').focus();
                return false;
            }
            $("#dvLoadingGif").show();
            var progAcc = $('#selectedProgramAccount option:selected');
            var pId = $("#id").val();
            $(progAcc).each(function (index, brand) {
                var data = $(this).val();
                programAccount.push(data);
            });
            var PlanModel = {
                id: pId,
                name: $("#name").val(),
                programId: $("#hdnProgramId").val(),
                noOfMealPasses: 0,
                noOfFlexPoints: 0,
                startDate: $("#startDate").val(),
                endDate: $("#endDate").val(),
                startTime: ConvertTimeformat("24", $("#startTime").val()),
                endTime: ConvertTimeformat("24", $("#endTime").val()),
                description: $('#description').val(),
                planId: $('#planId').val(),
                clientId: $('#clientId').val(),
                selectedProgramAccount: programAccount,
                Jpos_PlanEncId: $("#Jpos_PlanEncId").val()
            };
            programAccount = [];
            $.post("/Program/PostPlan", PlanModel, function (data) {
                $("#dvLoadingGif").hide(); if (data.data !== "" || data.data !== "0") {
                    var title = ''; if ($("#id").val() !== '' && $("#id").val() !== '0' && $("#id").val() !== '3000') { title = "Plan details has been updated successfully."; } else { title = "Plan details has been submitted successfully."; }
                    swal({ title: title, icon: "success" }, function () { tab = '2'; var stringElement = "tab_" + tab; $("#" + stringElement).addClass('active'); GetTabViewComponent(tab, "", "", "", ""); });
                } else {
                    swal({ title: "There is some issue in processing! Please try again later.", icon: "error" });
                }
            });
            return false;
        }
        else {
            $("#form").validate();
        }
    });
    //hide date picker on date selection
    $(document).on('change', '.datetimepicker', function (ev) {
        $(this).datepicker('hide');
        CheckForStartEndDate();
    });
    $("#addAccount").click(function () {
        $('.tab-content').load("/Program/AddAccount", function () {

        });
    });
    $(document).on('change', '.checkbox input[type="checkbox"]', function () {
        var checkboxes = $(".checkbox input[type='checkbox']");
        if (!checkboxes.is(":checked")) {
            $(".multiselect-native-select").next().show();
        }
        else { $(".multiselect-native-select").next().hide(); }
    });

    /* Added for ProgramInfo*/
    /***Starts****/
    $(document).on('click', '#btnProgramDetail', function () {
        if ($("#programInfoForm").validate() && $("#programInfoForm").valid()) {
            $("#dvLoadingGif").show();
            var customFieldsJsonContent = [];
            var totalCountCustomFields = parseInt($('.addCustomFields').attr('value'));
            for (var i = 1; i <= totalCountCustomFields; i++) {
                if ($("#CustomFieldName_" + i).val() !== undefined && $("#CustomFieldName_" + i).val() !== '') {
                    customFieldsJsonContent.push({
                        CustomFieldUniqueColumnID: $("#CustomFieldName_" + i).val().trim().replace(/\s/g, '-'),
                        CustomFieldName: $("#CustomFieldName_" + i).val().trim(),
                        CustomFieldDataType: $("#FieldType_" + i).val().trim(),
                        CustomFieldLength: $("#CustomFieldLength_" + i).val(),
                        CustomFieldIsRequired: $("#CustomFieldRequirement_" + i).val() === "" ? "false" : $("#CustomFieldRequirement_" + i).val()
                    });
                }
            }
            var ProgramDetailModel = {
                AccountHolderGroups: $("#AccountHolderGroups").val(),
                AccountHolderUniqueId: $("#AccountHolderUniqueId").val(),
                address: $("#hdnNameAddrs").val() + ", " + $("#hdnRoute").val(),  //+ $("#hdnStreet").val() + ", "
                city: $("#city").val(),
                country: $("#country").val(),
                customErrorMessaging: $("#customErrorMessaging").val(),
                customInputMask: $("#customInputMask").val(),
                customInstructions: $("#customInstructions").val(),
                customName: $("#customName").val(),
                ProgramEncId: $("#hdnProgramId").val(),
                ProgramCodeId: $("#ProgramCodeId").val(),
                name: $("#name").val(),
                programCustomFields: JSON.stringify(customFieldsJsonContent),
                ProgramTypeId: $("#ProgramTypeId").val(),
                state: $("#state").val(),
                timeZone: $("#timeZone").val(),
                website: $("#website").val(),
                zipcode: $("#zipcode").val(),
                OrganizationEncId: $("#hdnPrimaryOrgId").val(),
                IsAllNotificationShow: $("#IsAllNotificationShow").is(":checked"),
                IsRewardsShowInApp: $("#IsRewardsShowInApp").is(":checked"),
                JPOS_IssuerId: $("#JPOS_IssuerId").val()
            };

            $.post("/Program/PostProgramInformation",
                ProgramDetailModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" || data.data !== "0") {
                        if ($("#hdnProgramId").val() === '3000') {
                            var hrefmerchant = $(".hrefMerchant").attr('href');
                            hrefmerchant = hrefmerchant.replace("ppId=3000", "ppId=" + data.data) + "&ppN=" + data.programName;
                            $(".hrefMerchant").attr('href', hrefmerchant);
                            swal({
                                title: "Program detail is saved successfully!",
                                icon: "success"
                            });
                        }
                        else {
                            swal({
                                title: "Program detail is updated successfully!",
                                icon: "success"
                            });
                        }
                        tab = '2';
                        var stringElement = "tab_" + tab;
                        $("#" + stringElement).addClass('active');
                        //GetTabViewComponent(tab);
                        if ($("#hdnProgramId").val() === '3000') {

                            $("#hdnProgramName").val(data.programName);
                            $("#hdnProgramId").val(data.data);
                            var poId = 'poId=' + $.urlParam('poId');
                            var poN = 'poN=' + $.urlParam('poN');
                            var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname.replace('3000', data.data) + '?' + poId + '&ppN=' + data.programName + '&' + poN;
                            window.history.pushState({ path: newurl }, '', newurl);
                        }
                    }
                    else {
                        swal({
                            title: "Currently unable to process the request! Please try again later.",
                            icon: "error"
                        });
                    }
                });
            return false;
            //ajaxcall
        }
        else {
            $("#programInfoForm").validate();
        }
    });
    $(document).on("click", "#btnSaveApplyOverride", function () {
        if ($("#formsitelevel").validate() && $("#formsitelevel").valid()) {
            if ($("#siteLevelBitePayRatio").val() == '') {
                $("#spnErrorsiteLevelBitePayRatio").show();
                return false;
            }
            if ($("#siteLevelDcbFlexRatio").val() == '') {
                $("#spnErrorsiteLevelDcbFlexRatio").show();
                return false;
            }
            if ($("#siteLevelUserStatusVipRatio").val() == '') {
                $("#spnErrorsiteLevelUserStatusVipRatio").show();
                return false;
            }
            if ($("#siteLevelUserStatusRegularRatio").val() == '') {
                $("#spnErrorsiteLevelUserStatusRegularRatio").show();
                return false;
            }
            if ($("#siteLevelFirstTransactionBonus").val() == '') {
                $("#spnErrorsiteLevelFirstTransactionBonus").show();
                return false;
            }
          
            $("#dvLoadingGif").show();
           
            var Id = $("#id").val();
            var SiteLevelOverrideSettingViewModel = {
                id: $("#hdnSitesettingid").val(),
                programId: $("#txtpId").val(),
                siteLevelBitePayRatio: $("#siteLevelBitePayRatio").val(),
                siteLevelDcbFlexRatio: $("#siteLevelDcbFlexRatio").val(),
                siteLevelUserStatusVipRatio: $("#siteLevelUserStatusVipRatio").val(),
                siteLevelUserStatusRegularRatio: $("#siteLevelUserStatusRegularRatio").val(),
                FirstTransactionBonus: $("#siteLevelFirstTransactionBonus").val()
            };
            $.post("/Program/PostSiteLevelOverrideSettings",
                SiteLevelOverrideSettingViewModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                   
                    
                   // if (data.data !== "" && data.data !== "0") {
                       
                        var title = ''; if ($("#hdnSitesettingid").val() !== '' && $("#hdnSitesettingid").val() !== '0' && $("#hdnSitesettingid").val() !== '3000') { title = "Site Level Override Settings has been updated successfully."; } else { title = "Site Level Override Settings has been submitted successfully."; }
                        swal({ title: title, icon: "success" }, function () { tab = '23'; var stringElement = "tab_" + tab; $("#" + stringElement).addClass('active'); GetTabViewComponent(tab, "", "", "", ""); });
                  //  }
                   // else { swal({ title: data.message, icon: "error" }); }
                });
            return false;
        }
        else {
            $("#formsitelevel").validate();
        }
    });
    $(document).on('click', '.addCustomFields', function (e) {
        e.preventDefault();
        var currIndex = parseInt($(this).attr('value'));
        if (currIndex === 10) {
            swal({
                title: "You can add up to ten fields only.",
                icon: "error"
            });
            $(this).hide();
        }
        else {
            var newIndex = parseInt($(this).attr('value')) + 1;
            var cloneelement = $("#select-accounts-action_0").clone().attr("id", "select-accounts-action_" + newIndex);
            cloneelement.find("*[id]").each(function () {
                $(this).attr("id", $(this).attr("id").replace("0", newIndex.toString()));
                $(this).attr("name", $(this).attr("name").replace("0", newIndex.toString()));
            });
            cloneelement.append($('<a class="delete-action-mealp deleteCustomField" href="#"></a>'));
            cloneelement.show();
            cloneelement.find(".customTextBox").val('');

            cloneelement.appendTo("#additionalaccounts-action");
            $(this).attr('value', newIndex);
            if (newIndex === 10) { $(this).hide(); }

        } return false;
    });

    $(document).on('click', ".deleteCustomField", function (e) {
        e.preventDefault();
        $(this).closest(".accounts-action").remove();
        var currIndex = parseInt($('.addCustomFields').attr('value'));
        var newIndex = currIndex - 1;
        $('.addCustomFields').attr('value', newIndex);
        if (newIndex < 10) {
            $('.addCustomFields').show();
        }
        var holidayhrsdate = $('.accounts-action');
        $(holidayhrsdate).each(function (index, brand) {
            var oldindex = $(this).attr("id").split('_')[1];
            $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
            $(this).find("*[id]").each(function () {
                $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
                $(this).attr("name", $(this).attr("name").replace(oldindex.toString(), index.toString()));//.replace(index.toString(), oldindex.toString()));
            });
        });
    });

    /***Ends****/

    /* Added for Program Account*/
    /***Starts****/
    $(document).on("click", ".add-prog-acc", function () {
        var id = $(this).attr('data-id');
        GetTabViewComponent("8", id, "", "", "");
    });
    $(document).on("click", ".edit-prog-acc", function () {
        var id = $(this).attr('data-id');
        GetTabViewComponent("8", id, "", "", "");
    });
    $(document).on("change", "#accountTypeId", function () {
        $("#hdnAccountTypeAccount").val($(this).val());
        if ($(this).val() === "1") {
            $(".dv-pass-type").show();
            $(".row-exchange").show();
            $(".dv-exchange").show();
            $(".row-flex").hide();
            $(".row-vpl").hide();
            $("#lblAccountTypeName").html("Meals");
        }
        else if (parseInt($(this).val()) === 2) {
            $(".dv-pass-type").hide();
            $(".dv-initial-balance").show();
            $(".row-flex").show();
            $(".row-vpl").hide();
            $(".row-pass").hide();
            $(".row-exchange").hide();
            $("#lblAccountTypeName").html("Flex points");
        }
        else if (parseInt($(this).val()) === 3) { $(".dv-pass-type").hide(); $(".dv-initial-balance").hide(); $(".row-flex").hide(); $(".row-vpl").show(); $(".row-pass").hide(); $(".row-exchange").hide(); }
        else { $(".dv-pass-type").hide(); $(".dv-initial-balance").hide(); $(".row-flex").hide(); $(".row-pass").hide(); $(".row-vpl").hide(); $(".row-exchange").hide(); }
        ResetProgramAccount(1);
    });
    $(document).on("change", "#passType", function () {
        if ($(this).val() === "2") {
            $(".dv-initial-balance").show();
            $(".row-pass").show();
            $(".dv-block").show();
            $(".dv-reset-period").hide();
            $(".dv-reset-day-week").hide();
            $(".dv-reset-time").hide();
            $(".dv-reset-date-month").hide();
            $(".dv-maxpass-day").show();
            $(".dv-maxpass-week").show();
        }
        else if (parseInt($(this).val()) > 2) {
            $(".row-pass").show();
            $(".dv-block").hide();
            $(".dv-maxpass-day").hide();
            $(".dv-maxpass-week").hide();
            $(".dv-initial-balance").show();
            $(".dv-reset-period").show();
        }
        else {
            $(".dv-block").hide();
            $(".dv-initial-balance").hide();
            $(".dv-reset-period").hide();
            $(".dv-reset-day-week").hide();
            $(".dv-reset-time").hide();
            $(".dv-maxpass-day").hide();
            $(".dv-maxpass-week").hide();
        }
        $('#isPassExchangeEnabled').change();
        ResetProgramAccount(2);
    });

    $(document).on("change", "#resetPeriodType", function () {
        if ($(this).val() === "1") { $(".dv-reset-time").show(); $('#resetTime.timepicker').timepicker('remove'); $('#resetTime.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 }); $('#resetTime.timepicker').timepicker('update'); $(".dv-reset-date-month").hide(); $(".dv-maxpass-day").hide(); $(".dv-maxpass-week").hide(); $(".dv-reset-day-week").hide(); }
        else if (parseInt($(this).val()) === 2) { $(".dv-reset-day-week").show(); $(".dv-reset-time").show(); $('#resetTime.timepicker').timepicker('remove'); $('#resetTime.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 }); $('#resetTime.timepicker').timepicker('update'); $(".dv-maxpass-day").show(); $(".dv-reset-date-month").hide(); $(".dv-maxpass-week").hide(); }
        else if (parseInt($(this).val()) === 3) { $(".dv-reset-day-week").hide(); $(".dv-reset-time").hide(); $(".dv-reset-date-month").show(); $(".dv-maxpass-day").show(); $(".dv-maxpass-week").show(); }
        else { $(".dv-initial-balance").hide(); $(".dv-reset-period").hide(); $(".dv-reset-day-week").hide(); $(".dv-reset-time").hide(); $(".dv-maxpass-day").hide(); }
        ResetProgramAccount(3);
    });
    $(document).on("change", "#exchangeResetPeriodType", function () {
        if ($(this).val() === "1") {
            $(".dv-exreset-time").show();
            $('#exchangeResetTime.timepicker').timepicker('remove');
            $('#exchangeResetTime.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
            $('#exchangeResetTime.timepicker').timepicker('update');
            $(".dv-exreset-date-month").hide();
            $(".dv-exreset-day-week").hide();
        }
        else if (parseInt($(this).val()) === 2) {
            $(".dv-exreset-day-week").show();
            $(".dv-exreset-time").show();
            $('#exchangeResetTime.timepicker').timepicker('remove');
            $('#exchangeResetTime.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
            $('#exchangeResetTime.timepicker').timepicker('update');
            $(".dv-exreset-date-month").hide();
        }
        else if (parseInt($(this).val()) === 3) {
            $(".dv-exreset-day-week").hide();
            $(".dv-exreset-time").hide();
            $(".dv-exreset-date-month").show();
        }
        else {
            $(".dv-exreset-period").hide();
            $(".dv-exreset-day-week").hide();
            $(".dv-exreset-time").hide();
            $(".dv-exreset-date-month").hide();
        }
        ResetProgramAccount(5);
    });
    $(document).on("change", "#isPassExchangeEnabled", function () {
        if ($(this).val() === "True") {
            $(".dv-exchange").show();
        }
        else {
            $(".dv-exchange").hide();
            $(".dv-exreset-period").hide();
            $(".dv-exreset-day-week").hide();
            $(".dv-exreset-time").hide();
            $(".dv-exreset-date-month").hide();
        }
        ResetProgramAccount(4);
    });
    $(document).on("click", "#btnProgramAccountDetail", function () {
        if ($("#form").validate() && $("#form").valid()) {
            $("#dvLoadingGif").show();
            var Id = $("#id").val();
            var ProgramAccountModel = {
                id: Id,
                accountName: $("#accountName").val(),
                accountTypeId: $("#hdnAccountTypeAccount").val(),
                programId: $("#hdnProgramId").val(),
                passType: $("#passType").val(),
                intialBalanceCount: $("#intialBalanceCount").val(),
                resetPeriodType: $("#resetPeriodType").val(),
                resetDay: $("#resetDay").val(),
                resetTime: $("#resetTime").val(),
                maxPassUsage: $("#maxPassUsage").val(),
                isPassExchangeEnabled: $("#isPassExchangeEnabled").is(":checked") ? true : false,
                exchangePassValue: $("#exchangePassValue").val(),
                exchangeResetPeriodType: $("#exchangeResetPeriodType").val(),
                exchangeResetDay: $("#exchangeResetDay").val(),
                exchangeResetTime: $("#exchangeResetTime").val(),

                isRollOver: $("#isRollOver_Value").is(":checked") ? true : false,
                flexEndDate: $("#flexEndDate").val(),
                maxPassPerWeek: $("#maxPassPerWeek").val(),
                maxPassPerMonth: $("#maxPassPerMonth").val(),
                resetDateOfMonth: $("#resetDateOfMonth").val(),
                flexMaxSpendPerDay: $("#flexMaxSpendPerDay").val(),
                flexMaxSpendPerWeek: $("#flexMaxSpendPerWeek").val(),

                flexMaxSpendPerMonth: $("#flexMaxSpendPerMonth").val(),
                exchangeResetDateOfMonth: $("#exchangeResetDateOfMonth").val(),
                vplMaxBalance: $("#vplMaxBalance").val(),
                vplMaxAddValueAmount: $("#vplMaxAddValueAmount").val(),
                vplMaxNumberOfTransaction: $("#vplMaxNumberOfTransaction").val(),
                Jpos_ProgramAccountEncId: $("#Jpos_ProgramAccountEncId").val()
            };
            $.post("/Program/PostProgramAccount",
                ProgramAccountModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" || data.data !== "0") {

                        var title = '';
                        if ($("#id").val() !== '' && $("#id").val() !== '0' && $("#id").val() !== '3000') {

                            title = "Account details has been updated successfully.";
                        } else {
                            title = "Account details has been submitted successfully.";
                        }

                        swal({
                            title: title,
                            icon: "success"
                        }, function () {
                            if (parseInt($("#hdnAccountTypeAccount").val()) == 3) {
                                $("#liCardHolderAgreementMenu").css('display', 'block');
                            }
                            tab = '14';
                            var stringElement = "tab_" + tab;
                            $("#" + stringElement).addClass('active');
                            var accountTypeId = $("#accountTypeId").val() + ($("#accountTypeId").val() === "1" && $("#isPassExchangeEnabled").is(":checked") ? ",0" : "");
                            GetTabViewComponent(tab, "", "", accountTypeId, data.data);

                        });
                    }
                    else {
                        swal({
                            title: "There is some issue in processing! Please try again later.",
                            icon: "error"

                        });
                    }
                });
            return false;
        }
        else {
            $("#form").validate();
        }
    });
    $(document).on('click', '.select_all', function () {
        if (this.checked) {
            $('.merchantcheckbox').each(function () {
                this.checked = true;
            });
        } else {
            $('.merchantcheckbox').each(function () {
                this.checked = false;
            });
        }
    });
    $(document).on('click', '.merchantcheckbox', function () {
        if ($('.merchantcheckbox:checked').length === $('.merchantcheckbox').length) {
            $('.select_all').prop('checked', true);
        } else {
            $('.select_all').prop('checked', false);
        }
    });
    $(document).on('click', '#btnaddselectmerchants', function () {
        $("#dvLoadingGif").show();
        var chkCnt = 0;
        var isAnyCheckBox = false;
        $("input:checkbox[class=merchantcheckbox]").each(function () {
            isAnyCheckBox = true;
            var obj = {};
            if (this.checked) {
                obj["Id"] = $(this).attr("value");
                obj["IsSelected"] = true;
                chkCnt += 1;
            }
            else {
                obj["Id"] = $(this).attr("value");
                obj["IsSelected"] = false;
            }
            selectedMerchant.push(obj);
        });
        if (isAnyCheckBox == true && chkCnt === 0) {
            swal({
                title: "Please select at least one merchant.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
            return false;
        }
        var AccountMerchantRuleModel = {
            programAccountId: $("#hdnaccountId").val(),
            accountTypeId: $("#hdnaccountTypeId").val(),
            Merchants: selectedMerchant,
            programId: $("#hdnProgramId").val()
        };
        selectedMerchant = [];
        $.post("/Program/PostSelectedMerchant",
            AccountMerchantRuleModel,
            function (data) {
                $("#dvLoadingGif").hide();
                if (data.data !== "" || data.data !== "0") {
                    tab = '14';
                    var stringElement = "tab_" + tab;
                    $("#" + stringElement).addClass('active');
                    var accountTypeId = $("#hdnaccountTypeId").val();
                    var accountId = $("#hdnaccountId").val();
                    GetTabViewComponent(tab, "", "", accountTypeId, accountId);
                }
                else {
                    swal({
                        title: "There is some issue in processing! Please try again later.",
                        icon: "error"

                    });
                }
            });
        return false;
    });
    $(document).on('click', '#btnaddmerchantrules', function () {

        var array = $('#hdnaccountTypeId').val().split(",");
        $.each(array, function (k) {
            var i = 0;
            $('#merchant-rule #acctype_' + array[k] + ' tbody tr').each(function () {
                var atId = array[k];
                i++;
                var j;
                for (j = 0; j < 4; j++) {
                    var id = $('#hdnruleid_' + atId + "_" + i + '_' + (j + 1)).attr('value');
                    var MaxPassUsage = $('#maxPassUsage_' + atId + "_" + i + '_' + (j + 1)).val();
                    var MinPassValue = $('#minPassValue_' + atId + "_" + i + '_' + (j + 1)).val();
                    var MaxPassValue = $('#maxPassValue_' + atId + "_" + i + '_' + (j + 1)).val();
                    var TransactionLimit = $('#transactionLimit_' + atId + "_" + i + '_' + (j + 1)).val();
                    var obj = {};
                    obj["id"] = id;
                    obj["mealPeriodId"] = j + 1;
                    obj["maxPassUsage"] = MaxPassUsage;
                    obj["minPassValue"] = MinPassValue;
                    obj["maxPassValue"] = MaxPassValue;
                    obj["transactionLimit"] = TransactionLimit;
                    merchantRules.push(obj);
                }
            });
        });
        var AccountMerchantRuleModel = {
            programAccountId: $("#hdnaccountId").val(),
            accountTypeId: $("#hdnaccountTypeId").val(),
            Merchants: selectedMerchant,
            programId: $("#hdnProgramId").val(),
            AccountMerchantRuleAndDetail: merchantRules
        };
        merchantRules = [];
        $.post("/Program/PostSelectedMerchantRules",
            AccountMerchantRuleModel,
            function (data) {
                $("#dvLoadingGif").hide();
                if (data.data !== "" || data.data !== "0") {
                    tab = '14';
                    swal({
                        title: "Merchant rules added successfully.",
                        icon: "success"

                    });
                    var stringElement = "tab_" + tab;
                    $("#" + stringElement).addClass('active');
                    var accountTypeId = $("#hdnaccountTypeId").val();
                    var accountId = $("#hdnaccountId").val();
                    var businessType = '';
                    GetTabViewComponent(tab, "", businessType, accountTypeId, accountId);
                }
                else {
                    swal({
                        title: "There is some issue in processing! Please try again later.",
                        icon: "error"

                    });
                }
            });
        return false;
    });
    /***Ends****/

    /* Added for Program Branding*/
    /***Starts****/
    $(document).on('change', '#brandingColor', function () {
        $(".branding-logo").css("background-color", $(this).val());
        $(".h2-text").css("color", $(this).val());
        $(".spn-text").css("color", $(this).val());
    });
    $(document).on('click', '.view-cards-detail', function () {
        var idName = $(this).attr('id').replace('tab_', '');
        GetTabViewComponent(idName, '', "", "", "");
    });
    $(document).on('click', '.show-card-detail', function () {
        GetTabViewComponent('10', '', "", "", "");
    });
    $(document).on('change', '#programAccountID', function () {
        $.ajax({
            method: 'GET',
            url: '/Program/GetProgramAccountType',
            data: { id: $(this).val() }
        }).done(function (data, statusText, xhdr) {

            $('#accountType').val(data.data.accountType);
            $('#accountTypeId').val(data.data.accountTypeId);
            if (data.data.accountTypeId === 4) {
                $("#dvBrandingColorPreview").hide();
                $("#dvVIPCardPreview").show();
                $("#lblLogoBranding").html('VIP Card Image*');
                $("#pLogoInfo").html('');
                if (data.data.id > 0) {
                    $("#id").val(data.data.id);
                    if (data.data.ImagePath !== null || data.data.ImagePath !== '') {
                        $("#imgVIPCardPreview").attr('src', data.data.ImagePath);
                        $("#ImagePath").val(data.data.ImagePath);
                    }
                    else {
                        $("#imgVIPCardPreview").attr('src', 'data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs=');
                        $("#ImagePath").val('');
                    }

                }
                else { $("#id").val("0"); }
            }
            else {
                $("#dvBrandingColorPreview").show();
                $("#dvVIPCardPreview").hide();
                $("#lblLogoBranding").html('Logo*');
                $("#pLogoInfo").html('The logo uploaded will be used for Login and Registration.');
                $(".h2-text").text($("#programAccountID option:selected").text());
                $(".spn-text").text(data.data.accountType);
                if (data.data.id > 0) {
                    $("#id").val(data.data.id);
                    $("#brandingColor").val(data.data.brandingColor);
                    var bgcolor = $("#brandingColor").val();
                    if (!bgcolor) {
                        bgcolor = "#3F43CE";
                    }
                    $("#brandingColor").spectrum({
                        color: bgcolor,
                        preferredFormat: "hex"
                    });
                    $(".branding-logo").css("background-color", bgcolor);
                    $("#cardNumber").val(data.data.cardNumber);
                    if (data.data.ImagePath !== null || data.data.ImagePath !== '') {
                        $("#brand-logo").attr('src', data.data.ImagePath);
                        $("#ImagePath").val(data.data.ImagePath);
                    }
                    else {
                        $("#brand-logo").attr('src', '/images/icon-profile-lg.png');
                        $("#ImagePath").val('');
                    }
                }
                else { $("#id").val("0"); $("#cardNumber").val(data.data.cardNumber); }
            }
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
        });
    });
    $(document).on('change', '#PostedFileUpload', function () {
        // Prepare the preview for profile picture
        var imagepath = $('#wizardPicturePreview').attr('src');
        var file1 = $(this)[0].files[0];
        if (file1 !== undefined) {
            var formData = new FormData();
            formData.append("files", file1);
            // Get uploaded file extension
            var extension = $(this).val().split('.').pop().toLowerCase();
            // Create array with the files extensions that we wish to upload
            var validFileExtensions = ['jpeg', 'jpg', 'png', 'gif', 'bmp'];
            //Check file extension in the array.if -1 that means the file extension is not in the list.
            if ($.inArray(extension, validFileExtensions) === -1) {
                $('#input_file_upload_error_img').hide();
                $('#input_file_upload_error_img').addClass('field-validation-valid');
                $('#input_file_upload_error_img').removeClass('field-validation-error');
                $('#PostedFileUploadError').show();
                $('#PostedFileUploadError').addClass('field-validation-error');
                $('#PostedFileUploadError').removeClass('field-validation-valid');
                // Clear fileuload control selected file
                $(this).val('');
                $('#wizardPicturePreview').attr('src', imagepath);
                //Disable Submit Button
            }
            else if (file1.size > 10506316 || file1.fileSize > 10506316) {
                $('#input_file_upload_error_img').show();
                $('#input_file_upload_error_img').addClass('field-validation-error');
                $('#input_file_upload_error_img').removeClass('field-validation-valid');
                $('#wizardPicturePreview').attr('src', imagepath);
                $('#PostedFileUploadError').hide();
                $('#PostedFileUploadError').addClass('field-validation-valid');
                $('#PostedFileUploadError').removeClass('field-validation-error');
                $(this).val('');
            }
            else {
                readURL(this);
                $('#PostedFileUploadError').hide();
                $('#input_file_upload_error_img').hide();
                $('#PostedFileUploadError').addClass('field-validation-valid');
                $('#PostedFileUploadError').removeClass('field-validation-error');
                $('#input_file_upload_error_img').addClass('field-validation-valid');
                $('#input_file_upload_error_img').removeClass('field-validation-error');
                $.ajax({
                    type: "POST",
                    url: "/Account/UploadImage",
                    data: formData,
                    dataType: "json",
                    processData: false,
                    contentType: false,
                    success: function (data) {

                        if (data.data !== "") {
                            $("#ImagePath").val(data.data);
                            $("#PromotionImagePath").val(data.data);
                            $("#UserImagePath").val(data.data);
                            if ($("#accountTypeId").val() === "4") {
                                $("#imgVIPCardPreview").attr("src", data.data);
                            }
                            else {
                                $("#brand-logo").attr("src", data.data);
                            }
                        }
                        else {
                            swal({
                                title: "Failed to upload an image! Please try again later.",
                                icon: "error"

                            });
                        }
                    },
                    error: function () {

                    }
                });
            }
        }
    });
    $(document).on("click", "#btnbrandingdetailinfo", function () {
        if ($("#form").validate() && $("#form").valid()) {
            $("#dvLoadingGif").show();
            var Id = $("#id").val();
            var ProgramBrandingModel = {
                id: Id,
                programAccountID: $("#programAccountID").val(),
                accountName: $("#programAccountID option:selected").text(),
                programId: $("#hdnProgramId").val(),
                brandingColor: $("#brandingColor").val() === "" ? "#3F43CE" : $("#brandingColor").val(),
                accountTypeId: $("#accountTypeId").val(),
                cardNumber: $("#cardNumber").val(),
                ImagePath: $("#ImageFileName").val()
            };
            $.post("/Program/PostBrandingDetails",
                ProgramBrandingModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" && data.data !== "0") {
                        var title = ''; if ($("#id").val() !== '' && $("#id").val() !== '0' && $("#id").val() !== '3000') { title = "Branding details has been updated successfully."; } else { title = "Branding details has been submitted successfully."; }
                        swal({ title: title, icon: "success" }, function () { tab = '13'; var stringElement = "tab_" + tab; $("#" + stringElement).addClass('active'); GetTabViewComponent(tab, "", "", "", ""); });
                    }
                    else { swal({ title: data.message, icon: "error" }); }
                });
            return false;
        }
        else {
            $("#form").validate();
        }
    });
    //Delete Branding
    $(document).on('click', '.clsDeleteBranding', function () {
        var reId = $(this).attr('id');
        var reName = $(this).attr('data-org');

        swal({
            title: "Are you sure you want to delete " + reName + "?",
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
                DeleteBranding(reId);

            }
        }
        );
    });
    $(document).on('click', '#edtBranding', function () {
        var pid = $(this).attr('value');
        GetTabViewComponent('10', pid, "", "", "");
    });
    /***Ends****/
});


function GetTabViewComponent(idName, id, btId, atId, aId) {
    $("#dvLoadingGif").show();
    /* Added for ProgramInfo*/
    /***Starts****/
    if (idName === "1") {
        $.ajax({
            method: 'GET',
            url: '/Program/ProgramInfo',
            data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnProgramTab").html(data);
            $.validator.unobtrusive.parse($("#spnProgramTab"));
            $('.nav-tabs a[href="#program-info"]').tab('show');

            // Adding custom fields with the content dynamically on edit mode.
            if ($("#programCustomFields").val() !== '' && $("#programCustomFields").val() !== '[]') {
                var arrayCustomFields = JSON.parse($("#programCustomFields").val());
                var i = 1;
                $.each(arrayCustomFields, function (index, custom) {

                    var currIndex = parseInt($(".addCustomFields").attr('value'));
                    var newIndex = currIndex + 1;
                    if (i === 1) {
                        $("#CustomFieldName_" + currIndex).val(custom.CustomFieldName);
                        $("#FieldType_" + currIndex).val(custom.CustomFieldDataType);
                        $("#CustomFieldLength_" + currIndex).val(custom.CustomFieldLength);
                        $("#CustomFieldRequirement_" + currIndex).val(custom.CustomFieldIsRequired);
                        $(".addCustomFields").attr('value', currIndex);
                    } else {
                        var cloneelement = $("#select-accounts-action_0").clone().attr("id", "select-accounts-action_" + newIndex);
                        cloneelement.find("*[id]").each(function () {
                            $(this).attr("id", $(this).attr("id").replace("0", newIndex.toString()));
                            $(this).attr("name", $(this).attr("name").replace("0", newIndex.toString()));
                        });
                        cloneelement.append($('<a class="delete-action-mealp deleteCustomField" href="#"></a>'));
                        cloneelement.show();
                        cloneelement.find(".customTextBox").val('');
                        cloneelement.find("#CustomFieldName_" + newIndex).val(custom.CustomFieldName);
                        cloneelement.find("#FieldType_" + newIndex).val(custom.CustomFieldDataType);
                        cloneelement.find("#CustomFieldLength_" + newIndex).val(custom.CustomFieldLength);
                        cloneelement.find("#CustomFieldRequirement_" + newIndex).val(custom.CustomFieldIsRequired);
                        cloneelement.appendTo("#additionalaccounts-action");
                        $(".addCustomFields").attr('value', newIndex);
                    }
                    if (newIndex === 10) { $(".addCustomFields").hide(); }
                    i++;
                });
            }
            $("#dvLoadingGif").hide();
            $("#program-info").addClass("show");
        }).fail(function (xhdr, statusText, errorText) { swal({ title: "Currently unable to process the request! Please try again later.", icon: "error" }); $("#dvLoadingGif").hide(); });
    }
    /*Ends*/
    /***Ends*****/
    //Plan Listing=========
    else if (idName === "2") {
        $.ajax({ method: 'GET', url: '/Program/PlanList', data: { ppId: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() } }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data); var primaryprogId = $('#hdnprimaryProgId').attr('value'); var filename = $('#hdnpon').attr('value') + '-' + $('#hdnppn').attr('value') + '-Plan List';
            $("#plan").DataTable({
                "processing": true, "serverSide": true, "filter": true, "orderMulti": false, "pageLength": 10, "order": [[1, "desc"]], "dom": 'Bfrtip', "oLanguage": { "sEmptyTable": "No data available." },
                "ajax": { "url": "/Program/LoadPlanData", "data": { ppId: primaryprogId }, "type": "POST", "datatype": "json" },
                "columnDefs": [{ "targets": [0], "visible": false, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": true, "orderable": true }, { "targets": [2], "visible": true, "searchable": true, "orderable": true }, { "targets": [3], "visible": true, "searchable": true, "orderable": true }, { "targets": [4], "visible": true, "searchable": true, "orderable": false }, { "targets": [5], "searchable": true, "orderable": false }, { "targets": [6], "searchable": false, "orderable": false }, { "targets": [7], "searchable": false, "orderable": false }, { "targets": [8], "searchable": false, "orderable": false }],

                "columns": [{ "data": "StrId", "id": "StrId", "name": "StrId", "autoWidth": true, className: "text-center" }, { "data": "ClientId", "name": "ClientId", "autoWidth": true, className: "text-center" }, { "data": "InternalId", "name": "InternalId", "autoWidth": true, className: "text-center" }, { "data": "Name", "name": "Name", "autoWidth": true, className: "text-center" }, { "data": "Description", "name": "Description", "autoWidth": true, className: "text-center" }, { "data": "StartEnd", "name": "StartEnd", "autoWidth": true, className: "text-center" }, { "render": function (data, type, full, mets) { return $("<div>").html(full.Accounts).text().replace("lt;br />", ""); } },
                {
                    "render": function (data, type, full, mets) {
                        return "<div class='switch-reload'><label class='switch'><input type='checkbox'" + (full.Status === true ? "checked" : "") + "  onclick=ChangePlanStatus('" + full.Id + "',this)><span class='slider  slider-round round'></span></label></div>";
                    }
                },
                {
                    "render": function (data, type, full, mets) {
                        return "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                            "<div class='img-dots'></div></div>" +
                            "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>"
                            + "<ul><li><a class='edit-plan' data-id='" + full.StrId + "'>Edit Plan</a></li><li><a href='#' onclick=DeletePlanData('" + full.StrId + "','" + full.Jpos_PlanEncId + "');>Delete Plan</a></li><li><a href='#' data-name='" + full.Name + "' onclick=ClonePlan('" + full.StrId + "',this)>Clone Plan</a></li></ul>" +
                            "</div></div></div>";
                    }
                }
                ],
                initComplete: function () {
                    $('.dataTables_filter').show(); $('.dt-buttons').hide(); $('#aExport').on('click', function () {
                        var searchValue = $('.dataTables_filter input').val();
                        $.ajax({
                            type: "POST", url: "/Program/PlanExportExcel/?searchValue=" + searchValue + "&ppId=" + primaryprogId, cache: false, success: function (data) {
                                /*get the file name for download*/ if (data.fileName !== "") {  /*use window.location.href for redirect to download action for download the file*/ window.location.href = '/Program/Download/?fileName=' + data.fileName; }
                            },
                            error: function (data) { swal({ title: "There is some issue in processing!", icon: "error" }); }
                        });
                    });
                },
                buttons: [{
                    extend: 'excel', text: '', exportOptions: {
                        format: {
                            body: function (data, row, column, node) { // Strip $ from salary column to make it numeric
                                if (column === 4) { return data.replace(/<br\s*\/?>/g, " - "); } else if (column === 5) { return data.replace(/<br\s*\/?>/g, ", "); } else { return data; }
                            }
                        }, columns: [1, 2, 3, 4, 5, 6],
                        modifier: { search: 'applied', order: 'applied' }
                    }, title: filename
                }
                ]
            });
            $('.nav-tabs a[href="#plan-info"]').tab('show');
            $('#plan-info').addClass('show');
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) { swal({ title: "Currently unable to process the request! Please try again later.", icon: "error" }); });
    }
    //=====================
    //Add Plan
    else if (idName === "3") {
        $.ajax({
            method: 'GET',
            url: '/Program/CreatePlan',
            data: { id: id, ppId: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);

            $.validator.unobtrusive.parse($(".tab-content"));
            $('.datetimepicker').datepicker();
            $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
            $('.timepicker').timepicker('update');
            $('#selectedProgramAccount').multiselect({
                numberDisplayed: 2
            });
            $('input[type="checkbox"]').each(function () {

                if ($(this).is(":checked")) {

                    $(this).prop("disabled", "disabled");
                }
                else {
                    $(this).removeProp('disabled');
                }

            });
            if ($("#startDate").val() !== '') {
                $("#startDate").val(moment($("#startDate").val()).format('MM/DD/YYYY'));
            }
            if ($('#endDate').val() !== '') {
                $("#endDate").val(moment($("#endDate").val()).format('MM/DD/YYYY'));
            }
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({ title: "Currently unable to process the request! Please try again later.", icon: "error" }); $("#dvLoadingGif").hide();
        });
    }

    /* Added for AccountHolder List*/
    /***Starts****/
    else if (idName === "4") {
      //  debugger;
        $.ajax({ method: 'GET', url: '/Program/AccountHolderList', data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() } }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data); var filename = 'Account Holders List';
            $("#tblAccountHolder").DataTable({
                "processing": true, "serverSide": true, "filter": true, "orderMulti": false, "pageLength": 10, "order": [[4, "desc"]], "dom": 'Bfrtip', "oLanguage": { "sEmptyTable": "No data available." },
                "ajax": { "url": "/Program/LoadAccountHolders", "data": { prgId: $("#hdnProgramId").val(), orgId: $("#hdnPrimaryOrgId").val(), planId: null, currentPageNumber: 0 }, "type": "POST", "datatype": "json" },
                "columnDefs": [{ "targets": [0], "visible": false, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": true, "orderable": true }, { "targets": [2], "visible": true, "searchable": true, "orderable": true }, { "targets": [3], "visible": true, "searchable": false, "orderable": false }, { "targets": [4], "visible": true, "searchable": false, "orderable": true }, { "targets": [5], "searchable": false, "orderable": false }, { "targets": [6], "searchable": false, "orderable": false }, { "targets": [7], "searchable": false, "orderable": false }, { "targets": [8], "searchable": false, "orderable": false }],
                "columns": [{ "data": "Id", "id": "Id", "name": "Id", "autoWidth": true, className: "text-center" }, { "data": "AccountHolderID", "name": "AccountHolderID", "autoWidth": true, className: "text-center" }, {
                    "className": "text-center", "render": function (data, type, full, mets) {
                        return type !== 'export' ? "<div class='profile-accounts profile-accounts-sm'><image class='avatar' src=" + full.UserImagePath + "><a class='edit-accountholder' href='javascript:void(0);' style='width:140px' data-id='" + full.UserEncId + "'  ;> <em style='color:#007bff;'>" + full.Name + "</em></a></div>" : full.Name; } }, { "data": "Email", "name": "Email", "autoWidth": true, className: "text-center" }, {
                    "name": "DateAdded", "autoWidth": true, "render": function (data, type, full, mets) { return moment(full.DateAdded).format('DD MMMM YYYY'); }
                },
                { "data": "PlanName", "name": "PlanName", "autoWidth": true, className: "text-center" },
                {
                    "className": "text-center",
                    "render": function (data, type, full, mets) {
                        if (full.InvitationStatus === 3) {
                            return "<div class='invite-selector'><a  href='javascript:void(0);' data-encId='" + full.UserEncId + "' class='invite-all-btn reinvite'>Invited</a></div>";
                        }
                        if (full.InvitationStatus === 2) {
                            return "<div class='invite-selector'><a onclick='inviteAccountHolderUser(this);' href='javascript:void(0);' data-encId='" + full.UserEncId + "' class='invite-all-btn clsInvite'>Re-Invite</a></div>";
                        }
                        if (full.InvitationStatus === 1) {
                            return "<div class='invite-selector'><a onclick='inviteAccountHolderUser(this);' href='javascript:void(0);' data-encId='" + full.UserEncId + "' class='invite-all-btn clsInvite'>Invite</a></div>";
                        }
                    }
                },
                {
                    "className": "text-center",
                    "render": function (data, type, full, mets) {
                        if (full.Status === 3) {
                            return "<div class='status-point alert-point'></div>";
                        }
                        if (full.Status === 2) {
                            return "<div class='status-point orange-point'></div>";
                        }
                        if (full.Status === 1) {
                            return "<div class='status-point green-point'></div>";
                        }
                    }
                },
                {
                    "render": function (data, type, full, mets) {

                        return "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                            "<div class='img-dots'></div></div>" +
                            "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                      //      "<ul><li><a class='edit-accountholder' href='javascript:void(0);' data-id='" + full.UserEncId + "'>Edit</a></li><li><a href='javascript:void(0);' onclick=DeleteAccountHolderData('" + full.UserEncId + "','" + $("#hdnProgramId").val() + "','" + full.FirstName + "','" + full.LastName + "','" + full.Jpos_AccountEncId + "');>Delete</a></li></ul>" +
                            "<ul><li><a class='edit-accountholder' href='javascript:void(0);' data-id='" + full.UserEncId + "'>Edit</a></li></ul>" +                            "</div></div></div>";
                    }
                }
                ],
                initComplete: function () {
                    $('.dataTables_filter').show();
                    $('.dt-buttons').hide();
                    $('#aExport').on('click', function () {
                        var searchValue = $('.dataTables_filter input').val();
                        var orgId = $("#hdnPrimaryOrgId").val();
                        var pprogId = $("#hdnProgramId").val();
                        $.ajax({
                            type: "POST",
                            url: "/Program/AccountHolderExportExcel/?searchValue=" + searchValue + "&poId=" + orgId + "&ppId=" + pprogId,
                            cache: false,
                            success: function (data) {
                                //get the file name for download
                                if (data.fileName !== "") {
                                    //use window.location.href for redirect to download action for download the file
                                    window.location.href = '/Program/Download/?fileName=' + data.fileName;
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
                        extend: 'excelHtml5',

                        exportOptions: {
                            columns: [1, 2, 3, 4, 5],
                            orthogonal: 'export',
                            format: {
                                body: function (data, row, column, node) {

                                    if (column === 3) {
                                        return moment(data, 'DD MMMM YYYY').format('MM/DD/YYYY');
                                    }
                                    else {
                                        return data;
                                    }
                                }
                            },
                        },
                        title: filename
                    }
                ]
            });
            $('.accholder').addClass('active');
            $('.nav-tabs a[href="#tab-3-4"]').tab('show');
            $('#tab-3-4').addClass('show');
            $("#dvLoadingGif").hide();

        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });

        });
    }
    /*Ends*/
    /* Add/Edit Accountholder */
    else if (idName === "5") {
        $.ajax({
            method: 'GET',
            url: '/Program/ManageAccountHolder',
            data: { id: id, prgId: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnProgramTab").html(data);
            $.validator.unobtrusive.parse($("#spnProgramTab"));
            /* Dropzone */
            var create = { deleteDropZoneImage: "" };
            $("#my-awesome-dropzone").dropzone({
                url: "/Account/UploadImage", dictInvalidFileType: "Please upload only jpg .jpeg .png files",
                acceptedFiles: ".png,.jpeg,.jpg", paramName: "file", maxFiles: 1, maxFilesize: 10, uploadMultiple: false,
                init: function (element) {
                    this.on("maxfilesexceeded", function (file) {
                        swal({ title: 'No more files.', icon: "error" });
                        this.removeAllFiles(); this.addFile(file);
                    });
                    this.on('addedfile', function () {
                        /* Valid only in the dropzone . If a repetitive document shows ALERT and the previous item will disappear. */
                        this.on('addedfile', function (file) {
                            if (this.files.length > 1) { this.removeFile(this.files[0]); }
                        });
                    });
                    this.on("sending", function (file, xhr, formData) { $("#btnAccountHolderDetail").attr('disabled', 'disabled'); });
                    this.on("success", function (file, response, responseText) {
                        responseText = file.status;
                        if (responseText === "success") { $("#hdnIsNewUpload").val(1); $("#ImageFileName").val(response.data); } $("#btnAccountHolderDetail").removeAttr('disabled');
                    });
                },
                addRemoveLinks: true, removedfile: function (file) {
                    file.name; create.deleteDropZoneImage; if ($("#hdnIsNewUpload").val() === '1' || $("#UserEncId").val() === '3000') {
                        $.ajax({
                            method: 'GET', url: '/Organisation/GetDecryptCryptographicData', data: { value: $("#UserEncId").val() }
                        }).done(function (data, statusText, xhdr) {
                            $.ajax({
                                type: 'POST', url: '/Account/RemoveImage', data: { userId: data.data, imgPath: $("#ImageFileName").val(), userPhotoEntityType: $("#hdnUserImageType").val() },
                                success: function () { $("#ImageFileName").val(''); }
                            });
                        }).fail(function (xhdr, statusText, errorText) { });
                    }
                    else { $("#ImageFileName").val(''); }
                    var ref; return (ref = file.previewElement) !== null ? ref.parentNode.removeChild(file.previewElement) : void 0;
                }
            });

            if ($("#ImageFileName").val() !== '' && $("#ImageFileName").val() !== null) {
                $("#hdnIsNewUpload").val(2); myDropzone.removeAllFiles();
                var mockFile = { name: $("#ImageFileName").val(), size: 12345, accepted: true, kind: 'image' };
                myDropzone.options.maxFiles = 1; myDropzone.emit("addedfile", mockFile); myDropzone.files.push(mockFile); myDropzone.emit("thumbnail", mockFile, $("#UserImagePath").val()); myDropzone.emit("complete", mockFile); myDropzone._updateMaxFilesReachedClass();
            }

            /* Dropzone ends here.*/
            var startDate = new Date('1950-01-01'), endDate = new Date();
            $('.datetimepicker').datepicker({ startDate: startDate, endDate: endDate }); $('.nav-tabs a[href="#tab-3-5"]').tab('show'); $('#SelectedPlanIds').multiselect({ includeSelectAllOption: true });
            if (id !== '' && id !== '3000') {
                $('#Email').attr('readonly', 'readonly'); $('#PhoneNumber').attr('readonly', 'readonly'); $('#PhoneNumber').attr('disabled', 'disabled'); $('#btnAccountHolderDetail').html('UPDATE');
            }
            $("#dvLoadingGif").hide(); $("#tab-3-5").addClass("show"); $(".accholder").addClass("active"); $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 }); $('.timepicker').timepicker('update');
        }).fail(function (xhdr, statusText, errorText) {
            swal({ title: "Currently unable to process the request! Please try again later.", icon: "error" }); $("#dvLoadingGif").hide();
        });
    }
    else if (idName === "9") {
        $.ajax({
            method: 'GET',
            url: '/Program/MerchantListing', data: { ppId: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data); var primaryorgId = $('#hdnprimaryOrgId').attr('value'); var primaryprogId = $('#hdnprimaryProgId').attr('value'); var primaryprogName = $('#hdnprimaryProgName').attr('value'); var filename = $('#hdnpon').attr('value') + '-' + $('#hdnppn').attr('value') + '-Merchant List'; $("#merchant").DataTable({
                "processing": true, "serverSide": true, "filter": true, "orderMulti": false, "pageLength": 10, "order": [[5, "desc"]], "dom": 'Bfrtip', "oLanguage": { "sEmptyTable": "No data available." },
                "ajax": { "url": "/Program/LoadMerchantData", "data": { ppId: primaryprogId }, "type": "POST", "datatype": "json" },
                "columnDefs": [{ "targets": [0], "visible": false, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": true, "orderable": true }, { "targets": [2], "visible": true, "searchable": true, "orderable": true }, { "targets": [3], "visible": true, "searchable": false, "orderable": false }, { "targets": [4], "visible": true, "searchable": false, "orderable": true }, { "targets": [5], "searchable": false, "orderable": true }, { "targets": [6], "searchable": false, "orderable": false }, { "targets": [7], "searchable": false, "orderable": false }, { "targets": [8], "searchable": false, "orderable": false }],
                "columns": [{ "data": "Id", "id": "Id", "name": "Id", "autoWidth": true, className: "text-center" }, { "data": "MerchantId", "name": "MerchantId", "autoWidth": true, className: "text-center" },
                    {
                        "data": "MerchantName", "name": "MerchantName", "autoWidth": true, className: "text-center",
                        "render": function (data, type, full, mets) {
                            return '<a href=/Merchant/Create/' + full.Id + "?poId=" + primaryorgId.toString() + "&ppId=" + primaryprogId.toString() + "&ppN=" + primaryprogName + '>' + full.MerchantName + '</a>';

                        }
                    },
                    { "data": "Location", "name": "Location", "autoWidth": true, className: "text-center" }, { "className": "text-center", "name": "LastTransaction", "autoWidth": true, "render": function (data, type, full, mets) { return full.LastTransaction !== '' && full.LastTransaction !== null ? moment(full.LastTransaction).format('DD MMMM YYYY') : ""; } }, {
                    "name": "DateAdded", "autoWidth": true, "render": function (data, type, full, mets) { return moment(full.DateAdded).format('DD MMMM YYYY'); }
                },
                {
                    "render": function (data, type, full, mets) {
                        return $("<div>").html(full.AccountType).text().replace("lt;br />", "");
                    }
                },
                {
                    "className": "text-center",
                    "render": function (data, type, full, mets) {
                        if (full.Activity === 0) {
                            return "<div class='status-point alert-point'></div>";
                        }
                        if (full.Activity === 2) {
                            return "<div class='status-point orange-point'></div>";
                        }
                        if (full.Activity === 1) {
                            return "<div class='status-point green-point'></div>";
                        }
                    }
                },
                {
                    "render": function (data, type, full, mets) {
                        return "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                            "<div class='img-dots'></div></div>" +
                            "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                            "<ul><li><a href='/Merchant/Create/" + full.Id + "?poId=" + primaryorgId.toString() + "&ppId=" + primaryprogId.toString() + "&ppN=" + primaryprogName + "'>Edit Merchant</a></li><li><a href='#' onclick=DeleteData('" + full.Id + "','" + full.Jpos_MerchantEncId + "');>Delete Merchant</a></li><li><a href='#' data-name='" + full.MerchantName + "' onclick=Clone('" + full.Id + "',this)>Clone Merchant</a></li></ul>" +
                            "</div></div></div>";
                    }
                }
                ],
                initComplete: function () {
                    $('.dataTables_filter').show();
                    $('.dt-buttons').hide();
                    $('#aExport').on('click', function () {
                        var searchValue = $('.dataTables_filter input').val();

                        $.ajax({
                            type: "POST",
                            url: "/Program/MerchantExportExcel/?searchValue=" + searchValue + "&ppId=" + primaryprogId,
                            cache: false,
                            success: function (data) {
                                //get the file name for download
                                if (data.fileName !== "") {
                                    //use window.location.href for redirect to download action for download the file
                                    window.location.href = '/Program/Download/?fileName=' + data.fileName;
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
                        exportOptions: {
                            format: {
                                body: function (data, row, column, node) {
                                    // Strip $ from salary column to make it numeric
                                    if (column === 4) {
                                        return moment(data, 'MMMM - Do - YYYY').format('MM/DD/YYYY');
                                    }
                                    else if (column === 5) {
                                        return data.replace(/<br\s*\/?>/g, ", ");
                                    }
                                    else {
                                        return data;
                                    }
                                }
                            },
                            columns: [1, 2, 3, 4, 5, 6],
                            modifier: {
                                search: 'applied',
                                order: 'applied'
                            }
                        },
                        title: filename
                    }
                ]
            });
            $('.nav-tabs a[href="#merchant-list"]').tab('show');
            $('#merchant-list').addClass('show');
            $("#dvLoadingGif").hide();
            $('.merchant').addClass('active');
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });

        });
    }


    //=====================
    /*****Account Start******/
    //Program Account Listing=========
    if (idName === "7") {
        $.ajax({
            method: 'GET',
            url: '/Program/ProgramAccountList',
            data: { ppId: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data); var primaryprogId = $('#hdnprimaryProgId').attr('value'); var filename = $('#hdnpon').attr('value') + '-' + $('#hdnppn').attr('value') + '-Account List';
            $("#programaccount").DataTable({
                "processing": true, "serverSide": true, "filter": true, "orderMulti": false, "pageLength": 10, "order": [[1, "desc"]], "dom": 'Bfrtip', "oLanguage": { "sEmptyTable": "No data available." },
                "ajax": { "url": "/Program/LoadProgramAccountData", "data": { ppId: primaryprogId }, "type": "POST", "datatype": "json" },
                "columnDefs": [{ "targets": [0], "visible": false, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": true, "orderable": true }, { "targets": [2], "visible": true, "searchable": true, "orderable": true }, { "targets": [3], "visible": true, "searchable": true, "orderable": true }, { "targets": [4], "visible": true, "searchable": true, "orderable": false }, { "targets": [5], "searchable": false, "orderable": false }, { "targets": [6], "searchable": false, "orderable": false }],
                "columns": [{ "data": "StrId", "id": "StrId", "name": "StrId", "autoWidth": true, className: "text-center" }, { "data": "ProgramAccountId", "name": "ProgramAccountId", "autoWidth": true, className: "text-center" }, { "data": "AccountName", "name": "AccountName", "autoWidth": true, className: "text-center" }, { "data": "AccountType", "name": "AccountType", "autoWidth": true, className: "text-center" }, { "render": function (data, type, full, mets) { return $("<div>").html(full.PlanName).text().replace("lt;br />", ""); } },
                {
                    "render": function (data, type, full, mets) {
                        return "<div class='switch-reload'><label class='switch'><input type='checkbox'" + (full.Status === true ? "checked" : "") + "  onclick=ChangeAccountStatus('" + full.Id + "',this)><span class='slider  slider-round round'></span></label></div>";
                    }
                },
                {
                    "render": function (data, type, full, mets) {
                        return "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                            "<div class='img-dots'></div></div>" +
                            "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>"
                            + "<ul><li><a class='edit-prog-acc' data-id='" + full.StrId + "'>Edit Account</a></li><li><a href='#' onclick=DeleteAccountData('" + full.StrId + "','" + full.Jpos_ProgramEncAccountId + "');>Delete Account</a></li><li><a href='#' data-name='" + full.AccountName + "' onclick=CloneAccount('" + full.StrId + "',this)>Clone Account</a></li></ul>" +
                            "</div></div></div>";
                    }
                }
                ],
                initComplete: function () {
                    $('.dataTables_filter').show(); $('.dt-buttons').hide();
                    $('#aExport').on('click', function () {
                        var searchValue = $('.dataTables_filter input').val(); $.ajax({
                            type: "POST", url: "/Program/AccountExportExcel/?searchValue=" + searchValue + "&ppId=" + primaryprogId, cache: false,
                            success: function (data) { if (data.fileName !== "") { window.location.href = '/Program/Download/?fileName=' + data.fileName; } },
                            error: function (data) {
                                swal({
                                    title: "There is some issue in processing!",
                                    icon: "error"
                                });
                            }
                        });
                    });
                },
                buttons: [{
                    extend: 'excel', text: '', exportOptions: {
                        format: {
                            body: function (data, row, column, node) { if (column === 4) { return moment(data, 'MMMM - Do - YYYY').format('MM/DD/YYYY'); } else if (column === 5) { return data.replace(/<br\s*\/?>/g, ", "); } else { return data; } }
                        }, columns: [1, 2, 3, 4, 5, 6], modifier: { search: 'applied', order: 'applied' }
                    }, title: filename
                }]
            });
            $('.nav-tabs a[href="#tab-012"]').tab('show');
            $('#tab-012').addClass('show');
            $(".accs").addClass("active");
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });

        });
    }
    //=====================
    //Account Add/Edit================
    if (idName === "8") {
        $.ajax({
            method: 'GET',
            url: '/Program/CreateProgramAccount',
            data: { id: id, ppId: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $.validator.setDefaults({ ignore: ':hidden' });
            var at = $('#accountTypeId').val();
            var pt = $('#passType').val();
            var rpt = $('#resetPeriodType').val();
            var exrpt = $('#exchangeResetPeriodType').val();
            var ispassexenabled = $("input[name='isPassExchangeEnabled']:checked").val();
            $.validator.unobtrusive.parse($(".tab-content"));
            $("#hdnAccountTypeAccount").val($('#accountTypeId').val());
            if ($("#id").val() === '3000') {
                $("#intialBalanceCount").removeAttr('readonly');
                $('#accountTypeId').removeAttr("disabled");
            }
            else {
                $("#intialBalanceCount").attr('readonly', 'readonly');
                $('#accountTypeId').attr("disabled", true);
            }
            if (at === "1") {
                $(".dv-pass-type").show();
                $(".row-exchange").show();
                $(".dv-exchange").show();
                $(".row-flex").hide();
                $(".row-vpl").hide();
            }
            else if (parseInt(at) === 2) {
                $(".dv-pass-type").hide();
                $(".dv-initial-balance").show();
                $(".row-flex").show();
                $(".row-vpl").hide();
                $(".row-pass").hide();
                $(".row-exchange").hide();
            }
            else if (parseInt(at) === 3) {
                $(".dv-pass-type").hide();
                $(".dv-initial-balance").hide();
                $(".row-flex").hide();
                $(".row-vpl").show();
                $(".row-pass").hide();
                $(".row-exchange").hide();
            }
            else {
                $(".dv-pass-type").hide();
                $(".dv-initial-balance").hide();
                $(".row-flex").hide();
                $(".row-pass").hide();
                $(".row-vpl").hide();
                $(".row-exchange").hide();
            }
            if (pt === "2") {
                $(".dv-initial-balance").show();
                $(".row-pass").show();
                $(".dv-block").show();
                $(".dv-reset-period").hide();
                $(".dv-reset-day-week").hide();
                $(".dv-reset-time").hide();
                $(".dv-reset-date-month").hide();
                $(".dv-maxpass-day").show();
                $(".dv-maxpass-week").show();
            }
            else if (parseInt(pt) > 2) {
                $(".row-pass").show();
                $(".dv-block").hide();
                $(".dv-maxpass-day").hide();
                $(".dv-maxpass-week").hide();
                $(".dv-initial-balance").show();
                $(".dv-reset-period").show();
            }
            else if (pt !== "") {
                $(".dv-block").hide();
                $(".dv-initial-balance").hide();
                $(".dv-reset-period").hide();
                $(".dv-reset-day-week").hide();
                $(".dv-reset-time").hide();
                $(".dv-maxpass-day").hide();
                $(".dv-maxpass-week").hide();
            }
            if (rpt === "1") {
                $(".dv-reset-time").show();
                $('.timepicker').timepicker('remove');
                $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
                $('.timepicker').timepicker('update');
                $(".dv-reset-date-month").hide();
                $(".dv-maxpass-day").hide();
                $(".dv-maxpass-week").hide();
                $(".dv-reset-day-week").hide();
            }
            else if (parseInt(rpt) === 2) {
                $(".dv-reset-day-week").show();
                $(".dv-reset-time").show();
                $('.timepicker').timepicker('remove');
                $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
                $('.timepicker').timepicker('update');
                $(".dv-maxpass-day").show();
                $(".dv-reset-date-month").hide();
                $(".dv-maxpass-week").hide();
            }
            else if (parseInt(rpt) === 3) {
                $(".dv-reset-day-week").hide();
                $(".dv-reset-time").hide();
                $(".dv-reset-date-month").show();
                $(".dv-maxpass-day").show();
                $(".dv-maxpass-week").show();
            }
            else if (rpt !== "") {
                $(".dv-initial-balance").hide();
                $(".dv-reset-period").hide();
                $(".dv-reset-day-week").hide();
                $(".dv-reset-time").hide();
                $(".dv-maxpass-day").hide();
            }
            if (exrpt === "1") {
                $(".dv-exreset-time").show();
                $('.timepicker').timepicker('remove');
                $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
                $('.timepicker').timepicker('update');
                $(".dv-exreset-date-month").hide();
                $(".dv-exreset-day-week").hide();
            }
            else if (parseInt(exrpt) === 2) {
                $(".dv-exreset-day-week").show();
                $(".dv-exreset-time").show();
                $('.timepicker').timepicker('remove');
                $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
                $('.timepicker').timepicker('update');
                $(".dv-exreset-date-month").hide();
            }
            else if (parseInt(exrpt) === 3) {
                $(".dv-exreset-day-week").hide();
                $(".dv-exreset-time").hide();
                $(".dv-exreset-date-month").show();
            }
            else {
                $(".dv-exreset-period").hide();
                $(".dv-exreset-day-week").hide();
                $(".dv-exreset-time").hide();
                $(".dv-exreset-date-month").hide();
            }
            if (ispassexenabled === "true" || ispassexenabled === "True" || ispassexenabled === "1") {
                $(".dv-exchange").show();
            }
            else {
                $(".dv-exchange").hide();
                $(".dv-exreset-period").hide();
                $(".dv-exreset-day-week").hide();
                $(".dv-exreset-time").hide();
                $(".dv-exreset-date-month").hide();
            }
            $(".accs").addClass("active");
            $('.datetimepicker').datepicker();

            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }
    //=====================
    //Account Select Merchant Rules================
    if (idName === "14") {
        $.ajax({
            method: 'GET', url: '/Program/CreateProgramMerchantAccountRules', data: { id: id, ppId: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val(), btIdNatId: btId + '^' + atId, aId: aId }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $('#selectedBusinessType').multiselect({
                includeSelectAllOption: true, numberDisplayed: 2,
                onSelectAll: function () {
                    var brands = $('#selectedBusinessType option:selected'); var businessType = ''; $(brands).each(function (index, brand) { businessType = $(this).val() + "," + businessType; });
                    tab = '14'; var stringElement = "tab_" + tab; $("#" + stringElement).addClass('active'); var accountTypeId = $("#hdnaccountTypeId").val(); var accountId = $("#hdnaccountId").val(); GetTabViewComponent(tab, "", businessType, accountTypeId, accountId);
                },
                onDeselectAll: function () { var businessType = '0'; tab = '14'; var stringElement = "tab_" + tab; $("#" + stringElement).addClass('active'); var accountTypeId = $("#hdnaccountTypeId").val(); var accountId = $("#hdnaccountId").val(); GetTabViewComponent(tab, "", businessType, accountTypeId, accountId); },
                onChange: function (option, checked, select) {
                    var brands = $('#selectedBusinessType option:selected'); var businessType = '0'; $(brands).each(function (index, brand) { businessType = $(this).val() + "," + businessType; });
                    tab = '14'; var stringElement = "tab_" + tab; $("#" + stringElement).addClass('active'); var accountTypeId = $("#hdnaccountTypeId").val(); var accountId = $("#hdnaccountId").val(); GetTabViewComponent(tab, "", businessType, accountTypeId, accountId);
                }
            });
            if (!btId) { $('#selectedBusinessType').multiselect('selectAll', false); $("#selectedBusinessType").multiselect('updateButtonText'); }
            $('.nav-tabs a[href="#tab-014"]').tab('show'); $('#tab-014').addClass('show'); $(".accs").addClass("active"); $("#dvLoadingGif").hide(); if ($('.merchantcheckbox:checked').length === $('.merchantcheckbox').length) { $('.select_all').prop('checked', true); } else { $('.select_all').prop('checked', false); }
        }).fail(function (xhdr, statusText, errorText) { swal({ title: "Jquery..Currently unable to process the request! Please try again later.", icon: "error" }); $("#dvLoadingGif").hide(); });
    }
    //=====================

    /*****Account End ****/
    /* Added for Branding */
    /***Starts****/
    //Add/Edit Branding
    else if (idName === "10") {
        $.ajax({
            method: 'GET', url: '/Program/CreateBranding', data: { id: id, poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val(), ppId: $("#hdnProgramId").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data); $.validator.setDefaults({ ignore: '' }); $("#one-point").show(); $.validator.unobtrusive.parse($("#spnProgramTab")); var bgcolor = $("#brandingColor").val();
            if (!bgcolor) { bgcolor = "#3F43CE"; }
            $("#brandingColor").spectrum({ color: bgcolor, preferredFormat: "hex" });
            $('.nav-tabs a[href="#branding"]').tab('show'); $('#branding').addClass('show'); $(".branding-logo").css("background-color", bgcolor);
            if ($("#programAccountID").val() !== "") $(".h2-text").text($("#programAccountID option:selected").text()); $(".spn-text").text($("#accountType").val());
            if ($("#accountTypeId").val() === "4") { $("#dvBrandingColorPreview").hide(); $("#dvVIPCardPreview").show(); $("#lblLogoBranding").html('VIP Card Image*'); $("#pLogoInfo").html(''); }
            else if ($("#accountTypeId").val() !== "0") {
                $("#dvBrandingColorPreview").show(); $("#dvVIPCardPreview").hide(); $("#lblLogoBranding").html('Logo*');
                $("#pLogoInfo").html('The logo uploaded will be used for Login and Registration.'); $(".h2-text").text($("#programAccountID option:selected").text());
            }
            $(".h2-text").css("color", bgcolor); $(".spn-text").css("color", bgcolor); if ($("#id").val() !== "0") { $('#programAccountID').attr("disabled", true); } else { $('#programAccountID').removeAttr("disabled"); }
            /* Dropzone */
            var create = { deleteDropZoneImage: "" };
            $("#my-awesome-dropzone").dropzone({
                url: "/Account/UploadImage", dictInvalidFileType: "Please upload only .jpg .jpeg .png files", acceptedFiles: ".png,.jpeg,.jpg", paramName: "file", maxFiles: 1, maxFilesize: 10, uploadMultiple: false, init: function (element) {
                    this.on("maxfilesexceeded", function (file) {
                        swal({ title: 'No more files.', icon: "error" }); this.removeAllFiles(); this.addFile(file);
                    });
                    this.on('addedfile', function () { /* Valid only in the dropzone . If a repetitive document shows ALERT and the previous item will disappear. */
                        this.on('addedfile', function (file) { if (this.files.length > 1) { this.removeFile(this.files[0]); } });
                    });
                    this.on("sending", function (file, xhr, formData) { $("#btnbrandingdetailinfo").attr('disabled', 'disabled'); });
                    this.on("success", function (file, response, responseText) {
                        responseText = file.status; if (responseText === "success") {
                            $.ajax({
                                type: 'POST', url: '/Account/GetPresignedURLImage', data: { fileName: response.data }, success: function (data) {
                                    if ($("#accountTypeId").val() === "4") { $("#imgVIPCardPreview").attr("src", data.data); } else { $("#brand-logo").attr("src", data.data); }
                                    $("#ImagePath").val(data.data);
                                }
                            }); $("#hdnIsNewUpload").val(1); $("#ImageFileName").val(response.data);
                        }
                        $("#btnbrandingdetailinfo").removeAttr('disabled');
                    });
                },
                addRemoveLinks: true,
                removedfile: function (file) {
                    file.name; create.deleteDropZoneImage; if ($("#hdnIsNewUpload").val() === '1' || $("#accountTypeId").val() === '0') {
                        $.ajax({ type: 'POST', url: '/Account/RemoveImage', data: { userId: $("#accountTypeId").val(), imgPath: $("#ImagePath").val(), userPhotoEntityType: $("#hdnUserImageType").val() },  success: function () { if ($("#accountTypeId").val() === "4") { $("#imgVIPCardPreview").attr("src", 'data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs='); }
                                else { $("#brand-logo").attr("src", '/images/icon-profile-lg.png'); }
                                $("#ImagePath").val(''); $("#ImageFileName").val(''); }}); }
                    else { $("#ImagePath").val(''); $("#ImageFileName").val(''); if ($("#accountTypeId").val() === "4") { $("#imgVIPCardPreview").attr("src", 'data:image/gif;base64,R0lGODlhAQABAAD/ACwAAAAAAQABAAACADs='); } else {  $("#brand-logo").attr("src", '/images/icon-profile-lg.png');
                        }
                    }
                    var ref;
                    return (ref = file.previewElement) !== null ? ref.parentNode.removeChild(file.previewElement) : void 0;

                }
            });
            var imgDropzone = Dropzone.forElement("#my-awesome-dropzone");
            imgDropzone.removeAllFiles();
            /* Dropzone ends here.*/
            if ($("#ImageFileName").val() !== '' && $("#ImageFileName").val() !== null) {
                var mockFile = {
                    name: $("#ImageFileName").val(),
                    size: 12,
                    accepted: true,
                    kind: 'image'
                };
                imgDropzone.options.maxFiles = 1;
                imgDropzone.emit("addedfile", mockFile);
                imgDropzone.files.push(mockFile);

                // And optionally show the thumbnail of the file:
                imgDropzone.emit("thumbnail", mockFile, $("#ImagePath").val());

                imgDropzone.emit("complete", mockFile);
                imgDropzone._updateMaxFilesReachedClass();
                $("#hdnIsNewUpload").val(2);
                if ($("#accountTypeId").val() === "4") {
                    $("#imgVIPCardPreview").attr("src", $("#ImagePath").val());
                }
                else {
                    $("#brand-logo").attr("src", $("#ImagePath").val());
                }

            } else { $('#ImagePath').val(''); $("#ImageFileName").val(''); }
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });

        });
    }
    //Branding Listing
    else if (idName === "13") {
        $.ajax({
            method: 'GET',
            url: '/Program/BrandingListing',
            data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val(), bid: id }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $('.nav-tabs a[href="#branding"]').tab('show');
            $('#branding').addClass('show');
            $("#second-point").show();
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }
    /***Ends*****/
    //Loyality Listing
    else if (idName === "23") {
        debugger;
        $.ajax({
            method: 'GET',
            url: '/Program/SiteOverrideSettingListing',
            data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val(), bid: id }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $('.nav-tabs a[href="#loyality"]').tab('show');
            $('#loyality').addClass('show');
            $("#second-point").show();
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }
    /***Ends*****/
    else if (idName === "11") {

        $.ajax({
            method: 'GET',
            url: '/Program/ProgramLevelAdmin',
            data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $.validator.unobtrusive.parse($("#spnProgramTab"));

            $("#tblProgramLevelAdmin").dataTable({
                "processing": true, // for show progress bar
                "serverSide": true, // for process server side
                "filter": true, // this is for disable filter (search box)
                "orderMulti": false, // for disable multiple column at once
                "pageLength": 10,
                "order": [[4, "desc"]],
                "oLanguage": {
                    "sEmptyTable": "No data available."
                },
                "bLengthChange": false,
                "ajax": {
                    "url": "/Program/LoadAllProgramLevelAdmins",
                    "data": { id: $("#hdnProgramId").val() },
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
                        "searchable": true,
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
                        "searchable": true,
                        "orderable": true
                    },
                    {
                        "targets": [6],
                        "searchable": false,
                        "orderable": false
                    },
                    {
                        "targets": [7],
                        "searchable": false,
                        "orderable": false
                    },
                    {
                        "targets": [8],
                        "searchable": false,
                        "orderable": false
                    }
                    ],

                "columns": [
                    { "data": "UserId", "id": "UserId", "name": "UserId", "autoWidth": true },
                    {
                        "id": "Name", "name": "Name", "autoWidth": true,
                        "render": function (data, type, full, mets) {

                            return "<div class='profile-accounts'><image class='avatar' src=" + full.UserImagePath + "><a href='#' onclick=EditUserAdminData('" + full.UserId + "');> <em style='color:#007bff;'>" + full.Name + "</em></div>";
                        }
                    },
                    {
                        "data": "Title", "name": "Title", "autoWidth": true, "className": "text-center"
                    },
                    {
                        "data": "EmailAddress", "name": "EmailAddress", "autoWidth": true, "className": "text-center"
                    },

                    {
                        "data": "PhoneNumber", "name": "PhoneNumber", "autoWidth": true, "className": "text-center"
                    },
                    {
                        "name": "DateAdded",
                        "autoWidth": true,
                        "render": function (data, type, full, mets) { return moment(full.DateAdded).format('MMMM Do YYYY'); }
                    },
                    {
                        "render": function (data, type, full, mets) {
                            return "<div class='switch-reload'><label class='switch'><input type='checkbox'" + (full.Status === true ? "checked" : "") + "  onclick=changeAdminStatus('" + full.UserId + "',this)><span class='slider  slider-round round'></span></label></div>";
                        }
                    },
                    {
                        "render": function (data, type, full, mets) {
                            if (full.InvitationStatus === 2) {
                                return "<div class='invite-selector'><a href='javascript:void(0);' value='" + full.EmailAddress + "' class='invite-all-btn not-active'>Invited</a></div>";
                            }
                            if (full.InvitationStatus === 1) {
                                return "<div class='invite-selector'><a href='javascript:void(0);' value='" + full.EmailAddress + "' class='invite-all-btn invite'>Re-Invite</a></div>";
                            }
                            if (full.InvitationStatus === 0) {
                                return "<div class='invite-selector'><a href='javascript:void(0);' value='" + full.EmailAddress + "' class='invite-all-btn invite'>Invite</a></div>";
                            }
                        }
                    },
                    {
                        "render": function (data, type, full, mets) {
                            var linkDataContentEditDelete = "";
                            if (userRlN.toLowerCase() == "super admin" || userRlN.toLowerCase() == "organization full") {
                                linkDataContentEditDelete += "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                                    "<div class='img-dots'></div></div>" +
                                    "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                                    "<ul><li><a  href='#' onclick=EditUserAdminData('" + full.UserId + "');>Edit Admin</a></li><li><a href='#' onclick=DeleteData('" + full.UserId + "');>Delete Admin</a></li></ul>" +
                                    "</div></div></div>";
                            }
                            return linkDataContentEditDelete;
                        }
                    }]
            });

            $('#tblProgramLevelAdmin').DataTable();
            if (userRlN.toLowerCase() !== "super admin" && userRlN.toLowerCase() !== "organization full") {
                $("#tblProgramLevelAdmin").DataTable().column(6).visible(false);
                $("#tblProgramLevelAdmin").DataTable().column(7).visible(false);
            }
            else {
                //  $("#tblProgramLevelAdmin").DataTable().column(6).visible(true);
                //  $("#tblProgramLevelAdmin").DataTable().column(7).visible(true); 
            }
            $('.nav-tabs a[href="#program"]').tab('show');
            $('#program').addClass('show');
            $("#dvLoadingGif").hide();
            if ($("#my-awesome-dropzone").length > 0) {
                $("#my-awesome-dropzone").dropzone({
                    // var myDropzone = new Dropzone("#my-awesome-dropzone", {
                    url: "/Account/UploadImage", dictInvalidFileType: "Please upload only jpg .jpeg .png files",
                    acceptedFiles: ".png,.jpeg,.jpg",
                    paramName: "file",
                    maxFiles: 1,
                    maxFilesize: 10,
                    uploadMultiple: false,
                    init: function (element) {
                        this.on("maxfilesexceeded", function (file) {
                            swal({
                                title: 'No more files.',
                                icon: "error"
                            });
                            this.removeAllFiles();

                            this.addFile(file);
                        });
                        this.on('addedfile', function () {
                            /* Valid only in the dropzone . If a repetitive document shows ALERT and the previous item will disappear. */
                            this.on('addedfile', function (file) {
                                if (this.files.length > 1) {
                                    this.removeFile(this.files[0]);
                                }
                            });
                        });
                        this.on("sending", function (file, xhr, formData) {
                            $("#btnOrganisationAdminLevelDetail").attr('disabled', 'disabled');
                        });
                        this.on("success", function (file, response, responseText) {
                            responseText = file.status;// or however you would point to your assigned file status here;
                            if (responseText === "success") {

                                $("#hdnIsNewUpload").val(1);
                                $("#adminLevelModel_UserImagePath").val(response.data);
                            }
                            $("#btnOrganisationAdminLevelDetail").removeAttr('disabled');
                        });
                    },
                    addRemoveLinks: true,
                    removedfile: function (file) {
                        if ($("#hdnIsFormSubmitted").val() === "false" && $("#hdnIsNewUpload").val() === '1') {
                            $.ajax({
                                type: 'POST',
                                url: '/Account/RemoveImage',
                                data: { userId: data.data, imgPath: $("#adminLevelModel_UserImagePath").val(), userPhotoEntityType: $("#hdnUserImageType").val() },
                                success: function () {
                                    $("#adminLevelModel_UserImagePath").val('');
                                }
                            });


                        }
                        else { $("#adminLevelModel_UserImagePath").val(''); }
                        var ref;
                        return (ref = file.previewElement) !== null ? ref.parentNode.removeChild(file.previewElement) : void 0;

                    }
                });
            }
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
        });
    }
    else if (idName === "12") {
        $.ajax({
            method: 'GET',
            url: '/Program/TransactionView',
            data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $.validator.unobtrusive.parse($("#spnProgramTab"));
            var tb = $("#tblTransaction").dataTable({
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
                "bLengthChange": false,
                "ajax": {
                    "url": "/Program/LoadAllTransactions",
                    "type": "Post",
                    "data": { id: $("#hdnProgramId").val(), 'dateMonth': $("#hdnDateMonth").val() },
                    "datatype": "application/json",
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
                        "orderable": false
                    },
                    {
                        "targets": [2],
                        "visible": true,
                        "searchable": true,
                        "orderable": false
                    },
                    {
                        "targets": [3],
                        "visible": true,
                        "searchable": true,
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
                        "searchable": true,
                        "orderable": true
                    }
                    ],

                "columns": [
                    { "data": "Id", "id": "Id", "name": "Id", "autoWidth": true },
                    {
                        "id": "CreatedDate", "name": "CreatedDate", "autoWidth": true,
                        "render": function (data, type, full, mets) { return moment(full.TransactionDate).format('MMMM Do YYYY'); }
                    },
                    {
                        "data": "CreatedDate", "name": "CreatedDate", "autoWidth": true, "className": "text-center",
                        "render": function (data, type, full, mets) { return moment(full.TransactionDate).format('hh:mm:ss A'); }
                    },
                    {
                        "data": "AccountType", "name": "AccountType", "autoWidth": true, "className": "text-center"
                    },

                    {
                        "data": "MerchantName", "name": "MerchantName", "autoWidth": true, "className": "text-center"
                    },
                    {
                        "name": "TransactionDate", "autoWidth": true,
                        "render": function (data, type, full, mets) {

                            return full.Amount.toString();
                        }
                    }],
                initComplete: function () {
                    $('.dataTables_filter').show();
                    $('.dt-buttons').hide();
                    if (tb.fnGetData().length === 0) {
                        $('#aExportTransaction').hide();
                    }
                    else {
                        $('#aExportTransaction').show();
                    }
                    $('#aExportTransaction').on('click', function () {
                        $("#transactionFilter").text("");
                        var searchValue = $('.dataTables_filter input').val();
                        var primaryprogId = $("#hdnProgramId").val();
                        var dateMonth = $("#hdnDateMonth").val();
                        $.ajax({
                            type: "POST",
                            url: "/Program/ProgramTransactionExportExcel/?searchValue=" + searchValue + "&ppId=" + primaryprogId + "&dateMonth=" + dateMonth,
                            cache: false,
                            success: function (data) {
                                //get the file name for download
                                if (data.fileName !== "") {
                                    //use window.location.href for redirect to download action for download the file
                                    window.location.href = '/Program/Download/?fileName=' + data.fileName;
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
                        exportOptions: {
                            format: {
                                body: function (data, row, column, node) {
                                    if (column === 0) {
                                        return moment(data, 'MMMM Do YYYY').format('MM/DD/YYYY');
                                    }
                                    else if (column === 4) {
                                        var d = data.replace("$", "");
                                        return '$' + d;
                                    }
                                    else {
                                        return data;
                                    }
                                }
                            },
                            columns: [1, 2, 3, 4, 5],
                            modifier: {
                                search: 'applied',
                                order: 'applied'
                            }
                        },
                        title: ($('#h2text').text() + ' - ' + $('#h6text').text() + ' - ').replace(/\s/g, '') + 'Program Transactions List'
                    }
                ]//,
            });
            $('.nav-tabs a[href="#transactions"]').tab('show');
            $('#transactions').addClass('show');
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });

        });
    }
    else if (idName === "20") {
        $.ajax({
            method: 'GET',
            url: '/Program/CardHolderAgreement',
            data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $.validator.unobtrusive.parse($("#spnProgramTab"));
            $("#tblCardHolderAgreement").dataTable({
                "processing": true, // for show progress bar
                "serverSide": true, // for process server side
                "filter": true, // this is for disable filter (search box)
                "orderMulti": false, // for disable multiple column at once
                "pageLength": 10,
                "order": [[1, "asc"]],
                "oLanguage": {
                    "sEmptyTable": "No data available."
                },
                "bLengthChange": false,
                "ajax": {
                    "url": "/Program/LoadAllCardholderAgreements",
                    "data": { id: $("#hdnProgramId").val() },
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
                    //{
                    //    "targets": [2],
                    //    "visible": true,
                    //    "searchable": true,
                    //    "orderable": true
                    //},
                    {
                        "targets": [2],
                        "visible": true,
                        "searchable": true,
                        "orderable": true
                    },
                    {
                        "targets": [3],
                        "visible": true,
                        "searchable": true,
                        "orderable": true
                    },
                    {
                        "targets": [4],
                        "visible": true,
                        "searchable": false,
                        "orderable": false
                    }
                    ],

                "columns": [
                    { "data": "CardHolderAgreementId", "id": "CardHolderAgreementId", "name": "CardHolderAgreementId", "autoWidth": true },
                    {
                        "data": "SrNo", "name": "SrNo", "autoWidth": true, "className": "text-center"
                    },
                    //{
                    //    "name": "CardholderAgreement",
                    //    "autoWidth": true,
                    //    "render": function (data, type, full, mets) { return '<div  title=' + full.CardHoldrAgreementContent + '>' + full.CardHoldrAgreementContent.substring(0, 50) + '...</div>'; }
                    //    // "data": "CardHoldrAgreementContent", "name": "CardHoldrAgreementContent", "autoWidth": true, "className": "text-center"
                    //},
                    {
                        "data": "versionNo", "name": "versionNo", "autoWidth": true, "className": "text-center"
                    },
                    {
                        "autoWidth": true,
                        "className": "text-center",
                        "render": function (data, type, full, mets) { return moment(full.CreatedDate).format('MMMM Do YYYY'); }
                    },
                    {
                        "render": function (data, type, full, mets) {
                            var linkDataContentEditDelete = "";
                            if (userRlN.toLowerCase() == "super admin" || userRlN.toLowerCase() == "organization full" || userRlN.toLowerCase() == "program full") {
                                linkDataContentEditDelete += "<div class='linked-delete-custom-action'><div class='linked-down' onclick=dotclick(this)>" +
                                    "<div class='img-dots'></div></div>" +
                                    "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                                    "<ul><li><a  href='javascript:void(0);' onclick=CardholderDetail('" + full.ProgramIdEnc + "','" + full.CardHolderAgreementIdEnc + "');>View detail</a></li></ul>" +
                                    "</div></div></div>";
                            }
                            return linkDataContentEditDelete;
                        }
                    }],
                initComplete: function () {
                    CardholderDetail($("#hdnProgramId").val(), "");
                },
            });

            $('.nav-tabs a[href="#cardholderagreement"]').tab('show');
            $('#cardholderagreement').addClass('show');
            $("#dvLoadingGif").hide();

        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
        });
    }
    else if (idName === "21") {
        $.ajax({
            method: 'GET',
            url: '/Program/CardHolderAgreementHistory',
            data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnProgramTab").html(data);
            $.validator.unobtrusive.parse($("#spnProgramTab"));
            $("#tblCardHolderAgreementHistory").dataTable({
                "processing": true, // for show progress bar
                "serverSide": true, // for process server side
                "filter": true, // this is for disable filter (search box)
                "orderMulti": false, // for disable multiple column at once
                "pageLength": 10,
                "order": [[1, "asc"]],
                "oLanguage": {
                    "sEmptyTable": "No data available."
                },
                "bLengthChange": false,
                "ajax": {
                    "url": "/Program/LoadAllCardholderAgreementsHistory",
                    "data": { id: $("#hdnProgramId").val() },
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
                        "orderable": false
                    }
                    ],

                "columns": [
                    { "data": "UserId", "id": "UserId", "name": "UserId", "autoWidth": true },
                    {
                        "data": "RowNum", "name": "RowNum", "autoWidth": true, "className": "text-center"
                    },
                    {
                        "data": "CardHolderName", "name": "CardHolderName", "autoWidth": true, "className": "text-center"
                    },
                    {
                        "render": function (data, type, full, mets) {
                            var linkDataContentEditDelete = "";
                            if (userRlN.toLowerCase() == "super admin" || userRlN.toLowerCase() == "organization full" || userRlN.toLowerCase() == "program full") {
                                linkDataContentEditDelete += "<div><div>" +
                                    "<div></div></div>" +
                                    "<div><div >" +
                                    "<ul><li><a  href='javascript:void(0);' onclick=UserVersionHistory('" + full.ProgramIdEnc + "','" + full.CardHolderAgreementIdEnc + "','" + encodeURIComponent(full.CardHolderName) + "');>View User's Version History</a></li></ul>" +
                                    "</div></div></div>";
                            }
                            return linkDataContentEditDelete;
                        }
                    }],
                initComplete: function () {
                    // CardholderDetail($("#hdnProgramId").val(), "");
                },
            });

            $('.nav-tabs a[href="#cardholderagreementHistory"]').tab('show');
            $('#cardholderagreementHistory').addClass('show');
            oModalVersionTable = $('#tblCardHolderAgreementHistoryVersions').DataTable({
                "destroy": true,
                "bPaginate": true,
                "bLengthChange": false,
                "bServerSide": false,
                "searching": false,
                "ordering": false,
                "bStateSave": true,
                "info": false,
                "dom": 'lBfrtip',
                "oLanguage": {
                    "sEmptyTable": "No Versions available."
                },

                "columnDefs":
                    [{
                        "targets": [0],
                        "visible": true,
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
                    }],
                "columns": [
                    {
                        "data": "RowNum",
                        "name": "0",
                        "autoWidth": true,
                        "className": "text-center"
                    },
                    {
                        "data": "VersionNo",
                        "name": "1",
                        "autoWidth": true,
                        "className": "text-center"
                    },
                    {
                        "data": "DateAcceptedString",
                        "name": "2",
                        "autoWidth": true,
                        "className": "text-center"
                    }],
                initComplete: function () {
                    // var $buttons = $('.dt-buttons').hide();
                    //$('#aExportUsers').on('click', function () {
                    //    var id = $("#ActivelinkedId").val();
                    //    var dateMonth = $("#hdnDateMonth").val();
                    //    var dataSetPlan = $("#hdnPlan").val();
                    //    $.ajax({
                    //        type: "POST",
                    //        url: "/Benefactor/BenefactorTransactionExportExcel/?id=" + id + "&dateMonth=" + dateMonth + "&dataSetPlan=" + dataSetPlan,
                    //        cache: false,
                    //        success: function (data) {
                    //            //get the file name for download
                    //            if (data.fileName !== "") {
                    //                //use window.location.href for redirect to download action for download the file
                    //                window.location.href = '/Benefactor/Download/?fileName=' + data.fileName;
                    //            }
                    //        },
                    //        error: function (data) {
                    //            swal({
                    //                title: "There is some issue in processing!",
                    //                icon: "error"

                    //            });
                    //        }
                    //    });

                    //});
                },
                buttons: [
                    {
                        extend: 'excel',
                        text: '',
                        filename: 'Versions',
                        exportOptions: {
                            modifier: {
                                search: 'applied',
                                order: 'applied'
                            }
                        }
                    }
                ]//,

            });
            $("#dvLoadingGif").hide();

        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
        });
    }

}
//Merchant
function DeleteData(id, Jpos_MerchantEncId) {
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
            Delete(id, Jpos_MerchantEncId);
        }
    });
}
function Delete(id, Jpos_MerchantEncId) {
    var url = '/Merchant/Delete';
    $.post(url, { ID: id, JposMerId: Jpos_MerchantEncId }, function (data) {
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
function Clone(id, name) {
    $("#dvLoadingGif").show();
    var url = '/Merchant/CloneMerchant';
    $.post(url, { merchantId: id }, function (data) {
        if (data) {
            swal({
                title: "Merchant " + $(name).attr('data-name') + " has been cloned successfully!",
                icon: "success"

            });
            var oTable = $('#merchant').DataTable();
            oTable.draw();
            $("#dvLoadingGif").hide();
        }
        else {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        }
    });
}
//Plan
function ChangePlanStatus(planId, e) {
    var checked = e.checked;
    $("#dvLoadingGif").hide();
    $.ajax({
        type: "POST",
        url: "/Program/ChangePlanStatus/",
        data: { 'planId': planId, isActive: checked },
        dataType: "json",
        success: function (data) {
            $("#dvLoadingGif").hide();
            if (data.data > 0 && data.success) {
                swal({
                    title: "Status has been changed successfully!",
                    icon: "success"

                });
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
            $("#dvLoadingGif").hide();
        }
    });
}
function DeletePlanData(id, Jpos_PlanEncId) {
    swal({
        title: "Are you sure you want to delete the plan?",
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
            Deleteplan(id, Jpos_PlanEncId);
        }
    });
}
function Deleteplan(id, Jpos_PlanEncId) {
    var url = '/Program/DeletePlan';
    $.post(url, { ID: id, Jpos_PlanId: Jpos_PlanEncId }, function (data) {
        if (data) {
            swal({
                title: "Plan has been deleted successfully!",
                icon: "success"

            });
            var oTable = $('#plan').DataTable();
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
function ClonePlan(id, name) {
    $("#dvLoadingGif").show();
    var url = '/Program/ClonePlan';
    $.post(url, { planId: id }, function (data) {
        if (data) {
            swal({
                title: "Plan " + $(name).attr('data-name') + " has been cloned successfully!",
                icon: "success"

            });
            var oTable = $('#plan').DataTable();
            oTable.draw();
            $("#dvLoadingGif").hide();
        }
        else {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        }
    });
}
//ProgramAccount
function ChangeAccountStatus(Id, e) {
    var checked = e.checked;
    $("#dvLoadingGif").hide();
    $.ajax({
        type: "POST",
        url: "/Program/ChangeAccountStatus/",
        data: { 'Id': Id, isActive: checked },
        dataType: "json",
        success: function (data) {
            $("#dvLoadingGif").hide();
            if (data.data > 0 && data.success) {
                swal({
                    title: "Status has been changed successfully!",
                    icon: "success"

                });
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
            $("#dvLoadingGif").hide();
        }
    });
}
function DeleteAccountData(id, Jpos_ProgramEncAccountId) {
    swal({
        title: "Are you sure you want to delete the account?",
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
            Deleteaccount(id, Jpos_ProgramEncAccountId);
        }
    });
}
function Deleteaccount(id, Jpos_ProgramEncAccountId) {
    var url = '/Program/DeleteAccount';
    $.post(url, { ID: id, Jpos_PrgAccountId: Jpos_ProgramEncAccountId }, function (data) {
        if (data) {
            swal({
                title: "Account has been deleted successfully!",
                icon: "success"
            });
            var oTable = $('#programaccount').DataTable();
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
function CloneAccount(id, name) {
    $("#dvLoadingGif").show();
    var url = '/Program/CloneAccount';
    $.post(url, { Id: id }, function (data) {
        if (data) {
            swal({
                title: "Account " + $(name).attr('data-name') + " has been cloned successfully!",
                icon: "success"

            });
            var oTable = $('#programaccount').DataTable();
            oTable.draw();
            $("#dvLoadingGif").hide();
        }
        else {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        }
    });
}
function ResetProgramAccount(type) {
    if (type < 4) {
        if (type === 1)
            $("#passType").val('');
        if (type === 2)
            $("#intialBalanceCount").val('');
        $("#maxPassUsage").val('');
        $("#maxPassPerWeek").val('');
        $("#maxPassPerMonth").val('');
        if (type === 2)
            $("#resetPeriodType").val('');
        $("#resetDay").val('');
        $("#resetTime").val('12:00 AM');
        $("#resetDateOfMonth").val('');
        $("#flexMaxSpendPerDay").val('');
        $("#flexMaxSpendPerWeek").val('');
        $("#flexMaxSpendPerMonth").val('');
        $("#vplMaxBalance").val('');
        $("#vplMaxAddValueAmount").val('');
        $("#vplMaxNumberOfTransaction").val('');
        $("#exchangePassValue").val('');
        $("#exchangeResetPeriodType").val('');
        $("#exchangeResetDay").val('');
        $("#exchangeResetTime").val('12:00 AM');
        $("#exchangeResetDateOfMonth").val('');
        var $radios = $('input:radio[name=isPassExchangeEnabled]');
        $radios.filter('[value=True]').prop('checked', true);
    }
    else {
        if (type === 4) {
            $("#exchangePassValue").val('');
            $("#exchangeResetPeriodType").val('');
        }
        $("#exchangeResetDay").val('');
        $("#exchangeResetTime").val('12:00 AM');
        $("#exchangeResetDateOfMonth").val('');
    }
}
//Branding
function DeleteBranding(reId) {
    $("#dvLoadingGif").show();
    $.ajax({
        type: "POST",
        url: "/Program/DeleteBranding/",
        data: { 'Id': reId },
        dataType: "json",
        success: function (data) {
            $("#dvLoadingGif").hide();
            if (data.data > 0 && data.success) {
                swal({
                    title: "Branding has been deleted successfully!",
                    icon: "success"

                });

                var tab = '13';
                var stringElement = "tab_" + tab;
                $("#" + stringElement).addClass('active');
                GetTabViewComponent(tab, "", "", "", "");

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
//Common Methods
var CheckForStartEndDate = function () {
    $("#spnEndDateGreater").removeClass("field-validation-valid");
    $("#spnEndDateGreater").addClass("field-validation-error");
    var chkStartDate = new Date(moment($("#startDate").val(), 'MM/DD/YYYY'));
    var chkEndDate = new Date(moment($("#endDate").val(), 'MM/DD/YYYY'));
    if (chkStartDate != '' && chkEndDate != '') {
        if (chkStartDate >= chkEndDate) {
            $("#spnEndDateGreater").show();
            $("#spnEndDateGreater").html('<span>End date must be greater than start date.</span>');
            return false;
        }
        else { $("#spnEndDateGreater").hide(); return true; }
    }
};
function ConvertTimeformat(format, str) {
    var t = "";
    if (str !== undefined && str !== "") {
        var time = str;
        var hours = Number(time.match(/^(\d+)/)[1]);
        var minutes = Number(time.match(/:(\d+)/)[1]);
        var AMPM = time.match(/\s(.*)$/)[1];
        if (AMPM === "PM" && hours < 12) hours = hours + 12;
        if (AMPM === "AM" && hours === 12) hours = hours - 12;
        var sHours = hours.toString();
        var sMinutes = minutes.toString();
        if (hours < 10) sHours = "0" + sHours;
        if (minutes < 10) sMinutes = "0" + sMinutes;
        t = sHours + ":" + sMinutes;
    }
    return t;
}
//======================================
$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results === null) {
        return null;
    }
    else {
        return decodeURI(results[1]) || 0;
    }
};

function DeleteAccountHolder(id, programId, JposAccId) {
    var url = '/Program/DeleteAccountHolder';
    $.post(url, { ID: id, prgId: programId, jpos_AccountHolderId: JposAccId }, function (data) {
        if (data) {
            swal({
                title: "Account holder has been deleted successfully!",
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
    });
}

function DeleteAccountHolderData(id, programId, fName, lName, JposAccId) {
    swal({
        title: "Are you sure you want to delete the " + fName + " " + lName + "?",
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
            DeleteAccountHolder(id, programId, JposAccId);
        }
    });
}

function CardholderDetail(programId, cardHolderId) {
    $("#dvLoadingGif").show();
    $.ajax({
        type: "GET",
        url: "/Program/GetCardHolderAgreementById/",
        data: { 'programIdEnc': programId, 'cardHolderAgreementIdEnc': cardHolderId },
        dataType: "json",
        success: function (data) {
            if (data.data !== null || data.data !== undefined) {
                // $("#CardHolderAgreementContent").val(data.data.CardHolderAgreementContent);
                CKEDITOR.instances['CardHolderAgreementContent'].setData(data.data.CardHolderAgreementContent);
                if (data.data.versionNo != null) {
                    $("h2.class-haed").text("Edit Card holder agreement content*");
                    $("#lblVersionNo").html(data.data.versionNo);
                    $("#btnCardHolderAgreementDetail").text("Update");
                }
                else {
                    $("#lblVersionNo").html('Version.1');
                    $("h2.class-haed").text("Add Card Holder Agreement Content*");
                    $("#btnCardHolderAgreementDetail").text("Add");
                }
                $("#CardHolderAgreementId").val(data.data.CardHolderAgreementIdEnc);
                //$('html,body').animate({
                //    scrollTop: $("#CardHolderAgreementContent").offset().Bottom
                //}, 'slow');

                $("#dvLoadingGif").hide();
            } else {
                $("h2.class-haed").text("Edit Card holder agreement content");
                $("#btnCardHolderAgreementDetail").text("Update");
                $("#lblVersionNo").html('Version.1');
                $("#editor").html('Add your content here.');
                $("#CardHolderAgreementId").val(data.data.CardHolderAgreementId);
                $("#dvLoadingGif").hide();
            }
        },
        error: function () {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        }
    });
}

function UserVersionHistory(prgdId, UId, UserName) {
    $("#dvLoadingGif").show();
    $.ajax({
        type: "GET",
        url: "/Program/GetUserAgreementHistoryVersions/",
        data: { 'programIdEnc': prgdId, 'cardHolderAgreementIdEnc': UId },
        dataType: "json",
        success: function (data) {
            $("#lblUserNameVersion").html(decodeURIComponent(UserName));
            if (data.data !== null || data.data !== undefined) {

                oModalVersionTable.clear().draw();
                oModalVersionTable.rows.add(data.data); // Add new data
                oModalVersionTable.columns.adjust().draw();
                $("#dvLoadingGif").hide();
                $("#myModal").modal();
            }
        },
        error: function () {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        }
    });
}
