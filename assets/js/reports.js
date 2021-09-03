var $reportYears;
var $divPeriods;
var $lblYears;
var $divReportPeriodCard;
var $divReportPeriodYearEnd;
var $divReportPeriodQuarters;
var $divReportPeriodMonths;

var $btnUpload;
var $uploaderModal;
var $reportUploader;
var $uploader;
var $previews_view;
var $template_view;
var myDropzone_view;

//var $formFileUploader;
//var $formUploader;
Dropzone.autoDiscover = false;

$(function () {
    $reportYears = $("#reportYears");
    $divPeriods = $("div[id='divPeriod']");
    $lblYears = $("span[id='lblYear']");
    $divReportPeriodCard = $("div[id='divReportPeriodCard']");
    $divReportPeriodYearEnd = $("div[data-report-period='YearEnd']");
    $divReportPeriodQuarters = $("div[data-report-period^='Q']");
    $divReportPeriodMonths = $("div[data-report-period-is-month='True']");

    $btnUpload = $("a[id*='btnUpload']");
    $uploaderModal = $("#report-uploader-modal");
    $reportUploader = $("#reportUploader");
    $previews = $("#previews");
    $template = $("#template");
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
                $divReportPeriodYearEnd.hide();

                for (var i = 0; i < (12 - currentMonth + 1); i++) {
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

    });

    $btnUpload.click(function () {
        debugger
        let elUpload = $(this);

        $uploaderModal.modal('show');

        //templateHTML
        var previewNode = document.querySelector("#template");
        previewNode.id = "";
        var previewTemplate = previewNode.parentNode.innerHTML;
        previewNode.parentNode.removeChild(previewNode);

        myDropzone_view = new Dropzone("#reportUploader", { // Make the div a dropzone
            url: "/Needs/UploadFileAndSaves", // Set the url
            thumbnailWidth: 80,
            thumbnailHeight: 80,
            parallelUploads: 2,
            previewTemplate: previewTemplate,
            autoQueue: false, // Make sure the files aren't queued until manually added
            previewsContainer: "#previews", // Define the container to display the previews
            clickable: "#reportUploader", // Define the element that should be used as click trigger to select files.
            success: function (file, response) {
                debugger
                if (response != null && response.Status == 'Success') {
                    addAttachmentOnviewLoad([response.File], false, true);

                    $.each($('#previews_view .file-row '), function (key, value) {
                        if (value != null && value.children[2] != undefined && value.children[2].innerText == file.name) {
                            value.remove();
                        }
                    });
                    // $('#previews_view').empty();                      
                }
            }
        });

        //view Page
        myDropzone_view.on("addedfile", function (file) {
            file.previewElement.querySelector(".start").onclick = function () {

                var IsCanAddfiles = true;
                var filesList = $('#attachmentContainer h6');

                $.each(filesList, function (key, item) {
                    if (item != null && item.innerText == file.name) {
                        IsCanAddfiles = false;
                    }
                });
                if (IsCanAddfiles == true) {
                    myDropzone_view.enqueueFile(file);
                    var attachmentSpan = $('#attCount_' + gCurrentViewTaskId + ' span');
                    if (attachmentSpan.length > 0) {
                        $('#attCount_' + gCurrentViewTaskId).removeClass('d-none');
                        var AttachmentCount = $('#attCount_' + gCurrentViewTaskId + ' span')[0].innerText.trim();
                        var IsattachmentsysmbolAvil = $('#attCount_' + gCurrentViewTaskId + ' .fa-paperclip');
                        if (AttachmentCount != undefined && AttachmentCount != '') {
                            $('#attCount_' + gCurrentViewTaskId + ' span')[0].innerText = parseInt(AttachmentCount) + 1;
                        }

                    }

                }
                else {
                    ShowAlertBox('Exist!', 'Selected file is already attached.', 'warning');
                }

                //var att = '<div class="media align-items-center mb-3" id="att_' + 0 + '"><a class="text-decoration-none mr-3" href="' + item.FilePath + '" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(' + item.FilePath + ');"></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" data-fancybox="attachment-title">' + item.AttachedFileName + '</a></h6><span class="mx-1"></span><button data-dz-remove class=" cancel" style="border: none; background: transparent; font-size: 12px;" onclick="Removeattachment_view(' + item.Id + ',' + singlCode + item.AttachedFileName + singlCode + ',' + true + ')"><i class="glyphicon glyphicon-ban-circle"></i><span>Remove</span></button><p class="mb-0">Uploaded at ' + item.CreatedDateForDisplay + '</p></div></div>';
                //$('#attachmentContainer').prepend(att);
            };
        });

        //view Page
        myDropzone_view.on("sending", function (file) {
            file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
        });

    })

    bindPage();
});

var bindPage = function () {
    bindYears();
    $reportYears.trigger('change');
}

var bindYears = function () {
    for (var i = moment().year(); i >= 2018; i--) {
        $reportYears.append(`<option>${i}</option>`)
    }
}