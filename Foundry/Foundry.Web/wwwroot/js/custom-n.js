$(document).ready(function () {
    $(".mobile-menu").click(function () { $("body").toggleClass('open-sidebar'); });
    $(".mobile-overlay").click(function () { $("body").removeClass('open-sidebar');});
           $("#custom").spectrum({ color: "#f00"});
    $(".linked-down-actions").click(function () { $(this).parent(".linked-delete-box").children(".linked-data-actions").slideToggle(); $(this).toggleClass('img-dots-active');});
        $(".close").click(function () { $(this).parent(".linked-data-actions").slideUp(''); });
     $(".linked-down").click(function () { $(this).parent(".linked-delete-custom-action").children(".linked-down-data-s").slideToggle(); $(this).toggleClass('add-collpase-arrow'); });
    
    $('.dropdown-multiselect .dropdown-menu li').on('click', function (event) {/* The event won't be propagated up to the document NODE and therefore delegated events won't be fired*/ event.stopPropagation(); });
       
    $('.amount-options').on('click', '.radio-active', function () { $('.amount-options .radio-active').removeClass('active'); $(this).addClass('active');});

    //add programs//

    //  $(".add").click(function(e) {
    //e.preventDefault();
    //      $("#select-program").clone()
    //          .removeAttr("id")
    //          .append( $('<a class="delete" href="#">Remove</a>') )
    //          .appendTo("#additionalselects");
    //  });
    //  $("body").on('click',".delete", function() {
    //      $(this).closest(".input").remove();
    //  });

    //add account holders//

    //$(".add").click(function(e) {
    // 	e.preventDefault();
    //       $("#select-accounts-action").clone()
    //           .removeAttr("id")
    //           .append( $('<a class="delete-action" href="#">Remove</a>') )
    //           .appendTo("#additionalaccounts-action");
    //   });
    $("body").on('click', ".delete-action", function () {
        $(this).closest(".accounts-action").remove();
    });
    /**new**/

  /*Add Js Function Start*/  $(".add").click(function (e) { e.preventDefault(); $("#select-hours").clone().removeAttr("id").append($('<a class="delete-action" href="#">Remove</a>')).appendTo("#add-more-hours"); });/*Add Js Function End*/
    $("body").on('click', ".delete-action", function (e) {
        e.preventDefault();
        $(this).closest(".select-business-hours").remove();
    });

    $(".add-terminal-fields").click(function (e) {
        e.preventDefault();
        $("#select-terminal").clone()
            .removeAttr("id")
            .append($('<a class="delete-action-remove" href="#">Remove</a>'))
            .appendTo("#add-terminal");
    });
    $("body").on('click', ".delete-action-remove", function (e) { e.preventDefault(); $(this).closest(".terminal-area").remove();});

    $(".add-holidays").click(function (e) { e.preventDefault();$("#select-holidays-hours").clone().removeAttr("id").append($('<a class="delete-action" href="#">Remove</a>')).appendTo("#add-more-holidays"); });
    $("body").on('click', ".delete-action", function (e) {
        e.preventDefault();
        $(this).closest(".select-holidays-area").remove();
    });
    $(".add-date-hours").click(function (e) { e.preventDefault(); $("#select-date-hours").clone().removeAttr("id").append($('<a class="delete-date-action" href="#">Remove</a>')).appendTo("#add-more-datehours"); });
    $("body").on('click', ".delete-date-action", function (e) { e.preventDefault(); $(this).closest(".select-date-hours").remove(); });

    // $('.filter-option').click(function(e){
    //	e.preventDefault();
    //	if ($(window).width() < 991) {
    //		$('.filter-option').next().slideToggle();
    //	}		
    //});

    //$('.nav-slide-custom.nav-tabs .main-tab').click(function(e){
    //	e.preventDefault();
    //	if ($(window).width() < 991) {
    //		$('.nav-slide-custom.nav-tabs').slideUp(300);
    //	}		
    //});

    $(".view-rewards-detail").click(function () {
        $("#view-rewards-list").show();
        $("#add-rewards-list").hide();
    });

    $(".show-rewards-detail").click(function () {
        $("#add-rewards-list").show();
        $("#view-rewards-list").hide();
    });

    $(".add-brand-more").click(function (e) {
        e.preventDefault();
        $("#select-brands").clone()
            .removeAttr("id")
            .append($('<a class="delete" href="#">Remove</a>'))
            .appendTo("#add-brands-clone");
    });

    $("body").on('click', ".delete", function () {
        $(this).closest(".brand-input").remove();
    });

    // Prepare the preview for profile picture
    $("#wizard-picture").change(function () {
        readURL(this);
    });

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
                $('#wizardPicturePreview').attr('src', e.target.result).fadeIn('slow');
            }
            reader.readAsDataURL(input.files[0]);
        }
    }
});