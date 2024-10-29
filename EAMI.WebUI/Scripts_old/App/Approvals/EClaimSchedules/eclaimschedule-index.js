var $ddlECSSystems = $('#ddlECSSystems');
var $dtPickerFromRange = $('#dtPickerFromRange');
var $dtPickerToRange = $('#dtPickerToRange');
var $ddlECSStatusTypes = $('#ddlECSStatusTypes');

var $form = $("#frmECSSearch");

$ddlECSStatusTypes.change(function () {
    var statusTypeId = Number($ddlECSStatusTypes.val());    
    if (statusTypeId === 4 || statusTypeId === 6) {
        $('#fromRequiredMark').css("visibility", "visible");
        $('#toRequiredMark').css("visibility", "visible");
        $dtPickerFromRange.removeAttr('disabled');
        $dtPickerToRange.removeAttr('disabled');
        $dtPickerFromRange.val(moment().subtract(1, 'months').format('MM/DD/YYYY'));
        $dtPickerToRange.val(moment().format('MM/DD/YYYY'));
    }
    else {
        $('#fromRequiredMark').css("visibility", "hidden");
        $('#toRequiredMark').css("visibility", "hidden");
        $dtPickerFromRange.val('');
        $dtPickerToRange.val('');
        $dtPickerFromRange.attr('disabled', 'disabled');
        $dtPickerToRange.attr('disabled', 'disabled');
    }
});

$form.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        dtPickerFromRange: {
            required: true,
        },
        dtPickerToRange: {
            required: true,
        },
        ddlECSStatusTypes: {
            required: true
        }
    },
    messages: {
        dtPickerFromRange: {
            required: 'Please select from date',
        },
        dtPickerToRange: {
            required: 'Please select to date',
        },
        ddlECSStatusTypes: {
            required: 'Please select a status'
        }
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
    //EAMIShowAjaxLoadingContent('dvMasterGridHolder');

    $ddlECSSystems.selectpicker({
        title: "System"
        //,
        //liveSearch: "false"
    });

    $ddlECSStatusTypes.selectpicker({
        title: "Status",
        liveSearch: "Status",
    });

    $ddlECSSystems.val('1');
    $ddlECSSystems.selectpicker('refresh');
    $ddlECSSystems.change();

    $dtPickerFromRange.datepicker({
        numberOfMonths: 1
    })
    .on("change", function () {
        $dtPickerToRange.datepicker("option", "minDate", getDate(this));
    }),

    $dtPickerToRange.datepicker({
        numberOfMonths: 1
    })
    .on("change", function () {
        $dtPickerFromRange.datepicker("option", "maxDate", getDate(this));
    });

    //DEFAULT STATUS TO PENDING 
    $ddlECSStatusTypes.val('1');
    $ddlECSStatusTypes.selectpicker('refresh');
    $dtPickerFromRange.attr('disabled', 'disabled');
    $dtPickerToRange.attr('disabled', 'disabled');
    ECSSearch();

});


function getDate(element) {
    var date;
    try {
        date = $.datepicker.parseDate("mm/dd/yy", element.value);
    } catch (error) {
        date = null;
    }

    return date;
}
function ECSSearch() {
    $('#dvECSMasterGridHolder').css("visibility", "hidden");

    try {
        var selectedStatus = [];
        var statusTypeId = Number($ddlECSStatusTypes.val());
        var dateFrom = $dtPickerFromRange.val();
        var dateTo = $dtPickerToRange.val();
        var applyRulesFlag = true;

        $("#ddlECSStatusTypes :selected").each(function () {
            selectedStatus.push($(this).attr('value'));
        });
        
        if (statusTypeId != 4 && statusTypeId != 6) {
            dateFrom = moment().format('MM/DD/YYYY');
            dateTo = moment().format('MM/DD/YYYY');
            applyRulesFlag = false;
        }

        // Start Loading... Animation
        $('#divLoadingAnimation').css("display", "block");
        EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
        if (applyRulesFlag) {
            if ($form.valid()) {
                eami.service.approvalService.getECSSearchResults($ddlECSSystems.val(), dateFrom, dateTo, selectedStatus)
                    .done(function (data) {
                        $('#dvECSMasterGridHolder').css("visibility", "hidden");
                        if (data != null) {
                            $('#dvECSMasterGridHolder').css("visibility", "visible");
                                $('#dvECSMasterGridHolder').html(data);
                                $('#pnlECSSearch').show();
                            }
                        })
                        .always(function (data) {
                            $('#divLoadingAnimation_Inner').empty();
                            $('#divLoadingAnimation').css("display", "none");
                        });
            }
        }
        else {            
            eami.service.approvalService.getECSSearchResults($ddlECSSystems.val(), dateFrom, dateTo, selectedStatus)
                .done(function (data) {  
                    $('#dvECSMasterGridHolder').css("visibility", "hidden");
                        if (data != null) {   
                            $('#dvECSMasterGridHolder').css("visibility", "visible");
                           $('#dvECSMasterGridHolder').html(data);
                           $('#pnlECSSearch').show();
                       }
                    })
                    .always(function (data) {
                        $('#divLoadingAnimation_Inner').empty();
                        $('#divLoadingAnimation').css("display", "none");
                    });
        }
    }
    catch (e)
    { }

    return false;
}

function ECSReset() {
    $ddlECSStatusTypes.val('1');
    $ddlECSStatusTypes.selectpicker('refresh');
    $('#fromRequiredMark').css("visibility", "hidden");
    $('#toRequiredMark').css("visibility", "hidden");
    $dtPickerFromRange.val('');
    $dtPickerToRange.val('');
    $dtPickerFromRange.attr('disabled', 'disabled');
    $dtPickerToRange.attr('disabled', 'disabled');
    return false;
}