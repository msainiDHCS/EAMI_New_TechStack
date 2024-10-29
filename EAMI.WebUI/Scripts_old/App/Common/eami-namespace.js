var eami = eami || {
    service: {},
    common: {},
    utility: {},
    baseRoute: null,
    antiForgeryToken: null    
};

eami.utility = (function () {
    function enableDisableDDL($dropdownElement, enableFlag) {
        //If Request to Enable
        if (enableFlag) {
            $dropdownElement.selectpicker('refresh');
            $dropdownElement.prop('disabled', false);
            $dropdownElement.selectpicker('refresh');
        }
            //Else Disable Element
        else {
            $dropdownElement.empty().selectpicker('refresh');
            $dropdownElement.prop('disabled', true);
            $dropdownElement.empty().selectpicker('refresh');
            $dropdownElement.prop('disabled', true);
        }
    }

    function checkInputValidity(value) {
        var regex = new RegExp("^[a-zA-Z0-9\\s-_?%.',#+&$()/\"]+$");        
        if (!regex.test(value)) {
            return false;
        }
        else {
            return true;
        }
    }

    function replaceSpecialChars(value) {
        var returnValue = '';
        if (value !== '') {
            returnValue = value;
            returnValue = replaceAll(returnValue, '&#39;', "'");
            returnValue = replaceAll(returnValue, '&amp;', "&");
            returnValue = replaceAll(returnValue, '&quot;', "\"");
        }        
        return returnValue;
    }

    function replaceAll(str, find, replace) {
        return str.replace(new RegExp(find, 'g'), replace);
    }

    return {
        enableDisableDDL: enableDisableDDL,
        checkInputValidity: checkInputValidity,
        replaceSpecialChars: replaceSpecialChars
    }

}(eami.utility || {}));