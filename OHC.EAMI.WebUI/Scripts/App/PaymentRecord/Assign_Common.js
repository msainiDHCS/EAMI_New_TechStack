function rebindDtPaymentSuperGroup(outterMostTableId) {
    var dtPSG = $('#' + outterMostTableId).DataTable(
    {
        "filter": false, // this is for disable filter (search box)
        "destroy": true,    // unbinds previous datatable initialization binding
        "paging": false,
        "info": false,
        "autoWidth": false,
        "columnDefs": [
        {
            "type": "eami_currency",
            "targets": 6,
            "orderable": true
        }
        ],
        "order": [[3, "asc"]]
    });
    return dtPSG;
}

function rebindDtPaymentGroup(paymentSuperGroup_UniqueKey, boolHoldFlagOrderable, boolIHSSIconOrderable) {
    var dtPG = $('#tblPG_' + paymentSuperGroup_UniqueKey).DataTable({
        "destroy": true,    // unbinds previous datatable initialization binding
        "stateSave": true,      // first init will save settings below to sessionStorage, subsequent inits will call from sessionStorage. 
        "stateDuration": -1,    // -1 for use sessionStorage
        "paging": false,
        "info": false,
        "lengthChange": false,
        "filter": false, // this is for disable filter (search box)
        "autoWidth": false,
        //Set column definition initialization properties.
        "columnDefs": [
        {
            "targets": 0,
            "orderable": false
        }
        ,
        {
            "targets": 1,
            //"orderable": boolHoldFlagOrderable
            "orderable": true
        }
        ,
        {
            "targets": 2,
            "orderable": false
        }
        ,
        {
            "targets": 3,
            "class": "checkbox-level2",
            "checkboxes": {
                "selectRow": true
            }
        }
        ,
        {
            "type": "eami_currency",
            "targets": 8,
            "orderable": true
            }
        ]
        ,
        "select":
        {
            "style": "multi",
            "selector": "td:nth-child(4) input:checkbox"
        }
        ,
        "order": [[4, "asc"]]
    })

    $(dtPG.table().container()).addClass('no-padding');
    dtPG.rows().deselect();
    return dtPG;
}

function rebindDtPaymentGroupBasedOnFlags(paymentSuperGroup_UniqueKey) {
    return rebindDtPaymentGroup(paymentSuperGroup_UniqueKey, true, true);
}

function rebindDtPaymentRecord(paymentGroup_UniqueNumber) {
    var dtPR = $('#tblPR_' + paymentGroup_UniqueNumber).DataTable({
        "destroy": true,    // unbinds previous datatable initialization binding
        "stateSave": true,      // first init will save settings below to sessionStorage, subsequent inits will call from sessionStorage. 
        "stateDuration": -1,    // -1 for use sessionStorage
        "paging": false,
        "info": false,
        "lengthChange": false,
        "filter": false, // this is for disable filter (search box)
        "autoWidth": false,
        //Set column definition initialization properties.
        "columnDefs": [
        {
            "targets": 0,
            "orderable": false
        }
        ,
        {
            "targets": 1,
            "orderable": true
        }
        ,
        {
            "targets": 2,
            "orderable": true
        }
        ,
        {
            "type": "eami_currency",
            "targets": 3,
            "orderable": true
        }
        ]
        ,
        "order": [[3, "asc"], [1, "asc"]]
    })
    $(dtPR.table().container()).addClass('no-padding');
    return dtPR;
}