﻿const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
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
function baseFileAjaxCall(type, url, data, response, async) {
    if (isEmpty(async))
        async = true;

    $.ajax({
        type: type,
        url: url,
        data: data,
        async: async,
        contentType: false,// Not to set any content header  
        processData: false, // Not to process data        
        success: function (data) {
            response(data);
        },
        error: function (err) {

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
function postFileAjaxSync(url, data, response) {
    baseFileAjaxCall('POST', url, data, response, false);
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
function ShowConfirmBoxBase(title, text, type, confirmButtonText, confirmButtonColor, callback = null) {
    swal({
        title: title,
        text: text,
        type: type,
        showCancelButton: true,
        closeOnConfirm: false,
        confirmButtonText: confirmButtonText,
        confirmButtonColor: confirmButtonColor        
    }, callback);
}
function ShowConfirmBoxWarning(title, text, confirmButtonText, callback = null) {
    ShowConfirmBoxBase(title, text, "warning", confirmButtonText, "#ec6c62", callback);
}
function ShowConfirmBoxWarningwithYesOrNoCallback(title, text, confirmButtonText, cancelButtonText, callback = null) {
    swal({
        title: title,//"Are you sure?",
        text:  text, //"You will not be able to recover this imaginary file!",
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: confirmButtonText, // "Yes, delete it!",
        cancelButtonText: cancelButtonText, //"No, cancel plx!",
        closeOnConfirm: false,
        closeOnCancel: false       
    },callback);
}
function ShowEnterInput(title, text, inputType, inputPlacetext,Nullenterywarningmsg,callback =null) {
    swal({
        title: title,
        text: text,
        type: "input",
        inputType: inputType,
        showCancelButton: true,
        closeOnConfirm: false,
        inputPlaceholder: inputPlacetext,
    }, function (inputValue) {
        if (inputValue === false) return false;
        if (inputValue === "") {
            swal.showInputError(Nullenterywarningmsg);
            return false
        }
        callback(inputValue);
    });
}
//Alerts End

//Loader Start
function showWaitMeLoader(SELECTOR,loadingMessage) {
    $(SELECTOR).waitMe({
        //none, rotateplane, stretch, orbit, roundBounce, win8,
        //win8_linear, ios, facebook, rotation, timer, pulse,
        //progressBar, bouncePulse or img
        effect: 'win8_linear',
        text: loadingMessage,
        bg: 'rgba(0,0,0,0.75)',
        /*bg: 'rgba(255,255,255,0.7)',*/
        //color for background animation and text (string).
        color: '#000000',
        //change width for elem animation (string).
        sizeW: '',
        //change height for elem animation (string).
        sizeH: '',
        source: '',

    });
}
function hideWaitMeLoader(SELECTOR) {
    $(SELECTOR).waitMe('hide');
}
function showChatContentLoader() {
    showWaitMeLoader(".card-chat-content", "Loading...");
}
function hideChatContentLoader() {
    hideWaitMeLoader(".card-chat-content", "Loading...");
}
//Loader End

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
        bgimage = '../../assets/img/reports/pdf.png';
    }
    else if (fileExtension == 'DOCX' || fileExtension == 'DOC') {
        bgimage = '../../assets/img/kanban/I_Doc.png';
    }
    else if (fileExtension == 'RAR' || fileExtension == 'ZIP' || fileExtension == '7Z') {
        bgimage = '../../assets/img/kanban/I_Zip.png';
    }
    else if (fileExtension == 'XLSX' || fileExtension == 'XLS') {
        bgimage = '../../assets/img/reports/excel.png';
    }
    else if (fileExtension == 'TXT' || fileExtension == 'txt') {
        bgimage = '../../assets/img/kanban/I_TXT.png';
    }
    else if (fileExtension == 'CSV' || fileExtension == 'csv') {
        bgimage = '../../assets/img/kanban/I_CSV.png';
    }
    else if (fileExtension == 'PNG' || fileExtension == 'JPG' || fileExtension == 'JPEG' || fileExtension == 'TIFF' || fileExtension == 'GIF' || fileExtension == 'JFIF') {
        bgimage = '../../assets/img/reports/Image.png';
    }
    return bgimage;
}
var ConvertToUDS = function (inputAmount) {

    var usdAmount = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(inputAmount);
    return usdAmount;

}
var formatAmount = function (amount, wrapSpan) {
    if (isEmptyOrBlank(wrapSpan))
        wrapSpan = false;
    if (isEmptyOrBlank(amount))
        amount = 0;
    let usdAmount = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        currencySign: 'accounting'
    }).format(amount);
    if (wrapSpan === false)
        return usdAmount;
    if (amount < 0) {
        return `<span class='text-danger'>${usdAmount}</span>`;
    }
    else {
        return `<span>${usdAmount}</span>`;
    }
}

var formatDateMMDDYYYY = function (date) {
    if (isEmptyOrBlank(date))
        return '';
    return moment(date).format('MM/DD/YYYY');
}

function getParameterByName(name, url = window.location.href) {
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}
function removeItemByIndex(arr, index) {
    if (index > -1) {
        arr.splice(index, 1);
    }
    return arr;
}

var filterMap = function filterMap(map, pred) { 
    const result = new Map();
    for (let [k, v] of map) {
        if (pred(k, v)) {
            result.set(k, v);
        }
    }
    return result;
}
function ShowlottieLoader() {
    $("#Loader").show();
}
function HidelottieLoader() {
    $("#Loader").hide();

}
function imgError(image) {
    image.onerror = "";
    image.src = "../assets/img/team/default-logo.png";
    return true;
}


function UtcDateToLocalTime(utcDate) {

    if (utcDate != null) {
        var localTime;
        let timeZoneOffset = new Date().getTimezoneOffset();
        let utcServerDateTime = new Date(utcDate);
        let utcTimeInMilliseconds = utcServerDateTime.getTime();
        

        /*
            timeZoneOffset :minutes
         */
        switch ((timeZoneOffset > 0)) {
            case true:
                localTime = new Date(utcTimeInMilliseconds - (timeZoneOffset * 60000)); //in one minutes there is 60,000 milliseconds
                break;
            case false:
                localTime = new Date(utcTimeInMilliseconds + ((-1 * timeZoneOffset) * 60000));
                break;
        }

        return localTime;


    }

    return utcDate;
}

