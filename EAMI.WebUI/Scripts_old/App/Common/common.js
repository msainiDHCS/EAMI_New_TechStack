var DefaultDropDownText = "Please select...";
var DefaultDropDownValue = "0";

var DefaultMessageDropDownText = "ALL";
var DefaultMessageDropDownValue = "-1";

var IsDefaultDropDownEntryNeeded = true;

function get_browser_info() {
    var ua = navigator.userAgent, tem, M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*(\d+)/i) || [];
    if (/trident/i.test(M[1])) {
        tem = /\brv[ :]+(\d+)/g.exec(ua) || [];
        return { name: 'IE', version: (tem[1] || '') };
    }
    if (M[1] === 'Chrome') {
        tem = ua.match(/\bOPR\/(\d+)/)
        if (tem != null) { return { name: 'Opera', version: tem[1] }; }
    }
    M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
    if ((tem = ua.match(/version\/(\d+)/i)) != null) { M.splice(1, 1, tem[1]); }
    return {
        name: M[0],
        version: M[1]
    };
}

function extractDomain(url) {
    var domain;
    //find & remove protocol (http, ftp, etc.) and get domain
    if (url.indexOf("://") > -1) {
        domain = url.split('/')[2];
    }
    else {
        domain = url.split('/')[0];
    }

    //find & remove port number
    domain = domain.split(':')[0];

    return domain;
}

//here input ddlInput is of type dropdown
//please make sure to call this before filling any other entries
function EAMIAdminFillDefaultEntryForDropdown(ddlInput) {
    if ($(ddlInput) != null) {
        $(ddlInput).html('');
        $(ddlInput).append("<option value='" + DefaultDropDownValue + "'>" + DefaultDropDownText + "</option>");
        return ddlInput;
    }
}

function EAMIMessageFillDefaultEntryForDropdown(ddlInput) {
    if ($(ddlInput) != null) {
        $(ddlInput).html('');
        $(ddlInput).append("<option value='" + DefaultMessageDropDownValue + "'>" + DefaultMessageDropDownText + "</option>");
        return ddlInput;
    }
}

function RemoveBSAndTildaFromUrl(inputURL)//url be like Search/RunSearch etc
{
    var formattedURL = inputURL;

    while (formattedURL != null && formattedURL.length >= 1 && (formattedURL.substring(0, 1) == '/' || formattedURL.substring(0, 1) == '~' || formattedURL.substring(0, 1) == '.')) {
        formattedURL = formattedURL.replace(formattedURL.substring(0, 1), '');//replace first occurence
    }

    return formattedURL;
}

function getEAMIAbsoluteUrl(inputPartialUrl) {

    inputPartialUrl = RemoveBSAndTildaFromUrl(inputPartialUrl);

    var baseUrl = window.location.protocol + "//" + window.location.host + "/" + settings.baseUrl;

    return baseUrl + inputPartialUrl;

}

function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);
}

function setFormStatusMessage(formName, divcontainerName, isSuccess, message, gridColumnCount) {
    var container = $('#' + formName).find('#' + divcontainerName);

    var contentHtml = "<div class='row' style='margin-top:10px;'><div class='col-xs-" + gridColumnCount + "'><div class='alert alert-success' role='alert' style='display:none;'>";
    contentHtml = contentHtml + "</div><div class='alert alert-danger' role='alert' style='display:none;'></div></div></div>";
    $(container).html(contentHtml);

    if (container) {
        var messageDivSuccess = $(container).find('.alert-success');
        var messageDivError = $(container).find('.alert-danger');

        if (isSuccess) {
            $(container).show(); $(messageDivSuccess).show(); $(messageDivSuccess).html(message);
            $(messageDivError).hide();
        }
        else {
            $(container).show(); $(messageDivSuccess).hide();
            $(messageDivError).show(); $(messageDivError).html(message);
        }
    }
}

function getUrlJsonSync(url) {

    var jqxhr = $.ajax({
        type: "GET",
        url: url,
        dataType: 'json',
        cache: false,
        async: false
    });

    // 'async' has to be 'false' for this to work
    var response = { valid: jqxhr.statusText, data: jqxhr.responseJSON };

    return response;
}

