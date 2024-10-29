var $ddlPCSSystems = $('#ddlPCSSystems');
var $ddlPCSPayees = $('#ddlPCSPayees');
var $ddlPCSPaymentTypes = $('#ddlPCSPaymentTypes');
var $ddlPCSContractNumbers = $('#ddlPCSContractNumbers');
var $ddlPCSPayDates = $('#ddlPCSPayDates');
var isPayDateSearch = false;
var selectedPCSSuperGroup = [];
var selectedPCSSuperGroupIds = [];
var selectedPayDates = "";

$(document).ready(function () {
    $ddlPCSPayees.attr('data-selected-text-format', 'count');
    $ddlPCSPaymentTypes.attr('data-selected-text-format', 'count');
    $ddlPCSContractNumbers.attr('data-selected-text-format', 'count');
    $ddlPCSPayDates.attr('data-selected-text-format', 'count');

    $ddlPCSSystems.selectpicker({
        title: "System"
        //,
        //liveSearch: "false"
    });

    $ddlPCSPayees.selectpicker({
        title: "Payee",
        liveSearch: "false",
        multiple: true,
        actionsBox: true
    });

    $ddlPCSPaymentTypes.selectpicker({
        title: "Payment Type",
        liveSearch: "false",
        multiple: true,
        actionsBox: true
    });

    $ddlPCSContractNumbers.selectpicker({
        title: "Contract #",
        liveSearch: "false",
        multiple: true,
        actionsBox: true
    });

    $ddlPCSPayDates.selectpicker({
        title: "Pay Date"
    });

    //Fill out Pay Date
    getPayDates();

    $ddlPCSSystems.on('changed.bs.select', function (e) {

        var ddlvalue = $(this).val();

        var emptyValue = '';
        $ddlPCSPayees.empty();

        if ($.trim(ddlvalue) != '') {
            //Call EMAI Approval getPCSSearchResultDropdownvalues service
            eami.service.approvalService.getPCSSearchResultDropdownvalues('PAYEE', ddlvalue, emptyValue)
                .done(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $ddlPCSPayees.prepend("<option value='" + data[i]['value'] + "'>" + data[i]['text'] + "</option>");
                    }

                    $ddlPCSPayees.prop("disabled", false);
                    $ddlPCSPayees.selectpicker('refresh');
                    $ddlPCSPaymentTypes.prop("disabled", true);
                    $ddlPCSContractNumbers.prop("disabled", true);
                });
        }
    });

    $ddlPCSPayees.on('changed.bs.select', function (e) {
        var ddlvalue = $(this).val().toString().replace(/_/g, ',');
        var emptyValue = '';
        if ($.trim(ddlvalue) != '') {
            eami.service.approvalService.getPCSSearchResultDropdownvalues('PAYMENTTYPE', ddlvalue, emptyValue)
                .done(function (data) {
                    $ddlPCSPaymentTypes.empty();
                    for (var i = 0; i < data.length; i++) {
                        $ddlPCSPaymentTypes.prepend("<option value='" + data[i]['value'] + "'>" + data[i]['text'] + "</option>");
                    }
                
                    eami.utility.enableDisableDDL($ddlPCSPaymentTypes, true);                    
                    eami.utility.enableDisableDDL($ddlPCSContractNumbers, false);

                    PCSClearOtherSearchBar(false);
                });
        }
        else {

            eami.utility.enableDisableDDL($ddlPCSPayDates, true);                        

            $ddlPCSPaymentTypes.empty();
            eami.utility.enableDisableDDL($ddlPCSPaymentTypes, false);
            
            $ddlPCSContractNumbers.empty();
            eami.utility.enableDisableDDL($ddlPCSContractNumbers, false);            
        }
    });

    $ddlPCSPaymentTypes.on('changed.bs.select', function (e) {
        var ddlvalue = $ddlPCSPayees.val().toString().replace(/_/g, ',');
        var childValue = $ddlPCSPaymentTypes.val().toString();
        $ddlPCSContractNumbers.empty();

        if ($.trim(childValue) != '') {
            eami.service.approvalService.getPCSSearchResultDropdownvalues('CONTRACTNUMBER', ddlvalue, childValue)
                .done(function (data) {
                    for (var i = 0; i < data.length; i++) {
                        $ddlPCSContractNumbers.prepend("<option value='" + data[i]['value'] + "'>" + data[i]['text'] + "</option>");
                    }
                    eami.utility.enableDisableDDL($ddlPCSContractNumbers, true);                   
                });
        }
        else {
            eami.utility.enableDisableDDL($ddlPCSContractNumbers, false);           
        }
    });

    $ddlPCSSystems.val('1');                           //Initialize to CAPMAN
    $ddlPCSSystems.selectpicker('refresh');
    $ddlPCSSystems.change();
    $('#dvPCSSearchCriteria').show();
    PCSSearch();

});


