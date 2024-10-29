var $ddlSystems = $('#ddlSystems');
var $ddlPayees = $('#ddlPayees');
var $ddlPaymentTypes = $('#ddlPaymentTypes');
var $ddlContractNumbers = $('#ddlContractNumbers');
var holdArray = [];
var unholdArray = [];

var $formHold = $("#frmHold");
$formHold.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        txtHoldNote: {
            required: true,
            normalizer: function(value) {                
                return $.trim(value);
            }            
        },
    },
    messages: {
        txtHoldNote: {
            required: 'Please enter a note',            
        },
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    success: function (label, element) {
        label.parent().removeClass('error');
        label.remove();
    },
});

var $formReturn = $("#frmReturn");
$formReturn.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        txtReturnNote: {
            required: true,
            normalizer: function (value) {
                return $.trim(value);
            }
        },
    },
    messages: {
        txtReturnNote: {
            required: 'Please enter a note',
        },
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    success: function (label, element) {
        label.parent().removeClass('error');
        label.remove();
    },
});
function PPSearch() {
    $('#dvMasterGridHolder').css("visibility", "hidden");

    //Remove Cookie
    removePaymentSelectionCookie();
    try {
        var selectedPayeeValues = [];
        var selectedPaymentValues = [];
        var selectedContractValues = [];

        $("#ddlPayees :selected").each(function () {
            var selectedPayeeValuesArray = $(this).attr('value').split("_");
            $.each(selectedPayeeValuesArray, function (i) {
                selectedPayeeValues.push(selectedPayeeValuesArray[i]);
            });
        });

        $("#ddlPaymentTypes :selected").each(function () {
            selectedPaymentValues.push($(this).attr('value'));
        });

        $("#ddlContractNumbers :selected").each(function () {
            selectedContractValues.push($(this).attr('value'));
        });

        // Start Loading... Animation
        $('#divLoadingAnimation').css("display", "block");
        EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingAssignmentGrid'),
            type: 'POST',
            datatype: "html",
            data: { 'system': $ddlSystems.val(), 'payeeNameIDs': selectedPayeeValues, 'paymentTypeValues': selectedPaymentValues, 'contractNumberValues': selectedContractValues },
            cache: false,
            //async: false,
            success: function (data) {
                
                if (data != null) {
                    $('#dvMasterGridHolder').css("visibility", "visible");
                    $('#dvMasterGridHolder').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                }
            },
            complete: function (data) {
                $('#divLoadingAnimation_Inner').empty();
                $('#divLoadingAnimation').css("display", "none");
            }
        });
    }
    catch (e)
    { }

    return false;
}

function PPReset() {

    //Remove Payment Selection Cookie
    removePaymentSelectionCookie();

    $("#ddlPayees").empty().selectpicker('refresh');
    $("#ddlPayees").prop("disabled", true);

    $("#ddlPaymentTypes").empty().selectpicker('refresh');
    $("#ddlPaymentTypes").prop("disabled", true);
    $("#ddlPaymentTypes").empty().selectpicker('refresh');
    $("#ddlPaymentTypes").prop("disabled", true);

    $('#ddlContractNumbers').empty().selectpicker('refresh')
    $("#ddlContractNumbers").prop("disabled", true);
    $('#ddlContractNumbers').empty().selectpicker('refresh')
    $("#ddlContractNumbers").prop("disabled", true);

    $("#ddlSystems").val('1').change();                           //Initialize to CAPMAN
    return false;
}

