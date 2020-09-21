var refreshComponent = '';
var program = [];
var programtypes = [];
var accTypepChecked = [];
var selected = [];
var tab = '1';
var arrhrofoperation = [];
var arrholidayhrs = [];
var arrmealperiod = [];
var arrterminal = [];
var days = [];
var calendarDefaultDate;

$(document).ready(function () {
    calendarDefaultDate = new Date();

    GetTabViewComponent("1", "");
    $(".orgTab").click(function (e) {
        if (($("#hdnMerchantId").val() === "" || $("#hdnMerchantId").val() === "3000") && $(this).attr('id') !== 'tab_1') {
            swal({
                title: "Please add Merchant detail before moving further.",
                icon: "error"
            });
        }
        else {

            $(".orgTab").removeClass('active');
            var idName = $(this).attr('id').replace('tab_', '');
            GetTabViewComponent(idName, "");
            var stringElement = "tab_" + idName;
            $("#" + stringElement).addClass('active');
        }
        return false;
    });
    $(document).on('click', '#btnmerchantdetailinfo', function () {

        if ($("#form").validate() && $("#form").valid()) {
            $("#dvLoadingGif").show();
            debugger;
            if ($("#Location").val() == "") {
                $("#addresslatlongerror").show();
                $("#dvLoadingGif").hide();
                $("#Address").focus();
                return false;
            }
            $("#addresslatlongerror").hide();
            if ($(".clsCheckbox:checkbox:checked").length > 0) {
                $("#programselecterror").hide();
                if ($("#form").validate() && $("#form").valid()) {
                    var orprog = $('#SelectedOrgProgram option:selected');
                    var organisationId = $("#hdnMerchantId").val();
                    program = [];
                    var primaryPrg = {
                        ProgramId: $("#PrimaryProgramId").val(),
                        IsPrimaryAssociation: true
                    }
                    //  program.push($("#PrimaryProgramId").val());//Primary ProgramId
                    program.push(primaryPrg);
                    $(orprog).each(function (index, brand) {
                        //var data = $(this).val();
                        var data = {
                            ProgramId: $(this).val(),
                            IsPrimaryAssociation: false
                        };
                        program.push(data);
                    });
                    var orgAccountType = $('.chkaccType:checkbox:checked');
                    accTypepChecked = [];
                    $(orgAccountType).each(function (index, brand) {

                        var data = $(this).attr('id');
                        accTypepChecked.push(data);
                    });
                    var orgBusinessType = $('.chkbusinesstype:radio:checked').attr('id');
                    var MerchantDetailModel = {
                        OrganisationId: organisationId,
                        Address: $("#Address").val(),
                        City: $("#City").val(),
                        State: $("#State").val(),
                        Country: $("#Country").val(),
                        Zip: $("#Zip").val(),
                        ContactNumber: $("#ContactNumber").val(),
                        OrganisationName: $("#OrganisationName").val(),
                        OrganisationProgramSelect: program,
                        SelectedOrgAccType: accTypepChecked,
                        Description: $("#Description").val(),
                        Showmap: $("#ShowMap").is(":checked") ? true : false,
                        Website: $("#Website").val(),
                        FacebookURL: $("#FacebookURL").val(),
                        TwitterURL: $("#TwitterURL").val(),
                        InstagramHandle: $("#InstagramHandle").val(),
                        BusinessTypeId: orgBusinessType,
                        Location: $("#Location").val(),
                        ImagePath: $("#ImageFileName").val(),
                        PrimaryProgramIdEnc: $("#hdnPrimaryPPId").val(),
                        Jpos_MerchantEncId: $("#Jpos_MerchantEncId").val()
                    };
                    $.post("/Merchant/PostMerchnatDetailInformation",
                        MerchantDetailModel,
                        function (data) {
                            $("#dvLoadingGif").hide();
                            if (data.data !== "" || data.data !== "0") {
                                tab = '2';
                                var stringElement = "tab_" + tab;
                                $("#" + stringElement).addClass('active');
                                if ($("#hdnMerchantId").val() === '3000') {
                                    var poId = 'poId=' + $.urlParam('poId');
                                    var ppN = 'ppN=' + $.urlParam('ppN');
                                    var ppId = 'ppId=' + $.urlParam('ppId');
                                    var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname.replace('3000', data.data) + '?' + poId + '&' + ppId + '&' + ppN;
                                    window.history.pushState({ path: newurl }, '', newurl);
                                }
                                $("#hdnMerchantId").val(data.data);
                                $("#hdnMerchantName").val($("#OrganisationName").val());
                                GetTabViewComponent(tab, "");
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
                else return false;
            }
            else { $("#dvLoadingGif").hide(); $("#programselecterror").show(); return false; }
        }
        else {
            $("#form").validate();

            if ($(".clsCheckbox:checkbox:checked").length === 0) { $("#programselecterror").show(); } else {
                $("#programselecterror").hide();
                $('html,body').animate({
                    scrollTop: $(".chkaccType").offset().top
                }, 'slow');
            }
        }
    });
    $(document).on('click', '#btnOrganisationAdminLevelDetail', function () {
        if ($("#frmOrgAdminLevelDetail").validate() && $("#frmOrgAdminLevelDetail").valid()) {
            $("#dvLoadingGif").show();
            var organisationId = $("#hdnMerchantId").val();
            var brands = $('#MerchantAccessibility option:selected');
            var merchantAccess = [];
            $(brands).each(function (index, brand) {
                var data = {
                    merchantId: $(this).val()
                };
                merchantAccess.push(data);
            });
            var programTypes = [];
            programTypes.push(0);
            var adminLevelModel = {
                UserId: $("#adminLevelModel_UserId").val(),
                Name: $("#adminLevelModel_Name").val(),
                LastName: $("#adminLevelModel_LastName").val(),
                UserImagePath: $("#adminLevelModel_UserImagePath").val(),
                EmailAddress: $("#adminLevelModel_EmailAddress").val(),
                PhoneNumber: $("#adminLevelModel_PhoneNumber").val(),
                RoleId: $("#adminLevelModel_RoleId").val(),
            };
            var MerchantAdminDetailModel = {
                adminLevelModel:adminLevelModel,
                OrganisationId: organisationId,
                Custom1: $("#Custom1").val(),
                ProgramsAccessibility: programTypes,
                MerchantAccessibility: merchantAccess
            };
            $.post("/Merchant/PostMerchantAdminDetailInformation",
                MerchantAdminDetailModel,
                function (data, message) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" && data.data !== "0" && data.data !== null) {
                        $('#adminLevelModel_EmailAddress').removeAttr('readonly');
                        $('#adminLevelModel_PhoneNumber').removeAttr('readonly');
                        $('#adminLevelModel_PhoneNumber').removeAttr('disabled');
                        $(".clsHeaderForm").html('New Admin');
                        $("#adminLevelModel_UserImagePath").val('');
                        $('#Title').val('');
                        $("#frmOrgAdminLevelDetail")[0].reset();
                        $('#PostedFileUploadError').hide();
                        $('#wizardPicturePreview').attr('src', '/images/icon-profile.png');
                        $("#hdnIsFormSubmitted").val('true');
                        $("#hdnOrganisationId").val(organisationId);
                        $("#btnOrganisationAdminLevelDetail").html('ADD');
                        var imgDropzone = Dropzone.forElement("#my-awesome-dropzone");
                        imgDropzone.removeAllFiles();
                        if ($("#adminLevelModel_UserId").val() !== '' && $("#adminLevelModel_UserId").val() !== '0') {
                            swal({
                                title: "Admin details has been updated successfully.",
                                icon: "success"
                            });
                            $("#adminLevelModel_UserId").val('0');
                        } else {
                            swal({
                                title: "Admin details has been submitted successfully.",
                                icon: "success"
                            });
                            $("#adminLevelModel_UserId").val('0');
                        }

                        var oTable = $('#tblOrganisationAdmins').DataTable();
                        oTable.draw();

                        programTypes = [];
                        merchantAccess = [];
                        //    $('#MerchantAccessibility').multiselect("deselectAll", false);
                        $('#MerchantAccessibility').multiselect('refresh');
                    }
                    else {
                        swal({
                            title: data.message,
                            icon: "error"

                        });
                    }
                });
            return false;
            //ajaxcall
        }
        else { $("#form").validate(); }
    });
    $(document).on('click', '#btnmerchantrewardinfo', function () {
     
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
            var organisationId = $("#hdnMerchantId").val();
            var orgBusinessType = $('.chkbusinesstype:radio:checked').attr('id');
            var MerchantRewardModel = {
                MerchantId: organisationId,
                RewardTitle: $("#RewardTitle").val(),
                RewardSubTitle: $("#RewardSubTitle").val(),
                Description: $("#Description").val(),
                OfferTypeId: 2,
                OfferSubTypeId: $("#OfferSubTypeId").val(),
                Amount: $("#OfferSubTypeId").val() === "4" ? $("#Amount").val() : 0,
                Visits: $("#OfferSubTypeId").val() === "3" ? $("#Visits").val() : 0,
                StartDate: $("#StartDate").val(),
                StartTime: ConvertTimeformat("24", $("#StartTime").val()),
                EndDate: $("#EndDate").val(),
                EndTime: ConvertTimeformat("24", $("#EndTime").val()),
                BackGroundColor: $("#BackGroundColor").val() === "" ? "#3F43CE" : $("#BackGroundColor").val(),
                BusinessTypeId: orgBusinessType,
                IsPublished: $("#IsPublished").is(":checked") ? true : false,
                Id: $("#hdnPromotionId").val()
            };
            $.post("/Merchant/PostMerchnatRewardInformation",
                MerchantRewardModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" || data.data !== "0") {
                        var title = '';
                        if ($("#hdnPromotionId").val() === null || $("#hdnPromotionId").val() === '') {
                            title = "Reward has been added successfully.";
                        }
                        else {
                            title = "Reward has been updated successfully.";
                        }
                        swal({
                            title: title,
                            icon: "success"
                        }, function () {
                            tab = '7';
                            var stringElement = "tab_" + tab;
                            $("#" + stringElement).addClass('active');
                            GetTabViewComponent(tab, "");
                            $("#hdnMerchantId").val(organisationId);
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
    $(document).on('click', '#btnMerchantBusinessInfo', function () {
        var isReturnSucess = true;

        $('.spnShowHolidayNameError').each(function () {
            if (parseInt(countCheck) !== 0 && $('#HolidayHours_' + countCheck + '__HolidayDate').val() !== '') {
                var textBoxId = $(this).attr('id').replace('spn_', '');
                if ($('#' + textBoxId).val() === '') {
                    $(this).show();
                    isReturnSucess = false;
                } else { $(this).hide(); }
            }
        });
        if (!isReturnSucess)
            return false;
        if ($("#form").validate() && $("#form").valid()) {
            $("#dvLoadingGif").show();
            var organisationId = $("#hdnMerchantId").val();

            var hrofoperation = $('.select-business-hours');
            var holidayhrs = $('.select-date-hours');
            var mealperiod = $('.select-holidays-area');
            var terminal = $('.terminal-row');
            arrhrofoperation = [];
            $(hrofoperation).each(function (index, brand) {
                var HOfO = {};
                HOfO["WorkingDay"] = $('#HoursOfOperation_' + index + '__WorkingDay').val();
                HOfO["OpenTime"] = ConvertTimeformat("24", $('#HoursOfOperation_' + index + '__OpenTime').val());
                HOfO["ClosedTime"] = ConvertTimeformat("24", $('#HoursOfOperation_' + index + '__ClosedTime').val());
                HOfO["IsHoliday"] = "false";
                arrhrofoperation.push(HOfO);
            });
            arrholidayhrs = [];
            $(holidayhrs).each(function (index, brand) {
                var HH = {};
                HH["IsForHolidayNameToShow"] = $('#HolidayHours_' + index + '__IsForHolidayNameToShow').is(":checked") ? "true" : "false";
                HH["HolidayName"] = $('#HolidayHours_' + index + '__HolidayName').val();
                HH["HolidayDate"] = $('#HolidayHours_' + index + '__HolidayDate').val();
                HH["OpenTime"] = ConvertTimeformat("24", $('#HolidayHours_' + index + '__OpenTime').val());
                HH["ClosedTime"] = ConvertTimeformat("24", $('#HolidayHours_' + index + '__ClosedTime').val());
                HH["IsHoliday"] = "true";
                arrholidayhrs.push(HH);
            });
            arrmealperiod = [];
            $(mealperiod).each(function (index, brand) {
                if (parseInt(index) > 0) {
                    var MP = {};

                    MP["title"] = $('#MealPeriod_' + index + '__title').val();
                    MP["opentime"] = ConvertTimeformat("24", $('#MealPeriod_' + index + '__openTime').val());
                    MP["closetime"] = ConvertTimeformat("24", $('#MealPeriod_' + index + '__closeTime').val());
                    MP["isSelected"] = $('#MealPeriod_' + index + '__isSelected_Value').is(":checked") ? "true" : "false";
                    var selecteddays = $('#MealPeriod_' + index + '__Selecteddays option:selected');
                    days = [];
                    $(selecteddays).each(function (i) {
                        var data = $(this).val();
                        days += data + ",";
                    });
                    MP["days"] = days.length > 0 ? days.replace(/,\s*$/, "") : "";
                    arrmealperiod.push(MP);
                }
            });
            arrterminal = [];
            $(terminal).each(function (index, brand) {
                var MT = {};
                MT["terminalid"] = $('#MerchantTerminal_' + index + '__terminalId').val();
                MT["terminalName"] = $('#MerchantTerminal_' + index + '__terminalName').val();
                MT["terminalType"] = $('#MerchantTerminal_' + index + '__terminalType').val();
                MT["Jpos_TerminalEncId"] = $('#MerchantTerminal_' + index + '__Jpos_TerminalId').val();
                arrterminal.push(MT);
            });
            var isclosed = $("#Merchant_isClosed").is(":checked") ? true : false;
            var isMaxChartVisible = $("#Merchant_isTrafficChartVisible_Value").is(":checked") ? true : false;
            var Merchant = {
                MerchantId: $('#Merchant_MerchantId').val(),
                maxCapacity: $('#Merchant_maxCapacity').val(),
                dwellTime: $('#Merchant_dwellTime').val(),
                isClosed: isclosed,
                isTrafficChartVisible: isMaxChartVisible
            };
            var MerchantBusinessInfoModel = {
                Id: organisationId,
                HoursOfOperation: arrhrofoperation,
                HolidayHours: arrholidayhrs,
                Merchant: Merchant,
                MerchantTerminal: arrterminal,
                MealPeriod: arrmealperiod
            };
            $.post("/Merchant/PostMerchnatBusinessInformation",
                MerchantBusinessInfoModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" || data.data !== "0") {
                        tab = '3';
                        var stringElement = "tab_" + tab;
                        $("#" + stringElement).addClass('active');
                        GetTabViewComponent(tab, "");
                        $("#hdnMerchantId").val(organisationId);
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

    $(document).on('click', '#btnSchedulePromotion', function () {
        var retfalse = 0;

        if (!CheckForStartEndDate()) {
            $('#EndDate').focus();
            retfalse = 1;
        }
        if ($("#IsDaily").val() === true || $("#IsDaily").val() === "true") {
            if (!checkForDateRangeWithDay()) {
                $('#RepeatDay').focus();
                retfalse = 1;
            }
        }
        if ($("#StartTime").val() !== '' && $("#EndTime").val() !== '') {
            var strStartTime = $("#StartTime").val();
            var strEndTime = $("#EndTime").val();

            var startTime = new Date().setHours(GetHours(strStartTime), GetMinutes(strStartTime), 0);
            var endTime = new Date(startTime);
            endTime = endTime.setHours(GetHours(strEndTime), GetMinutes(strEndTime), 0);
            if (startTime > endTime) {
                $("#spnEndTimeGreater").show();
                return false;
            }
            else {
                $("#spnEndTimeGreater").hide();
            }

        }
        if ($("#frmSchedulePromotion").validate() && $("#frmSchedulePromotion").valid()) {
            if (retfalse === 1) {
                return false;
            }
            $("#dvLoadingGif").show();

            var organisationId = $("#hdnMerchantId").val();

            var OrganisationPromotionDetailModel = {
                MerchantId: organisationId,
                PromotionId: $("#PromotionId").val(),
                PromotionTypeId: $("#PromotionTypeId").val(),
                PromotionImagePath: $("#ImageFileName").val(),
                PromotionDescription: $("#PromotionDescription").val(),
                PromoDetail: $("#PromoDetail").val(),
                StartDate: $("#StartDate").val(),
                StartTime: $("#StartTime").val() !== '' ? ConvertTimeformat("24", $("#StartTime").val()) : null,
                EndDate: $("#EndDate").val(),
                EndTime: $("#StartTime").val() !== '' ? ConvertTimeformat("24", $("#EndTime").val()) : null,
                RepeatDay: $("#RepeatDay").val(),
                BannerTypeId: $("input[name='BannerType']:checked").val(),
                BannerDescription: $("#BannerDescription").val(),
                IsActive: $("#IsActive").is(":checked") ? true : false,
                IsDaily: $("#IsDaily").val()
            };

            calendarDefaultDate = new Date($("#StartDate").val());
            $.post("/Merchant/PostMerchantPromotionInformation",
                OrganisationPromotionDetailModel,
                function (data, message) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" && data.data !== "0" && data.data !== null) {
                        var title = '';
                        if ($("#PromotionId").val() === null || $("#PromotionId").val() === '') {
                            title = "Promotion has been added successfully.";
                        }
                        else {
                            title = "Promotion has been updated successfully.";
                        }
                        swal({
                            title: title,
                            icon: "success"

                        });
                        $("#signle-schedule").modal('hide');
                        ClearModalPopupControls();
                        GetTabViewComponent("3", "");

                    }
                    else {
                        swal({
                            title: "There is some issue in processing! Please try again later.",
                            icon: "error"

                        });
                    }
                });
            return false;
            //ajaxcall
        }
        else {
            $("#frmSchedulePromotion").validate();
        }
    });
    $(document).on('change', '.chkaccType', function () {
        if ($(".clsCheckbox:checkbox:checked").length > 0) {
            $("#programselecterror").hide();
        }
        else {
            $("#programselecterror").show(); return false;
        }
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

    $(document).on('change', '.chkChangeActiveReward', function () {
        var oReward = {
            rewardActiveStatus: $(this).is(":checked"),
            RewardId: $(this).attr('id').split('_')[1]
        };
        $.ajax({
            type: "POST",
            url: "/Merchant/EditPromotionRewardActiveStatus/",
            data: { oReward: oReward },
            dataType: "json",
            success: function (data) {
            },
            error: function () {
                swal({
                    title: "Currently unable to process the request! Please try again later.",
                    icon: "error"

                });

            }
        });
    });
    $(document).on('change', '#OfferSubTypeId', function () {
        var t = $(this).val();
        if (t === '3') {
            $('.nov').show();
            $('.amt-spent').hide();
            $('#Amount').val('');
        }
        else if (t === '4') {
            $('.amt-spent').show();
            $('.nov').hide();
            $('#Visits').val('');
        }
        else {
            $('.nov').hide();
            $('.amt-spent').hide();
            $('#Amount').val('');
            $('#Visits').val('');
        }
    });
    $(document).on('click', '.view-rewards-detail', function () {
        var idName = $(this).attr('id').replace('tab_', '');
        GetTabViewComponent(idName, '');
    });
    $(document).on('click', '.show-rewards-detail', function () {
        GetTabViewComponent('4', '');
    });
    $(document).on('click', '#edtPromotion', function () {
        var pid = $(this).attr('value');
        GetTabViewComponent('4', pid);
    });
    $(document).on('keyup', '#RewardTitle', function () {
        $("#spnRewardTitle").text($(this).val());
    });
    $(document).on('keyup', '#RewardSubTitle', function () {
        $("#spnRewardSubTitle").text($(this).val());
    });
    $(document).on('keyup', '#Description', function () {
        $("#spnDescription").text($(this).val());
    });
    $(document).on('change', '#BackGroundColor', function () {
        $(".bg-colors").css("background-color", $(this).val());
    });
    $(document).on('change', '.chkbusinesstype', function () {
        var id = $(this).attr("value");
        var img = $("#imgIconPath_" + id).attr("src").split(".");
        var imgName = img[0];
        var imgext = img[1];
        $("#imgBusinessIcon").attr("src", imgName + "-white." + imgext);

    });
    //hours of operation
    $(document).on('click', '.add', function (e) {
        e.preventDefault();
        var currIndex = parseInt($(this).attr('value'));
        var newIndex = parseInt($(this).attr('value')) + 1;
        var cloneelement = $("#select-hours_" + currIndex).clone().attr("id", "select-hours_" + newIndex);
        cloneelement.find("*[id]").each(function () {
            $(this).attr("id", $(this).attr("id").replace(currIndex.toString(), newIndex.toString()));
            $(this).attr("name", $(this).attr("name").replace(currIndex.toString(), newIndex.toString()));
        });
        if (currIndex === 0)
            cloneelement.append($('<a class="delete-action" href="#">Remove</a>'));
        cloneelement.appendTo("#add-more-hours");
        cloneelement.find(".timepicker").timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
        $(this).attr('value', newIndex);
    });
    $(document).on('click', ".delete-action", function (e) {
        e.preventDefault();
        $(this).closest(".form-group-inner").remove();
        var currIndex = parseInt($('.add').attr('value'));
        var newIndex = currIndex - 1;
        $('.add').attr('value', newIndex);
        var hrofoperation = $('.select-business-hours');
        $(hrofoperation).each(function (index, brand) {
            var oldindex = $(this).attr("id").split('_')[1];
            $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
            $(this).find("*[id]").each(function () {
                $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
                $(this).attr("name", $(this).attr("name").replace(oldindex.toString(), index.toString()));
            });

        });

    });
    //holiday hours
    $(document).on('click', '.add-date-hours', function (e) {
        e.preventDefault();
        var currIndex = parseInt($(this).attr('value'));
        var newIndex = parseInt($(this).attr('value')) + 1;
        var cloneelement = $("#select-date-hours_" + currIndex).clone().attr("id", "select-date-hours_" + newIndex);
        cloneelement.find("*[id]").each(function () {
            $(this).attr("id", $(this).attr("id").replace(currIndex.toString(), newIndex.toString()));
            $(this).attr("name", $(this).attr("name").replace(currIndex.toString(), newIndex.toString()));
        });
        if (currIndex === 0)
            cloneelement.append($('<a class="delete-action-hh" href="#">Remove</a>'));
        cloneelement.appendTo("#add-more-datehours");


        $('#HolidayHours_' + newIndex + '__IsForHolidayNameToShow').attr('checked', false);
        $('#HolidayHours_' + newIndex + '__HolidayName').val('');
        $('#HolidayHours_' + newIndex + '__HolidayDate').val('');
        $('#HolidayHours_' + newIndex + '__OpenTime').val('');
        $('#HolidayHours_' + newIndex + '__ClosedTime').val('');
        cloneelement.find(".timepicker").timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
        cloneelement.find(".datetimepicker").datepicker({ startDate: new Date() });

        cloneelement.find(".datetimepicker").val('');
        $(this).attr('value', newIndex);
    });
    $(document).on('click', ".delete-action-hh", function (e) {
        e.preventDefault();
        $(this).closest(".select-date-hours").remove();
        var currIndex = parseInt($('.add-date-hours').attr('value'));
        var newIndex = currIndex - 1;
        $('.add-date-hours').attr('value', newIndex);
        var holidayhrsdate = $('.select-date-hours');
        $(holidayhrsdate).each(function (index, brand) {
            var oldindex = $(this).attr("id").split('_')[1];
            $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
            $(this).find("*[id]").each(function () {
                $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
                $(this).attr("name", $(this).attr("name").replace(oldindex.toString(), index.toString()));
            });

        });
    });
    //Terminal
    $(document).on('click', '.add-terminal-fields', function (e) {
        e.preventDefault();
        var currIndex = parseInt($(this).attr('value'));
        var newIndex = parseInt($(this).attr('value')) + 1;
        if (currIndex === 3) {
            swal({
                title: "You can add up to four Terminals only",
                icon: "error"
            });
            return false;
        }
        else {
            var cloneelement = $("#select-terminal_" + currIndex).clone().attr("id", "select-terminal_" + newIndex);
            cloneelement.find("*[id]").each(function () {
                $(this).attr("id", $(this).attr("id").replace(currIndex.toString(), newIndex.toString()));
                $(this).attr("name", $(this).attr("name").replace(currIndex.toString(), newIndex.toString()));
            });
            if (currIndex === 0)
                cloneelement.append($('<a class="delete-action-ter" href="#">Remove</a>'));
            cloneelement.appendTo("#add-terminal");
            $(this).attr('value', newIndex);
        }
    });
    $(document).on('click', ".delete-action-ter", function (e) {
        e.preventDefault();
        $(this).closest(".terminal-area").remove();
        var currIndex = parseInt($('.add-terminal-fields').attr('value'));
        var newIndex = currIndex - 1;
        $('.add-terminal-fields').attr('value', newIndex);
        var holidayhrsdate = $('.terminal-area');
        $(holidayhrsdate).each(function (index, brand) {
            var oldindex = $(this).attr("id").split('_')[1];
            $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
            $(this).find("*[id]").each(function () {
                $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
                $(this).attr("name", $(this).attr("name").replace(oldindex.toString(), index.toString()));
            });

        });
    });
    //MealPeriod
    $(document).on('click', '.add-meal-period', function (e) {
        e.preventDefault();

        var newIndex = parseInt($(this).attr('value')) + 1;
        var cloneelement = $("#select-holidays-hours_0").clone().attr("id", "select-holidays-hours_" + newIndex);
        cloneelement.find("*[id]").each(function () {
            $(this).attr("id", $(this).attr("id").replace("0", newIndex.toString()));
            $(this).attr("name", $(this).attr("name").replace("0", newIndex.toString()));
        });
        cloneelement.append($('<a class="delete-action-mealp" href="#">Remove</a>'));
        cloneelement.show();
        cloneelement.find(".mpbox").val('');
        cloneelement.find('.mpmulti').addClass('multiselect');
        cloneelement.find('.multiselect').multiselect({ numberDisplayed: 2 });
        cloneelement.find(".mprb").prop("checked", false);
        cloneelement.appendTo("#add-more-mealperiod");
        cloneelement.find(".timepicker").timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
        $(this).attr('value', newIndex);

    });
    $(document).on('click', ".delete-action-mealp", function (e) {
        e.preventDefault();
        $(this).closest(".select-holidays-area").remove();
        var currIndex = parseInt($('.add-meal-period').attr('value'));
        var newIndex = currIndex - 1;
        $('.add-meal-period').attr('value', newIndex);
        var holidayhrsdate = $('.select-holidays-area');
        $(holidayhrsdate).each(function (index, brand) {
            var oldindex = $(this).attr("id").split('_')[1];
            $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
            $(this).find("*[id]").each(function () {
                $(this).attr("id", $(this).attr("id").replace(oldindex.toString(), index.toString()));
                $(this).attr("name", $(this).attr("name").replace(oldindex.toString(), index.toString()));
            });
        });
    });
    //Invite All
    $(document).on('click', ".inviteall", function (e) {
        $("#dvLoadingGif").show();
        if ($('#tblOrganisationAdmins').DataTable().data().count() === 0) {
            swal({
                title: "No data available.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        }
        else {
            $.ajax({
                type: "GET",
                url: "/Merchant/InviteMerchantAdmin/",
                data: { 'userEmail': "", 'merchantId': $("#hdnMerchantId").val() },
                dataType: "json",
                success: function (data) {
                    if (data.data !== null || data.data !== undefined) {
                        $("#dvLoadingGif").hide();
                        swal({
                            title: "Invitation sent successfully.",
                            icon: "error"

                        });
                        var oTable = $('#tblOrganisationAdmins').DataTable();
                        oTable.draw();
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
    });
    //Invite
    $(document).on('click', ".invite", function (e) {
        $("#dvLoadingGif").show();
        $.ajax({
            type: "GET",
            url: "/Merchant/InviteMerchantAdmin/",
            data: { 'userEmail': $(this).attr('value'), 'merchantId': $("#hdnMerchantId").val() },
            dataType: "json",
            success: function (data) {
                if (data.data !== null || data.data !== undefined) {
                    $("#dvLoadingGif").hide();
                    swal({
                        title: "Invitation sent successfully.",
                        icon: "error"

                    });
                    var oTable = $('#tblOrganisationAdmins').DataTable();
                    oTable.draw();
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
    });


    $(document).on('keyup', "#BannerDescription", function () {
        $(".dataBannerSet").html($(this).val());
        $('.banner-text').html('<h1 id="showBannerOff">' + $(this).val() + '</h1>');
    });
    $(document).on('change', "input[name='BannerType']", function () {
        var textBannerDesc = $("#BannerDescription").val();

        $(".dataBannerSet").html(textBannerDesc);
        $('.banner-text').html('<h1 id="showBannerOff">' + textBannerDesc + '</h1>');

    });


    $(document).on('click', '#daily', function () {
        $("#dvRepeatDay").show();
        $("#spnModalTitle").html('Schedule Daily Promotion');
        $("#PromotionTypeId").val("1");
        $("#IsDaily").val(true);
        $("#signle-schedule").modal('show');
        $.validator.unobtrusive.parse($("#modalValidDv"));

        ClearModalPopupControls();
        $('.datetimepicker').datepicker({ startDate: new Date() });
        $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
        $('.timepicker').timepicker('update');
        return false;
    });
    $(document).on('click', '#multi', function () {
        $("#dvRepeatDay").hide();
        $("#spnModalTitle").html('Schedule Multi day Promotion');
        $("#PromotionTypeId").val("2");
        $("#IsDaily").val(false);
        $.validator.unobtrusive.parse($("#modalValidDv"));

        $("#signle-schedule").modal('show');

        ClearModalPopupControls();
        $('.datetimepicker').datepicker({ startDate: new Date() });
        $('.timepicker').timepicker({
            defaultTime: '12:00 AM', minuteStep: 1, template: 'modal',
            modalBackdrop: true,
        });
        $('.timepicker').timepicker('update');
        return false;
    });

    //Delete Rewards
    $(document).on('click', '.clsDeleteRewards', function () {
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
                DeleteReward(reId);

            }
        }
        );
    });


    $(document).on('click', '.clsDeletePromotion', function () {
        var reId = $(this).attr('id');
        var reName = $(this).attr('data-org');
        reName = reName === '' ? 'this promotion' : reName;
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
                DeletePromotion(reId);

            }
        }
        );
    });
    //hide date picker on date selection
    $(document).on('change', '.datetimepicker', function (ev) {
        $(this).datepicker('hide');
        CheckForStartEndDate();
    });
    $(document).on('keyup', ".clsTextBoxHolidayName", function () {

        $('.spnShowHolidayNameError').each(function () {
            if ($(this).val() !== '') {
                $('#spn_' + $(this).attr('id')).hide();
            }
            else { $('#spn_' + $(this).attr('id')).show(); }
        });
    });

    $(document).on('keypress', '.RestrictNumbers', function (e) {
        var keyCode = e.which ? e.which : e.keyCode;
        if (keyCode !== 48 && keyCode !== 49 && keyCode !== 49 && keyCode !== 50 && keyCode !== 51 && keyCode !== 52 && keyCode !== 53 && keyCode !== 54 && keyCode !== 55 && keyCode !== 56 && keyCode !== 57) {
            return true;
        }
    });

    $(document).on('change', '#RepeatDay', function () {
        checkForDateRangeWithDay();
    });

    $(document).on('keyup', '.maxcapacityVal', function (evt) {
        if ($(this).val() > 100)
            return false;
        return true;
    });
});


function GetTabViewComponent(idName, pid) {
    $("#dvLoadingGif").show();
    if (idName === "1") {
        $.ajax({
            method: 'GET',
            url: '/Merchant/GetMerchantDetailViewComponent',
            data: { id: $("#hdnMerchantId").val(), ppId: $("#hdnPrimaryProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), poN: $("#hdnPrimaryOrgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnMerchantTab").html(data);

            $.validator.unobtrusive.parse($("#spnMerchantTab"));
            $('.nav-tabs a[href="#home"]').tab('show');
            $("#dvLoadingGif").hide();
            $('#home').addClass('show');
            $('#hdnMerchantName').val($('#hmerchantname').text());
            /* Dropzone */
            var create = {
                deleteDropZoneImage: ""
            };
            $("#my-awesome-dropzone").dropzone({
                //  var myDropzone = new Dropzone("#my-awesome-dropzone", {
                url: "/Account/UploadImage", dictInvalidFileType: "Please upload only jpg .jpeg .png files",
                acceptedFiles: ".png,.jpeg,.jpg",
                paramName: "file",
                maxFiles: 1,
                maxFilesize: 10,
                uploadMultiple: false,
                init: function (element) {
                    // myDropzone = this;
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
                        $("#btnmerchantdetailinfo").attr('disabled', 'disabled');
                    });
                    this.on("success", function (file, response, responseText) {
                        responseText = file.status;// or however you would point to your assigned file status here;
                        if (responseText === "success") {

                            $("#hdnIsNewUpload").val(1);
                            $("#ImageFileName").val(response.data);
                        }
                        $("#btnmerchantdetailinfo").removeAttr('disabled');
                    });
                },
                addRemoveLinks: true,
                removedfile: function (file) {

                    file.name;
                    create.deleteDropZoneImage;
                    if ($("#hdnIsNewUpload").val() === '1' || $("#hdnMerchantId").val() === '0') {
                        $.ajax({
                            type: 'POST',
                            url: '/Account/RemoveImage',
                            data: { userId: $("#hdnMerchantId").val(), imgPath: $("#ImagePath").val(), userPhotoEntityType: $("#hdnUserImageType").val() },
                            success: function () {
                                $("#ImageFileName").val('');
                            }
                        });
                    }
                    else { $("#ImageFileName").val(''); }
                    var ref;

                    return (ref = file.previewElement) !== null ? ref.parentNode.removeChild(file.previewElement) : void 0;

                }
            });

            /* Dropzone ends here.*/
            if ($("#ImageFileName").val() !== '' && $("#ImageFileName").val() !== null) {

                var imgDropzone = Dropzone.forElement("#my-awesome-dropzone");
                imgDropzone.removeAllFiles();
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
                //if ($("#hdnMerchantId").val() != '3000' || $("#hdnMerchantId").val() != '0') {
                //    $("#ContactNumber").attr('readonly', 'readonly');
                //    $("#ContactNumber").attr('disabled', 'disabled');
                //}
            }
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }
    else if (idName === "2") {
        $.ajax({
            method: 'GET',
            url: '/Merchant/CreateMerchantBusinessInfoViewComponent',
            data: { id: $("#hdnMerchantId").val(), pid: pid, poId: $("#hdnPrimaryOrgId").val(), ppId: $("#hdnPrimaryProgramId").val(), poN: $("#hdnPrimaryOrgName").val(), ppN: $("#hdnPrimaryProgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnMerchantTab").html(data);
            $.validator.unobtrusive.parse($("#spnMerchantTab"));
            $('.nav-tabs a[href="#business-information"]').tab('show');
            $("#dvLoadingGif").hide();
            $('.datetimepicker').datepicker({ startDate: new Date() });
            $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
            $("#dvLoadingGif").hide();
            $('#hmerchantname').text($('#hdnMerchantName').val());
            $('#business-information').addClass('show');
        }).fail(function (xhdr, statusText, errorText) {

            swal({
                title: errorText,
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }
    else if (idName === "3") {
        $.ajax({
            method: 'GET',
            url: '/Merchant/LoadAllPromotions',
            data: { id: $("#hdnMerchantId").val(), pid: pid, poId: $("#hdnPrimaryOrgId").val(), ppId: $("#hdnPrimaryProgramId").val(), poN: $("#hdnPrimaryOrgName").val(), ppN: $("#hdnPrimaryProgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnMerchantTab").html(data);
            $.validator.unobtrusive.parse($("#spnMerchantTab"));

            $('.nav-tabs a[href="#menu2"]').tab('show');
            $('#promotions').addClass('show');
            $('#hmerchantname').text($('#hdnMerchantName').val());
            $("#dvLoadingGif").hide();
            var create = {
                deleteDropZoneImage: ""
            };
            $("#my-awesome-dropzone").dropzone({
                // var myDropzone = new Dropzone("#my-awesome-dropzone", {
                url: "/Account/UploadImage", dictInvalidFileType: "Please upload only jpg .jpeg .png files",
                acceptedFiles: ".png,.jpeg,.jpg",
                paramName: "file",
                maxFiles: 1,
                maxFilesize: 10,
                uploadMultiple: false,
                init: function (element) {
                    // myDropzone = this;
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
                        $("#btnSchedulePromotion").attr('disabled', 'disabled');
                        $(".clsDeletePromotion").attr('disabled', 'disabled');
                    });
                    this.on("success", function (file, response, responseText) {
                        responseText = file.status;// or however you would point to your assigned file status here;
                        if (responseText === "success") {
                            $("#hdnIsNewUpload").val(1);
                            $("#ImageFileName").val(response.data);
                        }
                        $("#btnSchedulePromotion").removeAttr('disabled');
                        $(".clsDeletePromotion").removeAttr('disabled');

                    });
                },
                addRemoveLinks: true,
                removedfile: function (file) {

                    file.name;
                    create.deleteDropZoneImage;

                    if ($("#hdnIsNewUpload").val() === '1' || $("#PromotionId").val() === '0') {
                        $.ajax({
                            type: 'POST',
                            url: '/Account/RemoveImage',
                            data: { userId: data.data, imgPath: $("#ImageFileName").val(), userPhotoEntityType: $("#hdnUserImageType").val() },
                            success: function () {
                                $("#ImageFileName").val('');
                            }
                        });


                    }
                    else { $("#ImageFileName").val(''); }
                    var ref;

                    return (ref = file.previewElement) !== null ? ref.parentNode.removeChild(file.previewElement) : void 0;

                }
            });

        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        });
    }
    else if (idName === "4") {

        $.ajax({
            method: 'GET',
            url: '/Merchant/CreateMerchantRewardViewComponent',
            data: { id: $("#hdnMerchantId").val(), pid: pid, poId: $("#hdnPrimaryOrgId").val(), ppId: $("#hdnPrimaryProgramId").val(), poN: $("#hdnPrimaryOrgName").val(), ppN: $("#hdnPrimaryProgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnMerchantTab").html(data);
            $.validator.unobtrusive.parse($("#spnMerchantTab"));
            $.validator.addMethod("isdateafter", function (value, element, params) {
                var parts = element.name.split(".");
                var prefix = "";
                if (parts.length > 1)
                    prefix = parts[0] + ".";
                var startdatevalue = $('input[name="' + prefix + params.propertytested + '"]').val();
                if (!value || !startdatevalue)
                    return true;
                return (params.allowequaldates) ? Date.parse(startdatevalue) <= Date.parse(value) :
                    Date.parse(startdatevalue) < Date.parse(value);
            });
            $('.nav-tabs a[href="#rewards"]').tab('show');

            $("#add-rewards-list").show();
            $("#dvLoadingGif").hide();
            $('.datetimepicker').datepicker({ startDate: new Date() });
            $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
            var bgcolor = $("#BackGroundColor").val();
            if (!bgcolor) {
                bgcolor = "#3F43CE";
            }
            $("#BackGroundColor").spectrum({
                color: bgcolor,
                preferredFormat: "hex"
            });
            $('.amt-spent').hide();
            $(".bg-colors").css("background-color", bgcolor);
            $("#dvLoadingGif").hide();
            $('#rewards').addClass('show');
            var img = $("#imgIconPath_" + $("input[name='BusinessTypeId']:checked").attr("value")).attr("src").split(".");
            var imgName = img[0];
            var imgext = img[1];
            $("#imgBusinessIcon").attr("src", imgName + "-white." + imgext);

            if (parseInt($('#OfferSubTypeId').val()) === 3) {
                $('.nov').show();
                $('.amt-spent').hide();
                $('#Amount').val('');
            }
            else if (parseInt($('#OfferSubTypeId').val()) === 4) {
                $('.amt-spent').show();
                $('.nov').hide();
                $('#Visits').val('');
            }
            else {
                $('.nov').hide();
                $('.amt-spent').hide();
                $('#Amount').val('');
                $('#Visits').val('');
            }
            if (parseInt($('#Visits').val()) === 0) {
                $('#Visits').val('');
            }
            if (parseInt($('#Amount').val()) === 0) {
                $('#Amount').val('');
            }
            if ($("#StartDate").val() !== '') {
                $("#StartDate").val(moment($("#StartDate").val()).format('MM/DD/YYYY'));
            }
            if ($('#EndDate').val() !== '') {
                $("#EndDate").val(moment($("#EndDate").val()).format('MM/DD/YYYY'));
            }
            $('#hmerchantname').text($('#hdnMerchantName').val());
            if ($("#hdnPromotionId").val() === null || $("#hdnPromotionId").val() === '') {
                $("#btnmerchantrewardinfo").text('ADD');
            }
            else {
                $("#btnmerchantrewardinfo").text('UPDATE');
            }
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        });
    }
    else if (idName === "5") {
        $.ajax({
            method: 'GET',
            url: '/Merchant/GetMerchantAdminLevelViewComponent',
            data: { id: $("#hdnMerchantId").val(), poId: $("#hdnPrimaryOrgId").val(), ppId: $("#hdnPrimaryProgramId").val(), poN: $("#hdnPrimaryOrgName").val(), ppN: $("#hdnPrimaryProgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnMerchantTab").html(data);
            $('#hmerchantname').text($('#hdnMerchantName').val());
            $.validator.unobtrusive.parse($("#spnMerchantTab"));
            $('.nav-tabs a[href="#merchant-level-admin"]').tab('show');
            $("#merchant-level-admin").addClass('show');
            /* Dropzone */
            var create = {
                deleteDropZoneImage: ""
            };
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
                        // myDropzone = this;
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

                        file.name;
                        create.deleteDropZoneImage;

                        if ($("#hdnIsFormSubmitted").val() === "false" && $("#hdnIsNewUpload").val() === '1') {
                            $.ajax({
                                type: 'POST',
                                url: '/Account/RemoveImage',
                                data: { userId: $("#UserId").val(), imgPath: $("#adminLevelModel_UserImagePath").val(), userPhotoEntityType: $("#hdnUserImageType").val() },
                                success: function () {
                                    $("#adminLevelModel_UserImagePath").val('');
                                }
                            });
                        } else { $("#adminLevelModel_UserImagePath").val(''); }
                        var ref;
                        return (ref = file.previewElement) !== null ? ref.parentNode.removeChild(file.previewElement) : void 0;

                    }
                });
            }
            /* Dropzone ends here.*/
            $("#tblOrganisationAdmins").dataTable({
                "processing": true, // for show progress bar
                "serverSide": true, // for process server side
                "filter": true, // this is for disable filter (search box)
                "orderMulti": false, // for disable multiple column at once
                "pageLength": 10,
                "order": [[5, "desc"]],
                "oLanguage": {
                    "sEmptyTable": "No data available."
                },
                "bLengthChange": false,
                "ajax": {
                    "url": "/Merchant/LoadAllMerchantAdmins",
                    "data": { id: $("#hdnMerchantId").val() },
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
                    }, {
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

                            return "<div class='profile-accounts'><image class='avatar' src=" + full.UserImagePath + "><a style='width:200px;'  href='#' onclick=EditUserAdminData('" + full.UserId + "');> <em style='color:#007bff;'>" + full.Name + "</em></a></div>";
                        }
                    },
                    {
                        "data": "EmailAddress", "name": "EmailAddress", "autoWidth": true, "className": "text-center"
                    },
                    {
                        "data": "Title", "name": "Title", "autoWidth": true, "className": "text-center"
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
                            if (userRlN.toLowerCase() == "super admin" || userRlN.toLowerCase() == "organization full" || userRlN.toLowerCase() == "program full") {
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
            if (userRlN.toLowerCase() !== "super admin" && userRlN.toLowerCase() !== "organization full" && userRlN.toLowerCase() !== "program full") {
                $("#tblOrganisationAdmins").DataTable().column(6).visible(false);
                $("#tblOrganisationAdmins").DataTable().column(7).visible(false);

            }
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        });
    }
    else if (idName === "6") {
        $("#hdnDateMonth").val($(".aCurrentPeriod").attr('id'));
        $.ajax({
            method: 'GET',
            url: '/Merchant/GetMerchantTransactionViewComponent',
            data: { id: $("#hdnMerchantId").val(), poId: $("#hdnPrimaryOrgId").val(), ppId: $("#hdnPrimaryProgramId").val(), poN: $("#hdnPrimaryOrgName").val(), ppN: $("#hdnPrimaryProgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnMerchantTab").html(data);
            $.validator.unobtrusive.parse($("#spnMerchantTab"));
            $('.nav-tabs a[href="#transactions"]').tab('show');
            $('#transactions').addClass('show');
            $('#hmerchantname').text($('#hdnMerchantName').val());
            $("#tblMerchantTrasaction").dataTable({
                "processing": true, // for show progress bar
                "serverSide": true, // for process server side
                "filter": true, // this is for disable filter (search box)
                "orderMulti": false, // for disable multiple column at once
                "pageLength": 10,
                "order": [[1, "desc"]],
                "oLanguage": {
                    "sEmptyTable": "No data available."
                },
                "bLengthChange": false,
                "ajax": {
                    "url": "/Merchant/LoadAllMerchantTransaction",
                    "data": { id: $("#hdnMerchantId").val(), 'dateMonth': $("#hdnDateMonth").val() },
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
                    },
                    {
                        "targets": [6],
                        "searchable": true,
                        "orderable": false
                    }
                    ],

                "columns": [
                    { "data": "Id", "id": "Id", "name": "Id", "autoWidth": true },
                    { "data": "MerchantId", "id": "MerchantId", "name": "MerchantId", "autoWidth": true },
                    {
                        "name": "TransactionDate",
                        "autoWidth": true,
                        "render": function (data, type, full, mets) { return moment(full.TransactionDate).format('MMMM Do YYYY'); }
                    },
                    {
                        "name": "Time",
                        "className": "text-center",
                        "autoWidth": true,
                        "render": function (data, type, full, mets) { return moment(full.TransactionDate).format('hh:mm A'); }
                    },
                    { "data": "Account", "id": "Account", "name": "Account", "autoWidth": true },
                    { "data": "Name", "id": "Name", "name": "Name", "autoWidth": true, "className": "text-center" },
                    {
                        "name": "Amount",
                        "autoWidth": true,
                        "render": function (data, type, full, mets) {
                            return full.Amount;
                        }
                    }
                ],
                initComplete: function () {
                    $('.dataTables_filter').show();
                    $('.dt-buttons').hide();

                    $('#aExportTransaction').on('click', function () {
                        $("#transactionFilter").text("");
                        var searchValue = $('.dataTables_filter input').val();
                        var id = $("#hdnMerchantId").val();
                        var dateMonth = $("#hdnDateMonth").val();
                        $.ajax({
                            type: "POST",
                            url: "/Merchant/MerchantTransactionExportExcel/?searchValue=" + searchValue + "&id=" + id + "&dateMonth=" + dateMonth,
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
                ]
            });
            $("#dvLoadingGif").hide();
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }
    else if (idName === "7") {
        $.ajax({
            method: 'GET',
            url: '/Merchant/GetMerchantRewardListViewComponent',
            data: { id: $("#hdnMerchantId").val(), pid: $("#Id").val(), ppId: $("#hdnPrimaryProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), poN: $("#hdnPrimaryOrgName").val(), ppN: $("#hdnPrimaryProgName").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnMerchantTab").html(data);
            $('.nav-tabs a[href="#rewards"]').tab('show');
            $('#rewards').addClass('show');
            $("#view-rewards-list").show();
            $("#dvLoadingGif").hide();
            $('#hmerchantname').text($('#hdnMerchantName').val());
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
            $("#dvLoadingGif").hide();
        });
    }
}

function changeAdminStatus(userId, e) {
    var checked = e.checked;
    $("#dvLoadingGif").hide();
    $.ajax({
        type: "POST",
        url: "/Organisation/ChangeOrganisationAdminStatus/",
        data: { 'userId': userId, isActive: checked },
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

function EditUserAdminData(userId) {
    $("#dvLoadingGif").hide();
    $.ajax({
        type: "GET",
        url: "/Organisation/UserAdminInfo/",
        data: { 'userId': userId },
        dataType: "json",
        success: function (data) {
            if (data.data !== null || data.data !== undefined) {
                $("#adminLevelModel_Name").val(data.data.FirstName);
                $("#adminLevelModel_LastName").val(data.data.LastName);
                $("#adminLevelModel_EmailAddress").val(data.data.Email);
                $('#adminLevelModel_EmailAddress').attr('readonly', 'readonly');
                $("#adminLevelModel_PhoneNumber").val(data.data.PhoneNumber);
                $('#adminLevelModel_PhoneNumber').attr('readonly', 'readonly');
                $('#adminLevelModel_PhoneNumber').attr('disabled', 'disabled');
                $("#adminLevelModel_RoleId").val(data.data.RoleId);
                var imgDropzone = Dropzone.forElement("#my-awesome-dropzone");
                imgDropzone.removeAllFiles();
                if (data.data.UserImagePath !== null) {
                    $('#adminLevelModel_UserImagePath').val(data.data.ImageFileName);

                    var mockFile = {
                        name: data.data.ImageFileName,
                        size: 12,
                        accepted: true,
                        kind: 'image'
                    };
                    imgDropzone.options.maxFiles = 1;
                    imgDropzone.emit("addedfile", mockFile);
                    imgDropzone.files.push(mockFile);
                    // And optionally show the thumbnail of the file:
                    imgDropzone.emit("thumbnail", mockFile, data.data.UserImagePath);

                    imgDropzone.emit("complete", mockFile);
                    imgDropzone._updateMaxFilesReachedClass();
                    $("#hdnIsNewUpload").val(2);
                } else { $('#adminLevelModel_UserImagePath').val(''); }
                $("#Custom1").val(data.data.Title);

                $("#adminLevelModel_UserId").val(data.data.UserId);
                $("#btnOrganisationAdminLevelDetail").html('UPDATE');
                $(".clsHeaderForm").html('Edit Admin');
                $('.linked-delete-custom-action').children(".linked-down-data-s").hide();
                $('#MerchantAccessibility').multiselect("deselectAll", false);

                $('#MerchantAccessibility').multiselect('refresh');
                var queryArrStr = '';
                // validatorForm.resetForm();
                $('.field-validation-error').html('');

                $(data.data.MerchantIds).each(function (index) {
                    var _locName = data.data.MerchantIds[index].merchantId;
                    queryArrStr += _locName + ',';
                });

                var dataarray = queryArrStr.trimRight(',').split(",");


                $("#MerchantAccessibility").val(dataarray);
                $("#MerchantAccessibility").multiselect("refresh");

                $('html,body').animate({
                    scrollTop: $("#adminLevelModel_Name").offset().top
                }, 'slow');
                $("#dvLoadingGif").hide();
            } else {
                swal({
                    title: "Currently unable to process the request! Please try again later.",
                    icon: "error"

                });
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


function DeleteData(userId) {
    swal({
        title: "Are you sure you want to delete the merchant admin?",
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
            DeleteUser(userId);
        }
    }
    );
}

function DeleteUser(userId) {
    $("#dvLoadingGif").hide();
    $.ajax({
        type: "POST",
        url: "/Organisation/DeleteOrganisationAdminUser/",
        data: { 'userId': userId },
        dataType: "json",
        success: function (data) {
            $("#dvLoadingGif").hide();
            if (data.data > 0 && data.success) {
                $('#EmailAddress').removeAttr('readonly');
                $(".clsHeaderForm").html('New Admin');
                $("#UserImagePath").val('');
                $('#Title').val('');
                $("#frmOrgAdminLevelDetail")[0].reset();
                $('#PostedFileUploadError').hide();
                $('#wizardPicturePreview').attr('src', '/images/icon-profile.png');
                $("#btnOrganisationAdminLevelDetail").html('ADD');
                swal({
                    title: "Admin user has been deleted successfully!",
                    icon: "success"

                });
                var oTable = $('#tblOrganisationAdmins').DataTable();
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
            $("#dvLoadingGif").hide();
        }
    });
}
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

var CheckForDate = function (id) {
    $("#hdnDateMonth").val(id);
    GetDataTableContent();
    $("#transactions-panel-dropdown.collapse").collapse('hide');
};
var GetDataTableContent = function () {
    var oTable = $('#tblMerchantTrasaction').DataTable();
    oTable.on('preXhr.dt', function (e, settings, data) {
        data.dateMonth = $("#hdnDateMonth").val();
    });
    oTable.draw();
};
function GetHours(d) {
    var h = parseInt(d.split(':')[0]);
    if (d.split(':')[1].split(' ')[1] == "PM") {
        h = h + 12;
    }
    return h;
}
function GetMinutes(d) {
    return parseInt(d.split(':')[1].split(' ')[0]);
}
function DeleteReward(reId) {
    $("#dvLoadingGif").show();
    $.ajax({
        type: "POST",
        url: "/Merchant/DeleteReward/",
        data: { 'promotionId': reId },
        dataType: "json",
        success: function (data) {
            $("#dvLoadingGif").hide();
            if (data.data > 0 && data.success) {
                swal({
                    title: "Reward has been deleted successfully!",
                    icon: "success"

                });

                GetTabViewComponent("7", "");

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

var DeletePromotion = function (reId) {
    $("#dvLoadingGif").show();
    $.ajax({
        type: "POST",
        url: "/Merchant/DeleteReward/",
        data: { 'promotionId': reId },
        dataType: "json",
        success: function (data) {
            $("#dvLoadingGif").hide();
            if (data.data > 0 && data.success) {
                swal({
                    title: "Promotion has been deleted successfully!",
                    icon: "success"

                });
                $("#signle-schedule").modal('hide');
                ClearModalPopupControls();
                GetTabViewComponent("3", "");

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
};
var ClearModalPopupControls = function () {
    $("#frmSchedulePromotion")[0].reset();
    var $form = $("#frmSchedulePromotion");

    //reset jQuery Validate's internals
    $form.validate().resetForm();

    //reset unobtrusive field level, if it exists
    $form.find("[data-valmsg-replace]")
        .removeClass("field-validation-error")
        .addClass("field-validation-valid")
        .empty();

    $('#PostedFileUploadError').hide();
    $('#wizardPicturePreview').attr('src', '/images/icon-profile-lgx.png');
    $("#PromotionImagePath").val('');
    $("#encPromId").val('');
    $("#PromotionId").val('');

    $('.banner-text').html('');
    $(".clsDeletePromotion").hide();
    $("#btnSchedulePromotion").html('ADD');
    $(".dataBannerSet").html('');
    $("#spnEndTimeGreater").hide();
    $("#spnEndDateGreater").hide();
    $("#spnRepeatDayError").hide();


};

var GetPromotionDetailById = function (id) {

    $.ajax({
        type: "GET",
        url: "/Merchant/GetMerchantPromotionsById/",
        data: { 'promotionId': id },
        dataType: "json",
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            if (data.data !== null) {
                var result = data.data;

                $("#PromotionDescription").val(result.Name);
                $("#PromoDetail").val(result.Description);
                if (result.IsActive) {
                    $("#IsActive").prop("checked", "checked");
                }
                else {
                    $("#IsActive").removeProp("checked");
                }
                $('.datetimepicker').datepicker({ startDate: new Date() });
                $('.timepicker').timepicker({ defaultTime: '12:00 AM', minuteStep: 1 });
                $('.timepicker').timepicker('update');

                $("#StartDate").val(moment(result.StartDate).format('MM/DD/YYYY'));
                $("#StartTime").val(moment(result.StartTime, "HH:mm:ss").format("hh:mm A"));
                $("#EndDate").val(moment(result.EndDate).format('MM/DD/YYYY'));
                $("#EndTime").val(moment(result.EndTime, "HH:mm:ss").format("hh:mm A"));

                //radio button
                $("input[name='BannerType'][value='" + result.BannerTypeId + "']").prop('checked', true);

                $("#BannerDescription").val(result.BannerDescription);
                $(".dataBannerSet").html(result.BannerDescription);
                $('.banner-text').html('<h1 id="showBannerOff">' + result.BannerDescription + '</h1>');
                $("#encPromId").val(result.encPromId);
                $("#PromotionId").val(result.Id);
                var imgDropzone = Dropzone.forElement("#my-awesome-dropzone");
                imgDropzone.removeAllFiles();
                if (result.ImageFileName !== null) {
                    $("#hdnIsNewUpload").val(2);
                    $("#ImageFileName").val(result.ImageFileName);

                    var mockFile = {
                        name: result.ImageFileName,
                        size: 12,
                        accepted: true,
                        kind: 'image'
                    };

                    imgDropzone.options.maxFiles = 1;
                    imgDropzone.emit("addedfile", mockFile);
                    imgDropzone.files.push(mockFile);

                    // And optionally show the thumbnail of the file:
                    imgDropzone.emit("thumbnail", mockFile, result.ImagePath);


                    imgDropzone.emit("complete", mockFile);
                    imgDropzone._updateMaxFilesReachedClass();
                } else {
                    $("#ImageFileName").val('');
                    $("#PromotionImagePath").val('');
                }
                $("#IsDaily").val(result.IsDailyPromotion);
                var $form = $("#frmSchedulePromotion");

                //reset jQuery Validate's internals
                $form.validate().resetForm();

                //reset unobtrusive field level, if it exists
                $form.find("[data-valmsg-replace]")
                    .removeClass("field-validation-error")
                    .addClass("field-validation-valid")
                    .empty();

                $(".clsDeletePromotion").show();
                $(".clsDeletePromotion").attr('id', result.encPromId);
                $(".clsDeletePromotion").attr('data-org', '');
                $("#btnSchedulePromotion").html('UPDATE');
                if (result.IsDailyPromotion === true) {
                    $("#dvRepeatDay").show();
                    $("#RepeatDay").val(result.PromotionDay);
                    $("#PromotionTypeId").val("1");
                }
                else { $("#dvRepeatDay").hide(); $("#PromotionTypeId").val("2"); }

                $("#signle-schedule").modal('show');
            }

        },
        error: function () {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });
        }
    });
};

var checkForDateRangeWithDay = function () {

    var chkStartDate = new Date(moment($("#StartDate").val(), 'MM/DD/YYYY'));
    var chkEndDate = new Date(moment($("#EndDate").val(), 'MM/DD/YYYY'));

    chkStartDate.setHours(0, 0, 0, 1);  // Start just after midnight
    chkEndDate.setHours(23, 59, 59, 999);
    // To calculate the time difference of two dates 
    var Difference_In_Time = chkEndDate.getTime() - chkStartDate.getTime();

    // To calculate the no. of days between two dates 
    var Difference_In_Days = Difference_In_Time / (1000 * 3600 * 24);
    if (Difference_In_Days < 7) {
        var daysOfWeek = [];
        var daySelectDrp = $("#RepeatDay").val();
        for (var d = chkStartDate; d <= chkEndDate; d.setDate(d.getDate() + 1)) {
            var jDaysWeek = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");

            var getDayDate = d.getDay();
            daysOfWeek.push(jDaysWeek[getDayDate]);
        }

        var arraycontainsturtles = (daysOfWeek.indexOf(daySelectDrp) > -1);
        if (!arraycontainsturtles) {
            $("#spnRepeatDayError").show();
            $("#spnRepeatDayError").html('<span>Please select the day comes under the date range.</span>'); return false;
        }
        else { $("#spnRepeatDayError").hide(); return true; }
    }
    return true;
};

var CheckForStartEndDate = function () {
    var chkStartDate = new Date(moment($("#StartDate").val(), 'MM/DD/YYYY'));
    var chkEndDate = new Date(moment($("#EndDate").val(), 'MM/DD/YYYY'));
    if (chkStartDate != '' && chkEndDate != '') {
        if ($("#IsDaily").val() === false) {
            if (chkStartDate >= chkEndDate) {
                $("#spnEndDateGreater").show();
                $("#spnEndDateGreater").html('<span>End date must be greater than start date.</span>');
                return false;
            }
            else { $("#spnEndDateGreater").hide(); return true; }
        }
        else {
            if (chkStartDate.getTime() === chkEndDate.getTime()) {
                var jDays = new Array("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday");
                var dayOfDate = jDays[chkStartDate.getDay()];
                var daySelect = $("#RepeatDay").val();
                if (dayOfDate.toLowerCase() !== daySelect.toLowerCase()) {
                    $("#spnRepeatDayError").show();
                    $("#spnRepeatDayError").html('<span>Please select the day accordingly.</span>'); return false;
                }
                else { $("#spnRepeatDayError").hide(); return true; }

            }
            if (chkStartDate > chkEndDate) {
                $("#spnEndDateGreater").show();
                $("#spnEndDateGreater").html('<span>End date must be greater than start date.</span>');
                return false;
            }

            $("#spnEndDateGreater").hide(); return true;
        }

    }

};

$.urlParam = function (name) {
    var results = new RegExp('[\?&]' + name + '=([^&#]*)').exec(window.location.href);
    if (results === null) {
        return null;
    }
    else {
        return decodeURI(results[1]) || 0;
    }
};

