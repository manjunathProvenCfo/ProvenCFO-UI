var $reportYears;
var $divPeriods;
var $lblYears;
var $divReportPeriodCard;
var $divReportPeriodYearEnd;
var $divReportPeriodQuarters;
var $divReportPeriodMonths;

var $btnUpload;
var $btnDownloadAll;
var $btnDeleteAll;
var $uploaderModal;
var $reportUploader;
var $uploader;
var $previews_view;
var $template_view;
var $attachmentContainer;
var myDropzone_view;
var previewTemplate;
var $renameModal;
var $btnRename;
var isReceiveQuarterlyReportEnable = false;

//var $formFileUploader;
//var $formUploader;
Dropzone.autoDiscover = false;
const AllowdedMimeTypes = "image/*,application/pdf,text/plain,application/json,application/csv,text/csv,application/vnd.openxmlformats-officedocument.spreadsheetml.sheet,application/vnd.ms-excel.sheet.binary.macroEnabled.12,application/vnd.ms-excel,application/vnd.ms-excel.sheet.macroEnabled.12,application/msword,application/vnd.openxmlformats-officedocument.wordprocessingml.document,application/vnd.ms-powerpoint,application/zip,application/x-7z-compressed,application/x-rar-compressed,application/octet-stream,application/zip,application/x-zip-compressed,multipart/x-zip";