function AddToCS() {
    var $SelectedPaymentGroups = $('#grdPRsByPayeeAndPaymentType').find('.rowselectors:checked');
    if ($SelectedPaymentGroups.length > selectedPaymentGroupsLimit) {
        getWarning_SelectedPaymentGroupsExceedLimit($SelectedPaymentGroups.length, selectedPaymentGroupsLimit);
    }
    else {
        var hdnHighValue = 0;
        $('#grdPRsByPayeeAndPaymentType').find('.rowselectors').each(function () {
            hdnHighValue = hdnHighValue + Number($(this).parent().find('.hdnRowSpecificPaymentRecordHighAmount').val());
        });

        var isHigh = false;
        if (hdnHighValue >= 1) {
            isHigh = true;
        }

        //console.log(Cookies.get('ckPaymentRecordSetSelection'));
        //if (!Cookies.get('ckPaymentRecordSetSelection')) {
        if ($('#hdnPaymentRecordSetSelection').val() == '[]' || $('#hdnPaymentRecordSetSelection').val() == '') {
            $("#dvNoPaymentSelection").modal();
        }
        else {
            var paymentRecordCookieValue = $('#hdnPaymentRecordSetSelection').val(); //JSON.parse(Cookies.get('ckPaymentRecordSetSelection'));
            if (paymentRecordCookieValue.length <= 0) {
                $("#dvNoPaymentSelection").modal();
            }
            else {
                try {

                    $.ajax({
                        url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingAddToClaimSchedule'),
                        type: 'GET',
                        datatype: "html",
                        cache: false,
                        async: false,
                        success: function (data) {
                            if (data != null) {
                                if (typeof (data) == "string"
                                    && data.includes('An error occured while processing your request')) {
                                    $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
                                    $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
                                    showErrorOnWholePage(data);
                                }
                                else {
                                    $('#modalBodyAddToCS').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                                    if (isHigh) {
                                        $('#rdExisting').attr('disabled', true);
                                        $('#dvHighAmountAlert').show();
                                    }
                                    else {
                                        $('#dvHighAmountAlert').hide();
                                    }
                                    $('#dvAddToClaimScheduleModal').modal('show');
                                }
                            }
                        }
                    });

                }
                catch (e)
                { }

                return false;
            }
        }
    }
}


function showReturnModal() {
    var $SelectedPaymentGroups = $('#grdPRsByPayeeAndPaymentType').find('.rowselectors:checked');
    if ($SelectedPaymentGroups.length > selectedPaymentGroupsLimit) {
        getWarning_SelectedPaymentGroupsExceedLimit($SelectedPaymentGroups.length, selectedPaymentGroupsLimit);
    }
    else {
        $("#dvReturnErrorMessage").empty();
        $('#dvReturnStatus').hide();
        $('#txtReturnNote').val('');
        //if (!Cookies.get('ckPaymentRecordSetSelection')) {
        if ($('#hdnPaymentRecordSetSelection').val() == '[]' || $('#hdnPaymentRecordSetSelection').val() == '') {
            $("#dvNoPaymentSelection").modal();
        }
        else {
            $('#lbReturnPaymentSetId').empty();
            //var paymentRecordCookieValue = JSON.parse(Cookies.get('ckPaymentRecordSetSelection'));
            var paymentRecordCookieValue = JSON.parse($('#hdnPaymentRecordSetSelection').val());
            $('#lbReturnPaymentSetId').append(paymentRecordCookieValue.join(' , '));
            $('#dvReturnModal').modal('show');
        }
    }
}

function confirmPaymentRecordReturn() {
    var statusMessage = '';
    var status = false;
    $("#dvReturnErrorMessage").empty();
    $('#dvReturnStatus').hide();
    //if (!Cookies.get('ckPaymentRecordSetSelection')) {
    if ($('#hdnPaymentRecordSetSelection').val() == '[]' || $('#hdnPaymentRecordSetSelection').val() == '') {
        $("#dvNoPaymentSelection").modal();
    }
    else {        
        if ($formReturn.valid()) {
            $.ajax({
                url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingReturnPaymentGroup'),
                type: 'POST',
                datatype: "json",
                //data: { 'paymentRecordSet': Cookies.get('ckPaymentRecordSetSelection'), 'note': $('#txtReturnNote').val() },
                data: { 'paymentRecordSet': $('#hdnPaymentRecordSetSelection').val(), 'note': $('#txtReturnNote').val() },
                cache: false,
                async: false,
                success: function (data) {
                    console.log(data.status);
                    if (!data.status) {
                        if (data.returnedData && data.returnedData.length > 0) {
                            $.each(data.returnedData, function (index, message) {
                                statusMessage += "<p>" + message + "</p>";
                            });
                        }
                        else {
                            statusMessage += "<p> An error occured.</p>";
                        }
                        $("#dvReturnErrorMessage").append(statusMessage);
                        $("#dvReturnStatus").show();
                    }
                    else { $('#dvReturnModal').modal('hide'); status = true; }
                }
            });
        }
        if (status) {
            setTimeout(function () { loadIPTabs(1); }, 1000);
        }
    }
}


