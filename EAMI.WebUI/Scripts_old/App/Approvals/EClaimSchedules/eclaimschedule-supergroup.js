var $fromDate = $('#dtPickerFromRange');
var $toDate = $('#dtPickerToRange');
var $statusType = $('#ddlECSStatusTypes');

$('#txtApproveComment, #txtDeleteComment ').on('keypress', function (event) {
    var value = String.fromCharCode(!event.charCode ? event.which : event.charCode);
    if (!eami.utility.checkInputValidity(value)) {
        event.preventDefault();
    }
});

function showApproveECSModal(ecsNumber, ecsId) {
    document.getElementById("btnConfirmApprove").disabled = false;
    document.getElementById("btnECSModify").disabled = false;
    $("#dvECSApproveErrorMessage").empty();
    $('#dvECSApproveStatus').hide();
    $('#lbApproveECSId').empty()
    //$('#txtApproveComment').val('');
    $('#hdnApproveECSId').val('');
    if (ecsId != '') {
        $('#lbApproveECSId').append(ecsNumber);
        $('#hdnApproveECSId').val(ecsId);
        $("#dvECSApproveModal").modal();
    }
}

function confirmECSApprove() {
    var statusMessage = '';
    var status = false;
    $("#dvECSApproveErrorMessage").empty();
    $('#dvECSApproveStatus').hide();

     
    eami.service.approvalService.approveECS($('#hdnApproveECSId').val(), "")
                .done(function (data) {
                    if (!data.status) {
                        if (data.returnedData && data.returnedData.length > 0) {
                            $.each(data.returnedData, function (index, message) {
                                statusMessage += "<p>" + message + "</p>";
                            });
                        }
                        else {
                            document.getElementById("btnConfirmApprove").setAttribute("disabled", "");
                            statusMessage += "<p> An error occured. Please refresh the screen and try again.</p>";
                        }
                        $("#dvECSApproveErrorMessage").append(statusMessage);
                        $("#dvECSApproveStatus").show();
                    }
                    else {
                        $('#dvECSApproveModal').modal('hide'); status = true;
                    }
                });

    if (status) {
        setTimeout(function () {
            loadIPTabs(3);
        }, 1000);
    }
}

function confirmECSModify() {
    var statusMessage = '';
    var status = false;
    $("#dvECSModifyErrorMessage").empty();
    $('#dvECSModifyStatus').hide();
   
    eami.service.approvalService.pendingECS($('#hdnModifyECSId').val(), "")
        .done(function (data) {
            if (!data.status) {
                if (data.returnedData && data.returnedData.length > 0) {
                    $.each(data.returnedData, function (index, message) {
                        statusMessage += "<p>" + message + "</p>";
                    });
                }
                else {
                    document.getElementById("btnECSModify").setAttribute("disabled", "");
                    statusMessage += "<p> An error occured. Please refresh the screen and try again.</p>";
                }
                $("#dvECSModifyErrorMessage").append(statusMessage);
                $("#dvECSModifyStatus").show();
            }
            else {
                $('#dvECSModifyModal').modal('hide'); status = true;
            }
        });

    if (status) {
        setTimeout(function () {
            loadIPTabs(3);
        }, 1000);
    }
}

function showDeleteECSModal(ecsNumber, ecsId) {
    $("#dvECSDeleteErrorMessage").empty();
    $('#dvECSDeleteStatus').hide();
    $('#lbDeleteECSId').empty()
    //$('#txtDeleteComment').val('');
    $('#hdnDeleteECSId').val('');
    if (ecsId != '') {
        $('#lbDeleteECSId').append(ecsNumber);
        $('#hdnDeleteECSId').val(ecsId);
        $("#dvECSDeleteModal").modal();
    }
}

function showModifyECSModal(ecsNumber, ecsId) {
    $("#dvECSModifyErrorMessage").empty();
    //$('#dvECSModifyStatus').hide();
    $('#lbModifyECSId').empty()
    //$('#txtDeleteComment').val('');
    $('#hdnModifyECSId').val('');
    if (ecsId != '') {
        $('#lbModifyECSId').append(ecsNumber);
        $('#hdnModifyECSId').val(ecsId);
        $("#dvECSModifyModal").modal();
    }
    
}

function confirmECSDelete() {
    var statusMessage = '';
    var status = false;
    $("#dvECSDeleteErrorMessage").empty();
    $('#dvECSDeleteStatus').hide();

    //eami.service.approvalService.deleteECS($('#hdnDeleteECSId').val(), $('#txtDeleteComment').val())
    eami.service.approvalService.deleteECS($('#hdnDeleteECSId').val(), String.Empty)
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
                        $("#dvECSDeleteErrorMessage").append(statusMessage);
                        $("#dvECSDeleteStatus").show();
                    }
                    else {
                        $('#dvECSDeleteModal').modal('hide'); status = true;
                    }
                });

    if (status) {
        setTimeout(function () {
            loadIPTabs(3);
        }, 1000);
    }
}