function postUrlJsonSync(url) {

    var jqxhr = $.ajax({
        type: "POST",
        url: url,
        dataType: 'json',
        cache: false,
        async: false
    });

    // 'async' has to be 'false' for this to work
    var response = { valid: jqxhr.statusText, data: jqxhr.responseJSON };

    return response;
}

function HideFormStatusMessage(formName, divcontainerName) {
    var container = $('#' + formName).find('#' + divcontainerName);
    if (container) {
        $(container).hide();
    }
}

function getParameterByName(url, idtofindvaluefor) {
    idtofindvaluefor = idtofindvaluefor.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + idtofindvaluefor + "=([^&#]*)"),
        results = regex.exec(url);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function EAMIShowAjaxLoadingContent(divName) {
    $('#' + divName).html('<h1 class="ajax-loading-animation" style=\'padding-left:10px;\'><i class="fa fa-cog fa-spin"></i> Loading...</h1>');
}

function EAMIShowAjaxLoadingContentSmall(divName) {
    $('#' + divName).html('<h1 class="ajax-loading-animation-small" style=\'padding-left:10px;\'><i class="fa fa-cog fa-spin"></i> Loading...</h1>');
}

function Clear_Form(containerName) {

    var eleName = '#' + containerName;

    $(eleName).find(':input').each(function () {

        var isdisabled = $(this).is(':disabled');

        switch (this.type) {
            case 'password':
            case 'select-multiple':
            case 'select-one':
            case 'text':
            case 'tel':
            case 'email':
            case 'textarea':
                $(this).val('');
                break;
            case 'checkbox':
            case 'radio':
                this.checked = false;
        }
    });
}

function EAMIControlAllowOnlyAlphabets(kCode) {
    if ((kCode > 64 && kCode < 91) || (kCode > 96 && kCode < 123)) {
        return true;
    }
    else {
        event.keyCode = 0
        return false;
    }
};


function EAMIControlAllowOnlyAlphabetsWithSpace(kCode) {

    if ((kCode > 64 && kCode < 91) || (kCode > 96 && kCode < 123) || kCode == 32) {
        return true;
    }
    else {
        event.keyCode = 0
        return false;
    }
};

function getFirstDayOfMonth(date) {
    var dateArray = date.split("/");
    var newJsDate = new Date(dateArray[0] + "/01/" + dateArray[1]);
    return formatDate(newJsDate);
}

function formatDate(jsDate) {
    return ('0' + (jsDate.getMonth() + 1)).slice(-2) + '/' + ('0' + jsDate.getDate()).slice(-2) + '/' + jsDate.getFullYear();
}

function handleAjaxErrorReturned(response) {
    showErrorOnWholePage(response);
}

function showErrorOnWholePage(response) {
    //$("#LayoutBody > #container > #body > .wrapper").html(response);
    $("#LayoutBody > #container > #body > #outtermostWrapper").html(response);

}

function closePopUpsAndClearNav() {
    $('.modal').modal('hide') // closes all active pop ups.
    $('.modal-backdrop').remove() // removes the grey overlay.    
    $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
    $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
}

function handleProgramSessionError() {
    $('.modal-backdrop').removeClass('in'); // removes the grey overlay.    
    $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
    $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");

    //disable Download Report in Data Summary report
    $("#btnDownloadReport").prop("disabled", true);

    //To disable FS and TL Export buttons in case of any error... uncomment below lines if we want to show error msg on modal pop-up
    //document.getElementById('lnkPDF').style.pointerEvents = "none";
    //document.getElementById('lnkPDF').style.cursor = "default";
}

$(document).ready(function () {
});


$(document).ready(function () {
    $("#show_success").on("click", function (e) {
        $("#success_alert").slideDown().delay(5000).slideUp();
    });

    $("#show_warning").on("click", function (e) {
        $("#warning_alert").slideDown().delay(5000).slideUp();
    });

    $("#show_error").on("click", function (e) {
        $("#danger_alert").slideDown().delay(5000).slideUp();
    })
});

