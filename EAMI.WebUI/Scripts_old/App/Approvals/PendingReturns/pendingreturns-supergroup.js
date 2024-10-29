
$(document).ready(function () {

    $(function () {
        $('[data-toggle="popover"]').popover();
    });

    //set datetime format for the grid
    $.fn.dataTable.moment('MM/DD/YYYY hh:mm A');

    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + "grdPRsByPayeeAndPaymentType");

    var $superGroupGrid = $('#grdPRsByPayeeAndPaymentType').DataTable(
        {
            "destroy": true,    // unbinds previous datatable initialization binding
            "searching": false,
            processing: false,
            bPaginate: false,
            bInfo: false,
            "order": [[2, "asc"]],
            "columnDefs": [
                 {
                     "targets": [0, 2, 3, 4, 5],
                     "orderable": true
                 },
                 {
                     "targets": [1],
                     "orderable": false
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
        var row = $superGroupGrid.row(tr);// row.attr('id', 'ram');
        var idOfParentDiv = 'dv_' + $(tr).attr('id');

        var groupName = $(tr).attr('data-groupname')
        if (tr.hasClass('shown')) {
            $('div.slider', row.child()).slideUp(250);  //Don't use delay(0) on top level - causes column shifting.
            tr.removeClass('shown');
        }
        else {
            openPaymentGroup(row, tr, dataid, idOfParentDiv, groupName);
        }
    });

    function openPaymentGroup(row, tr, dataid, idOfParentDiv, groupName) {

        var returnHtml = '';
        try {
            eami.service.approvalService.getPRPaymentGroup(dataid, groupName)
                .done(function (data) {
                    if (data != null) {
                        row.child("<div class='slider' id=" + idOfParentDiv + ">" + data + "</div>", 'no-padding').show();
                        $('div.slider', row.child()).slideDown(250);  //Don't use delay(0) on top level - causes column shifting.
                        tr.addClass('shown');
                    }                    
                });
        }
        catch (e)
        { }

        return returnHtml;
    }
});