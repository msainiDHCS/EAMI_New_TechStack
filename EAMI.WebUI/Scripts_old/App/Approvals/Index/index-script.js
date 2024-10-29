$(document).ready(function () {
    $(function () {
        $("#tabs").tabs();
        loadIPTabs(1);
        $('#dvMainArea').show();
    });
    
});


function loadIPTabs(id) {
    $('#dvMasterGridHolder').css("visibility", "hidden");
    $('#dvPCSMasterGridHolder').css("visibility", "hidden");
    $('#dvECSMasterGridHolder').css("visibility", "hidden");

    var divName = '';
    var ViewName = '';

    switch (id) {
        case 1:
            $("#tabs").tabs("option", "active", 0);
            divName = 'dvtab1';
            ViewName = 'PendingReturnIndex';
            break;
        case 2:
            $("#tabs").tabs("option", "active", 1);
            divName = 'dvtab2';
            ViewName = 'PCSIndex';
            break;
        case 3:
            $("#tabs").tabs("option", "active", 2);
            divName = 'dvtab3';
            ViewName = 'ECSIndex';
            break;
    }    

    // Start Loading... Animation
    $('#divLoadingAnimation').css("display", "block");
    EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
    $.ajax({
        url: getEAMIAbsoluteUrl('~/Approvals/' + ViewName),
        type: 'GET',
        datatype: "html",
        cache: false,
        //async: false,
        success: function (data) {
            if (data != null) {
                $('#' + divName).html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
            }
        },
        error: function () {
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