// GLOBAL Ajax Settings / Event Handlers..
// Always add the X-CSRF-Token to the HTTP header for validation on the server-side. Only needed for Post, Put, and Delete...  
$(document).ajaxSend(function (event, jqXHR, settings) {
    if (settings.type.toLowerCase() != "get") {
        jqXHR.setRequestHeader('X-CSRF-Token', eami.antiForgeryToken)
    }
}).ajaxComplete(function () {
    // Always reset client-side counters after return from an AJAX call. Every hit to the web.api will reset the user session time-out on the server. 
    // So, we only need to reset the client-side counters...    
})
/*    .ajaxError(function (event, jqXHR, settings) {
    // Always redirect to error.aspx page in errors in the 500 range..    
        window.location.replace("../ErrorHandler/Index");   
    })
*/



//.on( "error",
//    function (event, jqXHR, options, exc) {


//        alert("alexajax errro2");

//        handleAjaxErrorReturned(jqXHR.responseText);
//    }
//);





;