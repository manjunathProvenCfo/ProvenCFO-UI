

var ImportDropzone_view;
var $attachmentContainer;

const AllowdedMimeTypes = ".html";
$(function () {
    $btnImportReconcilition = $("[id*='btnImportReconcilition']");
    $uploaderModal = $("#report-uploader-modal");
    $reportUploader = $("#reportUploader");
    $attachmentContainer = $("#attachmentContainer");

    $btnImportReconcilition.click(function (e) {
        
        e.stopPropagation();
        e.preventDefault();
        let elUpload = $(this);
        var agencyId = $("#ddlclient option:selected").val();
        var agencyName = $("#ddlclient option:selected").text();

   




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
    
        if(Dropzone.instances.length > 0) Dropzone.instances.forEach(dz => dz.destroy())
       
        ImportDropzone_view = new Dropzone("#reconciliationUploader", { // Make the div a dropzone
            url: `/Reconciliation/UploadReconcilationReports?agencyId=${agencyId}&agencyName=${agencyName}`, // Set the url
            acceptedFiles: AllowdedMimeTypes,
            maxFilesize: 40,
            thumbnailWidth: 80,
            thumbnailHeight: 80,
            parallelUploads: 20,
            previewTemplate: previewTemplate,
            autoQueue: false, // Make sure the files aren't queued until manually added
            previewsContainer: "#previews", // Define the container to display the previews
            clickable: "#reconciliationUploader", // Define the element that should be used as click trigger to select files.
            success: function (file, response) {
                //Load Reports               
                if (response != null && response.Status == 'Success') {
                    prepareAndPrependUploaderAttachment(response.File);
                }
            }
        });

        //view Page
        ImportDropzone_view.on("addedfile", function (file) {
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

});