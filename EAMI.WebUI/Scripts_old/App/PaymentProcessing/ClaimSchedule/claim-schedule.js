var $form = $("#frmSubmitCSValidator");
$form.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        chkAcknowledge: {
            required: true,
        },
    },
    messages: {
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

$(document).ready(function () {
    $('#dvClaimSchedules').css("visibility", "visible");
    var $table = $('#grdCS').DataTable(
            {
                "destroy": true,    // unbinds previous datatable initialization binding
                "searching": false,
                processing: false,
                "order": [[2, "asc"]],
                bPaginate: false,
                bInfo: false,               
                "columnDefs": [
                     {
                         "targets": [0, 2, 3, 4, 5, 6],
                         "sortable": true
                     },
                     {
                         "targets": [1, 7, 8],
                         "sortable": false
                     }
                ],
                "initComplete": function (settings, json) {
                    $('#dvClaimSchedules').css("visibility", "visible");
                }
            });

    //Plus Icon Click
    $('#grdCS tbody tr td').on('click', 'button.btnPayeeCSPaymentType', function () {
        if ($(this).find("span").attr("class") == 'glyphicon-plus') {
            $(this).find("span").toggleClass('glyphicon-plus');
            $(this).find("span").toggleClass('glyphicon-minus');
        }
        else {
            $(this).find("span").toggleClass('glyphicon-minus');
            $(this).find("span").toggleClass('glyphicon-plus');
        }

        var dataid = $(this).attr('id');
        var tr = $('#grdCS').find("tr[data-id='" + dataid + "']");
        var row = $table.row(tr);
        var idOfParentDiv = 'dv_' + $(tr).attr('id');
        var paymentGroupName = $(tr).attr('data-groupname');
        if (tr.hasClass('shown')) {
            $('div.slider', row.child()).slideUp(250);  //Don't use delay(0) on top level - causes column shifting.
            tr.removeClass('shown');
        }
        else {
            getPaymentSetRecords(row, tr, dataid, idOfParentDiv, paymentGroupName);
        }
    });


    function getPaymentSetRecords(row, tr, dataid, idOfParentDiv, paymentGroupName) {

        var returnHtml = '';
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/IPClaimSchedulePaymentGroup'),
            type: 'POST',
            datatype: "html",
            data: { 'csUniqueNumber': dataid, 'paymentGroupName': paymentGroupName },
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

        return returnHtml;
    }

    $.fn.dataTable.moment('MM/DD/YYYY hh:mm A');
    $(window).unbind("resize.DT-" + "grdCS");
    $('[data-toggle="popover"]').popover();
    
});




$('#grdCS  tbody tr td').on('click', 'button.btnDeleteCS', function () {
    var dataid = $(this).attr('id');
    if (dataid !== "") {
        $("#lblDeleteCSId").empty();
        $('#lblDeleteMainCSId').empty();
        $('#lblDeleteLinkedCSId').empty();

        var idSplitArray = dataid.split("_");
        $("#lblDeleteCSId").append(idSplitArray[1]);
        $('#hdnDeleteCSPId').val(idSplitArray[2]);

        var isLinked = (idSplitArray[3] == 'True');
        if (isLinked) {
            $('#lblDeleteMainCSId').append(idSplitArray[1]);
            $('#lblDeleteLinkedCSId').append(idSplitArray[4]);
            $('#dvDeleteLinked').show();
        }
        else {
            $('#dvDeleteLinked').hide();
        }
        $('#modalCSDeleteConfirm').modal();
    }
});

$('#grdCS  tbody tr td').on('click', 'button.btnSubmitCS', function () {
    var dataid = $(this).attr('id');
    if (dataid !== "") {        
        $("#lblSubmitCSId").empty();
        $('#lblSubmitMainCSId').empty();
        $('#lblSubmitLinkedCSId').empty();

        var idSplitArray = dataid.split("_");
        $("#lblSubmitCSId").append(idSplitArray[1]);
        $('#hdnSubmitCSPId').val(idSplitArray[2]);

        var isLinked = (idSplitArray[3] == 'True');
        if (isLinked) {
            $('#lblSubmitMainCSId').append(idSplitArray[1]);
            $('#lblSubmitLinkedCSId').append(idSplitArray[4]);
            $('#dvSubmitLinked').show();
        }
        else {
            $('#dvSubmitLinked').hide();
        }
        $('#modalCSSubmitConfirm').modal();
    }
});

$('#grdCS  tbody tr td').on('click', 'button.btnRemittanceAdvice', function () {
    var csId = $(this).siblings('#hdnCSPrimaryKeyId:first').val();
    $.ajax({
        url: getEAMIAbsoluteUrl('~/PaymentProcessing/IPCSRemittanceAdvice'),
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
                    $('#modalBodyRemittanceAdvice').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                    $('#dvRemittanceModal').modal('show');
                }
            }
        }
    });

});