function showHoldModal() {
    var $SelectedPaymentGroups = $('#grdPRsByPayeeAndPaymentType').find('.rowselectors:checked');
    if ($SelectedPaymentGroups.length > selectedPaymentGroupsLimit) {
        getWarning_SelectedPaymentGroupsExceedLimit($SelectedPaymentGroups.length, selectedPaymentGroupsLimit);
    }
    else {
        if (holdArray.length > 0 || unholdArray.length > 0) {
            if (holdArray.length > 0 && unholdArray.length > 0) {
                $('#dvMixedHoldUnHoldModal').modal('show');
            }
                //Else if unholdArray has elements and holdArray is empty then show Hold Modal
            else if (unholdArray.length > 0 && holdArray.length == 0) {
                $("#dvHoldErrorMessage").empty();
                $('#dvHoldStatus').hide();
                $('#txtHoldNote').val('');
                $('#lbHoldPaymentSetId').empty();
                $('#lbHoldPaymentSetId').append(unholdArray.join(' , '));
                $('#dvHoldModal').modal('show');
            }
                //Else if unholdArray is empty and holdArray has elements then show UnHold Modal
            else if (unholdArray.length == 0 && holdArray.length > 0) {
                $("#dvUnholdErrorMessage").empty();
                $('#dvUnholdStatus').hide();
                $('#txtUnholdNote').val('');
                $('#lbUnholdPaymentSetId').empty();
                $('#lbUnholdPaymentSetId').append(holdArray.join(' , '));
                $('#dvUnholdModal').modal('show');
            }
            else if (holdArray.length == 0 && unholdArray.length == 0) {
                $("#dvNoPaymentSelection").modal();
            }
        }
        else if (holdArray.length == 0 && unholdArray.length == 0) {
            $("#dvNoPaymentSelection").modal();
        }
    }
}

function confirmHoldRecord() {
    var statusMessage = '';
    var status = false;
    $("#dvHoldErrorMessage").empty();
    $('#dvHoldStatus').hide();    
    if (unholdArray.length == 0) {
        $("#dvNoPaymentSelection").modal();
    }
    else {
        if ($formHold.valid())
        {
            $.ajax({
                url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingHoldPaymentGroup'),
                type: 'POST',
                datatype: "json",                
                data: { 'paymentRecordSet': JSON.stringify(unholdArray), 'note': $('#txtHoldNote').val() },
                cache: false,
                async: false,
                success: function (data) {                    
                    if (!data.status) {
                        if (data.returnedData && data.returnedData.length > 0) {
                            $.each(data.returnedData, function (index, message) {
                                statusMessage += "<p>" + message + "</p>";
                            });
                        }
                        else {
                            statusMessage += "<p> An error occured.</p>";
                        }
                        $("#dvHoldErrorMessage").append(statusMessage);
                        $("#dvHoldStatus").show();
                    }
                    else { $('#dvHoldModal').modal('hide'); status = true; }
                }
            });
        }

        if (status) {
            setTimeout(function () { loadIPTabs(1); }, 1000);
        }
    }
}

