$(document).ready(function () {
    //set datetime format for the grid
    $.fn.dataTable.moment('MM/DD/YYYY');
    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + grdPRNameThird);


    var table = $('#' + grdPRNameThird).DataTable(
        {
            "searching": false,
            processing: false,
            bPaginate: false,
            bInfo: false,
            "order": [[3, "asc"], [1, "asc"]],
            "columnDefs": [
                {
                    "targets": [1, 2, 3],
                    "orderable": true,
                },
                {
                    "targets": [0, 4, 5],
                    "orderable": false,
                },
                {
                    type: 'date', targets: [2]
                },
                {
                    "type": "eami_currency",
                    "targets": 3,
                    "orderable": true
                }
            ]
        });
    $(table.table().container()).addClass('no-padding');
});
function fdModal(prid, prnumber, paymentSetNumber, paymentRecordId) {

    var returnHtml = '';
    try {
        $('#lblModalFunding_A_PCS').empty();
        $('#lblModalFunding_A_PCS').append(prnumber);

        eami.service.approvalService.getPCSFunding(paymentRecordId, prid, paymentSetNumber)
            .done(function (data) {
                if (data != null) {
                    if (typeof (data) == "string"
                        && data.includes('An error occured while processing your request')) {
                        $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
                        $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
                        showErrorOnWholePage(data);
                    }
                    else {
                        $('#modalBodyFundingDetail_A_PCS').html(data);
                        $('#dvFundingDetailModal_A_PCS').modal('show');
                    }
                }
            });
    }
    catch (e) { }

    return false;
}