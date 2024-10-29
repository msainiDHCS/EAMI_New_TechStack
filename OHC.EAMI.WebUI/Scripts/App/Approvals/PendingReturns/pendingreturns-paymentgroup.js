
var $formPRApprove = $("#frmPRApprove");
$formPRApprove.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        txtApproveComment: {
            required: true,
            normalizer: function (value) {
                return $.trim(value);
            }
        },
        chkAcknowledge: {
            required: true,
        },
    },
    messages: {
        txtApproveComment: {
            required: 'Please enter a comment',
        },
        chkAcknowledge: {
            required: '*',
        },
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    success: function (label, element) {
        label.parent().removeClass('error');
        label.remove();
    },
});

var $formPRDeny = $("#frmPRDeny");
$formPRDeny.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        txtDenyComment: {
            required: true,
            normalizer: function (value) {
                return $.trim(value);
            }
        },
    },
    messages: {
        txtDenyComment: {
            required: 'Please enter a comment',
        },
    },
    errorPlacement: function (error, element) {
        error.insertAfter(element);
    },
    success: function (label, element) {
        label.parent().removeClass('error');
        label.remove();
    },
});

$('#txtDenyComment, #txtPreviousComment, #txtApproveComment').on('keypress', function (event) {
    var value = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    if (!eami.utility.checkInputValidity(value)) {
        event.preventDefault();
    }
});


$(document).ready(function () {

    $(function () {
        $('[data-toggle="popover"]').popover();
    });

    //set datetime format for the grid
    $.fn.dataTable.moment('MM/DD/YYYY');

    //fix for window resize error for datatable in IE
    $(window).unbind("resize.DT-" + gridName);

    var table = $('#' + gridName).DataTable(
        {
            "destroy": true,    // unbinds previous datatable initialization binding
            "searching": false,
            processing: false,
            bPaginate: false,
            bInfo: false,
            "order": [[3, "asc"]],
            "columnDefs": [
                {
                    "targets": [1, 3, 4, 5],
                    "orderable": true,
                },
                {
                    "targets": [0, 2, 7, 8],
                    "orderable": false,
                },
                {
                    type: 'date', targets: [4]
                },
                {
                    "type": "eami_currency",
                    "targets": 6,
                    "orderable": true
                }
            ],
        });

    $(table.table().container()).addClass('no-padding');

    //calculate totals for all rows
    var localamount = 0;

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
            openPaymentRecord(row, tr, dataid, idOfParentDiv, topGroupID, topGroupName);
        }
    });

    function openPaymentRecord(row, tr, dataid, idOfParentDiv, topGroupID, topGroupName) {        
        try {                    
            // Start Loading... Animation
            $('#divLoadingAnimation').css("display", "block");
            EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');
            eami.service.approvalService.getPRPaymentRecord(dataid, topGroupID, topGroupName)
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

});


function copyPreviousNote() {
    $('#txtApproveComment').val('');
    $('#txtApproveComment').val($('#txtPreviousComment').val());
    return false;
}
function showPRApproveModal(paymentSetNumber, previousNote) {
    $("#dvPRApproveErrorMessage").empty();
    $('#dvPRApproveStatus').hide();
    $('#lbPRApprovePaymentSetId').empty();
    $('#hdnPaymentSetNumberApprove').val('');
    $('#txtPreviousComment').val('');
    $('#txtApproveComment').val('');
    $('#chkAcknowledge').prop('checked', false);
    if (paymentSetNumber != '') {
        $('#txtPreviousComment').val(eami.utility.replaceSpecialChars(previousNote));
        $('#hdnPaymentSetNumberApprove').val(paymentSetNumber);
        $('#lbPRApprovePaymentSetId').append(paymentSetNumber);
        $('#dvPRApproveModal').modal('show');
    }
}

function confirmPRApprove() {
    var statusMessage = '';
    var status = false;
    $("#dvPRApproveErrorMessage").empty();
    $('#dvPRApproveStatus').hide();

    if ($formPRApprove.valid()) {
        eami.service.approvalService.approvePR($('#hdnPaymentSetNumberApprove').val(), $('#txtApproveComment').val())
                .done(function (data) {
                    if (!data.status) {
                        if (data.returnedData && data.returnedData.length > 0) {
                            $.each(data.returnedData, function (index, message) {
                                statusMessage += "<p>" + message + "</p>";
                            });
                        }
                        else {
                            statusMessage += "<p> An error occured.</p>";
                        }
                        $("#dvPRApproveErrorMessage").append(statusMessage);
                        $("#dvPRApproveStatus").show();
                    }
                    else {
                        $('#dvPRApproveModal').modal('hide'); status = true;
                    }
                });
    }

    if (status) {
        setTimeout(function () { loadIPTabs(1); }, 1000);
    }
}

function showPRDenyModal(paymentSetNumber) {
    $("#dvPRDenyErrorMessage").empty();
    $('#dvPRDenyStatus').hide();
    $('#lbPRDenyPaymentSetId').empty()
    $('#txtDenyComment').val('');
    $('#hdnPaymentSetNumberDeny').val('');
    if (paymentSetNumber != '') {
        $('#lbPRDenyPaymentSetId').append(paymentSetNumber);
        $('#hdnPaymentSetNumberDeny').val(paymentSetNumber);
        $("#dvPRDenyModal").modal();
    }
}

function confirmPRDeny() {
    var statusMessage = '';
    var status = false;
    $("#dvPRDenyErrorMessage").empty();
    $('#dvPRDenyStatus').hide();

    if ($formPRDeny.valid()) {
        eami.service.approvalService.denyPR($('#hdnPaymentSetNumberDeny').val(), $('#txtDenyComment').val())
                .done(function (data) {
                    if (!data.status) {
                        if (data.returnedData && data.returnedData.length > 0) {
                            $.each(data.returnedData, function (index, message) {
                                statusMessage += "<p>" + message + "</p>";
                            });
                        }
                        else {
                            statusMessage += "<p> An error occured.</p>";
                        }
                        $("#dvPRDenyErrorMessage").append(statusMessage);
                        $("#dvPRDenyStatus").show();
                    }
                    else {
                        $('#dvPRDenyModal').modal('hide'); status = true;
                    }
                });
    }

    if (status) {
        setTimeout(function() {
            loadIPTabs(1);
        }, 1000);
    }
}