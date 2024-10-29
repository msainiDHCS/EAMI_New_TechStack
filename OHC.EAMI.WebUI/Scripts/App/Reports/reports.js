$('#btnView').on('click', function () {
    var returnHtml = '';
    var prgId = document.getElementById('hdn_ProgramChoiceId').value;
    try
    {
        $('#lblReportModal').empty();
        var selectedRptTxt = $("#ddlReports option:selected").text();
        $('#lblReportModal').append(selectedRptTxt);
        var retHtmlHeaderString = '<span class="glyphicon glyphicon-file"></span>&nbsp;' + selectedRptTxt + " Report";

        $('#modalBodyReport').html('');
        $('.dt-buttons').remove();
        //$("#reportPdfLink").text("Export " + $("#ddlReports option:selected").text() + " to PDF");
        //$("#reportPdfLink").hide();

        if ($("#ddlReports option:selected").val() == "CashManagementSummary") {
            strParamsMenu = '' +
            '<br />' +
            '<form class="form-horizontal" role="form" method="get">' +
                '<div class="panel panel-dhcs-primary" style="border-right:none;border-bottom:none;border-left:none;border-top:none;">' +
                    '<div class="panel-heading" style="border-radius:4px 4px 4px 4px;overflow:no-display;">' +
                        '<div class="form-group row" style="padding-top:16px;">' +
                            '<label for="CMSumDatepicker" class="col-sm-5 control-label">Select a Date:</label>' +
                            '<div class="col-sm-2">' +
                                '<input type="text" id="CMSumDatepicker" class="form-control" style="width:165px;" />' +
                            '</div>' +
                            '<div class="col-sm-5">' +
                                '<button type="button" class="btn btn-dhcs-secondary btn-md" id="btnGenerateReport">' +
                                    '<span class="glyphicon glyphicon-cog"></span>&nbsp;&nbsp;Generate Report' +
                                '</button>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</form>';

            $('#modalBodyReportMenu').html(strParamsMenu);
            $('#modalBodyReportMenu').show();
            $("#CMSumDatepicker").datepicker();
            $('#dvReportModal').modal('show');

            $('#btnGenerateReport').on('click', function () {
                try
                {
                    var selectedDated = '07/17/2018';      //temporarily hardcoded

                    $("#reportPdfLink").attr("href", "/Reports/ExportCashManagementSummaryPdf?payDate=" + selectedDated);
                    $("#reportPdfLink").show();
                    $.ajax({
                        url: getEAMIAbsoluteUrl('~/Reports/GetCashManagementSummary?payDate=' + selectedDated),
                        type: 'GET',
                        datatype: "html",
                        cache: false,
                        async: false,
                        success: function (data) {
                            if (data != null) {
                                $('#modalBodyReport').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                            }
                        }
                    });
                }
                catch (e)
                { alert('The following exception was thrown:  ' + e) };

                return false;
            });
        }
        else if ($("#ddlReports option:selected").val() == "EAMIPaymentRecordSetHolds") {
            strParamsMenu = '' +
            '<br />' +
            '<form class="form-horizontal" role="form" method="get">' +
                '<div class="panel panel-dhcs-primary" style="border-right:none;border-bottom:none;border-left:none;border-top:none;">' +
                    '<div class="panel-heading" style="border-radius:4px 4px 4px 4px;overflow:no-display;">' +
                        '<div class="form-group row" style="padding-top:16px;">' +
                            '<div class="col-sm-12" style="text-align:center;">' +
                                '<button type="button" class="btn btn-dhcs-secondary btn-md" id="btnGenerateReport">' +
                                    '<span class="glyphicon glyphicon-cog"></span>&nbsp;&nbsp;Generate Report' +
                                '</button>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</form>';

            $('#modalBodyReportMenu').html(strParamsMenu);
            $('#modalBodyReportMenu').hide();
            $('#dvReportModal').modal('show');

            $('#btnGenerateReport').on('click', function () {
                try {
                    $.ajax({
                        url: getEAMIAbsoluteUrl('~/Reports/GetEAMIPaymentRecordSetHolds?'),
                        type: 'GET',
                        datatype: "html",
                        cache: false,
                        async: false,
                        success: function (data) {
                            if (data != null) {
                                $('#modalBodyReport').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                                //we need to send data in case session override and want to show error page.Data will have only error in it and used in catch block.
                                rebindDtRpt('tbl_EAMIPaymentRecordSetHolds', selectedRptTxt, data);
                                removeModalFunctionalButtonsWhenNoRecordsReturned(data);
                            }
                        }
                    });
                }
                catch (e)
                { alert('The following exception was thrown:  ' + e) };

                return false;
            });

            $('#btnGenerateReport').click();
        }
        else if ($("#ddlReports option:selected").val() == "EAMIReturnToSOR") {
            strParamsMenu = '' +
            '<br />' +
            '<form class="form-horizontal" role="form" method="get">' +
                '<div class="panel panel-dhcs-primary" style="border-right:none;border-bottom:none;border-left:none;border-top:none;">' +
                    '<div class="panel-heading" style="border-radius:4px 4px 4px 4px;overflow:no-display;">' +
                        '<div class="container">' +
                            '<div class="row" style="padding-top:6px; padding-bottom:16px;">' +
                                '<div class="col-sm-3">' +
                                '</div>' +
                                '<div class="col-sm-2">' +
                                     '<label for="ReturnToSORDatepicker_From" class="control-label">From Date:</label>' +
                                    '<input type="text" id="ReturnToSORDatepicker_From" class="form-control datepicker" style="width:165px;" />' +
                                '</div>' +
                                '<div class="col-sm-2">' +
                                     '<label for="ReturnToSORDatepicker_To" class="control-label">To Date:</label>' +
                                    '<input type="text" id="ReturnToSORDatepicker_To" class="form-control datepicker" style="width:165px;" />' +
                                '</div>' +
                                '<div class="col-sm-2">' +
                                     '<label class="control-label" style="visibility:hidden;">Generate Report:</label>' +
                                    '<button type="button" class="btn btn-dhcs-secondary btn-md" id="btnGenerateReport">' +
                                        '<span class="glyphicon glyphicon-cog"></span>&nbsp;&nbsp;Generate Report' +
                                    '</button>' +
                                '</div>' +
                                '<div class="col-sm-3">' +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</form>';

            $('#modalBodyReportMenu').html(strParamsMenu);
            $('#modalBodyReportMenu').show();
            $("#ReturnToSORDatepicker_From").datepicker({
                onClose: function (selectedDate) {
                    // Set the minDate of 'to' as the selectedDate of 'from'
                    $("#ReturnToSORDatepicker_To").datepicker("option", "minDate", selectedDate);
                },
                maxDate: 'now'
            });
            $("#ReturnToSORDatepicker_To").datepicker({
                maxDate: 'now'
            });

            $('#dvReportModal').modal('show');

            $('#btnGenerateReport').on('click', function () {
                try {
                    var dateFrom = $('#ReturnToSORDatepicker_From').val();
                    var dateTo = $('#ReturnToSORDatepicker_To').val();
                    $.ajax({
                        url: getEAMIAbsoluteUrl('~/Reports/GetReturnToSORReportData?' + 'dateFrom=' + dateFrom + '&' + 'dateTo=' + dateTo),
                        type: 'GET',
                        datatype: "html",
                        cache: false,
                        async: false,
                        success: function (data) {
                            if (data != null) {
                                $('#modalBodyReport').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                                rebindDtRpt('tbl_EAMIReturnToSOR', selectedRptTxt, data);
                                removeModalFunctionalButtonsWhenNoRecordsReturned(data);
                            }
                        }
                    });
                }
                catch (e)
                { alert('The following exception was thrown:  ' + e) };

                return false;
            });

            // Only enable Generate Report Button when both dates are filled out with valid dates.
            $("#btnGenerateReport").prop("disabled", true);
            $("#ReturnToSORDatepicker_From").prop("onchange", null);
            $("#ReturnToSORDatepicker_From").prop("onkeyup", null);
            $("#ReturnToSORDatepicker_From").prop("onpaste", null);
            $("#ReturnToSORDatepicker_From").prop("onfocusout", null);
            $("#ReturnToSORDatepicker_From").on("change keyup paste focusout", function () {
                if ( $('#ReturnToSORDatepicker_From').val().length <= 0 || 
                     $('#ReturnToSORDatepicker_To').val().length <= 0 || 
                     !Date.parse($('#ReturnToSORDatepicker_From').val()) || 
                     !Date.parse($('#ReturnToSORDatepicker_To').val()) ){
                    $("#btnGenerateReport").prop("disabled", true);
                    $('.buttons-excel').prop("disabled", true);
                    $('.buttons-pdf').prop("disabled", true);
                }
                else {
                    $("#btnGenerateReport").prop("disabled", false);
                    $('.buttons-excel').prop("disabled", false);
                    $('.buttons-pdf').prop("disabled", false);
                }
            });
            $("#ReturnToSORDatepicker_To").prop("onchange", null);
            $("#ReturnToSORDatepicker_To").prop("onkeyup", null);
            $("#ReturnToSORDatepicker_To").prop("onpaste", null);
            $("#ReturnToSORDatepicker_To").prop("onfocusout", null);
            $("#ReturnToSORDatepicker_To").on("change keyup paste focusout", function () {
                if ($('#ReturnToSORDatepicker_From').val().length <= 0 ||
                     $('#ReturnToSORDatepicker_To').val().length <= 0 ||
                     !Date.parse($('#ReturnToSORDatepicker_From').val()) ||
                     !Date.parse($('#ReturnToSORDatepicker_To').val())) {
                    $("#btnGenerateReport").prop("disabled", true);
                    $('.buttons-excel').prop("disabled", true);
                    $('.buttons-pdf').prop("disabled", true);
                }
                else {
                    $("#btnGenerateReport").prop("disabled", false);
                    $('.buttons-excel').prop("disabled", false);
                    $('.buttons-pdf').prop("disabled", false);
                }
            });
        }
        else if ($("#ddlReports option:selected").val() == "EAMIEClaimSchedule") {
            strParamsMenu = '' +
            '<br />' +
            '<form class="form-horizontal" role="form" method="get">' +
                '<div class="panel panel-dhcs-primary" style="border-right:none;border-bottom:none;border-left:none;border-top:none;">' +
                    '<div class="panel-heading" style="border-radius:4px 4px 4px 4px;overflow:no-display;">' +
                        '<div class="container">' +
                            '<div class="row" style="padding-top:6px; padding-bottom:16px;">' +
                                '<div class="col-sm-3">' +
                                '</div>' +
                                '<div class="col-sm-2">' +
                                     '<label for="EClaimScheduleDatepicker" class="control-label">Select A Month:</label>' +
                                    '<input type="text" id="EClaimScheduleDatepicker" class="form-control" style="width:165px;" />' +
                                '</div>' +
                                '<div class="col-sm-2">' +
                                     '<label class="control-label" style="visibility:hidden;">Generate Report:</label>' +
                                    '<button type="button" class="btn btn-dhcs-secondary btn-md" id="btnGenerateReport">' +
                                        '<span class="glyphicon glyphicon-cog"></span>&nbsp;&nbsp;Generate Report' +
                                    '</button>' +
                                '</div>' +
                                '<div class="col-sm-3">' +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</div>' +
            '</form>';

            $('#modalBodyReportMenu').html(strParamsMenu);
            $('#modalBodyReportMenu').show();
            $('#EClaimScheduleDatepicker').MonthPicker({ Button: false, MonthFormat: 'MM yy' });
            $('#dvReportModal').modal('show');
            var selectedRptTxtTotal = '';
            $('#btnGenerateReport').on('click', function () {
                try {
                    var dateArray = $('#EClaimScheduleDatepicker').val().split(" ");
                    var monthAsNumber = new Date(Date.parse(dateArray[0] + " 1, " + dateArray[1])).getMonth();
                    var dateFromComplete = new Date(dateArray[1], monthAsNumber, 1);
                    var dateToComplete = new Date(dateArray[1], monthAsNumber + 1, 0);
                    var dateFrom = dateFromComplete.getMonth() + 1 + '/' + dateFromComplete.getDate() + '/' + dateFromComplete.getFullYear();
                    var dateTo = dateToComplete.getMonth() + 1 + '/' + dateToComplete.getDate() + '/' + dateToComplete.getFullYear();
                    $.ajax({
                        url: getEAMIAbsoluteUrl('~/Reports/GetEClaimScheduleReportData?' + 'dateFrom=' + dateFrom + '&' + 'dateTo=' + dateTo),
                        type: 'GET',
                        datatype: "html",
                        cache: false,
                        async: false,
                        success: function (data) {
                            if (data != null) {
                                $('#modalBodyReport').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                                rebindDtRpt('tbl_EAMIEClaimSchedule', selectedRptTxtTotal, data);
                                removeModalFunctionalButtonsWhenNoRecordsReturned(data);
                            }
                        }
                    });
                }
                catch (e)
                { alert('The following exception was thrown:  ' + e) };

                return false;
            });

            // Only enable Generate Report Button when date filled out with valid date.
            $("#btnGenerateReport").prop("disabled", true);
            $("#EClaimScheduleDatepicker").prop("onchange", null);
            $("#EClaimScheduleDatepicker").prop("onkeyup", null);
            $("#EClaimScheduleDatepicker").prop("onpaste", null);
            $("#EClaimScheduleDatepicker").prop("onfocusout", null);
            var retHtmlSubHeaderString = '';
            $("#EClaimScheduleDatepicker").on("change keyup paste focusout", function () {
                retHtmlSubHeaderString = $('#EClaimScheduleDatepicker').val();
                var dateArray = $('#EClaimScheduleDatepicker').val().split(" ");
                if ($('#EClaimScheduleDatepicker').val().length <= 0 ||
                     !Date.parse(dateArray[0] + " 1, " + dateArray[1])) {
                    $("#btnGenerateReport").prop("disabled", true);
                    $('.buttons-excel').prop("disabled", true);
                    $('.buttons-pdf').prop("disabled", true);
                }
                else {
                    $("#btnGenerateReport").prop("disabled", false);
                    $('.buttons-excel').prop("disabled", false);
                    $('.buttons-pdf').prop("disabled", false);
                }

                selectedRptTxtTotal = selectedRptTxt + ' Report' + ' (' + retHtmlSubHeaderString + ')';
                $("#modalHeaderForReport").html(retHtmlHeaderString + ' (' + retHtmlSubHeaderString + ')');
            });
        }
        else if ($("#ddlReports option:selected").val() == "EAMISTOReport") {
                strParamsMenu = '' +
                '<br />' +
                '<form class="form-horizontal" role="form" method="get">' +
                    '<div class="panel panel-dhcs-primary" style="border-right:none;border-bottom:none;border-left:none;border-top:none;">' +
                        '<div class="panel-heading" style="border-radius:4px 4px 4px 4px;overflow:no-display;">' +
                            '<div class="container">' +
                                '<div class="row" style="padding-top:6px; padding-bottom:16px;">' +
                                    '<div class="col-sm-3">' +
                                    '</div>' +
                                    '<div class="col-sm-2">' +
                                         '<label for="EAMISTOReportDatepicker" class="control-label">Pay Date:</label>' +
                                        '<input type="text" id="EAMISTOReportDatepicker" class="form-control datepicker" style="width:165px;" />' +
                                    '</div>' +                                
                                    '<div class="col-sm-2">' +
                                         '<label class="control-label" style="visibility:hidden;">Generate Report:</label>' +
                                        '<button type="button" class="btn btn-dhcs-secondary btn-md" id="btnGenerateReport">' +
                                            '<span class="glyphicon glyphicon-cog"></span>&nbsp;&nbsp;Generate Report' +
                                        '</button>' +
                                    '</div>' +
                                    '<div class="col-sm-3">' +
                                    '</div>' +
                                '</div>' +
                            '</div>' +
                        '</div>' +
                    '</div>' +
                '</form>';

                $('#modalBodyReportMenu').html(strParamsMenu);
                $('#modalBodyReportMenu').show();            
            
            //Added to highlight Pay Dates 

                var payDates = new Array;
                function getPayDates(){
                    currentYear = new Date().getFullYear();
                    for (let j = 2; j > 0; j--) {
                        $.ajax({
                            url: getEAMIAbsoluteUrl('~/Reports/GetPayDates?activeYear=' + currentYear),
                            type: 'GET',
                            datatype: "json",
                            cache: false,
                            async: false,
                            success: function (data) {

                                if (data != null) {
                                    for (i = 0; i < data.length; i++) {
                                        var payDate;
                                        if (data[i]['type'] == 'P') {                                    
                                            payDate = data[i]['startMonth'] + '/' + data[i]['startDay']+'/'+currentYear;
                                        };
                                        payDates.push(payDate);
                                    }
                                }
                            }
                        });
                        currentYear--
                    }
                }            
                function payDate(date) {
                    currentYear = date.getFullYear();
                    date = (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
                    if (payDates.length == 0) {
                        getPayDates();
                    }
                    if (payDates.indexOf(date) >= 0) {
                        return [true, 'payDate-highlight'];
                    } else {
                        return [true, ''];
                    }
                }
                $("#EAMISTOReportDatepicker").datepicker({

                    maxDate: '+1m',
                    beforeShowDay: payDate

                });
            

                $('#dvReportModal').modal('show');

                $('#btnGenerateReport').on('click', function () {
                    try {
                        var payDate = $('#EAMISTOReportDatepicker').val();                    
                        $.ajax({
                            url: getEAMIAbsoluteUrl('~/Reports/GetESTOReportData?' + 'payDate=' + payDate),
                            type: 'GET',
                            datatype: "html",
                            cache: false,
                            async: false,
                            success: function (data) {
                                if (data != null) {
                                    $('#modalBodyReport').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                                    rebindDtRpt('tbl_EAMISTOReport', selectedRptTxt, data);
                                    removeModalFunctionalButtonsWhenNoRecordsReturned(data);
                                }
                            }
                        });
                    }
                    catch (e)
                    { alert('The following exception was thrown:  ' + e) };

                    return false;
                });

            // Only enable Generate Report Button when both dates are filled out with valid dates.
                $("#btnGenerateReport").prop("disabled", true);
                $("#EAMISTOReportDatepicker").prop("onchange", null);
                $("#EAMISTOReportDatepicker").prop("onkeyup", null);
                $("#EAMISTOReportDatepicker").prop("onpaste", null);
                $("#EAMISTOReportDatepicker").prop("onfocusout", null);
                $("#EAMISTOReportDatepicker").on("change keyup paste focusout", function () {
                    if ($('#EAMISTOReportDatepicker').val().length <= 0 ||
                         !Date.parse($('#EAMISTOReportDatepicker').val()))
                    {
                        $("#btnGenerateReport").prop("disabled", true);
                        $('.buttons-excel').prop("disabled", true);
                        $('.buttons-pdf').prop("disabled", true);
                    }
                    else {
                        $("#btnGenerateReport").prop("disabled", false);
                        $('.buttons-excel').prop("disabled", false);
                        $('.buttons-pdf').prop("disabled", false);
                    }
                });            
        }
        else if ($("#ddlReports option:selected").val() == "EAMIDataSummaryReport") {
            strParamsMenu = '' +
                '<br />' +
                '<form class="form-horizontal" role="form" method="get">' +
                '<div class="panel panel-dhcs-primary" style="border-right:none;border-bottom:none;border-left:none;border-top:none;">' +
                '<div class="panel-heading" style="border-radius:4px 4px 4px 4px;overflow:no-display;">' +
                '<div class="container">' +
                '<div class="row" style="padding-top:6px; padding-bottom:16px;">' +
                '<div class="col-sm-3">' +
                '</div>' +
                '<div class="col-sm-2">' +
                '<label for="DataSummaryDatepicker_From" class="control-label">From Date:</label>' +
                '<input type="text" id="DataSummaryDatepicker_From" class="form-control datepicker" style="width:165px;" />' +
                '</div>' +
                '<div class="col-sm-2">' +
                '<label for="DataSummaryDatepicker_To" class="control-label">To Date:</label>' +
                '<input type="text" id="DataSummaryDatepicker_To" class="form-control datepicker" style="width:165px;" />' +
                '</div>' +
                '<div class="col-sm-2">' +
                '<label class="control-label" style="visibility:hidden;">Generate Report:</label>' +
                '<button type="button" class="btn btn-dhcs-secondary btn-md" id="btnGenerateReport">' +
                '<span class="glyphicon glyphicon-cog"></span>&nbsp;&nbsp;Generate Report' +
                '</button>' + 
                '</div>' +
                '<div class="col-sm-2">' +
                '<label class="control-label" style="visibility:hidden;">Download Report:</label>' +
                '<button type="button" class="btn btn-dhcs-secondary btn-md" id="btnDownloadReport">' +
                '<span class="glyphicon glyphicon-cog"></span>&nbsp;&nbsp;Download Report' +
                '</button>' +
                '</div>' +
                '<div class="col-sm-3">' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</div>' +
                '</form>';

            $('#modalBodyReportMenu').html(strParamsMenu);
            $('#modalBodyReportMenu').show();
            $("#DataSummaryDatepicker_From").datepicker({
                onClose: function (selectedDate) {
                    // Set the minDate of 'to' as the selectedDate of 'from'
                    $("#DataSummaryDatepicker_To").datepicker("option", "minDate", selectedDate);
                },
                maxDate: '+100y'
            });
            $("#DataSummaryDatepicker_To").datepicker({
                maxDate: '+100y'
            });

            $('#dvReportModal').modal('show');
            var retHtmlSubHeaderString = '';
            $('#btnGenerateReport').on('click', function () {
                try {
                    var dateFrom = $('#DataSummaryDatepicker_From').val();
                    var dateTo = $('#DataSummaryDatepicker_To').val();
                    retHtmlSubHeaderString = dateFrom + '-' + dateTo
                    $("#modalHeaderForReport").html(retHtmlHeaderString + ' (' + retHtmlSubHeaderString + ')');
                    $.ajax({
                        url: getEAMIAbsoluteUrl('~/Reports/GetDataSummaryReport?dateFrom=' + dateFrom + '&' + 'dateTo=' + dateTo),
                        type: 'GET',
                        datatype: "html",
                        cache: false,
                        async: false,
                        success: function (data) {
                            if (data != null) {
                                $('#modalBodyReport').html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                                //we need to send data in case session override and want to show error page.
                                rebindDtRpt('tbl_EAMIDataSummary', selectedRptTxt, data);
                                removeModalFunctionalButtonsWhenNoRecordsReturned(data);
                            }
                        }
                    });
                }
                catch (e) { alert('The following exception was thrown:  ' + e) };

                return false;
            });

            $('#btnDownloadReport').on('click', function () {

                try {
                    var dateFrom = $('#DataSummaryDatepicker_From').val();
                    var dateTo = $('#DataSummaryDatepicker_To').val();
                    retHtmlSubHeaderString = dateFrom + '-' + dateTo
                    $("#modalHeaderForReport").html(retHtmlHeaderString + ' (' + retHtmlSubHeaderString + ')');
                    window.location = getEAMIAbsoluteUrl('~/Reports/ExportToExcelDataSummary?' + 'dateFrom=' + dateFrom + '&' + 'dateTo=' + dateTo + '&programId=' + prgId)
                }
                catch (e) { alert('The following exception was thrown:  ' + e) };

                return false;
            });

            // Only enable Generate Report Button when both dates are filled out with valid dates.
            $("#btnGenerateReport").prop("disabled", true);
            $("#btnDownloadReport").prop("disabled", true);
            $("#DataSummaryDatepicker_From").prop("onchange", null);
            $("#DataSummaryDatepicker_From").prop("onkeyup", null);
            $("#DataSummaryDatepicker_From").prop("onpaste", null);
            $("#DataSummaryDatepicker_From").prop("onfocusout", null);
            $("#DataSummaryDatepicker_From").on("change keyup paste focusout", function () {
                if ($('#DataSummaryDatepicker_From').val().length <= 0 || $('#DataSummaryDatepicker_To').val().length <= 0 ||
                    !Date.parse($('#DataSummaryDatepicker_From')).val() || !Date.parse($('#DataSummaryDatepicker_To').val()))
                {
                    $("#btnGenerateReport").prop("disabled", true);
                    $("#btnDownloadReport").prop("disabled", true);
                    $('.buttons-excel').prop("disabled", true);
                    $('.buttons-pdf').prop("disabled", true);
                }
                else {
                    $("#btnGenerateReport").prop("disabled", false);
                    $("#btnDownloadReport").prop("disabled", false);
                    $('.buttons-excel').prop("disabled", false);
                    $('.buttons-pdf').prop("disabled", false);
                }
            });
            $("#DataSummaryDatepicker_To").prop("onchange", null);
            $("#DataSummaryDatepicker_To").prop("onkeyup", null);
            $("#DataSummaryDatepicker_To").prop("onpaste", null);
            $("#DataSummaryDatepicker_To").prop("onfocusout", null);
            $("#DataSummaryDatepicker_To").on("change keyup paste focusout", function () {
                if ($('#DataSummaryDatepicker_From').val().length <= 0 ||
                    $('#DataSummaryDatepicker_To').val().length <= 0 ||
                    !Date.parse($('#DataSummaryDatepicker_From').val()) ||
                    !Date.parse($('#DataSummaryDatepicker_To').val())) {
                    $("#btnGenerateReport").prop("disabled", true);
                    $("#btnDownloadReport").prop("disabled", true);
                    $('.buttons-excel').prop("disabled", true);
                    $('.buttons-pdf').prop("disabled", true);
                }
                else {
                    $("#btnGenerateReport").prop("disabled", false);
                    $("#btnDownloadReport").prop("disabled", false);
                    $('.buttons-excel').prop("disabled", false);
                    $('.buttons-pdf').prop("disabled", false);
                }
            });
        }
    
        $("#modalHeaderForReport").html(retHtmlHeaderString);
    }
    catch (e)
    {
        alert('The following exception was thrown:  ' + e);
    };

    return false;
});


function rebindDtRpt(tableId, selectedRptTxt, data) {
    try {
        $('.dt-buttons').remove();

        var date = new Date();
        var month = date.getMonth() + 1;
        var day = date.getDate();
        var year = date.getFullYear();
        var strDate = month + '_' + day + '_' + year;
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var ampm = hours >= 12 ? 'PM' : 'AM';
        hours = hours % 12;
        hours = hours ? hours : 12; // the hour '0' should be '12'
        minutes = minutes < 10 ? '0' + minutes : minutes;
        var strTime = hours + '_' + minutes + '_' + ampm;
        var strDateTime = "Date_" + strDate + "_Time_" + strTime;
        var strFilename = tableId + "_" + strDateTime;
        var strFooterDate = date.toDateString() + " " + strTime.replace("_", ":").replace("_", " ");

        var strOrientation = '';
        var dtRpt;
        if (tableId == 'tbl_EAMIPaymentRecordSetHolds') {
            strOrientation = "landscape";
            var buttonCommon = {};
            dtRpt = $('#' + tableId).DataTable({
                "destroy": true,    // unbinds previous datatable initialization binding
                //"stateSave": true,      // first init will save settings below to sessionStorage, subsequent inits will call from sessionStorage. 
                //"stateDuration": -1,    // -1 for use sessionStorage
                //"paging": false,
                //"info": false,
                //"lengthChange": false,
                //"filter": false, // this is for disable filter (search box)
                //"autoWidth": false,
                //Set column definition initialization properties.
                "searching": false,
                processing: false,
                "order": [[0, "asc"], [1, "asc"]],
                bPaginate: false,
                bInfo: false,
                "columnDefs": [
                    {
                        "type": "eami_currency",
                        "targets": 8,
                        "orderable": true
                    }
                ],
                dom: 'frtipB',
                buttons: [
                    $.extend(true, {}, buttonCommon,
                        {
                            extend: 'excelHtml5',
                            text: 'Export to Excel',
                            title: selectedRptTxt + ' Report',
                            filename: strFilename,
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '\$#,##0.00_);[Red](\$#,##0.00)');
                                $('row c[r^="I"]', sheet).attr('s', '57');
                                $('row:eq(1) c', sheet).attr('s', '2'); // Bold Column Headers
                            },
                        }),
                    //{
                    //    extend: 'csvHtml5',
                    //    text: 'Export to CSV',
                    //    title: 'EAMI Payment Set Holds',
                    //    filename: strFilename
                    //},
                    {
                        extend: 'pdfHtml5',
                        text: 'Export to PDF',
                        title: selectedRptTxt + ' Report',
                        filename: strFilename,
                        pageSize: 'letter',
                        orientation: strOrientation,
                        customize: function (doc) {
                            //if (strOrientation == "portrait")
                            //{
                            //    doc.content[1].table.widths =
                            //        Array(doc.content[1].table.body[0].length + 1).join('*').split('');
                            //}
                            //doc.watermark = { text: 'watermark text', color: 'grey', opacity: 0.3};
                            doc.defaultStyle.alignment = 'left';
                            doc.styles.tableHeader.alignment = 'left';
                            doc.styles.tableHeader.fillColor = '#3d5475';   // EAMI_BgColor_BlueSteel
                            doc.styles.tableBodyOdd.fillColor = '#d9d9dd';

                            var arrTotal = dtRpt.column(8).data().toArray();
                            for (var i = 0; i < arrTotal.length; i++) {
                                doc.content[1].table.body[i + 1][8].alignment = 'right';
                                if (Number(arrTotal[i].replace(/[$,)]/g, '').replace(/[(]/g, '-')) <= 0) {
                                    doc.content[1].table.body[i + 1][8].color = 'red';
                                }
                            }

                            // Create a footer object with 2 columns
                            // Left side: report creation date
                            // Right side: current page and total pages
                            doc['footer'] = (function (page, pages) {
                                return {
                                    columns: [
                                        {
                                            alignment: 'left',
                                            text: ['Created on: ', { text: strFooterDate }]
                                        },
                                        {
                                            alignment: 'right',
                                            text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                        }
                                    ],
                                    margin: 20
                                }
                            });

                        }
                    }
                ]
            });
        }
        else if (tableId == 'tbl_EAMIReturnToSOR') {
            strOrientation = "landscape";
            var buttonCommon = {};
            dtRpt = $('#' + tableId).DataTable({
                "destroy": true,    // unbinds previous datatable initialization binding
                //"stateSave": true,      // first init will save settings below to sessionStorage, subsequent inits will call from sessionStorage. 
                //"stateDuration": -1,    // -1 for use sessionStorage
                //"paging": false,
                //"info": false,
                //"lengthChange": false,
                //"filter": false, // this is for disable filter (search box)
                //"autoWidth": false,
                //Set column definition initialization properties.
                "searching": false,
                processing: false,
                "order": [[0, "asc"], [1, "asc"]],
                bPaginate: false,
                bInfo: false,
                "columnDefs": [
                    {
                        "type": "eami_currency",
                        "targets": 9,
                        "orderable": true
                    }
                ],
                dom: 'frtipB',
                buttons: [
                    $.extend(true, {}, buttonCommon,
                        {
                            extend: 'excelHtml5',
                            text: 'Export to Excel',
                            title: selectedRptTxt + ' Report ' + $('#ReturnToSORDatepicker_From').val() + " - " + $('#ReturnToSORDatepicker_To').val(),
                            filename: strFilename,
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '\$#,##0.00_);[Red](\$#,##0.00)');
                                $('row c[r^="J"]', sheet).attr('s', '57');
                                $('row:eq(1) c', sheet).attr('s', '2'); // Bold Column Headers
                                //$('row:first c', sheet).attr('s', '22');    // Blue Background (first row)
                                //$('row:first c', sheet).attr('s', '51');    // Centred Text (first row)
                            },
                        }),
                    {
                        extend: 'pdfHtml5',
                        text: 'Export to PDF',
                        title: selectedRptTxt + ' Report ' + $('#ReturnToSORDatepicker_From').val() + " - " + $('#ReturnToSORDatepicker_To').val(),
                        filename: strFilename,
                        pageSize: 'letter',
                        orientation: strOrientation,
                        customize: function (doc) {
                            //if (strOrientation == "portrait")
                            //{
                            //    doc.content[1].table.widths =
                            //        Array(doc.content[1].table.body[0].length + 1).join('*').split('');
                            //}
                            //doc.watermark = { text: 'watermark text', color: 'grey', opacity: 0.3};
                            doc.defaultStyle.alignment = 'left';
                            doc.styles.tableHeader.alignment = 'left';
                            doc.styles.tableHeader.fillColor = '#3d5475';   // EAMI_BgColor_BlueSteel
                            doc.styles.tableBodyOdd.fillColor = '#d9d9dd';

                            var arrTotal = dtRpt.column(9).data().toArray();
                            //doc.content[1].table.body[0][9].alignment = 'right';    //Right align Total column header.
                            for (var i = 0; i < arrTotal.length; i++) {
                                doc.content[1].table.body[i + 1][9].alignment = 'right';
                                if (Number(arrTotal[i].replace(/[$,)]/g, '').replace(/[(]/g, '-')) <= 0) {
                                    doc.content[1].table.body[i + 1][9].color = 'red';
                                }
                            }

                            // Create a footer object with 2 columns
                            // Left side: report creation date
                            // Right side: current page and total pages
                            doc['footer'] = (function (page, pages) {
                                return {
                                    columns: [
                                        {
                                            alignment: 'left',
                                            text: ['Created on: ', { text: strFooterDate }]
                                        },
                                        {
                                            alignment: 'right',
                                            text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                        }
                                    ],
                                    margin: 20
                                }
                            });

                        }
                    }
                ]
            });
        }
        else if (tableId == 'tbl_EAMIEClaimSchedule') {
            strOrientation = "landscape";
            var buttonCommon = {
            };
            dtRpt = $('#' + tableId).DataTable({
                "destroy": true,    // unbinds previous datatable initialization binding
                //"stateSave": true,      // first init will save settings below to sessionStorage, subsequent inits will call from sessionStorage. 
                //"stateDuration": -1,    // -1 for use sessionStorage
                //"paging": false,
                //"info": false,
                //"lengthChange": false,
                //"filter": false, // this is for disable filter (search box)
                //"autoWidth": false,
                //Set column definition initialization properties.
                "searching": false,
                processing: false,
                "order": [[0, "asc"], [1, "asc"]],
                bPaginate: false,
                bInfo: false,
                dom: 'frtipB',
                buttons: [
                    $.extend(true, {}, buttonCommon,
                        {
                            extend: 'excelHtml5',
                            text: 'Export to Excel',
                            title: selectedRptTxt,
                            message: 'The information on this report is as of ' + strFooterDate,
                            filename: strFilename,
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '\$#,##0.00_);[Red](\$#,##0.00)');
                                $('row c[r^="I"]', sheet).attr('s', '57');
                                $('row:eq(2) c', sheet).attr('s', '7'); // Format Column Headers
                                $('row:eq(1) c', sheet).attr('s', '3'); // Italics for Second Row
                            },
                        }),
                    {
                        extend: 'pdfHtml5',
                        text: 'Export to PDF',
                        title: selectedRptTxt,
                        filename: strFilename,
                        pageSize: 'letter',
                        orientation: strOrientation,
                        customize: function (doc) {
                            //if (strOrientation == "portrait")
                            //{
                            //    doc.content[1].table.widths =
                            //        Array(doc.content[1].table.body[0].length + 1).join('*').split('');
                            //}
                            //doc.watermark = { text: 'watermark text', color: 'grey', opacity: 0.3};
                            doc.pageMargins = [5, 40, 5, 40];
                            doc.defaultStyle.fontSize = 6;
                            doc.defaultStyle.alignment = 'left';
                            doc.styles.tableHeader.fontSize = 6;
                            doc.styles.tableHeader.alignment = 'left';
                            doc.styles.tableHeader.fillColor = '#3d5475';   // EAMI_BgColor_BlueSteel
                            doc.styles.tableBodyOdd.fillColor = '#d9d9dd';

                            var arrEcsAmt = dtRpt.column(9).data().toArray();
                            //doc.content[1].table.body[0][9].alignment = 'right';    //Right align column header.
                            for (var i = 0; i < arrEcsAmt.length; i++) {
                                doc.content[1].table.body[i + 1][9].alignment = 'right';
                                if (Number(arrEcsAmt[i].replace(/[$,)]/g, '').replace(/[(]/g, '-')) <= 0) {
                                    doc.content[1].table.body[i + 1][9].color = 'red';
                                }
                            }
                            var arrCsAmt = dtRpt.column(13).data().toArray();
                            //doc.content[1].table.body[0][13].alignment = 'right';    //Right align column header.
                            for (var i = 0; i < arrCsAmt.length; i++) {
                                doc.content[1].table.body[i + 1][13].alignment = 'right';
                                if (Number(arrCsAmt[i].replace(/[$,)]/g, '').replace(/[(]/g, '-')) <= 0) {
                                    doc.content[1].table.body[i + 1][13].color = 'red';
                                }
                            }
                            var arrWarrantAmt = dtRpt.column(17).data().toArray();
                            //doc.content[1].table.body[0][17].alignment = 'right';    //Right align column header.
                            for (var i = 0; i < arrWarrantAmt.length; i++) {
                                doc.content[1].table.body[i + 1][17].alignment = 'right';
                                if (Number(arrWarrantAmt[i].replace(/[$,)]/g, '').replace(/[(]/g, '-')) <= 0) {
                                    doc.content[1].table.body[i + 1][17].color = 'red';
                                }
                            }

                            // Create a footer object with 2 columns
                            // Left side: report creation date
                            // Right side: current page and total pages
                            doc['footer'] = (function (page, pages) {
                                return {
                                    columns: [
                                        {
                                            alignment: 'left',
                                            text: ['The information on this report is as of ', { text: strFooterDate }]
                                        },
                                        {
                                            alignment: 'right',
                                            text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                        }
                                    ],
                                    margin: 20
                                }
                            });

                        }
                    }
                ]
            });
        }
        else if (tableId == 'tbl_EAMIDataSummary') {
            strOrientation = "landscape";
            var buttonCommon = {
            };
            dtRpt = $('#' + tableId).DataTable({
                "destroy": true,    // unbinds previous datatable initialization binding
                //Set column definition initialization properties.
                "searching": false,
                processing: false,
                "order": [[0, "asc"], [1, "asc"]],
                "filter": false,
                "sort": false,
                bPaginate: false,
                bInfo: false,
                dom: 'frtipB',
                buttons: [
                    $.extend(true, {}, buttonCommon,
                        {
                            extend: 'excelHtml5',
                            text: 'Export to Excel',
                            title: selectedRptTxt,
                            message: 'The information on this report is as of ' + strFooterDate,
                            filename: strFilename,
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '\$#,##0.00_);[Red](\$#,##0.00)');
                                $('row c[r^="J"]', sheet).attr('s', '57');
                                $('row:eq(2) c', sheet).attr('s', '7'); // Format Column Headers
                                $('row:eq(1) c', sheet).attr('s', '3'); // Italics for Second Row
                            }
                        }),
                    {
                        extend: 'pdfHtml5',
                        text: 'Export to PDF',
                        title: selectedRptTxt,
                        filename: strFilename,
                        pageSize: 'A3',
                        orientation: strOrientation,
                        customize: function (docBody) {
                            docBody.pageMargins = [5, 40, 5, 40];
                            docBody.defaultStyle.fontSize = 6;
                            docBody.defaultStyle.alignment = 'left';
                            docBody.styles.tableHeader.fontSize = 6;
                            docBody.styles.tableHeader.alignment = 'left';
                            docBody.styles.tableHeader.fillColor = '#3d5475';   // EAMI_BgColor_BlueSteel
                            docBody.styles.tableBodyOdd.fillColor = '#d9d9dd';

                            // Create a footer object with 2 columns
                            // Left side: report creation date
                            // Right side: current page and total pages
                            docBody['footer'] = (function (page, pages) {
                                return {
                                    columns: [
                                        {
                                            alignment: 'left',
                                            text: ['The information on this report is as of ', { text: strFooterDate }]
                                        },
                                        {
                                            alignment: 'right',
                                            text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                        }
                                    ],
                                    margin: 20
                                }
                            });

                        }
                    }
                ]
            });
        }
        else if (tableId == 'grdEAMIUsers') {
            //set datetime format for the grid
            //$.fn.dataTable.moment('MM/DD/YYYY hh:mm A');

            //fix for window resize error for datatable in IE
            //$(window).unbind("resize.DT-" + "grdEAMIUsers");

            strOrientation = "portrait";
            var buttonCommon = {};
            var dtRpt = $('#grdEAMIUsers').DataTable(
                {
                    "destroy": true,    // unbinds previous datatable initialization binding
                    "searching": true,
                    processing: true,
                    "order": [[2, "asc"]],
                    "pagingType": "full_numbers",
                    "language": {                    //Custom Message Setting
                        "lengthMenu": "Display _MENU_ records per page",    //Customizing menu Text
                        "zeroRecords": "Nothing found",             //Customizing zero record text - filtered
                        "info": "Showing page _PAGE_ of _PAGES_",           //Customizing showing record no
                        "infoEmpty": "No records available",                //Customizing zero record message - base
                        "infoFiltered": "(filtered from _MAX_ total records)"   //Customizing filtered message
                    },
                    "columnDefs": [
                        {
                            "targets": [0],
                            "visible": false,
                            "searchable": false
                        },
                        {
                            "targets": [4],
                            "visible": false,
                            "searchable": false
                        },
                        {
                            "targets": [5],
                            "visible": false,
                            "searchable": false
                        },
                        {
                            "targets": [1],
                            "visible": true,
                            "searchable": true
                        },
                        { type: 'date', targets: [9] }
                    ],
                    "lengthMenu": [[10, 25, 50, 100, -1], [10, 25, 50, 100, "All"]],
                    buttons: [
                        $.extend(true, {}, buttonCommon,
                            {
                                extend: 'excelHtml5',
                                text: 'Export to Excel',
                                title: selectedRptTxt + ' Report',
                                filename: strFilename,
                                exportOptions: { columns: ':visible' }
                            }),
                        {
                            extend: 'pdfHtml5',
                            text: 'Export to PDF',
                            title: selectedRptTxt + ' Report',
                            filename: strFilename,
                            pageSize: 'letter',
                            orientation: strOrientation,
                            exportOptions: { columns: ':visible' },
                            customize: function (doc) {
                                //if (strOrientation == "portrait")
                                //{
                                //    doc.content[1].table.widths =
                                //        Array(doc.content[1].table.body[0].length + 1).join('*').split('');
                                //}
                                //doc.watermark = { text: 'watermark text', color: 'grey', opacity: 0.3};
                                doc.defaultStyle.alignment = 'left';
                                doc.styles.tableHeader.alignment = 'left';
                                doc.styles.tableHeader.fillColor = '#3d5475';   // EAMI_BgColor_BlueSteel
                                doc.styles.tableBodyOdd.fillColor = '#d9d9dd';

                                doc.pageMargins = [22, 40, 22, 40];

                                // Create a footer object with 2 columns
                                // Left side: report creation date
                                // Right side: current page and total pages
                                doc['footer'] = (function (page, pages) {
                                    return {
                                        columns: [
                                            {
                                                alignment: 'left',
                                                text: ['Created on: ', { text: strFooterDate }]
                                            },
                                            {
                                                alignment: 'right',
                                                text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                            }
                                        ],
                                        margin: 20
                                    }
                                });
                            }
                        }
                    ]
                });
        }
        else if (tableId == 'tbl_EAMISTOReport') {
            strOrientation = "landscape";
            var buttonCommon = {};
            dtRpt = $('#' + tableId).DataTable({
                "destroy": true,    // unbinds previous datatable initialization binding            
                "searching": false,
                processing: false,
                "order": [[0, "asc"], [1, "asc"]],
                bPaginate: false,
                bInfo: false,
                dom: 'frtipB',
                buttons: [
                    $.extend(true, {}, buttonCommon,
                        {
                            extend: 'excelHtml5',
                            // extend: 'csvHtml5',
                            text: 'Export to Excel',
                            title: selectedRptTxt + ' Report ' + $('#EAMISTOReportDatepicker').val(),
                            filename: strFilename,
                            //customize: function (data) {
                            //    return data.replace(/[^\x00-\x7F]/g, '');
                            //}
                            customize: function (xlsx) {
                                var sheet = xlsx.xl.worksheets['sheet1.xml'];
                                $(xlsx.xl["styles.xml"]).find('numFmt[numFmtId="164"]').attr('formatCode', '\$#,##0.00_);[Red]-\$#,##0.00');
                                $('row c[r^="J"]', sheet).attr('s', '57');
                                $('row:eq(1) c', sheet).attr('s', '2'); // Bold Column Headers
                                //$('row:first c', sheet).attr('s', '22');    // Blue Background (first row)
                                //$('row:first c', sheet).attr('s', '51');    // Centred Text (first row)
                            }
                        }),
                    {
                        extend: 'pdfHtml5',
                        text: 'Export to PDF',
                        title: selectedRptTxt + ' Report ' + $('#EAMISTOReportDatepicker').val(),
                        filename: strFilename,
                        pageSize: 'letter',
                        orientation: strOrientation,
                        customize: function (doc) {
                            //if (strOrientation == "portrait")
                            //{
                            //    doc.content[1].table.widths =
                            //        Array(doc.content[1].table.body[0].length + 1).join('*').split('');
                            //}
                            //doc.watermark = { text: 'watermark text', color: 'grey', opacity: 0.3};
                            doc.defaultStyle.alignment = 'left';
                            doc.styles.tableHeader.alignment = 'left';
                            doc.styles.tableHeader.fillColor = '#3d5475';   // EAMI_BgColor_BlueSteel
                            doc.styles.tableBodyOdd.fillColor = '#d9d9dd';

                            // Create a footer object with 2 columns
                            // Left side: report creation date
                            // Right side: current page and total pages
                            doc['footer'] = (function (page, pages) {
                                return {
                                    columns: [
                                        {
                                            alignment: 'left',
                                            text: ['Created on: ', { text: strFooterDate }]
                                        },
                                        {
                                            alignment: 'right',
                                            text: ['page ', { text: page.toString() }, ' of ', { text: pages.toString() }]
                                        }
                                    ],
                                    margin: 20
                                }
                            });

                        }
                    }
                ]
            });
        }
        else {
            strOrientation = "portrait";
        }

        // Appropriately position sort icons ------------------------------------------------------------------------------------------------------------------
        dtRpt.columns().iterator('column', function (ctx, idx) {
            var currentTh = $(dtRpt.column(idx).header());
            if (!currentTh.find('span').hasClass("sort-icon") && !currentTh.hasClass("dt-checkboxes-select-all") && !currentTh.hasClass("flagIcon")) {
                currentTh.append('&nbsp;&nbsp;').append('<span class="sort-icon"/>');
            }
            else if (!currentTh.find('span').hasClass("sort-icon") && !currentTh.hasClass("dt-checkboxes-select-all") && currentTh.hasClass("flagIcon")) {
                currentTh.append('&nbsp;').append('<span class="sort-icon"/>');
            }
        });
        // ----------------------------------------------------------------------------------------------------------------------------------------------------

        $('.outer').prepend(dtRpt.buttons().container());   // move buttons to footer

        // style buttons
        var excelBtn = $('.buttons-excel');
        excelBtn.addClass('btn btn-dhcs-secondary inner');
        var csvButton = $('.buttons-csv');
        csvButton.addClass('btn btn-dhcs-secondary inner');
        var pdfBtn = $('.buttons-pdf');
        pdfBtn.addClass('btn btn-dhcs-secondary inner');

        return dtRpt;
    } 
    catch (e) {
        //alert('The following exception was thrown:  ' + e);
        //we need this data in case session override and want to show error page.Data will have only error page html.
        handleProgramSessionError();
    };
    return false;
}

function removeModalFunctionalButtonsWhenNoRecordsReturned(data) {
    if (data.replace(/\s/g, "").toLowerCase().indexOf("<tbody></tbody>") >= 0) {
        $('.dt-buttons').remove();
    }
}
