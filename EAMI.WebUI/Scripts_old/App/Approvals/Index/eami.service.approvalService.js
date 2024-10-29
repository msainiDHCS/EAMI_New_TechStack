eami.service.approvalService = (function () {

    var controller = "Approvals";

    /**************************************************************/
    //Pending Return Service Calls
    /**************************************************************/
    function getPRSearchResultDropdownvalues(type, parentValue, childValue) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/GetPendingReturnFilterValues'),
            //data: { 'type': type, 'parentValue': JSON.stringify(parentValue), 'childValue': JSON.stringify(childValue) },
            data: { 'type': type, 'parentValue': parentValue, 'childValue': childValue },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function getPRSearchResults(system, selectedPayeeValues, selectedPaymentValues, selectedContractValues) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PendingReturnSuperGroup'),
            data: { 'system': system, 'payeeNameIDs': selectedPayeeValues, 'paymentTypeValues': selectedPaymentValues, 'contractNumberValues': selectedContractValues },
            dataType: 'html',
            cache: false
            //async: false
        });
    }

    function getPRPaymentGroup(dataid, paymentGroupName) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PendingReturnPaymentGroup'),
            data: { 'id': dataid, 'paymentGroupName': paymentGroupName },
            dataType: 'html',
            cache: false,
            async: false
        });
    }

    function getPRPaymentRecord(dataid, topGroupId, paymentGroupName) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PendingReturnPaymentRecord'),
            data: { 'paymentRecordSetNumber': dataid, 'topGroupID': topGroupId, 'paymentGroupName': paymentGroupName },
            dataType: 'html',
            cache: false
            //async: false
        });
    }

    function approvePR(paymentRecordSet, note) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PendingReturnApprovePaymentGroup'),
            data: { 'paymentRecordSet': paymentRecordSet, 'note': note },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function denyPR(paymentRecordSet, note) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PendingReturnDenyPaymentGroup'),
            data: { 'paymentRecordSet': paymentRecordSet, 'note': note },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function getPRFunding(paymentRecordID, topGroupID, paymentSetNumber) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PendingReturnFundingDetails'),
            data: { 'paymentRecordID': paymentRecordID, 'topGroupID': topGroupID, 'parentPaymentRecordSetNumber': paymentSetNumber },
            dataType: 'html',
            cache: false,
            async: false
        });
    }
    /**************************************************************/


    /**************************************************************/
    //Pending Claim Schedule Service Calls
    /**************************************************************/

    function getPCSSearchResultDropdownvalues(type, parentValue, childValue) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/GetPendingClaimScheduleFilterValues'),
            //data: { 'type': type, 'parentValue': JSON.stringify(parentValue), 'childValue': JSON.stringify(childValue) },
            data: { 'type': type, 'parentValue': parentValue, 'childValue': childValue },
            dataType: 'json',
            cache: false,
            async: false
        });
    }


    function getPCSSearchResults(system, selectedPayeeValues, selectedPaymentValues, selectedContractValues, selectedPayDateValue) {        
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSSuperGroup'),
            data: { 'system': system, 'payeeNameIDs': selectedPayeeValues, 'paymentTypeValues': selectedPaymentValues, 'contractNumberValues': selectedContractValues, 'payDateValue': selectedPayDateValue },
            dataType: 'html',
            cache: false
            //async: false
        });
    }

    function approveCS(claimScheduleIds) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSApprove'),
            data: { 'claimScheduleIds': claimScheduleIds },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function denyCS(claimScheduleId, note) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSDeny'),
            data: { 'claimScheduleId': claimScheduleId, 'note': note },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function getPCSRemittanceAdvice(csId) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSRemittanceAdvice'),
            data: { 'csID': csId },
            dataType: 'html',
            cache: false,
            async: false
        });
    }

    function pcsSaveRemittanceNote(claimScheduleId, note) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSSaveRemittanceNote'),
            data: { 'claimScheduleId': claimScheduleId, 'note': note },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function getPCSPaymentGroup(csUniqueNumber, paymentGroupName) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSPaymentGroup'),
            data: { 'csUniqueNumber': csUniqueNumber, 'paymentGroupName': paymentGroupName },
            dataType: 'html',
            cache: false,
            async: false
        });
    }

    function getPCSPaymentRecord(csUniqueNumber, paymentRecordSetNumber, payemntGroupId, paymentGroupName) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSPaymentRecord'),
            data: { 'csUniqueNumber': csUniqueNumber, 'paymentRecordSetNumber': paymentRecordSetNumber, 'payemntGroupId': payemntGroupId, 'paymentGroupName': paymentGroupName },
            dataType: 'html',
            cache: false
            //async: false
        });
    }

    function getPCSFunding(csUniqueNumber, paymentRecordID, parentPaymentRecordSetNumber) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSFundingDetails'),
            data: { 'csUniqueNumber': csUniqueNumber, 'paymentRecordID': paymentRecordID, 'parentPaymentRecordSetNumber': parentPaymentRecordSetNumber },
            dataType: 'html',
            cache: false,
            async: false
        });
    }

    function getPCSFundingSummary(claimScheduleIds) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PCSFundingSummary'),
            data: { 'claimScheduleIds': claimScheduleIds },
            dataType: 'html',
            cache: false,
            async: false
        });
    }



    /**************************************************************/
    //Electroni Claim Schedule Service Calls
    /**************************************************************/

    function getECSSearchResults(system, fromDate, toDate, statusTypeId) {        
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/ECSSearchView'),
            data: { 'system': system, 'fromDate': fromDate, 'toDate': toDate, 'statusTypeId': statusTypeId},
            dataType: 'html',
            cache: false
            //async: false
        });
    }

    function getECSChildRecord(ecsId, fromDate, toDate, statusTypeId) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/ECSChildRecord'),
            data: { 'ecsId': ecsId, 'fromDate': fromDate, 'toDate': toDate, 'statusTypeId': statusTypeId },
            dataType: 'html',
            cache: false,
            async: false
        });
    }

    function pendingECS(ecsId, note) {       
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/PendingECS'),
            data: { 'ecsId': ecsId, 'note': note },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

   
    function approveECS(ecsId, note) {
        
        
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/ApproveECS'),
            data: { 'ecsId': ecsId, 'note': note },
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function deleteECS(ecsId, note) {
        return $.ajax({
            type: "POST",
            url: getEAMIAbsoluteUrl('~/' + controller + '/DeleteECS'),
            data: { 'ecsId': ecsId, 'note': note},
            dataType: 'json',
            cache: false,
            async: false
        });
    }

    function getFaceSheet(ecsId) {
        return $.ajax({
            url: getEAMIAbsoluteUrl('~/Reports/GetFaceSheet?ecsID=' + ecsId),
            type: 'GET',
            datatype: "html",
            cache: false,
            async: false
        });
    }

    function getTransferLetter(ecsId) {
        return $.ajax({
            url: getEAMIAbsoluteUrl('~/Reports/GetTransferLetter?ecsID=' + ecsId),
            type: 'GET',
            datatype: "html",
            cache: false,
            async: false          
        });
    }

    /**************************************************************/
    return {
        getPRSearchResultDropdownvalues: getPRSearchResultDropdownvalues,
        getPRSearchResults: getPRSearchResults,
        getPRPaymentGroup: getPRPaymentGroup,
        getPRPaymentRecord: getPRPaymentRecord,
        approvePR: approvePR,
        denyPR: denyPR,
        getPRFunding: getPRFunding,
        getPCSSearchResultDropdownvalues: getPCSSearchResultDropdownvalues,
        getPCSSearchResults: getPCSSearchResults,
        approveCS: approveCS,
        denyCS: denyCS,
        getPCSRemittanceAdvice: getPCSRemittanceAdvice,
        pcsSaveRemittanceNote: pcsSaveRemittanceNote,
        getPCSPaymentGroup: getPCSPaymentGroup,
        getPCSPaymentRecord: getPCSPaymentRecord,
        getPCSFunding: getPCSFunding,
        getPCSFundingSummary: getPCSFundingSummary,
        getECSSearchResults: getECSSearchResults,
        getECSChildRecord: getECSChildRecord,
        approveECS: approveECS,
        pendingECS: pendingECS,
        deleteECS: deleteECS,
        getFaceSheet: getFaceSheet,
        getTransferLetter: getTransferLetter
    };

}(eami.service.approvalService || {}));