$(function () {
    $reportYears = $("#reportYears");
    $divPeriods = $("div[id='divPeriod']");
    $lblYears = $("span[id='lblYear']");
    $divReportPeriodCard = $("div[id='divReportPeriodCard']");
    $divReportPeriodYearEnd = $("div[data-report-period='YearEnd']");
    $divReportPeriodQuarters = $("div[data-report-period^='Q']");
    $divReportPeriodMonths = $("div[data-report-period-is-month='True']");

    $btnUpload = $("a[id*='btnUpload']");
    $btnDownloadAll = $("a[id*='btnDownloadAll']");
    $btnDeleteAll = $("a[id='btnDeleteAll']");
    $uploaderModal = $("#report-uploader-modal");
    $reportUploader = $("#reportUploader");
    $previews = $("#previews");
    $template = $("#template");
    $attachmentContainer = $("#attachmentContainer");
    $renameModal = $("#report-rename-modal");
    $btnRename = $("#btnRename");

    if (isReadonlyUser === false || isReadonlyUser === true) {
        addContextMenu();
        addDraggable();
    }

    $reportYears.change(function (e) {
       
        let year = parseInt($(this).val());
        $lblYears.text(year);

        //#region HideShowDivs
        let currentYear = moment().year();
        let currentMonth = parseInt(moment().format("MM"));
        if (year < currentYear) {
            $divReportPeriodCard.show();
            $divReportPeriodMonths.parents(".card").show();
        }
        else {
          
            if (year == currentYear) {
                $divReportPeriodYearEnd.hide();

                for (var i = 0; i < (11 - currentMonth + 1); i++) { 
                    if (i == 0)
                        $divReportPeriodQuarters.filter("[data-report-period='Q4']").hide();
                    else if (i == 3)
                        $divReportPeriodQuarters.filter("[data-report-period='Q3']").hide();
                    else if (i == 6)
                        $divReportPeriodQuarters.filter("[data-report-period='Q2']").hide();
                    else if (i == 9)
                        $divReportPeriodQuarters.filter("[data-report-period='Q1']").hide();

                    $divReportPeriodMonths.eq(i).parents(".card").hide();
                }
            }
        }
        //#endregion

        $divReportPeriodCard.each(function (i, obj) {
            obj.setAttribute("data-year", year.toString());
            obj.dataset.year = year.toString();
            $(obj).data("year", year.toString());
        })

        showHideQuarterBasedOnFlag();
        bindReports("");
        HidelottieLoader();
    });
    $btnUpload.click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        let elUpload = $(this);

        let data = elUpload.parents('div.card-body').data()
        let agencyId = $("#ddlclient").val();
        let year = data.year;
        let period = data.reportPeriod;

        $("#lblReportPeriod").text(`${period} ${year}`);
        $uploaderModal.modal('show');

        if (!isEmpty(myDropzone_view)) {
            myDropzone_view.removeAllFiles();
            myDropzone_view.destroy();
        }

        $attachmentContainer.html("");
        //Bind uploaer popup
        bindUploaderAttachments(agencyId, year, period);

        //templateHTML
        var previewNode = document.querySelector("#template");
        if (!isEmpty(previewNode)) {
            previewNode.id = "";
            previewTemplate = previewNode.parentNode.innerHTML;
            previewNode.parentNode.removeChild(previewNode);
        }

        myDropzone_view = new Dropzone("#reportUploader", { // Make the div a dropzone
            url: `/Reports/UploadReportAndSave?agencyId=${agencyId}&year=${year}&periodType=${period}`, // Set the url
            acceptedFiles: AllowdedMimeTypes,
            maxFilesize: 40,
            thumbnailWidth: 80,
            thumbnailHeight: 80,
            parallelUploads: 20,
            previewTemplate: previewTemplate,
            autoQueue: false, // Make sure the files aren't queued until manually added
            previewsContainer: "#previews", // Define the container to display the previews
            clickable: "#reportUploader", // Define the element that should be used as click trigger to select files.
            success: function (file, response) {
                //Load Reports
                bindReports(period);

                if (response != null && response.Status == 'Success') {
                    prepareAndPrependUploaderAttachment(response.File);
                }
            }
        });

        //view Page
        myDropzone_view.on("addedfile", function (file) {
            //Remove Preview Div
            $(".file-row .preview img").each(function (i, obj) {
                let attr = $(obj).attr("src");
                if (isEmptyOrBlank(attr)) {
                    $(obj).parent('.preview').remove();
                }

            });

            //Remove Upload Button for errored files
            setTimeout(function () { $(".file-row.dz-error #btnDropzoneUpload").remove(); }, 10)


            file.previewElement.querySelector(".start").onclick = function () {
                if (file.status === "error") {
                    ShowAlertBoxError("Error", `You can't upload ${file.name} file.`);
                    return;
                }
                var IsCanAddfiles = true;
                var filesList = $('#attachmentContainer h6');

                $.each(filesList, function (key, item) {

                    if (item != null && item.innerText == file.name) {
                        IsCanAddfiles = false;
                    }
                });
                if (IsCanAddfiles == true) {
                    myDropzone_view.enqueueFile(file);
                }
                else {
                    ShowAlertBoxError('Exist!', 'Selected file is already uploaded.');
                }

            };
        });

        //view Page
        myDropzone_view.on("sending", function (file) {
            file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
        });

        myDropzone_view.on("complete", function (file) {
            if (file.status != "error")
                myDropzone_view.removeFile(file);
        });


    });
    $btnDownloadAll.click(function (e) {
        e.stopPropagation();
        let elDownloadAll = $(this);
        let data = elDownloadAll.parents('div.card-body').data()
        let agencyId = $("#ddlclient").val();
        let year = data.year;
        let period = data.reportPeriod;
        window.open(`/Reports/DownloadAll?agencyId=${agencyId}&year=${year}&periodType=${period}`);
        return false;
    });

    $btnDeleteAll.click(function (e) {
        e.stopPropagation();
        let el = $(this);
        let parentDiv = el.parents("#divReportPeriodCard");
        let data = parentDiv.data();
        let agencyId = $("#ddlclient").val();
        let year = data.year;
        let period = data.reportPeriod;
        let deleteReportsIds = parentDiv.find("[id*=reportItem_]").map(function (i, obj) { return parseInt(obj.dataset.id); })
        deleteReportsIds = deleteReportsIds.toArray();

        ShowConfirmBoxWarning("Are you sure?", "Do you really want to remove this report?", "Yes, remove it!", function (isConfirmed) {
            if (isConfirmed == false)
                return;
            deleteReports(deleteReportsIds, period);
            ShowAlertBoxSuccess("", "Files has been removed successfully!")
        });
    });

    $btnRename.click(function (e) {
        if (isEmptyOrBlank($("#txtNewFileName").val())) {
            ShowAlertBoxError("", "Please enter New File Name");
            return;
        }
        let period = $("#hdnPeriod").val();
        let pdata = { Id: $("#hdnId").val(), FileName: $("#txtNewFileName").val() };
        postAjax(`/Reports/Rename`, JSON.stringify(pdata), function (response) {
            if (!isEmptyOrBlank(period))
                bindReports(period);
            $renameModal.modal('hide');
            ShowAlertBoxSuccess("", "Report has been renamed successfully!")
        });
    });

    bindPage();
});

var AgencyDropdownPartialViewChange = function () {
    
    ShowlottieLoader();
    SetUserPreferencesForAgency();
    bindQuarter();
    bindReports("");
   /* HidelottieLoader();*/
    window.location.reload();
}

var bindPage = function () {
    bindYears();
    bindQuarter();
    bindReports("");
    $reportYears.trigger('change');
}

