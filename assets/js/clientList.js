
function deleteClient(Id) {

    swal({
        title: "Are you sure?",
        text: "Do you really want to delete this Client?",
        type: "warning",
        showCancelButton: true,
        closeOnConfirm: false,
        confirmButtonText: "Yes, delete it!",
        confirmButtonColor: "#ec6c62"
    },
        function () {

            postAjax('/Client/DeleteClient/' + Id, null, function (data) {

                if (data === false) {
                    ShowAlertBoxError("Couldn't Delete!", "This Client is associated with User", function () { window.location.reload(); });

                }
                else {
                    ShowAlertBoxSuccess("Deleted!", "This Client is successfully deleted!", function () { window.location.reload(); });
                }

            });
        });

}
var ExportToExcel = function () {

    ExportFileReq("Client", "ExportToExcel");
}

var ExportFileReq = function (Controller, jAction, rData) {
    if (rData == undefined) rData = "{}";
    $.ajax({
        type: "POST",
        url: '/' + Controller + '/' + jAction,
        contentType: "application/json; charset=utf-8",
        data: rData,
        dataType: "json",
    }).done(function (FileName) {
        //get the file name for download
        if (FileName != "") {
            //use window.location.href for redirect to download action for download the file
            var dPath = '/Base/Download?fileName=' + FileName;
            window.location.href = dPath;

        }
    });
};


$(document).ready(function () {
    HidelottieLoader();
    Array.prototype.forEach.bind($(".utc-time"))(

        _td => {
            if (_td.innerText == "") {
                return;
            }
            let _date = _td.innerText;

            let utcDate = _date;

            let timeZoneOffset = new Date().getTimezoneOffset();
            let utcServerDateTime = new Date(utcDate);
            let utcTimeInMilliseconds = utcServerDateTime.getTime();

            Array.prototype.forEach.bind(document.getElementsByClassName("utc-time"))(e => {


                _td => {
                    if (_td.innerText == "") {
                        return;
                    }
                    let _date = _td.innerText;

                    let utcDate = _date;


                    var localTime = UtcDateToLocalTime(utcDate);

                    debugger;
                    _date = `${localTime.getMonth() + 1}/${localTime.getDate()}/${localTime.getFullYear()} ${localTime.getHours()}:${localTime.getMinutes()} ${(localTime.getHours() >= 12 ? "PM" : "AM")}`;

                    _td.innerText = _date;
                }

            });


        });
});