﻿var $reportYears;
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
    //$formFileUploader = $("#formFileUploader");

    //$formUploader = $formFileUploader.fileupload({
    //    dataType: 'json'
    //});
    //$uploader = $reportUploader.dropzone({ url: "" });

    $reportYears.change(function (e) {

        let year = parseInt($(this).val());
        $lblYears.text(year);
        $divReportPeriodCard.attr("data-year", year);

        let currentYear = moment().year();
        let currentMonth = parseInt(moment().format("MM"));
        if (year < currentYear) {
            $divReportPeriodCard.show();
            $divReportPeriodMonths.parents(".card").show();
        }
        else {
            if (year == currentYear) {
                $divReportPeriodYearEnd.show();

                for (var i = 0; i < (12 - currentMonth + 1); i++) {

                    if (i == 0)
                        $divReportPeriodQuarters.filter("[data-report-period='Q4']").show();
                    else if (i == 3)
                        $divReportPeriodQuarters.filter("[data-report-period='Q3']").show();
                    else if (i == 6)
                        $divReportPeriodQuarters.filter("[data-report-period='Q2']").hide();
                    else if (i == 9)
                        $divReportPeriodQuarters.filter("[data-report-period='Q1']").hide();

                    $divReportPeriodMonths.eq(i).parents(".card").hide();
                }
            }
        }
        bindReports("");

    });

    $btnUpload.click(function (e) {
        e.stopPropagation();
        let elUpload = $(this);

        let data = elUpload.parents('div.card-body').data()
        let agencyId = $("#ddlclient").val();
        let year = data.year;
        let period = data.reportPeriod;

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
    $("a[id='btnDelete']").on("click", function () {
        debugger
        let elDelete = $(this);
        let data = elDelete.data();
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

                }

            })

        });
    }
    //Draggable End
    $btnDeleteAll.click(function (e) {
        e.stopPropagation();
        let el = $(this);
        debugger
        let parentDiv = el.parents("#divReportPeriodCard");
        let data = parentDiv.data();
        let agencyId = $("#ddlclient").val();
        let year = data.year;
        let period = data.reportPeriod;

        let deleteReportsIds = parentDiv.find("[id*=reportItem_]").map(function (i, obj) { return parseInt(obj.dataset.id); })
        deleteReportsIds = deleteReportsIds.toArray();
        deleteReports(deleteReportsIds, period);
    });
    bindPage();
});

var AgencyDropdownPartialViewChange = function () {
    SetUserPreferencesForAgency();
    bindReports("");
    //window.location.reload();
}

var bindPage = function () {
    bindYears();
    bindReports("");
    $reportYears.trigger('change');
}

var bindYears = function () {
    for (var i = moment().year(); i >= 2018; i--) {
        $reportYears.append(`<option>${i}</option>`)
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
    var reportAttachment = `<div class="media align-items-center mb-3" id="att_${obj.Id}"><a class="text-decoration-none mr-3" href="${thumbnail}" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(${thumbnail.replace(/ /g, '%20')});background-size:115px 60px" onclick=""></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" onclick="" data-fancybox="attachment-title">${obj.FileName}</a></h6><button class="cancel" style="border: none; background: transparent; font-size: 12px;padding-left: 0px;" onclick="javascript:window.open('${obj.FilePath}')"><i class="glyphicon glyphicon-ban-circle"></i><span>Download</span></button><button data-dz-remove class=" cancel" style="border: none; background: transparent; font-size: 12px;padding-left: 0px;" onclick="RemoveSavedFile(event,${obj.Id},'${obj.PeriodType}')"><i class="glyphicon glyphicon-ban-circle"></i><span>Remove</span></button><p class="mb-0">Uploaded at ${moment(obj.CreatedDate).format("MM/DD/YYYY")}</p></div></div>`;
    $attachmentContainer.prepend(reportAttachment);
}

var getReports = function (agencyId, year, period) {
    getAjax(`/Reports/GetReports?agencyId=${agencyId}&year=${year}&periodType=${period}`, null, function (response) {
        if (response.Status == "Error")
            ShowAlertBoxError("Error", "Error while fethcing reports!!");
        let reports = response.Data;
        if (isEmptyOrBlank(period)) {
            $(".report-card-body .report").remove()
        }
        else {
            $(`.report-card-body[data-report-period='${period}'] .report`).remove()
        }
        reports.forEach(function (obj) {

            let thumbnail = getSampleBGImageByFileExtension(obj.FileExtention);
            if (isEmptyOrBlank(thumbnail))
                thumbnail = obj.FilePath;
            obj.FilePath = obj.FilePath.replace("~/", "../../");
            thumbnail = thumbnail.replace("~/", "../../");
            var reportHTML = `<div class="col-2 text-center report notes-item" id="reportItem_${obj.Id}" data-id="${obj.Id}" data-position="${obj.Position}"> <h2 class="book-title d-flex justify-content-center"><i class="fa fa-star mr-2"></i>${obj.FileName}</h2><figure class="book-cover"> <img class="img-fluid" src="${thumbnail}" alt="" /> </figure> <p class="publish-options mb-0"><a href="${obj.FilePath}" target="_blank"><span title="View"><i class="fa fa-eye" aria-hidden="true"></i></span></a><a href="${obj.FilePath}" download="${obj.FileName}"><span title="Download"><i class="fa fa-download" aria-hidden="true"></i></span></a><a id="btnDelete" href="#" data-Id="${obj.Id}" onclick="deleteReportOnCliCk(event,${obj.Id})"><span title="Delete"><i class="fa fa-trash" aria-hidden="true"></i></span></a></p></div>`;
            $(`.report-card-body[data-report-period='${obj.PeriodType}'] .row`).append(reportHTML);
        })
    });
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
                ShowAlertBoxSuccess("", "File has been removed successfully!")
            }
        });
    });
}
var deleteReportOnCliCk = function (e, id) {
    ShowConfirmBoxWarning("Are you sure?", "Do you really want to remove this report?", "Yes, remove it!", function (isConfirmed) {
        if (isConfirmed == false)
            return;
        $(`#reportItem_${id}`).remove();
        deleteReports([id]);
        ShowAlertBoxSuccess("", "File has been removed successfully!")
    });
    return false;
}
var deleteReports = function (deleteIds, period) {
    let pdata = { Ids: deleteIds };
    postAjax(`/Reports/Delete`, JSON.stringify(pdata), function (response) {
        debugger
        if (!isEmptyOrBlank(period))
            bindReports(period);
    });
}

