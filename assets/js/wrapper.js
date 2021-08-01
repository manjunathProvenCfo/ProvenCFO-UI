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
        datatype: 'json',
        success: function (data) {
            response(data);
        },
        error: function (err) {
            debugger
            console.log(err);
            showErrorAlert(err);
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
    baseAjaxCall('GET', url, data, response,false);
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

//TODO:Implement
function showErrorAlert() {

}