var bindYears = function () {
    for (var i = moment().year(); i >= 2018; i--) {
        $reportYears.append(`<option>${i}</option>`)
    }
}
var bindQuarter = function () {
    getAjaxSync(`/Reports/GetIsReceiveQuarterlyReportEnable?agencyId=${getClientId()}`, null, function (response) {
        if (response.Status === "Success") {
            isReceiveQuarterlyReportEnable = response.Data;
            showHideQuarterBasedOnFlag();
        }
    });
}

var showHideQuarterBasedOnFlag = function () {
    if (isReceiveQuarterlyReportEnable === true) {
        let year = $reportYears.val();
        let currentYear = moment().year();
        let currentMonth = parseInt(moment().format("MM"));

        if (year === currentYear) {
            $divReportPeriodQuarters.show();

            for (var i = 0; i < (12 - currentMonth + 1); i++) {
                if (i === 0)
                    $divReportPeriodQuarters.filter("[data-report-period='Q4']").hide();
                else if (i === 3)
                    $divReportPeriodQuarters.filter("[data-report-period='Q3']").hide();
                else if (i === 6)
                    $divReportPeriodQuarters.filter("[data-report-period='Q2']").hide();
                else if (i === 9)
                    $divReportPeriodQuarters.filter("[data-report-period='Q1']").hide();
            }
        }
    }
    else {
        $divReportPeriodQuarters.hide();
    }
}
var bindReports = function (reportPeriod) {
    let agencyId = $("#ddlclient").val();
    let year = $reportYears.val();
    let period = reportPeriod;
    getReports(agencyId, year, period);
}

var bindUploaderAttachments = function (agencyId, year, period) {
    getAjax(`/Reports/GetReports?agencyId=${agencyId}&year=${year}&periodType=${period}`, null, function (response) {
        if (response.Status == "Error")
            ShowAlertBoxError("Error", "Error while fethcing reports!!");
        let reports = response.Data;
        reports = reports.reverse();
        $attachmentContainer.html("");
        reports.forEach(function (obj) {
            prepareAndPrependUploaderAttachment(obj);
        })
    });
}
var prepareAndPrependUploaderAttachment = function (obj) {
    let thumbnail = getSampleBGImageByFileExtension(obj.FileExtention);
    if (isEmptyOrBlank(thumbnail))
        thumbnail = obj.FilePath;
    obj.FilePath = obj.FilePath.replace("~/", "../../");
    thumbnail = thumbnail.replace("~/", "../../");

    var reportAttachment = `<div class="media align-items-center mb-3" id="att_${obj.Id}"><a class="text-decoration-none mr-3" href="${thumbnail}" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(${thumbnail.replace(/ /g, '%20')});background-size:115px 60px" onclick=""></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" onclick="" data-fancybox="attachment-title">${obj.FileName}${obj.FileExtention}</a></h6><button class="cancel" style="border: none; background: transparent; font-size: 12px;padding-left: 0px;" onclick="javascript:window.open('${obj.FilePath}')"><i class="glyphicon glyphicon-ban-circle"></i><span>Download</span></button><button data-dz-remove class=" cancel" style="border: none; background: transparent; font-size: 12px;padding-left: 0px;" onclick="RemoveSavedFile(event,${obj.Id},'${obj.PeriodType}')"><i class="glyphicon glyphicon-ban-circle"></i><span>Remove</span></button><p class="mb-0">Uploaded at ${moment(obj.CreatedDate).format("MM/DD/YYYY")}</p></div></div>`;
    $attachmentContainer.prepend(reportAttachment);
}