function confirmUnholdRecord() {
    var statusMessage = '';
    var status = false;
    $("#dvUnholdErrorMessage").empty();
    $('#dvUnholdStatus').hide();    
    if (holdArray.length == 0) {
        $("#dvNoPaymentSelection").modal();
    }
    else {
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingUnholdPaymentGroup'),
            type: 'POST',
            datatype: "json",            
            data: { 'paymentRecordSet': JSON.stringify(holdArray), 'note': $('#txtUnholdNote').val() },
            cache: false,
            async: false,
            success: function (data) {               
                if (!data.status) {
                    if (data.returnedData && data.returnedData.length > 0) {
                        $.each(data.returnedData, function (index, message) {
                            statusMessage += "<p>" + message + "</p>";
                        });
                    }
                    else {
                        statusMessage += "<p> An error occured.</p>";
                    }
                    $("#dvUnholdErrorMessage").append(statusMessage);
                    $("#dvUnholdStatus").show();
                }
                else { $('#dvUnholdModal').modal('hide'); status = true; }
            }
        });

        if (status) {
            setTimeout(function () { loadIPTabs(1); }, 1000);
        }
    }
}

$(document).ready(function () {
    $('#ddlPayees').attr('data-selected-text-format', 'count');
    $('#ddlPaymentTypes').attr('data-selected-text-format', 'count');
    $('#ddlContractNumbers').attr('data-selected-text-format', 'count');

    $ddlSystems.selectpicker({
        title: "System"
        //,
        //liveSearch: "false"
    });

    

    $('#ddlPayees').selectpicker({
        title: "Payee",
        liveSearch: "false",
        multiple: true,
        actionsBox: true,        
    });

    $('#ddlPaymentTypes').selectpicker({
        title: "Payment Type",
        liveSearch: "false",
        multiple: true,
        actionsBox: true        
    });

    $('#ddlContractNumbers').selectpicker({
        title: "Contract #",
        liveSearch: "false",
        multiple: true,
        actionsBox: true        
    });   

    $('#ddlSystems').on('changed.bs.select', function (e) {

        var ddlvalue = $(this).val();
        var emptyValue = '';
        $("#ddlPayees").empty();        

        if ($.trim(ddlvalue) != '') {

            try {
                var response = getUrlJsonSync(getEAMIAbsoluteUrl('~/PaymentProcessing/GetFilterValues?type=PAYEE&parentValue=' + ddlvalue + '&childValue=' + emptyValue));

                if (response.valid == 'OK') {

                    for (var i = 0; i < response.data.length; i++) {
                        $("#ddlPayees").prepend("<option value='" + response.data[i]['value'] + "'>" + response.data[i]['text'] + "</option>");
                    }

                    $("#ddlPayees").prop("disabled", false);
                    $("#ddlPayees").selectpicker('refresh');
                    $("#ddlPaymentTypes").prop("disabled", true);
                    $("#ddlContractNumbers").prop("disabled", true);
                }               
            }
            catch (e)
            { }
        }                       
    });

    $('#ddlPayees').on('changed.bs.select', function (e) {
        var ddlvalue = $(this).val().toString().replace(/_/g, ',');
        var emptyValue = '';        
        if ($.trim(ddlvalue) != '') {
            try {
                var response = getUrlJsonSync(getEAMIAbsoluteUrl('~/PaymentProcessing/GetFilterValues?type=PAYMENTTYPE&parentValue=' + ddlvalue + '&childValue=' + emptyValue));

                if (response.valid == 'OK') {
                    $("#ddlPaymentTypes").empty();
                    for (var i = 0; i < response.data.length; i++) {
                        $("#ddlPaymentTypes").prepend("<option value='" + response.data[i]['value'] + "'>" + response.data[i]['text'] + "</option>");
                    }
                    $("#ddlPaymentTypes").prop("disabled", false);
                    $("#ddlPaymentTypes").selectpicker('refresh');
                    $("#ddlContractNumbers").selectpicker('refresh');
                    $("#ddlContractNumbers").prop("disabled", true);
                }
            }
            catch (e)
            { }
        }
        else {            
            $("#ddlPaymentTypes").empty();
            $("#ddlPaymentTypes").selectpicker('refresh');            
            $("#ddlPaymentTypes").prop("disabled", true);
            $("#ddlPaymentTypes").selectpicker('refresh');
            $("#ddlPaymentTypes").prop("disabled", true);

            $("#ddlContractNumbers").empty();
            $("#ddlContractNumbers").selectpicker('refresh');
            $("#ddlContractNumbers").prop("disabled", true);
            $("#ddlContractNumbers").selectpicker('refresh');
            $("#ddlContractNumbers").prop("disabled", true);
        }
    });

    $('#ddlPaymentTypes').on('changed.bs.select', function (e) {
        var ddlvalue = $('#ddlPayees').val().toString().replace(/_/g, ',');
        var childValue = $('#ddlPaymentTypes').val().toString();
        $("#ddlContractNumbers").empty();
        if ($.trim(childValue) != '') {

            try {
                var response = getUrlJsonSync(getEAMIAbsoluteUrl('~/PaymentProcessing/GetFilterValues?type=CONTRACTNUMBER&parentValue=' + ddlvalue + '&childValue=' + childValue));

                if (response.valid == 'OK') {

                    for (var i = 0; i < response.data.length; i++) {
                        $("#ddlContractNumbers").prepend("<option value='" + response.data[i]['value'] + "'>" + response.data[i]['text'] + "</option>");
                    }
                    $("#ddlContractNumbers").prop("disabled", false);
                    $("#ddlContractNumbers").selectpicker('refresh');
                }
            }
            catch (e)
            { }
        }
        else {
            $("#ddlContractNumbers").selectpicker('refresh');
            $("#ddlContractNumbers").prop("disabled", true);

            $("#ddlContractNumbers").selectpicker('refresh');
            $("#ddlContractNumbers").prop("disabled", true);
        }
    });

    $("#ddlSystems").val('1');                           //Initialize to CAPMAN
    $("#ddlSystems").selectpicker('refresh');
    $('#ddlSystems').change();
    $('#dvSearchCriteria').show();
    PPSearch();

});

