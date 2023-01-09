
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




HidelottieLoader();

async function GetQbToken() {
    var qs = window.location.href;

    if (!(/((?<=[a -zA-Z0-9\_]=).[a-zA-Z0-9\%2C\+]+)+/.test(qs))) {
        return;
    }
    var yt = qs.matchAll("((?<=[a-zA-Z0-9\_]=).[a-zA-Z0-9\%2C\+]+)+")
    let index = 0;
    var generatedCode = [{ "type": "code" }, { "type": "state" }, { "type":"realmeId"}];
    debugger;

    for (let data of yt) {
        
        generatedCode[index].value = data[0];

        index++;
    }
    ShowlottieLoader();

    await fetch(`POST /oauth2/v1/tokens/bearer?grant_type=authorization_code&code=${generatedCode[0].value}&redirect_uri=${window.location.origin + window.location.pathname}`);
    
}
GetQbToken();