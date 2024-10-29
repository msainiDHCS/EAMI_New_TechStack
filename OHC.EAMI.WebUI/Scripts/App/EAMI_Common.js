function parseJsonDate(jsonDateString) {
    return $.datepicker.formatDate('mm/dd/yy', new Date(parseInt(jsonDateString.replace('/Date(', ''))));
}

function GetSelectedOptionsAsArray(idDropDownList, boolWantValues, boolWantText) {
    var options = $("#" + idDropDownList + " option:selected");
    var retArray = null;
    if (boolWantValues) {
        var values = $.map(options, function (option) {
            return option.value;
        });
        retArray = values;
    }
    else if (boolWantText) {
        var texts = $.map(options, function (option) {
            return option.text;
        });
        retArray = texts;
    }
    else {  //catch-all
        var texts = $.map(options, function (option) {
            return option.text;
        });
        retArray = texts;
    }
    return retArray;
}

//to handle session override issue when user clicks on buttons on popup ex hold/unhold.
function ajax_ErrorCallBack() {
    var retHtmlString = '';//'<div class="panel panel-default modal-content" style="width:97%;min-height:250px;margin-left:20px;margin-right:20px;margin-top:20px;" id="divuuppanel">'            
    retHtmlString = retHtmlString +
        '<br/><div class="modal-header">' +
        '<h4 class="modal-title" > <span class="glyphicon glyphicon-warning-sign EAMI_Text_Danger"></span> Error</h4>' +
        '</div>' +
        '<div class="container-fluid">' +
        '<div class="alert alert-danger" style="margin-top: 20px;text-align:center;vertical-align:central;line-height:250px;" id="warning_danger">' +
        '<b>An error occured while processing your request.&nbsp;&nbsp;EAMI admin has been notified.&nbsp;&nbsp;Please click on the EAMI icon to Refresh.</b>' +
        '</div>' +
        //'</div>' +
        '</div>';
    $("#modalBodyForHold").html(retHtmlString);
    $('.modal-backdrop').removeClass('in'); // removes the grey overlay.    
    $("#LayoutBody > #container > #header .EAMI_Text_White").html("");
    $("#LayoutBody > #container > #header .EAMI_BgColor_BlueSteel").html("");
    $('#modalFooterForHold_HoldBtn').hide();
    $('#modalWrapperForHold > .modal-dialog').removeClass('modal-md');
    $('#modalWrapperForHold > .modal-dialog').addClass('modal-lg');
}

function GetJQuerySetElementsAsArray(ptrJQuerySet) {
    var options = ptrJQuerySet;
    var retArray = null;
    var texts = $.map(options, function (item) {
        return $(item).text();
    });
    retArray = texts;
    return retArray;
}

jQuery.extend(jQuery.fn.dataTableExt.oSort, {
    "eami_currency-pre": function (a) {
        a = a.replace(/[)]/g, "").replace(/[(]/g, "-");
        a = (a === "-") ? 0 : a.replace(/[^\d\-\.]/g, "");
        return parseFloat(a);
    },

    "eami_currency-asc": function (a, b) {
        return a - b;
    },

    "eami_currency-desc": function (a, b) {
        return b - a;
    }
});
