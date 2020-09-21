
const DomUtils = {
    getEl: (selector) => window.document.querySelector(selector),
    hasClass: (el, cssClass) => {
        if (el.classList) {
            return el.classList.contains(cssClass);
        }
    },

    removeClass: (el, cssClass) => {
        if (el.classList) {
            el.classList.remove(cssClass);
        } else if (DomUtils.hasClass(el, cssClass)) {
            const reg = new RegExp(`(\\s|^)${cssClass}(\\s|$)`);
            el.className = el.className.replace(reg, ' ');
        }
    },
};

const SubmitButton = {

    buttonElement: DomUtils.getEl('[data-submit-btn]'),
    loaderElement: DomUtils.getEl('.btn__loader'),

    enable: () => {
        SubmitButton.buttonElement.disabled = false;
        DomUtils.removeClass(SubmitButton.buttonElement, 'disabled-bkg');
    },

    setSubmitState: () => {
        SubmitButton.buttonElement.disabled = true;
        SubmitButton.loaderElement.style.display = 'inline-block';
    },

    removeSubmitState: () => {
        SubmitButton.buttonElement.disabled = false;
        SubmitButton.loaderElement.style.display = 'none';
    }
};

const config = {

    fields: {
        card: {
            selector: '[data-cc-card]',
        },
        cvv: {
            selector: '[data-cc-cvv]',
        },
        exp: {
            selector: '[data-cc-exp]',
        },
        name: {
            selector: '[data-cc-name]',
            placeholder: 'Full Name',
        },
    },

    styles: {
        input: {
            'font-size': '16px',
            color: '#00a9e0',
            'font-family': 'monospace',
            background: 'black',
        },
        '.card': {
            'font-family': 'monospace',
        },
        ':focus': {
            color: '#00a9e0',
        },
        '.valid': {
            color: '#43B02A',
        },
        '.invalid': {
            color: '#C01324',
        },
        '@media screen and (max-width: 700px)': {
            input: {
                'font-size': '18px',
            },
        },
        'input:-webkit-autofill': {
            '-webkit-box-shadow': '0 0 0 50px white inset',
        },
        'input:focus:-webkit-autofill': {
            '-webkit-text-fill-color': '#00a9e0',
        },
        'input.valid:-webkit-autofill': {
            '-webkit-text-fill-color': '#43B02A',
        },
        'input.invalid:-webkit-autofill': {
            '-webkit-text-fill-color': '#C01324',
        },
        'input::placeholder': {
            color: '#aaa',
        },
    },

    classes: {
        empty: 'empty',
        focus: 'focus',
        invalid: 'invalid',
        valid: 'valid',
    },
};

function authorizeSession(callback) {
    //debugger;
    $("#SaveCDChkBox").val($("#isCardDetailSave").prop("checked") ? "1" : "0");
    var CardToSave = $("#isCardDetailSave").prop("checked") ? "1" : "0";
    if (CardToSave == "1") {
     //   alert("s");
        if ($("#nickname").val() == '') {
            $("#spnErrornickname").show();
            $("#spnErrornickname_user").show();
            $("#spnErrornickname").focus();
            $("#spnErrornickname_user").focus();
            SubmitButton.removeSubmitState();
            return true;
        }
        else {
            $("#spnErrornickname").hide();
            $("#spnErrornickname_user").hide();
        }
    }
   
    /*Ajax Post Request Start */
    var urlPaymentPost = '/Benefactor/PaymentGateway';
    $.post(urlPaymentPost, { reloadUserId: $("#ReloadUserId").val(), IsCardDetailToSave: CardToSave, NickName: $("#nickname").val() }, function (responseText, status) {
       
        if (status == "success") {
            callback(JSON.parse(responseText));
        }
        else {
          //  alert(responseText);
            throw new Error("error response: " + responseText);
        }
    });
   
}

const hooks = {
    preFlowHook: authorizeSession,
};

const onCreate = (paymentForm) => {
   // debugger;
   // paymentForm.destroyFields(() => { });
    const onSuccess = (clientToken) => {
        $("#ClientTokenPG").val(clientToken);
        $("#IsNewCardTransaction").val("1");
        SubmitButton.removeSubmitState();
        paymentForm.reset(() => { });
        $("#isCardDetailSave").prop("checked", true);
        if ($("#isCardDetailSave").prop("checked")) {
            $.ajax({
                //url: "/Benefactor/GetCardDropdownList",
                //type: 'GET',
                //dataType: 'json', // added data type
                url: "/Benefactor/GetCardDropdownList1",
                type: 'GET',
                dataType: 'json', // added data type
                data: { id: $("#ReloadUserId").val() },
                success: function (data) {
                   /// alert(data.data);
                    $("#hdnExistingCard").val(data.length);
                    var iscardvalid = true;
                    var ddlCard = $("[id*=ddlcardslection]");
                    ddlCard.empty().append('<option selected="selected" value=""> -- Select payment method-- </option>');
                    $.each(data.data, function () {
                     //   if (this['Value'] == clientToken && (this['Disabled']==true )) {
                     //iscardvalid = false;
                     //   }
                     //   if (this['Disabled'] == false) {
                            ddlCard.append($("<option></option>").val(this['Value']).html(this['Text']));
                       // }
                    });

                     
                   // if (iscardvalid) {
                    $("#ddlcardslection").val(clientToken);
                    document.getElementById("txtIpg").value = "";

                        swal({
                            title: 'Your card is added in the dropdown. Please proceed with your payment.',
                            icon: "success"
                        });
                  //  }
                    //else {
                    //    swal({
                    //        title: 'Invalid card',
                    //        icon: "error"
                    //    });
                    //}
                  //  $('#dvChooseCard').show();
                    $('#btnReloadAmount').show();
                    if ($("#hdnUserType").val() == "basic user") {
                        $("#dvAddPaymentCard").hide();
                        $('#PaymentCardGateway1').modal('hide');
                    }
                    else {
                        $('#PaymentCardGateway').modal('hide');
                    }
                },
                error: function () {
                    swal({
                        title: 'An error occured in binding a dropdown.',
                        icon: "error"
                    });
                },
            });
        } else {
            swal({
                title: 'Your card is processed. Please proceed with your payment.',
                icon: "success"
            });
          //  window.location.href = window.location.href;
            $('#btnReloadAmount').show();
            if ($("#hdnUserType").val() == "basic user") {
                $("#dvAddPaymentCard").hide();
                $('#PaymentCardGateway1').modal('hide');
            }
            else {
                $('#PaymentCardGateway').modal('hide');
            }
        }
    };

    const onError = (error) => {
        swal({
            title: error.message,
            icon: "error"
        });

        SubmitButton.removeSubmitState();
    };

    const form = DomUtils.getEl('#frmPaymentFD')
    //form submit handler
   
    form.addEventListener('submit', (e) => {

        e.preventDefault();
        SubmitButton.setSubmitState();
        paymentForm.onSubmit(onSuccess, onError);
    });

    const resetBtn = DomUtils.getEl('[data-reset-btn]')
    resetBtn.addEventListener('click', (e) => {
        e.preventDefault();
        paymentForm.reset();
        return false;
    });


    const ccFields = window.document.getElementsByClassName('payment-fields');
    for (let i = 0; i < ccFields.length; i++) {
        DomUtils.removeClass(ccFields[i], 'disabled');
    }
    SubmitButton.enable();
};
window.firstdata.createPaymentForm(config, hooks, onCreate);

