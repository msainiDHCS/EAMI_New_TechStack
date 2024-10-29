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
                    "targets": [2, 3, 4, 5],
                    "orderable": true,
                },
                {
                    "targets": [0, 1],
                    "orderable": false,
                },
                {
                    type: 'date', targets: [3]
                },
                {
                    "type": "eami_currency",
                    "targets": 5,
                    "orderable": true
                }
            ]
        });

    $(table.table().container()).addClass('no-padding');
    $('#' + gridName + ' tbody tr td').on('click', 'button.btnPRSet', function () {
        if ($(this).find("span").attr("class") == 'glyphicon-plus') {
            $(this).find("span").toggleClass('glyphicon-plus');
            $(this).find("span").toggleClass('glyphicon-minus');
        }
        else {
            $(this).find("span").toggleClass('glyphicon-minus');
            $(this).find("span").toggleClass('glyphicon-plus');
        }
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
        }
        else {
            // Open this row
            getPaymentSetRecordChildRows(row, tr, idOfParentDiv, csUniqueNumber, paymentSetRecordNumber, payemntGroupId, paymentGroupName);
        }
    });

    function getPaymentSetRecordChildRows(row, tr, idOfParentDiv, csUniqueNumber, paymentSetRecordNumber, payemntGroupId, paymentGroupName) {
        
        try {
            // Start Loading... Animation
            $('#divLoadingAnimation').css("display", "block");
            EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
            eami.service.approvalService.getPCSPaymentRecord(csUniqueNumber, paymentSetRecordNumber, payemntGroupId, paymentGroupName)
                .done(function (data) {
                    if (data != null) {
                        row.child("<div class='slider' id=" + idOfParentDiv + ">" + data + "</div>", 'no-padding').show();
                        //The delay(0) forces child table to fully load and initialize complete before sliding down.
                        $('div.slider', row.child()).delay(0).slideDown(250);
                        tr.addClass('shown');
                    }
                })
                .always(function (data) {
                    $('#divLoadingAnimation_Inner').empty();
                    $('#divLoadingAnimation').css("display", "none");
                });
        }
        catch (e)
        { }       
    }

    $('[data-toggle="popover"]').popover();
    $.fn.dataTable.moment('MM/DD/YYYY');
    $(window).unbind("resize.DT-" + gridName);
});
