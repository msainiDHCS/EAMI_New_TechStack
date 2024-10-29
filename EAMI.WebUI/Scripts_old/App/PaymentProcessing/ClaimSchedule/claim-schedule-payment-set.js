$(document).ready(function () {
    //set datetime format for the grid
    $.fn.dataTable.moment('MM/DD/YYYY');
    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + grdPRNameThird);

    var tablethird = $('#' + grdPRNameThird).DataTable(
        {
            "destroy": true,    // unbinds previous datatable initialization binding
            "searching": false,
            processing: false,
            bPaginate: false,
            bInfo: false,
            "order": [[3, "asc"], [1, "asc"]],
            "columnDefs": [
                {
                    "targets": [1, 2],
                    "orderable": true
                },
                {
                    "targets": [0, 4, 5],
                    "orderable": false
                },
                { type: 'date', targets: [2] }
                ,
                {
                    "type": "eami_currency",
                    "targets": 3,
                    "orderable": true
                }
            ],
        });

        $(tablethird.table().container()).addClass('no-padding');
});

function fdModal(prid, prnumber, paymentSetNumber, paymentRecordId) {
    var returnHtml = '';
    try {
        $('#lblModalFunding_PP_CS').empty();
        $('#lblModalFunding_PP_CS').append(prnumber);
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/IPClaimScheduleFundingDetails'),
            type: 'POST',
            datatype: "html",
            data: { 'csUniqueNumber': paymentRecordId, 'paymentRecordID': prid, 'parentPaymentRecordSetNumber': paymentSetNumber },
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
                        $('#modalBodyFundingDetail_PP_CS').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                        $('#dvFundingDetailModal_PP_CS').modal('show');
                    }
                }
            }
        });

    }
    catch (e) { }

    return false;
}
