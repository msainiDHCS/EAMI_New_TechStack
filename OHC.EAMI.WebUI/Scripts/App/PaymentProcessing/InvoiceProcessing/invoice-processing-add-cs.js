var $form = $("#frmAddToCSValidator");
$form.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        ClaimScheduleID: {
            required: true,
        },
    },
    messages: {
        ClaimScheduleID: {
            required: 'Please select a valid claim schedule number',
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

$(document).ready(function () {

    $('#dvCSSelect').hide();
    $('#ddlCSs').selectpicker({
        title: "Claim Schedule #",
        liveSearch: "true"
    });
    $("#dvStatus").hide();
    $("#dvErrorMessage").empty();

});

function AssignPRSetsToClaimSchedule() {

    var status = false;
    $("#dvStatus").hide();
    $("#dvErrorMessage").empty();

    var existingCS = $('#rdExisting');
    var newCS = $('#rdNew');

    var statusMessage = '';
    if (existingCS.is(':checked')) {
        if ($form.valid()) {
            var csValue = $('#ddlCSs').find(":selected").val();
            $.ajax({
                url: getEAMIAbsoluteUrl('~/PaymentProcessing/AddPaymentGroupToClaimSchedule'),
                type: 'POST',
                datatype: "json",
                //data: { 'paymentRecordSet': Cookies.get('ckPaymentRecordSetSelection'), 'claimScheduleId': csValue },
                data: { 'paymentRecordSet': $('#hdnPaymentRecordSetSelection').val(), 'claimScheduleId': csValue },
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
                            if (!data.status) {
                                if (data.returnedData && data.returnedData.length > 0) {
                                    $.each(data.returnedData, function (index, message) {
                                        statusMessage += "<p>" + message + "</p>";
                                    });
                                }
                                else {
                                    handleAjaxErrorReturned(xhr.responseText);
                                }
                                $("#dvErrorMessage").append(statusMessage);
                                $("#dvStatus").show();
                            }
                            else { $('#dvAddToClaimScheduleModal').modal('hide'); status = true; }
                        }
                    }
                }
            });
        }
    }
    else {
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/AssignNewClaimSchedule'),
            type: 'POST',
            datatype: "json",
            data: { 'paymentRecordSet': $('#hdnPaymentRecordSetSelection').val() }, //Cookies.get('ckPaymentRecordSetSelection') },
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
                        if (!data.status) {
                            if (data.returnedData && data.returnedData.length > 0) {
                                $.each(data.returnedData, function (index, message) {
                                    statusMessage += "<p>" + message + "</p>";
                                });
                            }
                            else {
                                handleAjaxErrorReturned(xhr.responseText);
                            }
                            $("#dvErrorMessage").append(statusMessage);
                            $("#dvStatus").show();
                        }
                        else {
                            $('#dvAddToClaimScheduleModal').modal('hide'); status = true;
                        }
                    }
                }
            }
        });
    }

    if (status) {
        loadIPTabs(2);
    }

}

function SetAddToCSOption(id) {

    if (id == 1) {
        $("#dvStatus").hide();
        $("#dvErrorMessage").empty();
        $('#dvCSSelect').show();
    }
    else {
        $("#dvStatus").hide();
        $("#dvErrorMessage").empty();
        $('#dvCSSelect').hide();
    }

    return true;
}
