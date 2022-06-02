

var ImportDropzone_view;
var $attachmentContainer;

const AllowdedMimeTypes = ".html";
const AllowdedMimeQuickBookTypes = ".xls, .xlsx, .csv";
$(function () {
    
    $btnImportReconcilition = $("[id*='btnImportReconcilition']");
    $uploaderModal = $("#report-uploader-modal");
    $reportUploader = $("#reportUploader");
    $attachmentContainer = $("#attachmentContainer");
    $btnCloseImportreconciliation = $("#btnCloseImportreconciliation");
    $btnImportQuickBookReconcilition = $("[id*='btnImportQuickBookReconcilition']");

    $btnImportReconcilition.click(function (e) {


        e.stopPropagation();
        e.preventDefault();
        let elUpload = $(this);
        var agencyId = $("#ddlclient option:selected").val();
        var agencyName = $("#ddlclient option:selected").text();
        var encodeAgencyName = encodeURIComponent(agencyName)
       
        $uploaderModal.modal('show');

        if (!isEmpty(ImportDropzone_view)) {
            ImportDropzone_view.removeAllFiles();
            ImportDropzone_view.destroy();
        }

        $attachmentContainer.html("");
        //Bind uploaer popup
        //bindUploaderAttachments(agencyId, year, period);

        //templateHTML
        var previewNode = document.querySelector("#template");
        if (!isEmpty(previewNode)) {
            previewNode.id = "";
            previewTemplate = previewNode.parentNode.innerHTML;
            previewNode.parentNode.removeChild(previewNode);
            
        }
        var year, period = 10;
        Dropzone.autoDiscover = false;

        if (Dropzone.instances.length > 0) Dropzone.instances.forEach(dz => dz.destroy())
        var AccountingPackage = $('#hdAccointingPackage').val();
        
        if (AccountingPackage == 2) {
            
            ImportUploader = `/Reconciliation/UploadReconcilationQuickBookReportsAsync?agencyId=${agencyId}&agencyName=${encodeAgencyName}`; // Set the u
            acceptedFiles = AllowdedMimeQuickBookTypes;
        }
        else {
            ImportUploader = `/Reconciliation/UploadReconcilationReportsAsync?agencyId=${agencyId}&agencyName=${encodeAgencyName}`; 
            acceptedFiles = AllowdedMimeTypes;
        }
       
        ImportDropzone_view = new Dropzone("#reconciliationUploader", { // Make the div a dropzone
            
            url: ImportUploader, // Set the url
            acceptedFiles: acceptedFiles,
            maxFilesize: 50000,           
            thumbnailWidth: 80,
            thumbnailHeight: 80,
            parallelUploads: 20,
            timeout: 0,
            previewTemplate: previewTemplate,
            autoQueue: false, // Make sure the files aren't queued until manually added
            previewsContainer: "#previews", // Define the container to display the previews
            clickable: "#reconciliationUploader", // Define the element that should be used as click trigger to select files.
            success: function (file, response) {
               
                if (    response != null && response.Status == 'Success') {
                    setTimeout(function () {
                        uploadedstatusload(response);
                    }, 100);
                }
                else if (response != null && response.Status == 'Inprogress')
                {
                    setTimeout(function () {
                        uploadedstatusload(response);
                    }, 100);
                    ShowAlertBoxInfo("Info", response.result.ValidationMessage);
                }
                else {
                }
            },
            error: function (file, response) {
              
                var data = {
                    Status : "false",
                    ValidationMessage : response

                }
                var objErrorresult = {
                    result :data , FileName : file.fileName
                }
                uploadedstatusload(objErrorresult);
                
            }
        });
        //view Page
        ImportDropzone_view.on("addedfile", function (file) {
            if (AccountingPackage == 2) {
               
                if (this.files.length > 1) {
                    this.removeAllFiles()
                    this.addFile(file);
                }
            }
            var BankAccount = $("#BA_filterBankAccounts option:selected").text();
            if (AccountingPackage == 2) {
                if (BankAccount == "Select Bank Account") {

                    $("#btnDropzoneUpload").attr('disabled', true);
                    $('#btnDropzoneUpload').css('cursor', 'not-allowed');
                    $("#btnDropzoneUpload").attr('title', 'Bank Account are not selected.');

                }
                

            }
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
                if (AccountingPackage == 2) {
                    if (file.status === "error") {
                        ShowAlertBoxError("Error", 'You  can only upload files with extension .xls, .xlsx, or .csv.');
                        return;
                    }
                }
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
                    ImportDropzone_view.enqueueFile(file);
                }
                else {
                    ShowAlertBoxError('Exist!', 'Selected file is already uploaded.');
                }

            };
        });

        //view Page
        ImportDropzone_view.on("sending", function (file) {
            file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
        });

        ImportDropzone_view.on("complete", function (file) {
            if (file.status != "error")
                ImportDropzone_view.removeFile(file);
        });


    });

    var uploadedstatusload = function (obj) {
        let thumbnail = '../../assets/img/kanban/I_Success.png';
        var status = 'Success';
        var ErrorMsg = '';
        if (obj.result.Status == false) {
            thumbnail = '../../assets/img/kanban/I_failure.png';
            status = 'Error'
            ErrorMsg = obj.result.ValidationMessage;
        }
        
        thumbnail = thumbnail.replace("~/", "../../");
        var objdate = new Date();
        var fileName = obj.FileName;
        if (obj.result.Status == true) {

            var reportAttachment = `<div class="media align-items-center mb-3" id="att_${0}"><a class="text-decoration-none mr-3" href="${thumbnail}" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(${thumbnail.replace(/ /g, '%20')});background-size:115px 60px" onclick=""></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" onclick="" data-fancybox="attachment-title">${obj.FileName}</a></h6><span>Status:</span> <span>${status}</span><p class="mb-0">Uploaded at ${moment(objdate.CreatedDate).format("MM/DD/YYYY")}</p></div></div>`;
        }
        if (obj.result.Status == false) {
            reportAttachment = `<div class="media align-items-center mb-3" id="att_${0}"><a class="text-decoration-none mr-3" href="${thumbnail}" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(${thumbnail.replace(/ /g, '%20')});background-size:115px 60px" onclick=""></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" onclick="" data-fancybox="attachment-title">${obj.FileName}</a></h6><span>Status:</span> <span>${status}</span></br><span>Message: </span> <span>${ErrorMsg}</span><p class="mb-0">Uploaded at ${moment(objdate.CreatedDate).format("MM/DD/YYYY")}</p></div></div>`;
        }       
        $attachmentContainer.prepend(reportAttachment);
    }
    $btnCloseImportreconciliation.click(function (e) {
        ShowlottieLoader();
        window.location.reload();
    });
    function RemoveFile(e) {
    }

});