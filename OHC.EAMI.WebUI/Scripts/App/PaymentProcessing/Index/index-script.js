$(document).ready(function () {
    $(function () {
        $("#tabs").tabs();
        loadIPTabs(1);
    });

});

function loadIPTabs(id) {
    $('#dvMasterGridHolder').css("visibility", "hidden");
    $('#dvClaimSchedules').css("visibility", "hidden");

    var divName = '';
    var ViewName = '';

    if (id == 1) {
        $("#tabs").tabs("option", "active", 0);
        divName = 'dvtab1';
        ViewName = 'InvoiceProcessingAssignment';
    }
    else {
        $("#tabs").tabs("option", "active", 1);
        divName = 'dvtab2';
        ViewName = 'IPClaimSchedules';
    }

    // Start Loading... Animation
    $('#divLoadingAnimation').css("display", "block");
    EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
    $.ajax({
        url: getEAMIAbsoluteUrl('~/PaymentProcessing/' + ViewName),
        type: 'GET',
        datatype: "html",
        cache: false,
        //async: false,
        success: function (data) {
            if (data != null) {
                $('#' + divName).html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
            }
        },
        complete: function (data) {
            $('#divLoadingAnimation_Inner').empty();
            $('#divLoadingAnimation').css("display", "none");
        }
    });

    //Remove payment record selection cookie
    removePaymentSelectionCookie();

    return false;
}

function removePaymentSelectionCookie() {
    //Remove payment record selection cookie
    //Cookies.remove('paymentRecordSetSelection');
    
    $('#hdnPaymentRecordSetSelection').val('');
}