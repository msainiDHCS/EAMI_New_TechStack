
$(document).ready(function () {

    $(function () {
        $('[data-toggle="popover"]').popover();
    });


    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + gridName);

    var table = $('#' + gridName).DataTable(
        {
            "destroy": true,    // unbinds previous datatable initialization binding
            "searching": false,
            processing: false,
            bPaginate: false,
            bInfo: false,
            "order": [[4, "asc"]],
            "columnDefs": [
                {
                    "targets": [1, 4, 5],
                    "orderable": true,
                },
                {
                    "targets": [0, 2, 3],
                    "orderable": false,
                },
                {
                    type: 'date', targets: [5]
                },
                {
                    "type": "eami_currency",
                    "targets": [6],
                    "orderable": true
                }
            ],
        });

    $(table.table().container()).addClass('no-padding');

    //calculate totals for all rows
    var localamount = 0;
    $('#' + gridName + ' tbody tr td').find('.rowselectors').each(function () {
        var adjAmount = $(this).parent().siblings('.RowAmount').html();
        localamount = localamount + numeral(adjAmount).value();
    });

    //update parent row
    var parentGridName = 'grdPRsByPayeeAndPaymentType';
    $('#' + parentGridName).find("tr[data-id='" + '@ViewBag.ID' + "']").find('.ParentAmount').text(numeral(localamount).format('$0,0.00'));


    //add child payment records grid

    // Add event listener for opening and closing details
    $('#' + gridName + ' tbody tr td').on('click', 'button.btnPRSet', function () {
        if ($(this).find("span").attr("class") == 'glyphicon-plus') {
            $(this).find("span").toggleClass('glyphicon-plus');
            $(this).find("span").toggleClass('glyphicon-minus');
        }
        else {
            $(this).find("span").toggleClass('glyphicon-minus');
            $(this).find("span").toggleClass('glyphicon-plus');
        }

        //get td data-id
        var dataid = $(this).attr('id');
        var tableName = $(this).parent().parent().parent().parent().attr("id");
        var topGroupID = $(this).parent().parent().parent().parent().attr("data-id");
        var tr = $('#' + tableName).find("tr[data-id='" + dataid + "']");
        var row = table.row(tr);
        var idOfParentDiv = 'dv_' + $(tr).attr('id');
        var topGroupName = $('#' + tableName).attr('data-top-groupname');
        if (tr.hasClass('shown')) {
            // This row is already open - close it
            //The delay(0) forces child table to fully load and initialize complete before sliding up.
            $('div.slider', row.child()).delay(0).slideUp(250);
            tr.removeClass('shown');
        }
        else {
            // Open this row
            format3(row, tr, dataid, idOfParentDiv, topGroupID, topGroupName);
        }
    });

    function format3(row, tr, dataid, idOfParentDiv, topGroupID, topGroupName) {
        var returnHtml = '';
        try {
            // Start Loading... Animation
            $('#divLoadingAnimation').css("display", "block");
            EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
            $.ajax({
                url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingPaymentSetDetails?paymentRecordSetNumber=' + dataid + "&topGroupID=" + topGroupID + "&paymentGroupName=" + topGroupName),
                type: 'GET',
                datatype: "html",
                cache: false,
                //async: false,
                success: function (data) {
                    if (data != null) {
                        row.child("<div class='slider' id=" + idOfParentDiv + ">" + data + "</div>", 'no-padding').show();
                        //The delay(0) forces child table to fully load and initialize complete before sliding down.
                        $('div.slider', row.child()).delay(0).slideDown(250);
                        tr.addClass('shown');
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

        return returnHtml;
    }

});

function createPaymentSelectionCookie() {
    var paymentRecordCookieValue = [];
    var stringifyArray = JSON.stringify(paymentRecordCookieValue);
    //$('#hdnPaymentRecordSetSelection').val('');
    if ($('#hdnPaymentRecordSetSelection').val() == '[]' || $('#hdnPaymentRecordSetSelection').val() == '') {
        $('#hdnPaymentRecordSetSelection').val(stringifyArray);
    }
    //var paymentRecordCookie = Cookies.get('ckPaymentRecordSetSelection');
    //if (!paymentRecordCookie) {
    //    Cookies.set('ckPaymentRecordSetSelection', stringifyArray, { expires: 1 });
    //}
}

function AddRemovePaymentSelection(action, element) {
    var paymentRecordCookieValue = [];
    var $hdnPaymentRecordSetSelection = $('#hdnPaymentRecordSetSelection');
    var $hdnRowSpecificPaymentRecordHighAmount = element.closest('td').find('.hdnRowSpecificPaymentRecordHighAmount')
    var recordValue = element.closest('td').next('td').find('span.PRSNumber').html();

    //var recordAmount = element.closest('td').next('td');
    if (action == "add") {
        var setAmount = element.closest('tr').find('td.PP_PG_Amount').html();
        setAmount = setAmount.replace(/\$/g, '');
        setAmount = setAmount.replace(/,/g, '');

        if (Number(setAmount) > 99999999.99) {
            $hdnRowSpecificPaymentRecordHighAmount.val(1);
        }

        //Access and Store in Cookie
        //paymentRecordCookieValue = JSON.parse(Cookies.get('ckPaymentRecordSetSelection'));
        paymentRecordCookieValue = JSON.parse($hdnPaymentRecordSetSelection.val());

        if (paymentRecordCookieValue.indexOf(recordValue) < 0) {
            paymentRecordCookieValue.push(recordValue);
        }

        var holdTdChilren = element.closest('td').next('td').find('.wrapperOnHold');
        if (holdTdChilren.length == 1) {
            if ($.inArray(recordValue, holdArray) < 0) {
                holdArray.push(recordValue);
            }
        }
        else {            
            if ($.inArray(recordValue, unholdArray) < 0) {
                unholdArray.push(recordValue);
            }
        }
    }

    else if (action = "remove") {
        //Access and Store in Cookie            
        //paymentRecordCookieValue = JSON.parse(Cookies.get('ckPaymentRecordSetSelection'));
        paymentRecordCookieValue = JSON.parse($hdnPaymentRecordSetSelection.val());
        var index = paymentRecordCookieValue.indexOf(recordValue);
        if (index > -1) {
            paymentRecordCookieValue.splice(index, 1);
        }

        var setAmount = element.closest('tr').find('td.PP_PG_Amount').html();
        setAmount = setAmount.replace(/\$/g, '');
        setAmount = setAmount.replace(/,/g, '');

        if (Number(setAmount) > 99999999.99) {
            $hdnRowSpecificPaymentRecordHighAmount.val(0);
        }
       
        var holdTdChilren = element.closest('td').next('td').find('.wrapperOnHold');
        if (holdTdChilren.length == 1) {
            var index = $.inArray(recordValue, holdArray);
            if (index > -1) {
                holdArray.splice(index, 1);
            }
        }
        else {
            var index = $.inArray(recordValue, unholdArray);
            if (index > -1) {
                unholdArray.splice(index, 1);
            }
        }
    }

    $hdnPaymentRecordSetSelection.val(JSON.stringify(paymentRecordCookieValue));
    //Cookies.set("ckPaymentRecordSetSelection", JSON.stringify(paymentRecordCookieValue));
}

function DetermineCheckedTotal(gridcheckboxcolumn, parentGroupID) {

    //Create / Check Payment Selection Field
    createPaymentSelectionCookie();

    var parentGridName = 'grdPRsByPayeeAndPaymentType';

    var contextGridName = $(gridcheckboxcolumn).attr('name').replace('_PaymentRecordSetRow', '');
    var parentAllCheckbox = contextGridName + '_All';
    var spanAmount = $('#' + parentGridName).find("tr[data-id='" + parentGroupID + "']").find('.ParentAmount');
    var localamount = 0;
    var AreAllChildCheckboxesChecked = true;

    $('#' + contextGridName).find('.rowselectors').each(function () {

        if ($(this).prop('checked')) {
            var adjAmount = $(this).parent().siblings('.RowAmount').html();
            localamount = localamount + numeral(adjAmount).value();
            AddRemovePaymentSelection("add", $(this));
        }
        else {
            AddRemovePaymentSelection("remove", $(this));
            AreAllChildCheckboxesChecked = false;
        }

    });

    $('#' + contextGridName).find('.AllCheckBox').prop('checked', AreAllChildCheckboxesChecked);

    spanAmount.text(numeral(localamount).format('$0,0.00'));
    if (Number(localamount) > 0) {
        spanAmount.removeClass('negative-amount');
    }
    else if (Number(localamount) <= 0) {
        spanAmount.addClass('negative-amount');
    }

    return true;
}

function SetAllGridCheckboxes(selectallcheckboxname, parentGroupID) {

    //Create / Check Payment Selection Cookie
    createPaymentSelectionCookie();

    var selectvallValue = $(selectallcheckboxname).prop('checked');
    var contextGridName = $(selectallcheckboxname).attr('name').replace('_All', '');
    var parentGridName = 'grdPRsByPayeeAndPaymentType';
    var spanAmount = $('#' + parentGridName).find("tr[data-id='" + parentGroupID + "']").find('.ParentAmount');
    $('#' + contextGridName).find('.rowselectors').prop('checked', selectvallValue);

    var localamount = 0;

    if (selectvallValue == true) {
        $('#' + contextGridName).find('.rowselectors').each(function () {
            var adjAmount = $(this).parent().siblings('.RowAmount').html();
            localamount = localamount + numeral(adjAmount).value();
            AddRemovePaymentSelection("add", $(this));
        });
    }
    else {
        $('#' + contextGridName).find('.rowselectors').each(function () {
            AddRemovePaymentSelection("remove", $(this));
        });
    }

    spanAmount.text(numeral(localamount).format('$0,0.00'));
    if (Number(localamount) > 0) {
        spanAmount.removeClass('negative-amount');
    }
    else if (Number(localamount) <= 0) {
        spanAmount.addClass('negative-amount');
    }

    return true;
}