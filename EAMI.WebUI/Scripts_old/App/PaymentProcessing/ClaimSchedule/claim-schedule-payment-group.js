$(document).ready(function () {    
    var table = $('#' + gridName).DataTable(
        {
            "destroy": true,    // unbinds previous datatable initialization binding
            "searching": false,
            processing: false,
            bPaginate: false,
            bInfo: false,
            "order": [[2, "asc"]],           
            "columnDefs": [
                {
                    "targets": [2, 3, 4],
                    "orderable": true,
                },
                {
                    "targets": [0, 1, 5],
                    "orderable": false,
                },
                {
                    type: 'date', targets: [3]
                },
                {
                    "type": "eami_currency",
                    "targets": 4,
                    "orderable": true
                }
            ]
        });

    $(table.table().container()).addClass('no-padding');

    $('#' + gridName + ' tbody tr td').on('click', 'button.btnPRSet', function () {
        // Having the toggles here did not work for this page for some reason, so toggling after slideup/slidedown instead.
        //if ($(this).find("span").attr("class") == 'glyphicon-plus') {
        //    $(this).find("span").toggleClass('glyphicon-plus');
        //    $(this).find("span").toggleClass('glyphicon-minus');
        //}
        //else {
        //    $(this).find("span").toggleClass('glyphicon-minus');
        //    $(this).find("span").toggleClass('glyphicon-plus');
        //}

        var paymentSetRecordNumber = $(this).attr('id');        
        var tableName = $(this).parent().parent().parent().parent().attr("id");        
        var tr = $('#' + tableName).find("tr[data-id='" + paymentSetRecordNumber + "']");        
        var payemntGroupId = tr.attr('data-top-group-id');
        var paymentGroupName = $('#' + tableName).attr('data-top-groupname');
        var csUniqueNumber = $('#' + tableName).attr('data-cs-unique-number');
        var row = table.row(tr);
        var idOfParentDiv = 'dv_' + $(tr).attr('id');        
        if (tr.hasClass('shown')) {
            // This row is already open - close it
            //The delay(0) forces child table to fully load and initialize complete before sliding up.
            $('div.slider', row.child()).delay(0).slideUp(250);
            tr.removeClass('shown');
            $(this).find("span").removeClass('glyphicon-minus');
            $(this).find("span").addClass('glyphicon-plus');
        }
        else {
            // Open this row
            getPaymentSetRecordChildRows(row, tr, idOfParentDiv, csUniqueNumber, paymentSetRecordNumber, payemntGroupId, paymentGroupName, $(this));
        }
    });

    function getPaymentSetRecordChildRows(row, tr, idOfParentDiv, csUniqueNumber, paymentSetRecordNumber, payemntGroupId, paymentGroupName, $this) {

        var returnHtml = '';
        try {
            // Start Loading... Animation
            $('#divLoadingAnimation').css("display", "block");
            EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
            $.ajax({
                url: getEAMIAbsoluteUrl('~/PaymentProcessing/IPClaimSchedulePaymentSetDetails'),
                type: 'POST',
                datatype: "html",
                data: { 'csUniqueNumber': csUniqueNumber, 'paymentRecordSetNumber': paymentSetRecordNumber, 'payemntGroupId': payemntGroupId, 'paymentGroupName': paymentGroupName },
                cache: false,
                //async: false,
                success: function (data) {
                    if (data != null) {
                        row.child("<div class='slider' id=" + idOfParentDiv + ">" + data + "</div>", 'no-padding').show();
                        //The delay(0) forces child table to fully load and initialize complete before sliding down.
                        $('div.slider', row.child()).delay(0).slideDown(250);
                        tr.addClass('shown');
                        $this.find("span").removeClass('glyphicon-plus');
                        $this.find("span").addClass('glyphicon-minus');
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

    $('[data-toggle="popover"]').popover();
    $.fn.dataTable.moment('MM/DD/YYYY');
    $(window).unbind("resize.DT-" + gridName);
});
function removePaymentRecord(csId, csPId, paymentRecordSetNumber, count, linkedFlag, linkedSets) {    
    $('#spanSingleRemove').hide();
    $('#spanMultiRemove').hide();
    $('#lblPaymentRecordSet').empty();
    $('#lblPaymentRecordSet').append(paymentRecordSetNumber);
    $('#lblSingleDeleteMainCSId').empty();
    $('#lblSingleDeleteLinkedCSId').empty();
    $('#lblCSId').empty();
    $('#lblCSId').append(csId);
    $('#hdnCSPId').val(csPId);
    $('#hdnPaymentRecordSet').val(paymentRecordSetNumber);

    var isLinked = (linkedFlag == 'True');
    if (isLinked) {
        $('#lblSingleDeleteMainCSId').append(csId);
        $('#lblSingleDeleteLinkedCSId').append(linkedSets);
        $('#dvPSDeleteLinked').show();
    }
    else {
        $('#dvPSDeleteLinked').hide();
    }

    if (count == '1') {
        $('#spanSingleRemove').show();
    }
    else {
        $('#spanMultiRemove').show();
    }
    $('#modalRemovePaymentConfirm').modal();   
}

function confirmRemovePaymentRecordSet() {
    var csID = $("#hdnCSPId").val();
    var paymentSetId = $('#hdnPaymentRecordSet').val();

    var statusMessage = '';
    var status = false;
    if (csID && paymentSetId) {
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/RemovePaymentRecordFromClaimSchedule'),
            type: 'POST',
            datatype: "json",
            data: { 'claimScheduleId': csID, 'paymentRecordSetNumber': paymentSetId },
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
                    $("#dvErrorMessage").append(statusMessage);
                    $("#dvStatus").show();
                }
                else { $('#modalRemovePaymentConfirm').modal('hide'); status = true; }
            }
        });
    }

    if (status) {
        setTimeout(function () { loadIPTabs(2); }, 1000);

    }
}

function closePaymentRemoveModal() {
    $('#hdnCSPId').val('');
    $('#hdnPaymentRecordSet').val('');
    $('#modalRemovePaymentConfirm').modal('hide');
    $('#dvErrorMessage').empty();
    $('#dvStatus').hide();
}
