
function dic_getSDicTypeResource(callback) {

    if (dic.dicDicTypeResource) {
        if (callback) callback();
    } else if (zxc.dicDicTypeResource) {
        if (callback) zxc.dicDicTypeResource.add(callback);
    } else {
        zxc.dicDicTypeResource = $.Callbacks();
        $.post(rootUrl + 'ReportAnalyse/getSDicTypeResource', function (data) {
            if (data.ErrorMessage) {
                openAlertWindow(data.ErrorMessage);
            } else {
                dic.dicDicTypeResource = data.ListItems;
                zxc.dicDicTypeResource.fire().empty();
                if (callback) callback();
            }
        });

    }

}

function dic_getDicRstReport(callback) {

    if (dic.dicRstReport) {
        if (callback) callback();
    } else if (zxc.dicRstReport) {
        if (callback) zxc.dicRstReport.add(callback);
    } else {
        zxc.dicRstReport = $.Callbacks();
        $.post(rootUrl + 'ReportAnalyse/getDicRstReport', function (data) {
            if (data.ErrorMessage) {
                openAlertWindow(data.ErrorMessage);
            } else {
                dic.dicRstReport = data.ListItems;
                zxc.dicRstReport.fire().empty();
                if (callback) callback();
            }
        });

    }

}

function dic_getDicKato(callback) {

    if (dic.dicKato) {
        if (callback) callback();
    } else if (zxc.dicKato) {
        if (callback) zxc.dicKato.add(callback);
    } else {
        zxc.dicKato = $.Callbacks();
        $.post(rootUrl + 'ReportAnalyse/getDicKato', function (data) {
            if (data.ErrorMessage) {
                openAlertWindow(data.ErrorMessage);
            } else {
                dic.dicKato = data.ListItems;
                zxc.dicKato.fire().empty();
                if (callback) callback();
            }
        });

    }

}

function dic_getDicCato(callback) {

    if (dic.dicCato) {
        if (callback) callback();
    } else if (zxc.dicCato) {
        if (callback) zxc.dicCato.add(callback);
    } else {
        zxc.dicCato = $.Callbacks();
        $.post(rootUrl + 'ReportAnalyse/getDicKato1', function (data) {
            if (data.ErrorMessage) {
                openAlertWindow(data.ErrorMessage);
            } else {
                dic.dicCato = data.ListItems;
                zxc.dicCato.fire().empty();
                if (callback) callback();
            }
        });

    }

}