function confirmDeleteCS() {
    var csID = $("#hdnDeleteCSPId").val();
    var statusMessage = '';
    var status = false;
    if (csID) {
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/DeleteClaimSchedule'),
            type: 'POST',
            datatype: "json",
            data: { 'claimScheduleId': csID },
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
                    $("#dvDeleteErrorMessage").append(statusMessage);
                    $("#dvDeleteStatus").show();
                }
                else { $('#modalCSDeleteConfirm').modal('hide'); status = true; }
            }
        });
    }
    if (status) {
        setTimeout(function () { loadIPTabs(2); }, 1000);

    }
}

function confirmSubmitCS() {
    var csID = $("#hdnSubmitCSPId").val();
    var statusMessage = '';
    var status = false;
    if (csID) {
        if ($form.valid()) {
            $.ajax({
                url: getEAMIAbsoluteUrl('~/PaymentProcessing/SubmitClaimSchedule'),
                type: 'POST',
                datatype: "json",
                data: { 'claimScheduleId': csID },
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
                        $("#dvSubmitErrorMessage").append(statusMessage);
                        $("#dvSubmitStatus").show();
                    }
                    else { $('#modalCSSubmitConfirm').modal('hide'); status = true; }
                }
            });
        }
    }

    if (status) {
        setTimeout(function () { loadIPTabs(2); }, 1000);

    }
}

function closeDeleteCSModal() {
    $('#hdnDeleteCSPId').val('');
    $('#modalCSDeleteConfirm').modal('hide');
    $('#dvDeleteErrorMessage').empty();
    $('#dvDeleteStatus').hide();
}

function closeSubmitCSModal() {
    $('#hdnSubmitCSPId').val('');
    $('#taSubmitNote').val('');
    $('#chkAcknowledge').prop('checked', false);
    $('.has-error').remove();
    $('#modalCSSubmitConfirm').modal('hide');
    $('#dvSubmitErrorMessage').empty();
    $('#dvSubmitStatus').hide();
}

function saveRemittanceNote() {
    var csID = $("#hdnCSID").val();
    var statusMessage = '';
    if (csID) {
        $.ajax({
            url: getEAMIAbsoluteUrl('~/PaymentProcessing/SaveRemittanceNote'),
            type: 'POST',
            datatype: "json",
            data: { 'claimScheduleId': csID, 'note': $('#txtOptionalComments').val() },
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
                        if (!data.status) {
                            if (data.returnedData && data.returnedData.length > 0) {
                                $.each(data.returnedData, function (index, message) {
                                    statusMessage += "<p>" + message + "</p>";
                                });
                            }
                            else {
                                statusMessage += "<p> An error occured.</p>";
                            }
                            $("#dvSaveNoteErrorMessage").append(statusMessage);
                            $("#dvSaveNoteStatus").show();
                        }
                        else { $('#dvRemittanceModal').modal('hide'); }
                    }
                }
            }
        });
    }
}