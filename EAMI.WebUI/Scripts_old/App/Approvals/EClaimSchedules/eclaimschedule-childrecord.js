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
                    "targets": [1, 2, 3, 4, 5, 6, 7, 8],
                    "orderable": true,
                },
                {
                    "targets": [0],
                    "orderable": false,
                },
                {
                    type: 'date', targets: [6]
                },
                {
                    "type": "eami_currency",
                    "targets": 8,
                    "orderable": true
                }
            ]
        });

    $(table.table().container()).addClass('no-padding');

    $.fn.dataTable.moment('MM/DD/YYYY hh:mm A');
    $(window).unbind("resize.DT-" +  gridName);
    $('[data-toggle="popover"]').popover();
});

$('#' + gridName + ' tbody tr td').on('click', 'button.btnRemittanceAdvice', function () {
    var csId = $(this).siblings('#hdnCSPrimaryKeyId:first').val();
    $.ajax({
        url: getEAMIAbsoluteUrl('~/Approvals/ECSRemittanceAdvice'),
        type: 'POST',
        data: { 'csID': csId },
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
                    $('#modalBodyRemittanceAdvice_A_ECS').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                    $('#dvRemittanceModal_A_ECS').find('#txtOptionalComments').prop("disabled", "disabled");
                    $('#dvRemittanceModal_A_ECS').modal('show');
                }
               
            }
        }
    });

});