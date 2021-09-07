//Ultility Functions
function isEmpty(value) {
    return (typeof value === "undefined" || value === null);
}

function isEmptyOrBlank(value) {
    return (typeof value === "undefined" || value === null || value === "");
}

//Ajax wrapper start
function baseAjaxCall(type, url, data, response, async) {
    if (isEmpty(async))
        async = true;

    $.ajax({
        type: type,
        url: url,
        data: data,
        async: async,
        contentType: "application/json; charset=utf-8",
        datatype: 'json',
        success: function (data) {
            response(data);
        },
        error: function (err) {
            debugger
            console.log(err);
            if (err.status == 5000) {
                ShowAlertBoxError("Internal server error");
            }
            if (err.state() == "rejected") {
                throw err;
                //window.location.reload();
            }
        }
    });
}
function getAjax(url, data, response) {
    baseAjaxCall('GET', url, data, response);
}
function getAjaxSync(url, data, response) {
    baseAjaxCall('GET', url, data, response, false);
}
function postAjax(url, data, response) {
    baseAjaxCall('POST', url, data, response);
}
function postAjaxSync(url, data, response) {
    baseAjaxCall('POST', url, data, response, false);
}
function deleteAjax(url, data, response) {
    baseAjaxCall('DELETE', url, data, response);
}
function putAjax(url, data, response) {
    baseAjaxCall('PUT', url, data, response);
}
function patchAjax(url, data, response) {
    baseAjaxCall('PATCH', url, data, response);
}
//Ajax wrapper end

//Alerts Start
function ShowAlertBoxBase(title, text, type, callback) {
    if (isEmptyOrBlank(callback))
        callback = function () { }

    sweetAlert
        ({
            title: title, //"Exist!",
            text: text,//"This User Role is Exist !",
            type: type //"warning"
        }, callback);
}
function ShowAlertBoxInfo(title, text, callback = null) {
    ShowAlertBoxBase(title, text, 'info', callback);
}
function ShowAlertBoxSuccess(title, text, callback = null) {
    ShowAlertBoxBase(title, text, 'success', callback);
}
function ShowAlertBoxWarning(title, text, callback = null) {
    ShowAlertBoxBase(title, text, 'warning', callback);
}
function ShowAlertBoxError(title, text, callback = null) {
    ShowAlertBoxBase(title, text, 'error', callback);
}
//Alerts End

//select2 start
function addMultipleSelectValues(selector, valuesSpaceSeprated) {
    if (!isEmptyOrBlank(valuesSpaceSeprated)) {
        let el = $(selector);
        el.val(valuesSpaceSeprated.split(' '));
        el.trigger('change');
    }
}
//select2 end
var getSampleBGImageByFileExtension = function (fileExtension) {
    fileExtension = fileExtension.replace(".", "").toUpperCase();
    let bgimage = "";
    if (fileExtension == 'PDF') {
        bgimage = '../../assets/img/kanban/I_PDF.png';
    }
    else if (fileExtension == 'DOCX' || fileExtension == 'DOC') {
        bgimage = '../../assets/img/kanban/I_Doc.png';
    }
    else if (fileExtension == 'RAR' || fileExtension == 'ZIP' || fileExtension == '7Z') {
        bgimage = '../../assets/img/kanban/I_Zip.png';
    }
    else if (fileExtension == 'XLSX' || fileExtension == 'XLS') {
        bgimage = '../../assets/img/kanban/I_XLS.png';
    }
    else if (fileExtension == 'TXT' || fileExtension == 'txt') {
        bgimage = '../../assets/img/kanban/I_TXT.png';
    }
    else if (fileExtension == 'CSV' || fileExtension == 'csv') {
        bgimage = '../../assets/img/kanban/I_CSV.png';
    }
    return bgimage;
}

