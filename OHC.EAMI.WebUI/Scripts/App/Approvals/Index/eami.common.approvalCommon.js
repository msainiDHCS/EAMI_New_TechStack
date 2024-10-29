eami.common.approvalCommon = (function () {

    function processingModal(showModal, label) {       
        if (label) {
            console.log("Here");
            $('#lblProcessingModal').empty();
            $('#lblProcessingModal').append(label);
        }
        else {
            $('#lblProcessingModal').empty();
            $('#lblProcessingModal').append("Processing");
        }
        if (showModal) {
            $('#processingModal').modal();
        }
        else {
            setTimeout(function () {
                $('#processingModal').modal('hide');
            }, 700);
        }
    }
    

    return {
        processingModal: processingModal        
    }

}(eami.common.approvalCommon || {}));