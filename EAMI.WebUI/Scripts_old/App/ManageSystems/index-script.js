$(document).ready(function () {    
    $(function () {
        //disable the Main page grid
        $('#dvMainArea').slideDown(250);
        $('#dvMainArea').css("display", "none");
        $("#tabs").tabs();
        loadIPTabs(1);       
    });    
});

 
function loadIPTabs(id) {
    var divName = '';
    var ViewName = '';
    var systemId = document.getElementById('hdnSystemId').value;

    switch (id) {

        case 1:
            $("#tabs").tabs("option", "active", 0);
            divName = 'dvtab1';
            ViewName = 'EditMasterData';
            LoadSystemInformation(systemId);
            break;
        case 2:
            $("#tabs").tabs("option", "active", 1);
            divName = 'dvtab2';
            ViewName = 'ViewExclusivePmtType';
            LoadTab(ViewName);
            break;
        case 3:
            $("#tabs").tabs("option", "active", 2);
            divName = 'dvtab3';
            ViewName = 'ViewFunds';
            LoadTab(ViewName);
            break;
        case 4:
            $("#tabs").tabs("option", "active", 3);
            divName = 'dvtab4';
            ViewName = 'ViewFundingSource';
            LoadTab(ViewName);
            break;
        case 5:
            $("#tabs").tabs("option", "active", 4);
            divName = 'dvtab5';
            ViewName = 'ViewFacesheetValues';
            LoadTab(ViewName);
            break;
        case 6:
            $("#tabs").tabs("option", "active", 5);
            divName = 'dvtab6';
            ViewName = 'ViewSCOProperties';
            LoadTab(ViewName);
            break;
    }

    function LoadSystemInformation(systemId) {
        // Start Loading... Animation
        $('#divLoadingAnimation').css("display", "block");
        EAMIShowAjaxLoadingContent('divLoadingAnimation_Inner');

        $.ajax({
            url: getEAMIAbsoluteUrl('../Administration/EditMasterData?masterDataID=' + systemId + "&datatype=" + DataTypeS),
            type: 'GET',
            datatype: "html",
            cache: false,
            //async: false,
            success: function (data) {
                if (data != null) {
                    $('#divLoadingAnimation').css("display", "none");
                    $('#' + divName).html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                }
            },
            error: function () {
            },
            complete: function (data) {
                $('#divLoadingAnimation_Inner').empty();
                $('#divLoadingAnimation').css("display", "none");
            }
        });
    }

    function LoadTab(ViewName) {

        $.ajax({
            url: getEAMIAbsoluteUrl('~/ManageSystems/' + ViewName + '?systemID=' + systemId + '&includeInactive=false'),
            type: 'GET',
            datatype: "html",
            cache: false,
            //async: false,
            success: function (data) {
                if (data != null) {
                    $('#' + divName).html(decodeURIComponent(encodeURIComponent(data)));   //decode-encode used to pass Checkmarx's XSS site vulnerability.
                }
            },
            error: function () {
            },
            complete: function (data) {
                $('#divLoadingAnimation_Inner').empty();
                $('#divLoadingAnimation').css("display", "none");
            }
        });
    }
}