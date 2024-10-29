var $ddlSystems = $('#ddlSystems');
var $ddlPayees = $('#ddlPayees');
var $ddlPaymentTypes = $('#ddlPaymentTypes');
var $ddlContractNumbers = $('#ddlContractNumbers');

$(document).ready(function () {
    $ddlPayees.attr('data-selected-text-format', 'count');
    $ddlPaymentTypes.attr('data-selected-text-format', 'count');
    $ddlContractNumbers.attr('data-selected-text-format', 'count');

    $ddlSystems.selectpicker({
        title: "System"
        //,
        //liveSearch: "false"
    });


    $ddlPayees.selectpicker({
        title: "Payee",
        liveSearch: "false",
        multiple: true,
        actionsBox: true        
    });

    $ddlPaymentTypes.selectpicker({
        title: "Payment Type",
        liveSearch: "false",
        multiple: true,
        actionsBox: true        
    });

    $ddlContractNumbers.selectpicker({
        title: "Contract #",
        liveSearch: "false",
        multiple: true,
        actionsBox: true       
    });

    $ddlSystems.on('changed.bs.select', function (e) {

        var ddlvalue = $(this).val();

        var emptyValue = '';
        $ddlPayees.empty();

        if ($.trim(ddlvalue) != '') {
            //Call EMAI Approval GetPRSearchResultDropdownValues service
            eami.service.approvalService.getPRSearchResultDropdownvalues('PAYEE', ddlvalue, emptyValue)
                .done(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $ddlPayees.prepend("<option value='" + data[i]['value'] + "'>" + data[i]['text'] + "</option>");
                    }

                    $ddlPayees.prop("disabled", false);
                    $ddlPayees.selectpicker('refresh');
                    $ddlPaymentTypes.prop("disabled", true);
                    $ddlContractNumbers.prop("disabled", true);
                });
        }
    });

    $ddlPayees.on('changed.bs.select', function (e) {
        var ddlvalue = $(this).val().toString().replace(/_/g, ',');
        var emptyValue = '';
        if ($.trim(ddlvalue) != '') {
            eami.service.approvalService.getPRSearchResultDropdownvalues('PAYMENTTYPE', ddlvalue, emptyValue)
                .done(function (data) {
                    $ddlPaymentTypes.empty();
                    for (var i = 0; i < data.length; i++) {
                        $ddlPaymentTypes.prepend("<option value='" + data[i]['value'] + "'>" + data[i]['text'] + "</option>");
                    }

                    $ddlPaymentTypes.prop("disabled", false);
                    $ddlPaymentTypes.selectpicker('refresh');
                    $ddlContractNumbers.selectpicker('refresh');
                    $ddlContractNumbers.prop("disabled", true);
                });
        }
        else {
            $ddlPaymentTypes.empty();
            $ddlPaymentTypes.selectpicker('refresh');
            $ddlPaymentTypes.prop("disabled", true);
            $ddlPaymentTypes.selectpicker('refresh');
            $ddlPaymentTypes.prop("disabled", true);

            $ddlContractNumbers.empty();
            $ddlContractNumbers.selectpicker('refresh');
            $ddlContractNumbers.prop("disabled", true);
            $ddlContractNumbers.selectpicker('refresh');
            $ddlContractNumbers.prop("disabled", true);
        }
    });

    $ddlPaymentTypes.on('changed.bs.select', function (e) {
        var ddlvalue = $('#ddlPayees').val().toString().replace(/_/g, ',');
        var childValue = $ddlPaymentTypes.val().toString();
        $ddlContractNumbers.empty();
        if ($.trim(childValue) != '') {

            eami.service.approvalService.getPRSearchResultDropdownvalues('CONTRACTNUMBER', ddlvalue, childValue)
                .done(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $ddlContractNumbers.prepend("<option value='" + data[i]['value'] + "'>" + data[i]['text'] + "</option>");
                    }
                    $ddlContractNumbers.prop("disabled", false);
                    $ddlContractNumbers.selectpicker('refresh');
                });
        }
        else {
            $ddlContractNumbers.selectpicker('refresh');
            $ddlContractNumbers.prop("disabled", true);

            $ddlContractNumbers.selectpicker('refresh');
            $ddlContractNumbers.prop("disabled", true);
        }
    });

    $ddlSystems.val('1');                           //Initialize to CAPMAN
    $ddlSystems.selectpicker('refresh');
    $ddlSystems.change();
    $('#dvSearchCriteria').show();
    Search();

});

function Search() {
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
        eami.service.approvalService.getPRSearchResults($('#ddlSystems').val(), selectedPayeeValues, selectedPaymentValues, selectedContractValues)
            .done(function (data) {
                $('#dvMasterGridHolder').css("visibility", "hidden");
                    if (data != null) {
                        $('#dvMasterGridHolder').css("visibility", "visible");
                        $('#dvMasterGridHolder').html(data);
                    }
                })
                .always(function (data) {
                    $('#divLoadingAnimation_Inner').empty();
                    $('#divLoadingAnimation').css("display", "none");
                });
    }
    catch (e)
    { }

    return false;
}

function Reset() {

    //Remove Payment Selection Cookie
    removePaymentSelectionCookie();

    $ddlPayees.empty().selectpicker('refresh');
    $ddlPayees.prop("disabled", true);

    $ddlPaymentTypes.empty().selectpicker('refresh');
    $ddlPaymentTypes.prop("disabled", true);
    $ddlPaymentTypes.empty().selectpicker('refresh');
    $ddlPaymentTypes.prop("disabled", true);

    $ddlContractNumbers.empty().selectpicker('refresh')
    $ddlContractNumbers.prop("disabled", true);
    $ddlContractNumbers.empty().selectpicker('refresh')
    $ddlContractNumbers.prop("disabled", true);

    $ddlSystems.val('1').change();                           //Initialize to CAPMAN
    return false;
}

function removePaymentSelectionCookie() {
    $('#hdnPaymentRecordSetSelection').val('');

}