var getReports = function (agencyId, year, period) {
    getAjax(`/Reports/GetReports?agencyId=${agencyId}&year=${year}&periodType=${period}`, null, function (response) {
        if (response.Status == "Error")
            ShowAlertBoxError("Error", "Error while fethcing reports!!");
        let reports = response.Data;
        if (isEmptyOrBlank(period)) {
            $(".report-card-body .report").remove();
        }
        else {
            $(`.report-card-body[data-report-period='${period}'] .report`).remove();
        }        
        reports.forEach(function (obj) {

            let thumbnail = getSampleBGImageByFileExtension(obj.FileExtention);
            if (isEmptyOrBlank(thumbnail))
                thumbnail = obj.FilePath;
            obj.FilePath = obj.FilePath.replace("~/", "../../");
            thumbnail = thumbnail.replace("~/", "../../");
            let staredReportHTML = "";
            let deleteReportHTML = `<a class="d-none" id="aDelete" href="#" data-Id="${obj.Id}" onclick="deleteReportOnCliCk(event,${obj.Id})"><span title="Delete"><i class="fa fa-trash" aria-hidden="true"></i></span></a>`;
            let monthlySummaryReportHTML = `<a class="d-none" href="#" onclick="monthlySummaryOnClick(event,${obj.Id})" id="aMonthlySummary"><span title="Make it Monthly Summary report"><i class="fa fa-star" aria-hidden="true"></i></span></a>`;
            let renameReportHTML = `<a class="d-none" id="aRename" href="#" onclick="renameReportOnClick(event,${obj.Id},'${obj.FileName}')"></a>`;
            if (isReadonlyUser) {
                deleteReportHTML = '';
                monthlySummaryReportHTML = '';
            }
            if (!isEmptyOrBlank(obj.IsMonthlySummary) && obj.IsMonthlySummary === true)
                staredReportHTML = `<i class="fa fa-star mr-2"></i>`;
            var reportHTML = `<div class="col-2 text-center report notes-item context-menu py-2" id="reportItem_${obj.Id}" data-id="${obj.Id}" data-position="${obj.Position}"> 
                                <h2 class="book-title d-flex justify-content-center">${staredReportHTML}${obj.FileName}</h2>
                                <a class="data-fancybox" href="${obj.FilePath}" data-fancybox="group-${obj.PeriodType.toLowerCase()}" data-caption="${obj.FileName}${obj.FileExtention}">
                                <figure class="book-cover"> 
                                <img class="img-fluid" src="${thumbnail}" alt="" /> 
                                </figure> 
                                </a>
                                <p class="publish-options mb-0 d-none">
                                <a class="d-none" href="${obj.FilePath}" target="_blank" id="aView"><span title="View"><i class="fa fa-eye" aria-hidden="true"></i></span></a>
                                <a class="d-none" href="${obj.FilePath}" download="${obj.FileName}" id="aDownload"><span title="Download"><i class="fa fa-download" aria-hidden="true"></i></span></a>
                                ${deleteReportHTML}
                                ${renameReportHTML}
                                ${monthlySummaryReportHTML}
                                </p></div>`;
            $(`.report-card-body[data-report-period='${obj.PeriodType}'] .row`).append(reportHTML);
        });
        HidelottieLoader();
    });
}
var addContextMenu = function () {
    if (isReadonlyUser == false) {
        let menus = {
            "view": {
                name: "View",
                callback: function (itemKey, opt, e) {
                    opt.$trigger.find("#aView")[0].click();
                }
            },
            "download": {
                name: "Download",
                callback: function (itemKey, opt, e) {
                    opt.$trigger.find("#aDownload")[0].click();
                }
            },
            "delete": {
                name: "Delete",
                callback: function (itemKey, opt, e) {
                    opt.$trigger.find("#aDelete")[0].click();
                }
            },
            "rename": {
                name: "Rename",
                callback: function (itemKey, opt, e) {
                    opt.$trigger.find("#aRename")[0].click();
                }
            },
            "defaultReport": {
                name: "Set as Default",
                callback: function (itemKey, opt, e) {
                    opt.$trigger.find("#aMonthlySummary")[0].click();
                }
            }
        }
        $.contextMenu({
            selector: '.context-menu',
            items: menus
        });
    }
    else {
        let menus = {

            "download": {
                name: "Download",
                callback: function (itemKey, opt, e) {
                    opt.$trigger.find("#aDownload")[0].click();
                }
            }
        }

        $.contextMenu({
            selector: '.context-menu',
            items: menus
        });
    }
}
var addDraggable = function () {
    //Draggable Start
    const Selectors = {
        BODY: 'body',
        NOTES_CONTAINER: '.notes-container',
        NOTES_ITEMS_CONTAINER: 'div.notes-items-container',
        NOTES_ITEM: '.notes-item',
    };

    let arrItemsContainer = $(Selectors.NOTES_ITEMS_CONTAINER).toArray();
    for (var i = 0; i < arrItemsContainer.length; i++) {
        let sortable = new window.Draggable.Sortable(arrItemsContainer[i], {
            draggable: Selectors.NOTES_ITEM,
            delay: 200,
            mirror: {
                appendTo: Selectors.BODY,
                constrainDimensions: true
            }
        });
        sortable.on('drag:stop', function (e) {
            var $this = $(e.data.source);
            var $itemContainer = $this.closest(Selectors.NOTES_ITEMS_CONTAINER);
            let $items = $itemContainer.find('.notes-item:visible');
            let ids = [];
            let positions = [];
            let counter = $items.length;
            for (var i = 0; i < $items.length; i++) {
                ids.push(parseInt($items[i].getAttribute("data-id")));
                positions.push(counter);
                counter = counter - 1;
            }
            let pdata = { Ids: ids, Positions: positions };

            postAjax('/reports/UpdatePositions', JSON.stringify(pdata), function (response) {
                if (response.Message == 'Success') {
                    //
                }

            })

        });
    }
    //Draggable End
}
var RemoveSavedFile = function (e, reportId, period) {
    let report = $(e.currentTarget).parents('.media');
    ShowConfirmBoxWarning("Are you sure?", "Do you really want to remove this report?", "Yes, remove it!", function (isConfirmed) {
        if (isConfirmed == false)
            return;
        report.remove();
        postAjax(`/Reports/SoftDeleteFile?Id=${reportId}&PeriodType=${period}`, null, function (response) {
            if (response.Status == "Success") {
                bindReports(response.PeriodType);
                ShowAlertBoxSuccess("", "Report has been removed successfully!")
            }
        });
    });
}
var deleteReportOnCliCk = function (e, id) {
    e.preventDefault();
    ShowConfirmBoxWarning("Are you sure?", "Do you really want to remove this report?", "Yes, remove it!", function (isConfirmed) {
        if (isConfirmed == false)
            return;
        $(`#reportItem_${id}`).remove();
        deleteReports([id]);
        ShowAlertBoxSuccess("", "Report has been removed successfully!")
    });
    return false;
}
var deleteReports = function (deleteIds, period) {
    let pdata = { Ids: deleteIds };
    postAjax(`/Reports/Delete`, JSON.stringify(pdata), function (response) {
        if (!isEmptyOrBlank(period))
            bindReports(period);
    });
}