$ddlPCSPayDates.on('changed.bs.select', function (e) {
    var ddlvalue = $(this).val();
    if ($.trim(ddlvalue) != '') {
        selectedPCSSuperGroup = [];
        selectedPCSSuperGroupIds = [];

        $ddlPCSSystems.selectpicker('refresh');
        $ddlPCSSystems.prop("disabled", true);
        $ddlPCSSystems.selectpicker('refresh');
        
        eami.utility.enableDisableDDL($ddlPCSPaymentTypes, false);
        eami.utility.enableDisableDDL($ddlPCSPaymentTypes, false);
        isPayDateSearch = true;

        PCSClearOtherSearchBar(true);
    }
    else {
        isPayDateSearch = false;
    }
});

function getPayDates() {
    eami.service.approvalService.getPCSSearchResultDropdownvalues('PAYDATE', "", "")
               .done(function (data) {
                   $ddlPCSPayDates.empty();
                   for (var i = 0; i < data.length; i++) {
                       $ddlPCSPayDates.prepend("<option value='" + moment(data[i]['value']).format('MM/DD/YYYY') + "'>" + moment(data[i]['text']).format('MM/DD/YYYY') + "</option>");
                   }
                   $ddlPCSPayDates.selectpicker('refresh');
               });
}
function PCSSearch() {
    $('#dvPCSMasterGridHolder').css("visibility", "hidden");

    try {
        selectedPCSSuperGroup = [];
        selectedPCSSuperGroupIds = [];
        selectedPayDates = "";
        var selectedPayeeValues = [];
        var selectedPaymentValues = [];
        var selectedContractValues = [];
        

        $("#ddlPCSPayees :selected").each(function () {
            var selectedPayeeValuesArray = $(this).attr('value').split("_");
            $.each(selectedPayeeValuesArray, function (i) {
                selectedPayeeValues.push(selectedPayeeValuesArray[i]);
            });
        });

        $("#ddlPCSPaymentTypes :selected").each(function () {
            selectedPaymentValues.push($(this).attr('value'));
        });

        $("#ddlPCSContractNumbers :selected").each(function () {
            selectedContractValues.push($(this).attr('value'));
        });

        if ($("#ddlPCSPayDates :selected").attr('value') !== '') {
            selectedPayDates = $("#ddlPCSPayDates :selected").attr('value');
        }

        // Start Loading... Animation
        $('#divLoadingAnimation').css("display", "block");
        EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
        eami.service.approvalService.getPCSSearchResults($ddlPCSSystems.val(), selectedPayeeValues, selectedPaymentValues, selectedContractValues, selectedPayDates)
            .done(function (data) {
                $('#dvPCSMasterGridHolder').css("visibility", "hidden");
                if (data != null) {                    
                    $('#dvPCSMasterGridHolder').css("visibility", "visible");
                        $('#dvPCSMasterGridHolder').html(data);
                        if (isPayDateSearch) {
                            $('#dvPayButtons').prop('disabled', false);
                            $('#PCS_Approve_Popover').popover('disable');
                        }
                        else {
                            $('#dvPayButtons').prop('disabled', true);
                            $('#PCS_Approve_Popover').popover('enable');
                        }
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

function PCSReset() {
    selectedPCSSuperGroup = [];
    selectedPCSSuperGroupIds = [];
    $('#dvPayButtons').prop('disabled', true);

    eami.utility.enableDisableDDL($ddlPCSPayees, false);    
    eami.utility.enableDisableDDL($ddlPCSPaymentTypes, false);
    eami.utility.enableDisableDDL($ddlPCSContractNumbers, false);

    $ddlPCSPayDates.val('0').change();
    getPayDates();
    eami.utility.enableDisableDDL($ddlPCSPayDates, true);
    
    $ddlPCSSystems.val('1').change();      
    eami.utility.enableDisableDDL($ddlPCSSystems, true);
    return false;
}

function PCSClearOtherSearchBar(isPayDateSearch) {
    if (isPayDateSearch)
    {
        $ddlPCSSystems.val('1').change();
        eami.utility.enableDisableDDL($ddlPCSSystems, true);
        $ddlPCSPayees.val('0').change();
        eami.utility.enableDisableDDL($ddlPCSPayees, true);
        $ddlPCSPaymentTypes.val('0').change();
        eami.utility.enableDisableDDL($ddlPCSPaymentTypes, false);
        $ddlPCSContractNumbers.val('0').change();
        eami.utility.enableDisableDDL($ddlPCSContractNumbers, false);
    }
    else
    {
        $ddlPCSPayDates.val('0').change();
        eami.utility.enableDisableDDL($ddlPCSPayDates, true);
    }
    return false;
}