function generateFaceSheet(labelTxt, ecsId) {
    $('#hdnReportECSId').val('');
    $('#hdnReportECSId').val(ecsId);
    $('#lnkPDF').text('Export ' + labelTxt);
    var prgId = document.getElementById('hdn_ProgramChoiceId').value;
    //console.log(prgId)

    $('#lnkPDF').attr("href", "../Reports/ExportFaceSheetPdf?ecsID=" + ecsId + '&programId=' + prgId);

    eami.service.approvalService.getFaceSheet(ecsId)
        .done(function (data) {
            if (data != null) {
                if (typeof (data) == "string"
                    && data.includes('An error occured while processing your request')) {
                    $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
                    $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
                    showErrorOnWholePage(data);
                }
                else {
                    $('#modalBodyReport').html(data);
                    $('#dvReportModal').modal('show');
                }
            }
        });
}

function generateTransferLetter(labelTxt, ecsId) {
    $('#hdnReportECSId').val('');
    $('#hdnReportECSId').val(ecsId);
    $('#lnkPDF').text('Export ' + labelTxt);
    var prgId = document.getElementById('hdn_ProgramChoiceId').value;
    //console.log(prgId)

    $('#lnkPDF').attr("href", "../Reports/ExportTransferLetterPdf?ecsID=" + ecsId + '&programId=' + prgId);

    eami.service.approvalService.getTransferLetter(ecsId)
        .done(function (data) {
            if (data != null) {
                if (typeof (data) == "string"
                    && data.includes('An error occured while processing your request')) {
                    $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
                    $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
                    showErrorOnWholePage(data);
                }
                else {
                    $('#modalBodyReport').html(data);
                    $('#dvReportModal').modal('show');
                }
            }
        });
}

$(document).ready(function () {
    const btnCancelRefresh = document.getElementById("btnCancelECSApproval");
    const btnCancelECSModifyRefresh = document.getElementById("btnCancelECSModify");
    function handleClick() {
        history.go(0);
        // window.location.reload();
    }
    if (btnCancelRefresh != null) {
        btnCancelRefresh.addEventListener("click", handleClick);
    }
    if (btnCancelECSModifyRefresh != null) {
        btnCancelECSModifyRefresh.addEventListener("click", handleClick);
    }    

    var $table = $('#grdECS').DataTable(
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
                         "targets": [1, 7],
                         "sortable": false
                     },
                ],
                "initComplete": function (settings, json) {
                    $('#dvECSMasterGridHolder').css("visibility", "visible");
                }
            });

    $('#grdECS tbody tr td').on('click', 'button.btnPayeePaymentType', function () {
        if ($(this).find("span").attr("class") == 'glyphicon-plus') {
            $(this).find("span").toggleClass('glyphicon-plus');
            $(this).find("span").toggleClass('glyphicon-minus');
        }
        else {
            $(this).find("span").toggleClass('glyphicon-minus');
            $(this).find("span").toggleClass('glyphicon-plus');
        }

        var dataid = $(this).attr('id');
        var tr = $('#grdECS').find("tr[data-id='" + dataid + "']");
        var row = $table.row(tr);
        var idOfParentDiv = 'dv_' + $(tr).attr('id');
        if (tr.hasClass('shown')) {
            $('div.slider', row.child()).delay(0).slideUp(250);  //Normally Don't use delay(0) on top level - causes column shifting.  BUT slide jiggling noticeable here so used delay to fix. 
            tr.removeClass('shown');
        }
        else {
            getPaymentSetRecords(row, tr, dataid, idOfParentDiv);
        }
    });


    function getPaymentSetRecords(row, tr, dataid, idOfParentDiv) {
        var returnHtml = '';
        try {
            var statusTypeId = Number($ddlECSStatusTypes.val());
            var selectedStatus = [];
            var dateFrom = $fromDate.val()
            var dateTo = $toDate.val();

            $("#ddlECSStatusTypes :selected").each(function () {
                selectedStatus.push($(this).attr('value'));
            });

            
            if (statusTypeId != 4 && statusTypeId != 6) {
                dateFrom = moment().format('MM/DD/YYYY');
                dateTo = moment().format('MM/DD/YYYY');                
            }

            eami.service.approvalService.getECSChildRecord(dataid, dateFrom, dateTo, selectedStatus).done(function (data) {
                if (data != null) {
                    row.child("<div class='slider' id=" + idOfParentDiv + ">" + data + "</div>", 'no-padding').show();
                    $('div.slider', row.child()).delay(0).slideDown(250);  //Normally Don't use delay(0) on top level - causes column shifting.  BUT slide jiggling noticeable here so used delay to fix. 
                    tr.addClass('shown');
                    }
                });
        }
        catch (e)
        { }

        return returnHtml;
    }

    $.fn.dataTable.moment('MM/DD/YYYY hh:mm A');
    $(window).unbind("resize.DT-" + "grdCS");
    $('[data-toggle="popover"]').popover();

});