var monthlySummaryOnClick = function (e, id) {
   
    e.preventDefault();
    let el = $(e.currentTarget);
    let parentDiv = el.parents("#divReportPeriodCard");
    let data = parentDiv.data();
  /*  var ClientID = $("#ddlclient option:selected").val();*/
    let agencyId = $("#ddlclient").val();
    let year = data.year;
    let period = data.reportPeriod;

    postAjax(apiurl + `Reports/MakeItMonthlySummary?Id=${parseInt(id)}&Year=${parseInt(year)}&PeriodType=${period}&agencyId=${agencyId}`, null, function (response) {     
        if (response.message == "Success") {
            ShowAlertBoxSuccess("", "Report has been marked as Default report!")
            bindReports(period, agencyId);
        }
    });
    return false;
}

var renameReportOnClick = function (e, id, fileName) {
    e.preventDefault();
    $renameModal.modal('show');
    let el = $(e.currentTarget);
    let parentDiv = el.parents("#divReportPeriodCard");
    let data = parentDiv.data();

    $("#lblFileName").text(fileName);
    $("#hdnId").val(id);
    $("#hdnPeriod").val(data.reportPeriod);
    $("#txtNewFileName").val('');
}
//function getClientDate(ClientName) {
//    getAjaxSync(apiurl + `Reconciliation/getLastSentDate?ClientName=${ClientName}`, null, function (response) {
//        sessionStorage.setItem("LastMailSent", response.resultData);
//    });

//}
$(function () {

    $("#email").click(function () {
        var url = window.location.href;
        var ClientName = $("#ddlclient option:selected").text();
        var ClientId = $("#ddlclient option:selected").val();
       /* getClientDate(ClientName);*/
        ClientName = encodeURIComponent(ClientName);
        /*var LastMailSent = sessionStorage.getItem("LastMailSent");*/
        getAjax(`/Reports/EmailSend?ClientName=${ClientName}&ClientId=${ClientId}&url=${url}`, null, function (response) {
            if (response.Status == 'Success') {
                var text = response.Recipients.toString().split(",");
                var str = text.join(', ');
                $("#email-to").val(str);
                if (str == "") {
                    $("#sendbutton").attr("disabled", true);
                }
                $("#email-subject").val(response.Subject);
                $("#ibody").html(response.Body);
                $("#ifooter").html(response.LastSent);

            }
        });

    });
});
function sendMail() {
    debugger;
    var recip = $("#email-to").val();
    var subject = $("#email-subject").val();
    var body = $("#ibody").html();
    var pdata = {
        Recipents: recip,
        Subject: subject,
        Body: body,
    };
    postAjaxSync(apiurl + `Reports/sendReportEmail`, JSON.stringify(pdata), function (response) {
        ShowAlertBoxSuccess("", "Email has been sent ", function () { window.location.reload(); });
    });
}