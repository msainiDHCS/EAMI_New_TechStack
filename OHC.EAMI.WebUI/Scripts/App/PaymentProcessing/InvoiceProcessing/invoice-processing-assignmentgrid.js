
$(document).ready(function () {
    $(function () {
        $('[data-toggle="popover"]').popover();
    });

    //set datetime format for the grid
    $.fn.dataTable.moment('MM/DD/YYYY hh:mm A');

    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + "grdPRsByPayeeAndPaymentType");

    var table1 = $('#grdPRsByPayeeAndPaymentType').DataTable(
    {
            "destroy": true,    // unbinds previous datatable initialization binding
            "searching": false,
            processing: false,
            "order": [[3, "asc"]],
            bPaginate: false,
            bInfo: false,
            "pagingType": "full_numbers",
            "language": {                    //Custom Message Setting
                "lengthMenu": "Display _MENU_ records per page",    //Customizing menu Text
                "zeroRecords": "Nothing found",             //Customizing zero record text - filtered
                "info": "Showing page _PAGE_ of _PAGES_",           //Customizing showing record no
                "infoEmpty": "No records available",                //Customizing zero record message - base
                "infoFiltered": "(filtered from _MAX_ total records)"   //Customizing filtered message
            },
            "columnDefs": [
                 {
                     "targets": [2],
                     "sortable": false
                 },
                 {
                     "targets": [0,1,3,4,5,6,7],
                     "sortable": true
                 }
            ],
            "initComplete": function (settings, json) {
                $('#dvMasterGridHolder').css("visibility", "visible");
            }
    });

    // Add event listener for opening and closing details
    $('#grdPRsByPayeeAndPaymentType tbody tr td').on('click', 'button.btnPayeePaymentType', function () {
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
        var tr = $('#grdPRsByPayeeAndPaymentType').find("tr[data-id='" + dataid + "']");
        var row = table1.row(tr);// row.attr('id', 'ram');
        var idOfParentDiv = 'dv_' + $(tr).attr('id');

        var contextualGroupName = $(tr).attr('data-groupname');
        
        if (tr.hasClass('shown')) {
            // This row is already open - close it
            $('div.slider', row.child()).slideUp(250);  //Don't use delay(0) on top level - causes column shifting.
            tr.removeClass('shown');
        }
        else {
            // Open this row
            format2(row, tr, dataid, idOfParentDiv, contextualGroupName);
        }
    });

    function format2(row, tr, dataid, idOfParentDiv, contextualGroupName) {
        var returnHtml = '';
        try {

            $.ajax({
                url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingAssignmentChild?id=' + dataid + "&paymentGroupName=" + contextualGroupName),
                type: 'GET',
                datatype: "html",
                cache: false,
                async: false,
                success: function (data) {
                    if (data != null) {
                        row.child("<div class='slider' id=" + idOfParentDiv + ">" + data + "</div>", 'no-padding').show();
                        //The delay(0) forces child table to fully load and initialize complete before sliding down.
                        //SlideDown use to be 250 but experienced issues with some checkboxes on bottom of list not selectable.  That is,
                        //if slide down not complete before user begins to scroll down, then interferes with some checkboxes being hoverable
                        //so sped up to 0 to fix -- ensures slide down complete before user has chance to scroll down.
                        $('div.slider', row.child()).slideDown(0);  //Don't use delay(0) on top level - causes column shifting.
                        tr.addClass('shown');
                    }
                }
            });

        }
        catch (e)
        { }

        return returnHtml;
    }
});

//function createClaimSchedule() {
//    var status = AssignPRSetsToClaimSchedule();
//    console.log(status);
//    if (status === true) {
//        $('#dvAddToClaimScheduleModal').modal('hide');
//    }
//}