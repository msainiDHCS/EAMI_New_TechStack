function fdModal(prid, prnumber, paymentSetNumber, topGroupId) {
    var returnHtml = '';
    try {
        $('#lblFundingModal_PP_A').empty();
        $('#lblFundingModal_PP_A').append(prnumber);
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/InvoiceProcessingFundingDetails?paymentRecordID=' + prid + "&topGroupID=" + topGroupId +
                '&ParentPaymentRecordSetNumber=' + paymentSetNumber),
            type: 'GET',
            datatype: "html",
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
                        $('#modalBodyFundingDetail_PP_A').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                        $('#dvFundingDetailModal_PP_A').modal('show');
                    }
                }
            }
        });
    }
    catch (e) { }

    return false;
}

$(document).ready(function () {           
    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + grdPRNameThird);
    console.log(grdPRNameThird);

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