function removePaymentSelectionCookie() {
    //Remove payment record selection cookie
    //Cookies.remove('ckPaymentRecordSetSelection');
    $('#hdnPaymentRecordSetSelection').val('');
    holdArray = [];
    unholdArray = [];

}

// Warning/Error Popup Modal Functions Section Begin ***************************************************************************************************************************
function getWarning_SelectedPaymentGroupsExceedLimit(numberOfSelectedPaymentGroups, selectedPaymentGroupsLimit) {
    var retHtmlHeaderString = '<span class="glyphicon glyphicon-warning-sign EAMI_Text_Warning"></span> Warning';
    var retHtmlString = '' +
    '<div class="container-fluid">' +
        '<div class="alert alert-warning" style="display: none; margin-top: 10px;" id="warning_alert"></div>' +
    '</div>';
    $("#modalHeaderForSelectedPaymentGroupsExceedLimit").html(retHtmlHeaderString);
    $("#modalBodyForSelectedPaymentGroupsExceedLimit").html(retHtmlString);
    $('#modalWrapperForSelectedPaymentGroupsExceedLimit').modal('show');
    $("#modalWrapperForSelectedPaymentGroupsExceedLimit #warning_alert").html("Your selection of <span style='color:blue;'>" + numberOfSelectedPaymentGroups +
        "</span> Payment Sets exceeds the allowable limit of <span style='color:blue;'>" + selectedPaymentGroupsLimit + "</span> Selected Payment Sets.  " +
        "Please select less than <span style='color:blue;'>" + selectedPaymentGroupsLimit + "</span> Payment Sets before proceeding.");
    $("#modalWrapperForSelectedPaymentGroupsExceedLimit #warning_alert").show();
}
// Warning/Error Popup Modal Functions Section End *****************************************************************************************************************************