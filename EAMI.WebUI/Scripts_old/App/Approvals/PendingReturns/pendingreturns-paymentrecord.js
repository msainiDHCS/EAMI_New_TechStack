$(document).ready(function () {   
    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + grdPRNameThird);
    //alert($('#' + grdPRName));
    var paymentRecordTable = $('#' + grdPRNameThird).DataTable(
        {
            "searching": false,
            processing: false,
            bPaginate: false,
            bInfo: false,
            "order": [[3, "asc"],[1, "asc"]],
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
    $(paymentRecordTable.table().container()).addClass('no-padding');
});

function showFundingModal(prid, prnumber, paymentSetNumber, topGroupId) {
    var returnHtml = '';
    try {
        $('#lblFundingModal_A_PR').empty();
        $('#lblFundingModal_A_PR').append(prnumber);
        eami.service.approvalService.getPRFunding(prid, topGroupId, paymentSetNumber)
            .done(function (data) {
                if (data != null) {
                    if (typeof (data) == "string"
                        && data.includes('An error occured while processing your request')) {
                        $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
                        $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
                        showErrorOnWholePage(data);
                    }
                    else {
                        $('#modalBodyFundingDetail_A_PR').html(data);
                        $('#dvFundingDetailModal_A_PR').modal('show');
                    }
                }

            });
    }
    catch (e) { }

    return false;
}