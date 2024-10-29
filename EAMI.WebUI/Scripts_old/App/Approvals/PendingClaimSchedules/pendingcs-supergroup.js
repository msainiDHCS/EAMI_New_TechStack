var $formDeny = $("#frmDenyCSValidator");
var $formApprove = $('#frmApproveCSValidator');
var hasLinkedCS = [];

$formDeny.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        txtDenyCS: {
            required: true,
            normalizer: function (value) {
                return $.trim(value);
            }
        },
    },
    messages: {
        txtDenyCS: {
            required: 'Comment is required',
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

$formApprove.validate({
    errorElement: 'span',
    ignore: "",
    rules: {
        chkPaydateWarn: {
            required: '#chkShowWarning:checked',
        },
    },
    messages: {
        chkPaydateWarn: {
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


$('#txtDenyCS').on('keypress', function (event) {
    var value = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    if (!eami.utility.checkInputValidity(value)) {
        event.preventDefault();
    }
});

$(document).ready(function () {
    var $table = $('#grdCS').DataTable(
            {
                "destroy": true,    // unbinds previous datatable initialization binding
                "searching": false,
                processing: false,
                "order": [[3, "asc"]],
                bPaginate: false,
                bInfo: false,
                "columnDefs": [
                     {
                         "targets": [0, 3, 4, 5, 6, 7],
                         "sortable": true
                     },
                     {
                         "targets": [1, 2, 8],
                         "sortable": false
                     },
                ],
                "initComplete": function (settings, json) {
                    $('#dvPCSMasterGridHolder').css("visibility", "visible");
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
        try {
            eami.service.approvalService.getPCSPaymentGroup(dataid, paymentGroupName).done(function (data) {
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

    //Select All Checkboxes
    if (isPayDateSearch) {
        $('.chkPCS').prop('checked', true);
        selectAllChkPCS();
    }
    else {
        $('.chkPCS').prop('checked', false);
        selectAllChkPCS();
    }
    $.fn.dataTable.moment('MM/DD/YYYY hh:mm A');
    $(window).unbind("resize.DT-" + "grdCS");
    $('[data-toggle="popover"]').popover();

});




$('#grdCS  tbody tr td').on('click', 'button.btnDeleteCS', function () {
    var dataid = $(this).attr('id');
    if (dataid !== "") {
        $("#lblDeleteCSId").empty();
        $("#lblDenyMainCSId").empty();
        $("#lblDenyLinkedCSId").empty();

        var idSplitArray = dataid.split("_");
        $("#lblDeleteCSId").append(idSplitArray[1]);
        $('#hdnDeleteCSPId').val(idSplitArray[2]);
        var isLinked = (idSplitArray[3] == 'True');
        if (isLinked) {
            $('#lblDenyMainCSId').append(idSplitArray[1]);
            $('#lblDenyLinkedCSId').append(idSplitArray[4]);
            $('#dvDenyLinked').show();
        }
        else {
            $('#dvDenyLinked').hide();
        }

        $('#dvDeleteErrorMessage').empty();
        $('#txtDenyCS').val('');
        $('.has-error').remove();
        $('#dvDeleteStatus').hide();
        $('#modalCSDeleteConfirm').modal();
    }
});

//$('#grdCS  tbody tr td').on('click', 'button.btnSubmitCS', function () {
//    var dataid = $(this).attr('id');
//    if (dataid !== "") {
//        $("#lblSubmitCSId").empty();
//        $('#lblApproveMainCSId').empty();
//        $('#lblApproveLinkedCSId').empty();

//        var idSplitArray = dataid.split("_");        
//        $("#lblSubmitCSId").append(idSplitArray[1]);
//        $('#hdnSubmitCSPId').val(idSplitArray[2]);

//        var payDate = moment(idSplitArray[3]);
//        var currentDate = moment(idSplitArray[4]);
//        var duration = (moment.duration(payDate.diff(currentDate))).asHours();
//        var isLinked = (idSplitArray[5] == 'True');
//        if (isLinked) {
//            $('#lblApproveMainCSId').append(idSplitArray[1]);
//            $('#lblApproveLinkedCSId').append(idSplitArray[6]);
//            $('#dvApproveLinked').show();
//        }
//        else {
//            $('#dvApproveLinked').hide();
//        }
//        if (Number(duration) < 72) {
//            $('#chkShowWarning').prop('checked', true);
//            $('.csPaydateWarn').show();
//        }
//        else {
//            $('#chkShowWarning').prop('checked', false);
//            $('.csPaydateWarn').hide();
//        }

//        $('.has-error').remove();
//        $('#dvSubmitErrorMessage').empty();
//        $('#dvSubmitStatus').hide();
//        $('#modalCSSubmitConfirm').modal();
//    }
//});

$('#grdCS  tbody tr td').on('click', 'button.btnRemittanceAdvice', function () {
    var csId = $(this).siblings('#hdnCSPrimaryKeyId:first').val();
    eami.service.approvalService.getPCSRemittanceAdvice(csId).done(function (data) {
        if (data != null) {
            if (typeof (data) == "string"
                && data.includes('An error occured while processing your request')) {
                $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
                $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
                showErrorOnWholePage(data);
            }
            else {
                $('#modalBodyRemittanceAdvice').html(data);
                $('#dvRemittanceModal').modal('show');
            }
        }
    });
});

function confirmDeleteCS() {
    var csID = $("#hdnDeleteCSPId").val();
    var statusMessage = '';
    var status = false;
    if (csID) {
        if ($formDeny.valid()) {
            eami.service.approvalService.denyCS(csID, $('#txtDenyCS').val()).done(function (data) {
                if (!data.status) {
                    if (data.returnedData && data.returnedData.length > 0) {
                        $.each(data.returnedData, function (index, message) {
                            statusMessage += "<p>" + message + "</p>";
                        });
                    }
                    else {
                        statusMessage += "<p> An error occured.</p>";
                    }
                    $("#dvDeleteErrorMessage").empty();
                    $("#dvDeleteErrorMessage").append(statusMessage);
                    $("#dvDeleteStatus").show();
                }
                else { $('#modalCSDeleteConfirm').modal('hide'); status = true; }
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
    $('#txtDenyCS').val('');
    $('.has-error').remove();
    $('#dvDeleteStatus').hide();
}



function saveRemittanceNote() {
    var csID = $("#hdnCSID").val();
    var statusMessage = '';
    if (csID) {
        eami.service.approvalService.pcsSaveRemittanceNote(csID, $('#txtOptionalComments').val())
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
                $("#dvSaveNoteErrorMessage").append(statusMessage);
                $("#dvSaveNoteStatus").show();
            }
            else { $('#dvRemittanceModal').modal('hide'); }
        });
    }
}

function chkPCSSuperGroup(gridcheckboxcolumn) {
    var checkBoxName = $(gridcheckboxcolumn).attr('name');
    var cs = checkBoxName.replace('chk_', '');

    var splitArray = cs.split('_');
    var valueIndex = selectedPCSSuperGroup.indexOf(splitArray[0]);
    var idIndex = selectedPCSSuperGroupIds.indexOf(splitArray[1]);
    var isLinked = (splitArray[2] === 'True') ? true : false;


    if ($(gridcheckboxcolumn).prop("checked")) {
        if (valueIndex < 0 && idIndex < 0) {
            selectedPCSSuperGroup.push(splitArray[0]);
            selectedPCSSuperGroupIds.push(splitArray[1]);
            if (isLinked)
            { hasLinkedCS.push(true); }
        }
    }
    else {
        if (valueIndex >= 0 && idIndex >= 0) {
            selectedPCSSuperGroup.splice(valueIndex, 1);
            selectedPCSSuperGroupIds.splice(idIndex, 1);
            if (isLinked)
            { hasLinkedCS.pop(); }
        }
    }
}

function selectAllChkPCS() {
    var gridcheckboxcolumn = $('.chkPCS');
    var selectvallValue = $(gridcheckboxcolumn).prop('checked');
    $('#grdCS').find('.rowselectors').prop('checked', selectvallValue);

    if (selectvallValue) {
        $('#grdCS').find('.rowselectors').each(function () {
            var csId = $(this).attr('name').replace('chk_', '');
            var splitArray = csId.split('_');
            var valueIndex = selectedPCSSuperGroup.indexOf(splitArray[0]);
            var idIndex = selectedPCSSuperGroupIds.indexOf(splitArray[1]);
            var isLinked = (splitArray[2] === 'True') ? true : false;
            if (valueIndex < 0 && idIndex < 0) {
                selectedPCSSuperGroup.push(splitArray[0]);
                selectedPCSSuperGroupIds.push(splitArray[1]);
                if (isLinked)
                { hasLinkedCS.push(true); }
            }
        });
    }
    else {
        $('#grdCS').find('.rowselectors').each(function () {
            var csId = $(this).attr('name').replace('chk_', '');
            var splitArray = csId.split('_');
            var valueIndex = selectedPCSSuperGroup.indexOf(splitArray[0]);
            var idIndex = selectedPCSSuperGroupIds.indexOf(splitArray[1]);
            var isLinked = (splitArray[2] === 'True') ? true : false;
            if (valueIndex >= 0 && idIndex >= 0) {
                selectedPCSSuperGroup.splice(valueIndex, 1);
                selectedPCSSuperGroupIds.splice(idIndex, 1);
                if (isLinked)
                { hasLinkedCS.pop(); }
            }
        });

    }
}

function approveAll() {
    if (selectedPCSSuperGroup.length > 0 && selectedPCSSuperGroupIds.length > 0) {
        $("#lblSubmitCSId").empty();
        $('#lblApproveMainCSId').empty();
        $('#lblApproveLinkedCSId').empty();
        $('#chkShowWarning').prop('checked', false);
        $("#lblSubmitCSId").append($("#ddlPCSPayDates :selected").attr('value'));

        var payDate = moment(selectedPayDates);
        var currentDate = moment();
        var duration = (moment.duration(payDate.diff(currentDate))).asHours();

        if (Number(duration) < 72) {
            $('#chkShowWarning').prop('checked', true);
            $('.csPaydateWarn').show();
        }
        else {
            $('#chkShowWarning').prop('checked', false);
            $('.csPaydateWarn').hide();
        }

        if (hasLinkedCS.length > 0) {
            $('#dvLinkWarning').show();
        }
        else {
            $('#dvLinkWarning').hide();
        }
        $('.has-error').remove();
        $('#dvSubmitErrorMessage').empty();
        $('#dvSubmitStatus').hide();
        $('#modalCSSubmitConfirm').modal();
    }
    else {
        $('#dvNoCSSelection').modal('show');
    }
}

function confirmSubmitCS() {
    var statusMessage = '';
    var status = false;
    if (selectedPCSSuperGroup.length > 0 && selectedPCSSuperGroupIds.length > 0) {
        if ($formApprove.valid()) {
            hasLinkedCS = [];
            eami.service.approvalService.approveCS(selectedPCSSuperGroupIds).done(function (data) {
                if (!data.status) {
                    if (data.returnedData && data.returnedData.length > 0) {
                        $.each(data.returnedData, function (index, message) {
                            statusMessage += "<p>" + message + "</p>";
                        });
                    }
                    else {
                        statusMessage += "<p> An error occured.</p>";
                    }
                    $("#dvSubmitErrorMessage").empty();
                    $("#dvSubmitErrorMessage").append(statusMessage);
                    $("#dvSubmitStatus").show();
                }
                else { $('#modalCSSubmitConfirm').modal('hide'); status = true; }
            });
        }
    }

    if (status) {
        setTimeout(function () { loadIPTabs(2); }, 1000);

    }
}

function closeSubmitCSModal() {
    $('.has-error').remove();
    $('#modalCSSubmitConfirm').modal('hide');
    $('#dvSubmitErrorMessage').empty();
    $('#dvSubmitStatus').hide();
}

function getFundingSummary() {
    if (selectedPCSSuperGroup.length > 0 && selectedPCSSuperGroupIds.length > 0) {
        eami.service.approvalService.getPCSFundingSummary(selectedPCSSuperGroupIds).done(function (data) {
            if (data != null) {
                if (typeof (data) == "string"
                    && data.includes('An error occured while processing your request')) {
                    $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
                    $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
                    showErrorOnWholePage(data);
                }
                else {
                    $('#modalBodyFundingSummary').html(data);
                    $('#dvFundingSummaryModal').modal('show');
                }
            }
        });
    }
    else {
        $('#dvNoCSSelection').modal('show');
    }
}
