var refreshComponent = '';
var programTypes = [];
var programsChecked = [];
var selected = [];
var tab = '1';
$(document).ready(function () {
    $(document).ready(function () { $('#ProgramsAccessibility').multiselect({ includeSelectAllOption: true }); });
   
    GetTabViewComponent("1");
    if ($("#hdnOrgProgramType").val() !== undefined && $("#hdnOrgProgramType").val() !== '') {
        var dataarray = $("#hdnOrgProgramType").val().split(",");
        // Set the value
        $("#ProgramType").val(dataarray);
    }

    $(".orgTab").click(function (e) {

        if ($("#hdnOrganisationId").val() === "" && $(this).attr('id') !== 'tab_1') {
            swal({
                title: "Please add organization detail before moving further.",
                icon: "error"

            });
        }
        else {

            $(".orgTab").removeClass('active');
            var idName = $(this).attr('id').replace('tab_', '');
            GetTabViewComponent(idName);

            var stringElement = "tab_" + idName;
            $("#" + stringElement).addClass('active');
        }
        return false;

    });
    $(document).on("click", ".edit-accountholder", function () {
        var userId = $(this).attr('data-id');
        var programid = $(this).attr('data-programid');
        GetTabViewComponent("7", userId, programid);
    });
    $(document).on('change', '.checkbox input[type="checkbox"]', function () {

        var checkboxes = $(".checkbox input[type='checkbox']");
        if (!checkboxes.is(":checked")) {
            $(".multiselect-native-select").next().show();
        }
        else { $(".multiselect-native-select").next().hide(); }

    });

    $(document).on('click', '#btnOrganisationDetail', function () {
        if ($("#frmOrgDetail").validate() && $("#frmOrgDetail").valid()) {
            $("#dvLoadingGif").show();

            var brands = $('#SelectedOrganisationProgramType option:selected');
            var organisationId = $("#hdnOrganisationId").val();
            programTypes = [];
            $(brands).each(function (index, brand) {
                var data = $(this).val();
                programTypes.push(data);
            });
            var orgName = $("#OrganisationSubTitle").val();
            var OrganisationDetailModel = {
                OrganisationId: organisationId,
                OrganisationName: $("#OrganisationName").val(),
                OrganisationSubTitle: $("#OrganisationSubTitle").val(),
                Address: $("#Address").val(),
                Website: $("#Website").val(),
                ContactName: $("#ContactName").val(),
                ContactTitle: $("#ContactTitle").val(),
                ContactNumber: $("#ContactNumber").val(),
                EmailAddress: $("#EmailAddress").val(),
                Description: $("#Description").val(),
                SelectedOrganisationProgramType: programTypes,
                FacebookURL: $("#FacebookURL").val(),
                TwitterURL: $("#TwitterURL").val(),
                SkypeHandle: $("#SkypeHandle").val(),
                JPOS_MerchantId: $("#JPOS_MerchantId").val()
            };

            $.post("/Organisation/PostOrganisationDetailInformation",
                OrganisationDetailModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" && data.data !== "0") {
                        tab = '2';
                        var stringElement = "tab_" + tab;
                        $("#" + stringElement).addClass('active');
                        GetTabViewComponent(tab);
                        $("#hdnOrganisationName").val(orgName);
                        $("#hdnOrganisationId").val(data.data);
                        var newurl = window.location.protocol + "//" + window.location.host + window.location.pathname + '/' + data.data + '?org=' + data.dataOrgAppend;
                        window.history.pushState({ path: newurl }, '', newurl);
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
        else return false;
    });   
   
    $(document).on("click", "#btnSaveGlobalSettings", function () {    
        if ($("#formorgglobal").validate() && $("#formorgglobal").valid()) {
            $("#dvLoadingGif").show();
          
            debugger;
            var Id = $("#id").val();
            var LoyalityGlobalSettingDetailModel = {
                id: $("#hdnsettingid").val(),
                organisationId: $("#hdnSodexhonOrgId").val(),
                loyalityThreshhold: $("#loyalityThreshhold").val(),
                globalReward: $("#globalReward").val(),
                globalRatePoints: $("#globalRatePoints").val(),
                bitePayRatio: $("#bitePayRatio").val(),
                dcbFlexRatio: $("#dcbFlexRatio").val(),
                userStatusVipRatio: $("#userStatusVipRatio").val(),
                userStatusRegularRatio: $("#userStatusRegularRatio").val(),
                FirstTransactionBonus: $("#FirstTransactionBonus").val()

            };
            $.post("/Organisation/PostOrgLoyalityGlobalSettings",
                LoyalityGlobalSettingDetailModel,
                function (data) {
                    $("#dvLoadingGif").hide();
                    //if (data.data !== "" && data.data !== "0") {
                        var title = ''; if ($("#hdnsettingid").val() !== '' && $("#hdnsettingid").val() !== '0' && $("#hdnsettingid").val() !== '3000') { title = "Global Settings has been updated successfully."; } else { title = "Global Settings has been submitted successfully."; }
                        swal({ title: title, icon: "success" }, function () { tab = '3'; var stringElement = "tab_" + tab; $("#" + stringElement).addClass('active'); GetTabViewComponent(tab, "", "", "", ""); });
                  //  }
                  //  else { swal({ title: data.message, icon: "error" }); }
                });
            return false;
        }
        else {
            $("#formorgglobal").validate();
        }
    });
    $(document).on('click', '#btnOrganisationProgramDetail', function () {
        $("#dvLoadingGif").show();
        $("#hdnOrganisationId").val();

        tab = '4';
        var stringElement = "tab_" + tab;
        $("#" + stringElement).addClass('active');
        GetTabViewComponent(tab);
        return false;
    });

    $(document).on('click', '#btnOrganisationAdminLevelDetail', function () {

        if ($("#frmOrgAdminLevelDetail").validate() && $("#frmOrgAdminLevelDetail").valid()) {
            debugger;
            $("#dvLoadingGif").show();
            var brands = $('#ProgramsAccessibility option:selected');
            var organisationId = $("#hdnOrganisationId").val();
            programTypes = [];
            $(brands).each(function (index, brand) {
                var data = {
                    ProgramTypeId: $(this).val(),
                    OrganisationId: organisationId
                };
                programTypes.push(data);
            });
            var adminLevelModel = {
                UserId: $("#adminLevelModel_UserId").val(),
                Name: $("#adminLevelModel_Name").val(),
                LastName: $("#adminLevelModel_LastName").val(),
                UserImagePath: $("#adminLevelModel_UserImagePath").val(),
                EmailAddress: $("#adminLevelModel_EmailAddress").val(),
                PhoneNumber: $("#adminLevelModel_PhoneNumber").val(),
                RoleId: $("#adminLevelModel_RoleId").val(),
            };
            var OrganisationAdminDetailModel = {
                adminLevelModel:adminLevelModel,
                OrganisationId: organisationId,
                ProgramsAccessibility: programTypes
            };

            $.post("/Organisation/PostOrganisationAdminDetailInformation",
                OrganisationAdminDetailModel,
                function (data, message) {
                    $("#dvLoadingGif").hide();
                    if (data.data !== "" && data.data !== "0" && data.data !== null) {

                        $('#adminLevelModel_EmailAddress').removeAttr('readonly');
                        $('#adminLevelModel_PhoneNumber').removeAttr('readonly');
                        $('#adminLevelModel_PhoneNumber').removeAttr('disabled');
                        $(".clsHeaderForm").html('New Admin');
                        programTypes = [];
                        var imgDropzone = Dropzone.forElement("#my-awesome-dropzone");
                        imgDropzone.removeAllFiles();
                        $('#ProgramsAccessibility').multiselect("deselectAll", false);
                        $("#adminLevelModel_UserImagePath").val('');
                        $('#ProgramsAccessibility').multiselect('refresh');
                        $("#frmOrgAdminLevelDetail")[0].reset();
                        $('#PostedFileUploadError').hide();
                        $('#wizardPicturePreview').attr('src', '/images/icon-profile.png');
                        $("#hdnOrganisationId").val(organisationId);
                        $("#hdnIsFormSubmitted").val('true');
                        $("#btnOrganisationAdminLevelDetail").html('ADD');
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
        else return false;
    });

    $(document).on('change', '#PostedFileUpload', function () {

        if ($(this)[0].files[0] !== undefined && $(this)[0].files[0] !== null) {
            // Prepare the preview for profile picture

            var file1 = $(this)[0].files[0];

            var formData = new FormData();
            formData.append("files", file1);
            // Get uploaded file extension
            var extension = $(this).val().split('.').pop().toLowerCase();
            // Create array with the files extensions that we wish to upload
            var validFileExtensions = ['jpeg', 'jpg', 'png', 'gif', 'bmp'];
            //Check file extension in the array.if -1 that means the file extension is not in the list. 
            if ($.inArray(extension, validFileExtensions) === -1) {
                $('#PostedFileUploadError').show();
                // Clear fileuload control selected file
                $("#UserImagePath").val('');
                $('#wizardPicturePreview').attr('src', '/images/icon-profile.png');
                $(this).val('');
                //Disable Submit Button

            }
            else if (file1.size > 10506316 || file1.fileSize > 10506316) {
                $('#input_file_upload_error_img').show();
                $('#input_file_upload_error_img').addClass('field-validation-error');
                $('#input_file_upload_error_img').removeClass('field-validation-valid');
                $('#wizardPicturePreview').attr('src', '');
                $(this).val('');
            }
            else {
                readURL(this);
                $('#PostedFileUploadError').hide();
                $.ajax({
                    type: "POST",
                    url: "/Account/UploadImage",
                    data: formData,
                    dataType: "json",
                    processData: false,
                    contentType: false,//'application/json; charset=utf-8',
                    success: function (data) {

                        if (data.data !== "") {
                            $("#UserImagePath").val(data.data);
                            $('#ankremoveimg').show();
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
    $(document).on('click', '.clsAddorgPrg', function () {
        var prg = $(this).attr('data-prg');
        var orgName = '';
        $.ajax({
            method: 'GET',
            url: '/Organisation/GetCryptographicData',
            data: { value: $("#hdnOrganisationName").val() }
        }).done(function (data, statusText, xhdr) {
            orgName = data.data;
            window.location.href = "/Program/Index/" + prg + "?poId=" + $("#hdnOrganisationId").val() + "&poN=" + orgName;
        }).fail(function (xhdr, statusText, errorText) {

        });

    });
});


function AccountHolderList() {
    if ($("#hdnOrganisationId").val() === "" && $(this).attr('id') !== 'tab_1') {
        swal({
            title: "Please add organization detail before moving further.",
            icon: "error"
        });
    }
    else {
        GetTabViewComponent('6');
    }
}
function RefreshViewComponent() {

    var container = $("#spnOrganisationsTab");  //quoteProcedureComponentContainer
    refreshComponent = function () {
        if (tab === '1') {
            $.get("/Organisation/GetOrganisationDetailViewComponent", { id: $("#hdnOrganisationId").val() }, function (data) {
                container.html(data);
            });
        }
        else if (tab === '2') { $.get("/Organisation/GetOrganisationProgramDetailViewComponent", { id: $("#hdnOrganisationId").val(), name: '' }, function (data) { container.html(data); }); }
        else if (tab === '3') {
            $.get("/Organisation/GetOrganisationGlobalSettingViewComponent", { id: $("#hdnOrganisationId").val() }, function (data) {
                container.html(data);
            });
        }
    };
}

function GetTabViewComponent(idName, id,programid) {
    $("#dvLoadingGif").show();
    if (idName === "1") {
        $.ajax({
            method: 'GET',
            url: '/Organisation/GetOrganisationDetailViewComponent',
            data: { id: $("#hdnOrganisationId").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnOrganisationsTab").html(data);
            $.validator.unobtrusive.parse($("#spnOrganisationsTab"));
            $('.nav-tabs a[href="#home"]').tab('show');
            $("#dvLoadingGif").hide();
            $("#home").addClass("show");
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: statusText,//"Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        });

        $('#OrganisationProgramType').multiselect({ includeSelectAllOption: true });

    }
    else if (idName === "2") {
        $.ajax({
            method: 'GET',
            url: '/Organisation/GetOrganisationProgramDetailViewComponent',
            data: { id: $("#hdnOrganisationId").val() }
        }).done(function (data, statusText, xhdr) {
            $("#spnOrganisationsTab").html(data);
            $('.nav-tabs a[href="#menu1"]').tab('show');
            $("#lblOrgName").text($("#hdnOrganisationName").val());
            $("#dvLoadingGif").hide();
            $("#menu1").addClass("show");
        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"
            });
            $("#dvLoadingGif").hide();
        });
    }

    else if (idName === "3") {
        
        $.ajax({
            method: 'GET',
            url: '/Organisation/GetOrganisationGlobalSettingViewComponent',
            data: { id: $("#hdnOrganisationId").val() }
        }).done(function (data, statusText, xhdr) {
     
            $("#spnOrganisationsTab").html(data);
            $.validator.unobtrusive.parse($("#spnOrganisationsTab"));
            $('.nav-tabs a[href="#menu2"]').tab('show');
          //  $("#lblOrgName").text($("#hdnOrganisationName").val());
            $("#dvLoadingGif").hide();
            $("#menu2").addClass("show");
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
            url: '/Organisation/GetOrganisationAdminLevelViewComponent',
            data: { id: $("#hdnOrganisationId").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnOrganisationsTab").html(data);
            $("#menu3").addClass("show");
            $.validator.unobtrusive.parse($("#spnOrganisationsTab"));
            $('.nav-tabs a[href="#menu3"]').tab('show');
            $("#lblOrgName").text($("#hdnOrganisationName").val());
            /* Dropzone */
            var create = {
                deleteDropZoneImage: ""
            };

            if ($("#my-awesome-dropzone").length > 0) {
                $("#my-awesome-dropzone").dropzone({
                    //  var myDropzone = new Dropzone("#my-awesome-dropzone", {
                    url: "/Account/UploadImage", dictInvalidFileType: "Please upload only .jpg .jpeg .png files",
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

            $("#tblOrganisationAdmins").DataTable({
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
                    "url": "/Organisation/LoadAllOrgAdmins",
                    "data": { id: $("#hdnOrganisationId").val() },
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
                        "searchable": false,
                        "orderable": false
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
                        "searchable": true,
                        "orderable": false
                    },
                    {
                        "targets": [6],
                        "searchable": true,
                        "orderable": true
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
                        "render": function (data, type, full, mets) {

                            return "<div class='profile-accounts'><image class='avatar' src=" + full.UserImagePath + "><a  href='#' style='width:140px;' onclick=EditUserAdminData('" + full.UserId + "');> <em style='color:#007bff;'>" + full.Name + "</em></a></div>";
                        }
                    },
                    { "data": "EmailAddress", "name": "EmailAddress", "autoWidth": true, className: "text-center" },
                    { "data": "PhoneNumber", "name": "PhoneNumber", "autoWidth": true, className: "text-center" },
                    {
                        "name": "DateAdded",
                        "autoWidth": true,
                        "render": function (data, type, full, mets) { return moment(full.DateAdded).format('MMMM Do YYYY'); }
                    },
                    {
                        "render": function (data, type, full, mets) {
                            return "<span> " + full.ProgramsAccessibility + "</span>";
                        }
                    },
                    { "data": "RoleName", "name": "RoleName", "autoWidth": true },
                    {
                        "render": function (data, type, full, mets) {
                            return "<div class='switch-reload'><label class='switch'><input type='checkbox' " + (full.Status === true ? "checked" : "") + "  onclick=changeAdminStatus('" + full.UserId + "',this)><span class='slider  slider-round round'></span></label></div>";
                        }
                    },
                    {
                        "render": function (data, type, full, mets) {
                            var linkDataContentEditDelete = "";
                            if (userRlN.toLowerCase() == "super admin") {
                                linkDataContentEditDelete += "<div class='linked-delete-custom-action'><div class='linked-down'>" +
                                    "<div class='img-dots'></div></div>" +
                                    "<div class='linked-down-data-s'><div class='plan-panel-dropdown'>" +
                                    "<ul><li><a  href='#' onclick=EditUserAdminData('" + full.UserId + "');>Edit</a></li><li><a href='#' onclick=DeleteData('" + full.UserId + "');>Delete</a></li></ul>" +
                                    "</div></div></div>";
                            }

                            return linkDataContentEditDelete;

                        }
                    }]
            });
            if (userRlN.toLowerCase() !== "super admin") {
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

    else if (idName === "5") {
        //debugger;
        $.ajax({ method: 'GET', url: '/Organisation/GetNotificationLogsViewComponent', data: { id: $("#hdnOrganisationId").val() } }).done(function (data, statusText, xhdr) {
           
            $("#spnOrganisationsTab").html(data);
            $("#menu5").addClass("show");
            $.validator.unobtrusive.parse($("#spnOrganisationsTab"));
            $('.nav-tabs a[href="#menu5"]').tab('show'); $("#tblNotificationlogs").DataTable({
                "processing": true, "serverSide": true, "filter": true, "searching": false, "orderMulti": false, 
                "pageLength": 10,
                "order": [[1, "desc"]],
                "oLanguage": {
                    "sEmptyTable": "No data available."
                },
                "bLengthChange": true,
                "ajax": { "url": "/Organisation/LoadAllNotificationLogs", "data": { currentPageNumber: 0 }, "type": "POST", "datatype": "json" },
                "columnDefs": [{ "targets": [0], "visible": true, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": false, "orderable": true }, { "targets": [2], "visible": true, "searchable": false, "orderable": true }, { "targets": [3], "visible": true, "searchable": false, "orderable": false }, { "targets": [4], "visible": true, "searchable": false, "orderable": false }, { "targets": [5], "visible": true, "searchable": false, "orderable": false }],
                "columns": [{ "data": "PartnerUserId", "name": "PartnerUserId", "autoWidth": true},
                    {
                        "data": "CreatedDate", "name": "CreatedDate", "autoWidth": true,
                        "render": function (data, type, full, mets) {
                            return moment(full.CreatedDate).format('YYYY-MM-DD hh:mm:ss ');
                        }
                    },
                { "data": "ApiName", "name": "ApiName", "autoWidth": true, className: "text-center" },
             
                    { "data": "Request", "name": "Request", "autoWidth": true, className: "text-center" },
                    { "data": "Response", "name": "Response", "autoWidth": true, className: "text-center" },
                    { "data": "Status", "name": "Status", "autoWidth": true, className: "text-center" },                ],

            });
           
           
            $('#notificationlogs').addClass('show');
            $("#dvLoadingGif").hide();

        }).fail(function (xhdr, statusText, errorText) {
            swal({
                title: "Currently unable to process the request! Please try again later.",
                icon: "error"

            });

        });
    }

    else if (idName === "6") {
        //debugger;
        $.ajax({ method: 'GET', url: '/Organisation/AccountHolderList', data: { id: $("#hdnProgramId").val(), poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrganisationName").val(), orgId: $("#hdnOrganisationId").val() } }).done(function (data, statusText, xhdr) {
            $("#spnOrganisationsTab").html(data); var filename = 'Account Holders List';
            $("#tblAccountHolder").DataTable({
                "processing": true, "serverSide": true, "filter": true, "orderMulti": false, "pageLength": 10, "order": [[4, "desc"]], "dom": 'Bfrtip', "oLanguage": { "sEmptyTable": "No data available." },
                "ajax": { "url": "/Organisation/LoadAccountHoldersByOrganization", "data": { prgId: $("#hdnProgramId").val(), orgId: $("#hdnOrganisationId").val(), planId: null, currentPageNumber: 0 }, "type": "POST", "datatype": "json" },
                "columnDefs": [{ "targets": [0], "visible": false, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": true, "orderable": true }, { "targets": [2], "visible": true, "searchable": true, "orderable": true }, { "targets": [3], "visible": true, "searchable": false, "orderable": false }, { "targets": [4], "visible": true, "searchable": false, "orderable": true }, { "targets": [5], "searchable": false, "orderable": false }, { "targets": [6], "searchable": false, "orderable": false }, { "targets": [7], "searchable": false, "orderable": false }, { "targets": [8], "searchable": false, "orderable": false }],
                "columns": [{ "data": "Id", "id": "Id", "name": "Id", "autoWidth": true, className: "text-center" }, { "data": "AccountHolderID", "name": "AccountHolderID", "autoWidth": true, className: "text-center" }, {
                    "className": "text-center", "render": function (data, type, full, mets) {
                        return type !== 'export' ? "<div class='profile-accounts profile-accounts-sm'><image class='avatar' src=" + full.UserImagePath + "><a class='edit-accountholder' href='javascript:void(0);' data-programid='" + full.ProgramId + "' style='width:140px' data-id='" + full.UserEncId + "'  ;> <em style='color:#007bff;'>" + full.Name + "</em></a></div>" : full.Name;
                    }
                }, { "data": "Email", "name": "Email", "autoWidth": true, className: "text-center" }, {
                    "name": "DateAdded", "autoWidth": true, "render": function (data, type, full, mets) { return moment(full.DateAdded).format('DD MMMM YYYY'); }
                },
                { "data": "PlanName", "name": "PlanName", "autoWidth": true, className: "text-center" },
                {
                    "className": "text-center",
                    "render": function (data, type, full, mets) {
                        //debugger;
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
                            "<ul><li><a class='edit-accountholder' href='javascript:void(0);' data-programid='" + full.ProgramId + "' data-id='" + full.UserEncId + "'>Edit</a></li></ul>" + "</div></div></div>";
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
    else if (idName === "7") {
        $.ajax({
            method: 'GET',
            url: '/Program/ManageAccountHolder',
            data: { id: id, prgId: programid, poId: $("#hdnPrimaryOrgId").val(), ppN: $("#hdnProgramName").val(), poN: $("#hdnOrgName").val() }
        }).done(function (data, statusText, xhdr) {

            $("#spnOrganisationsTab").html(data);
            $.validator.unobtrusive.parse($("#spnOrganisationsTab"));
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

function DeleteData(userId) {
    $('.linked-delete-custom-action').children(".linked-down-data-s").hide();
    swal({
        title: "Are you sure you want to delete the organisation admin?",
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
                swal({
                    title: "Admin User has been deleted successfully!",
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

$(document).on('click', '#btnFilter', function () {
    var ddapiname = $("#ddlApiName").val();
    var st = $("#ddlStatus").val();
    var dt = $("#txtdate").val();
    var ddlprog = $("#ddlPrg").val();
    $("#tblNotificationlogs").DataTable({
        "processing": true, "serverSide": true, "filter": true, "searching": false, "orderMulti": false, "destroy": true,
        "pageLength": 10,
        "order": [[1, "desc"]],
        "oLanguage": {
            "sEmptyTable": "No data available."
        },
        "bLengthChange": true,
        "ajax": { "url": "/Organisation/LoadAllNotificationLogsWIthFiter", "data": { apiname: ddapiname, status: st, date: dt, programid: ddlprog }, "type": "POST", "datatype": "json" },
        "columnDefs": [{ "targets": [0], "visible": true, "searchable": false, "orderable": false }, { "targets": [1], "visible": true, "searchable": false, "orderable": true }, { "targets": [2], "visible": true, "searchable": false, "orderable": true }, { "targets": [3], "visible": true, "searchable": false, "orderable": false }, { "targets": [4], "visible": true, "searchable": false, "orderable": false }, { "targets": [5], "visible": true, "searchable": false, "orderable": false }],
        "columns": [{ "data": "PartnerUserId", "name": "PartnerUserId", "autoWidth": true },
        {
            "data": "CreatedDate", "name": "", "autoWidth": true,
            "render": function (data, type, full, mets) {
                return moment(full.CreatedDate).format('YYYY-MM-DD hh:mm:ss');
            }
        },
        { "data": "ApiName", "name": "ApiName", "autoWidth": true, className: "text-center" },

        { "data": "Request", "name": "Request", "autoWidth": true, className: "text-center" },
        { "data": "Response", "name": "Response", "autoWidth": true, className: "text-center" },
        { "data": "Status", "name": "Status", "autoWidth": true, className: "text-center" },],

    });


    $('#notificationlogs').addClass('show');
    $("#dvLoadingGif").hide();

});
   // });
//});