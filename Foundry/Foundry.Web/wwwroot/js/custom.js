$(document).ready(function () {

    // set autocomplete off for all the pages
    $("input").attr("autocomplete", "off");


    $(".mobile-menu").click(function () {
        $("body").toggleClass('open-sidebar');
    });
    $(".mobile-overlay").click(function () {
        $("body").removeClass('open-sidebar');
    });
    $("#custom").spectrum({
        color: "#f00"
    });
   
    $(document).on("click", ".linked-down-actions", function () {

        $('.linked-data-actions').slideUp();
        if ($(this).hasClass('active1')) {
            $(this).removeClass('active1');
        }
        else {
            $(this).next().slideDown();
            $('.active1').removeClass('active1');
            $(this).addClass('active1');
        }
    });
    //**actions-popup-for-table**//
    $(document).on("click", ".linked-down", function () {

        $('.linked-down-data-s').slideUp();
        if ($(this).hasClass('active-actions')) {
            $(this).removeClass('active-actions');
        }
        else {
            $(this).next().slideDown();
            $('.active1').removeClass('active-actions');
            $(this).addClass('active-actions');
        }
    });

    $('.filter-option1').click(function (e) {
        e.preventDefault();
        if ($(window).width() < 991) {
            $('.nav-slide-custom').slideToggle();
        }
    });

    $('.nav-slide-custom.nav-tabs .main-tab').click(function (e) {
        e.preventDefault();
        if ($(window).width() < 991) {
            $('.nav-slide-custom.nav-tabs').slideUp(300);
        }
    });

    $('.nav-tabs.nav-org-prog .main-tab').click(function (e) {
        e.preventDefault();
        if ($(window).width() < 991) {
            $('.nav-tabs.nav-org-prog').slideUp(300);
        }
    });
       $(document).on("click", ".close", function () {
        $(this).parent(".linked-data-actions").slideUp('');
    });

    $(".close").click(function () {
        $(this).parent(".linked-data-actions").slideUp('');


    });
   
    $('.amount-options').on('click', '.radio-active', function () {
        $('.amount-options .radio-active').removeClass('active');
        $(this).addClass('active');
    });
    $('.amount-options1').on('click', '.radio-active1', function () {
        $('.amount-options1 .radio-active1').removeClass('active1');
        $(this).addClass('active1');
    });
       
    $(document).on('click', '.linked-down', function () {
        $('.linked-data-actions').slideUp();
        if ($(this).hasClass('active1')) {
            $(this).removeClass('active1');
        }
        else {
            $(this).next().slideDown();
            $('.active1').removeClass('active1');
            $(this).addClass('active1');
        }
    });

    
    $('.dropdown-multiselect .dropdown-menu li').on('click', function (event) {
        // The event won't be propagated up to the document NODE and 
        // therefore delegated events won't be fired
        event.stopPropagation();
    });

    //add programs//

    $(".add").click(function (e) {
        e.preventDefault();
        $("#select-program").clone()
            .removeAttr("id")
            .append($('<a class="delete" href="#">Remove</a>'))
            .appendTo("#additionalselects");
    });
    $("body").on('click', ".delete", function () {
        $(this).closest(".input").remove();
    });

    //add account holders//

    $(".add").click(function (e) {e.preventDefault(); $("#select-accounts-action").clone().removeAttr("id").append($('<a class="delete-action" href="#">Remove</a>')).appendTo("#additionalaccounts-action");});
    $("body").on('click', ".delete", function () {$(this).closest(".accounts-action").remove();});


    $('.filter-option').click(function (e) {
        e.preventDefault();
        if ($(window).width() < 991) {
            $('.filter-option').next().slideToggle();
        }
    });

    $('.nav-slide-custom.nav-tabs .main-tab').click(function (e) {
        e.preventDefault();
        if ($(window).width() < 991) {
            $('.nav-slide-custom.nav-tabs').slideUp(300);
        }
    });
    // Prepare the preview for profile picture
    $("#wizard-picture").change(function () {
        readURL(this);
    });

    $(document).on('keypress', '.ForNumericOnly', function (e) {

        //if the letter is not digit then display error and don't type anything
        if (e.which !== 8 && e.which !== 0 && (e.which < 48 || e.which > 57)) {
            //display error message
                      return false;
        }
    });
    $(document).on('keypress', '.ForDecimals', function (evt) {
        var self = $(this);
        self.val(self.val().replace(/[^0-9\.]/g, ''));
        if ((evt.which !== 46 || self.val().indexOf('.') !== -1) && (evt.which < 48 || evt.which > 57)) {
            return false;
        }
    });
});

function TestPasswordExp(content) {

    var passwordvalid = new RegExp("^(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\\S+$).{8,15}$"); // /^(?=.*[A-Z])(?=.*[@#$%^&+=!])(?=\\S+$).{8,15}$/;    

    if (!passwordvalid.test(content)) {
        return "Password is not valid. Password must be 8 to 15 characters long and should contain atleast one capital and one special character.";

    }
    return "";
}

function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#wizardPicturePreview').attr('src', e.target.result).fadeIn('slow');
        };
        reader.readAsDataURL(input.files[0]);
    }
}

$(document).mouseup(function (e) {
    var containera = $(".linked-data-actions");

    // if the target of the click isn't the container nor a descendant of the container
    if (!containera.is(e.target) && containera.has(e.target).length === 0) {
        containera.hide();
    }
});


$(document).mouseup(function (e) {
    var containeras = $(".linked-down-data-s");

    // if the target of the click isn't the container nor a descendant of the container
    if (!containeras.is(e.target) && containeras.has(e.target).length === 0) {
        containeras.hide();
        $(".linked-down").removeClass("active1");
    }
});

$(document).on('click', '.timepicker', function (e) {
    e.preventDefault();
});


