//Chat Code start
var currentChannelUniqueNameGuid = "";
var IsEnableAutomation = false;
var IsSeletedAll = false;
const scrollChatState = {
    size: 20,
    pageNo: 2,
    reconciliatioId: '',
    target: null
};
$(document).ready(function () {

    hideParticipantsSidebar();
    bindEnableAutomation();
    bindEnablePlaid();
    EnableSelectedBulkUpdateButton();
    //bindIsSeletedAll();
    lastModify();

    $("#ichat").click(function () {
        let elCheckbox = $("table tr.bg-300 td:first .checkbox-bulk-select-target");
        if (elCheckbox.length === 0) {
            ShowAlertBoxWarning("Please select reconciliation row!");
        }
        else {
            let reconciliaitonId = elCheckbox.attr("id");

            showReconciliationChat(reconciliaitonId);
            resetScrollChatState(reconciliaitonId);
        }
    });
    $(document).on("click", "#tblreconcilation tr", async function (e) {
        $("#tblreconcilation tr").removeClass("bg-200");
        $(this).addClass("bg-200");

        if ($('#divChat:visible').length > 0 && e.target.nodeName != "svg") {
            let elComment = $(this).find("#btnComment");

            resetScrollChatState(elComment.data().id);
            await showReconciliationChat(elComment.data().id);
        }
    });

    $(document).on("click", "button[id=btnComment]", async function (e) {

        resetScrollChatState(e.currentTarget.dataset.id);
        await showReconciliationChat(e.currentTarget.dataset.id);
    });

    var showReconciliationChat = async function (channelUniqueNameGuid) {
      
        resetScrollChatState(channelUniqueNameGuid);
        $('#divFilter').hide();
        $('#divFilter').addClass('d-none');
        $('#divBulkUpdate').hide();
        $('#divBulkUpdate').addClass('d-none');
        $('#divChat').show();
        $('#divChat').removeClass('d-none');
        $('#divTable').addClass('col-md-8').removeClass('col-md-12');

        await loadCommentsPage(channelUniqueNameGuid);

    }

    function hideParticipantsSidebar() { $(".chat-sidebar").hide(); }

    var agencyId = $("#ddlclient option:selected").val();

    $.ajax({
        url: '/Reconciliation/GetEndYearLockDateAsync?id=' + agencyId,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            if (data.DOMO_Last_batchrun_time != null) {

                $('#domoLastBatchRun').show();
                let roughDate = data.DOMO_Last_batchrun_time;
                let dateTimeMill = Number(roughDate.match(/\d+/)[0]);

                let utcDateTime = new Date(dateTimeMill);
                var localDateTime = utcDateTime.toLocaleString();

                $('#domoLastBatchRunTime')[0].innerText = localDateTime;

            } else {
                /*$('#domoLastBatchRun').remove();*/
                $('#domoLastBatchRun').show();
                $('#domoLastBatchRunTime')[0].innerText = "N/A";


            }

            if (data.End_Of_YearLockDate != null) {

                $('#endOfYearLock').show();
                let roughEndDate = data.End_Of_YearLockDate;
                let endDateTimeMill = Number(roughEndDate.match(/\d+/)[0]);

                let utcDateTime = new Date(endDateTimeMill);
                var localEndDateTime = utcDateTime.toLocaleDateString();
                $('#endOfYearLockDate')[0].innerText = localEndDateTime;

            } else {
                if (data.ThirdPartyAccountingApp_ref != 2) {
                    $('#endOfYearLock').show();
                    $('#endOfYearLockDate')[0].innerText = "N/A";
                }
                else {
                    $('#endOfYearLock').hide();
                }
                
            }
        },
        error: function (error) {
            console.log(error);
            $('#domoLastBatchRun').remove();
            $('#endOfYearLock').remove();
        }
    })
});

var lastModify = function () {
    $(".lastmodified").each(function () {
        var utctime = $(this).find('select').attr("utcdate");
        var ModifiedBy = $(this).find('select').attr("ModifiedBy");

        if (utctime != undefined && ModifiedBy != undefined && utctime != '' && ModifiedBy != '') {
            var localtime = getLocalTime(utctime);
            var msg = "Last Modified by <br> " + ModifiedBy + " <br> " + localtime;
            $(this).attr("data-original-title", msg);
        }
        else {
            $(this).attr("data-original-title", "No Modification yet.");
        }
    });
}
var bindEnableAutomation = function () {
    getAjaxSync(`/Reconciliation/GetIsEnableAutomation?agencyId=${getClientId()}`, null, function (response) {
        if (response.Status === "Success") {
            IsEnableAutomation = response.Data;
            if (IsEnableAutomation === false) {
                $("#OnDemandData").attr('disabled', true);
                $("#OnDemandData").attr('title', 'Request on demand data has been disabled.');
                $("#OnDemandData").addClass('d-none');
            }
            else {
                $("#OnDemandData").removeClass('d-none');
            }

        }
    });
}
var bindEnablePlaid = function () {
    getAjaxSync(`/Reconciliation/GetIsEnablePlaid?agencyId=${getClientId()}`, null, function (response) {
        if (response.Status === "Success") {
            IsEnablePlaid = response.Data;
            if (IsEnablePlaid === false) {
                $("#OnDemandDataPlaid").addClass('d-none');
            }
            else {
                $("#OnDemandDataPlaid").removeClass('d-none');
                $("#OnDemandDataPlaid").attr('title', 'Request on demand plaid data.');
            }

        }
    });
}
var EnableSelectedBulkUpdateButton = function () {
    var IsAllSelected = $('#checkbox-bulk-purchases-select')[0].checked;
    var SelectedItems = sessionStorage.getItem('SelectedRecords');
    var unselectedItems = sessionStorage.getItem("UnSelectedRecords");


    if ((SelectedItems != null && SelectedItems != '') || IsAllSelected == true) {
        $("#ibulkupdate").attr('disabled', false);
        $("#ibulkupdate").attr('title', 'Bulk Update');

    }

    else {

        if (unselectedItems == null) {

            $("#ibulkupdate").attr('disabled', true);
            $("#ibulkupdate").attr('title', 'Select A Row to perform BulkUpdate.');
        }

        if (SelectedItems == "" && unselectedItems.split(",").length < 2) {

            $("#ibulkupdate").attr('disabled', true);
            $("#ibulkupdate").attr('title', 'Select A Row to perform BulkUpdate.');
        }


    }


}




const resetScrollChatState = function (rec) {

    if (rec != "" && rec != null) {

        scrollChatState.reconciliationId = rec;

        scrollChatState.pageNo = 2;
        scrollChatState.size = 20;
    };
}
function SetupDynamicLoaderOnScroll() {



    $("#channel-messages")[0].addEventListener("scroll", function (e) {
        e.preventDefault();
        if (e.target.scrollTop == 0) {
            LoadOnDemandCommentsPagination(scrollChatState.reconciliationId,
                1, scrollChatState.pageNo * scrollChatState.size);

            scrollChatState.pageNo++;

            scrollChatState.target = e.target;
        }
    });



    scrollChatState.pageNo = 2;
    scrollChatState.size = 10;

}

var type = sessionStorage.getItem('Type');
$('#ddlclient').on("change", function () {
    sessionStorage.removeItem('Type');
    type = 0;
})
$('#menu_reconciliation').on('click', function () {
    sessionStorage.removeItem('Type');
})

$(document).ready(function () {

    bindNotInBooksAndBanksCount();
    LoadFilterData();
    SetupDynamicLoaderOnScroll();

    XeroConnectionUpdate();
    $('#divFilter').hide();
    $('#divChat').hide();
    sessionStorage.removeItem('SelectedRecords');
    sessionStorage.removeItem('UnSelectedRecords');
    if (type != null) {

        if (type == "0") {
            $('#tabNotinBooks').addClass('tabselect');
            $('#tabNotinBanks').removeClass('tabselect');
            sessionStorage.removeItem('Type');
        }
        else {
            $('#tabNotinBooks').removeClass('tabselect');
            $('#tabNotinBanks').addClass('tabselect');
            //sessionStorage.clear();
        }

    }

    else {
        $('#tabNotinBooks').addClass('tabselect');
        $('#tabNotinBanks').removeClass('tabselect');

    }


    $('.checkbox-bulk-select-target').click(function () {

        if ($(this).is(":checked")) {

            $(this).closest('tr').addClass('bg-300');
        }
        else {
            $(this).closest("tr").removeClass('bg-300');
        }
    });

    $('.checkbox-bulk-select').change(function () {
        if ($(this).is(":checked", true)) {

            $(this).closest('table').addClass('bg-300');
        }
        else {
            $(this).closest("table").removeClass('bg-300');
        }
    });

    $('#Cancel').click(function () {
        if ($('#divTable')[0].className.indexOf('col-md-8') != -1) {
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');
            $('#divChat').hide();
        }
        else {
            $('#divTable').addClass('col-md-8').removeClass('col-md-12');
            $('#divChat').show();
        }
    });
    $('#Filter').click(function () {

        if ($("#divChat").is(":visible") || $("#divBulkUpdate").is(":visible")) {
            $('#divChat').hide();
            $('#divChat').addClass('d-none');
            $('#divBulkUpdate').hide();
            $('#divBulkUpdate').addClass('d-none');
            $('#divFilter').show();
            $('#divFilter').removeClass('d-none');
        }
        else if ($("#divFilter").is(":visible")) {
            $('#divChat').hide();
            $('#divChat').addClass('d-none');
            $('#divFilter').hide();
            $('#divFilter').addClass('d-none');
            $('#divBulkUpdate').hide();
            $('#divBulkUpdate').addClass('d-none');
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');
        }
        else {
            $('#divFilter').show();
            $('#divFilter').removeClass('d-none');
            $('#divTable').addClass('col-md-8').removeClass('col-md-12');
        }
    });
    $('#ibulkupdate').click(function () {
        if ($("#divFilter").is(":visible") || $("#divChat").is(":visible")) {
            $('#divChat').hide();
            $('#divChat').addClass('d-none');
            $('#divBulkUpdate').show();
            $('#divBulkUpdate').removeClass('d-none');
            $('#divFilter').hide();
            $('#divFilter').addClass('d-none');
            $('.chat-message').remove();

        }
        else if ($("#divBulkUpdate").is(":visible")) {

            $('#divBulkUpdate').hide();
            $('#divBulkUpdate').addClass('d-none');
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');
        }
        else {
            $('#divBulkUpdate').show();
            $('#divBulkUpdate').removeClass('d-none');
            $('#divTable').addClass('col-md-8').removeClass('col-md-12');

            setTimeout(_ => addBulkMentionPlugin(), 1000);
        }
    });

    $("#bulkRule_New").change(function () {
        
        $("#bulk-checkbox-wrapper").fadeOut(200, function () {
            $(this).find("#bulkRule_New").attr("checked", !$(this).find("#bulkRule_New").attr("checked")).end().fadeIn(200);
        });

        if (this.checked) {
            $("label[for='bulkRule_New']")
                .css("color", "green")
                .fadeOut(200, function () {
                    $(this).text("Checked!").fadeIn(200);
                });
        } else {
            $("label[for='bulkRule_New']")
                .css("color", "red")
                .fadeOut(200, function () {
                    $(this).text("Unchecked!").fadeIn(200);
                });
        }
    });

    $('#Cancel1').click(function () {
        if ($('#divTable')[0].className.indexOf('col-md-8') != -1) {
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');
            $('#divTable1').addClass('d-none');
            $('#divChat').hide();
            $('#divFilter').hide();
            $('#divFilter').addClass('d-none');
            location.reload();
        }
        else {
            $('#divTable').addClass('col-md-8').removeClass('col-md-12');
            $('#divTable1').removeClass('d-none');
            $('#divChat').show();
        }
    });
    $('#CancelBulkupdate').click(function () {
        if ($('#divTable')[0].className.indexOf('col-md-8') != -1) {
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');
            $('#divTable1').addClass('d-none');
            $('#divChat').hide();
            $('#divFilter').hide();
            $('#divBulkUpdate').hide();
            $('#divFilter').addClass('d-none');
            location.reload();
        }
        else {
            $('#divTable').addClass('col-md-8').removeClass('col-md-12');
            $('#divBulkUpdate').removeClass('d-none');
            $('#divBulkUpdate').show();
        }
    });
    $("#OnDemandDataPlaid").click(function () {

        if (IsEnablePlaid === false) {
            $("#OnDemandDataPlaid").attr('disabled', true);
            return;
        }
        $("#Loader").removeAttr("style");
        var ClientID = $("#ddlclient option:selected").val();
        postAjax('/Reconciliation/OnDemandDataRequestFromPlaid?AgencyId=' + ClientID, null, function (response) {
            if (response.Status == true) {
                sessionStorage.removeItem("NotInBooksData");
                sessionStorage.removeItem("NotInBanksData");
                ShowAlertBoxSuccess("Success", response.Message, function () {
                    window.location.reload();
                });
            }
            else {
                ShowAlertBoxError(response.ErrorType == null ? "Error" : response.ErrorType, response.data, function () {
                    window.location.reload();
                });
            }

        });
    });
    $("#OnDemandXero").click(function () {
        if (IsEnableAutomation === false) {
            $("#OnDemandXero").attr('disabled', true);
            return;
        }
        $("#Loader").removeAttr("style");
        var ClientID = $("#ddlclient option:selected").val();
        var RequestType = "On Demand";
        var RequestedAtUTC = '';
        var CurrentStatus = "New Request";
        var RequestCompletedAtUTC = '';
        var Remark = '';
        var AgencyName = '';
        var CreatedBy = '';
        var CreatedDate = '';
        var pdata = { RequestType: RequestType, RequestedAtUTC: RequestedAtUTC, CurrentStatus: CurrentStatus, RequestCompletedAtUTC: RequestCompletedAtUTC, Remark: Remark, AgencyId: ClientID, AgencyName: AgencyName, CreatedBy: CreatedBy, CreatedDate: CreatedDate };
        postAjax('/Reconciliation/XeroOnDemandDataRequest', JSON.stringify(pdata), function (response) {
            if (response.Message == 'Success') {
                setTimeout(() => {
                    reconcilationonstatusDemand(response.data.Id);
                }, 1000);
            }
        });
    });
    $("#OnDemandData").click(function () {
        if (IsEnableAutomation === false) {
            $("#OnDemandData").attr('disabled', true);
            return;
        }
        $("#Loader").removeAttr("style");
        var ClientID = $("#ddlclient option:selected").val();
        var RequestType = "On Demand Azure"; //On Demand
        var RequestedAtUTC = '';
        var CurrentStatus = "New_Azure"; //New
        var RequestCompletedAtUTC = '';
        var Remark = '';

        var AgencyName = '';
        var CreatedBy = '';
        var CreatedDate = '';
        var RequestID = 0;
        var IsNotinBooks = $('#tabNotinBooks.tabselect').length > 0 ? 1 : 0;
        var IsNotinBanks = $('#tabNotinBanks.tabselect').length > 0 ? 1 : 0;
        var AzureFunctionReconUrl = $('#AzureFunctionReconUrl').val();
        var pdata = { RequestType: RequestType, RequestedAtUTC: RequestedAtUTC, CurrentStatus: CurrentStatus, RequestCompletedAtUTC: RequestCompletedAtUTC, Remark: Remark, AgencyId: ClientID, AgencyName: AgencyName, CreatedBy: CreatedBy, CreatedDate: CreatedDate };
        postAjax('/Reconciliation/AddNewXeroOnDemandDataRequest', JSON.stringify(pdata), function (response) {
            if (response.Message == 'Success') {
                RequestID = response.data.Id;
                getAjax(AzureFunctionReconUrl + `?AgencyId=${getClientId()}&NotInbooks=${IsNotinBooks}&NotInbanks=${IsNotinBanks}`, null, function (Azureresponse) {
                    Azureresponse = JSON.parse(Azureresponse); 
                    Azureresponse.message = Azureresponse.message.replace("expaired", "expired");
                    UpdateXeroonDemandDatarequestStatus(Azureresponse, RequestID);
                    if (Azureresponse.status === true && Azureresponse.statusCode == 200) {
                        sessionStorage.removeItem("NotInBooksData");
                        sessionStorage.removeItem("NotInBanksData");

                        //uncampatible on safari.
                        /*    let finalAzureMessage = ((((Azureresponse.message.replace("Sucess : ", "")).replace(" =", "=")).replace(/=/g, ": ")).replace(/(?=Not)|(?=In)/g, " ")).replace(/b/g, "B");*/
                        let finalAzureMessage = ((((Azureresponse.message.replace("Sucess : ", "")).replace(" =", "=")).replace(/=/g, ": ")).replace(/b/g, "B")).replace(/([a-z])([A-Z])/g, '$1 $2');
                       
                        ShowAlertBoxSuccess("Success!", "Successfully synced with Xero. \n" + finalAzureMessage, function () { window.location.reload(); });
                    }
                    else if (Azureresponse.status === false && Azureresponse.statusCode != 500)
                    {
                        ShowAlertBoxError("Error!", `${Azureresponse.message}.(Req #${RequestID})`, function () { window.location.reload(); });
                    }
                    else {
                        ShowAlertBoxError("Error!", `Sorry, there was a problem getting data from Xero. Please try again later.(Req #${RequestID})`, function () { window.location.reload(); });
                    }
                });

                //setTimeout(() => {
                //    reconcilationonstatusDemand(response.data.Id);
                //}, 1000);
            }

        });

    });
    function UpdateXeroonDemandDatarequestStatus(response, RequestId) {
        var pdata = {
            RequestId: RequestId,
            CurrentStatus: response.status == true ? "Success" : "Failed",
            ErrorDescription: response.message
        };
        postAjax('/Reconciliation/AddNewXeroOnDemandDataRequest', JSON.stringify(pdata), function (response) {
            if (response.Message == 'Success') {
                return true;
            }
        });
    }

    function LoadFilterData() {
        var filterData = JSON.parse(sessionStorage.getItem('Filter'));
        if (filterData) {
            $('#Filter').css("background-color", "#edf2f9");
            if (filterData.accounts != '' && filterData.accounts.split(',').length > 0) {
                $('#filteraccounts').val(filterData.accounts.split(',')).trigger('change');
            }
            $("#timepicker2").val(filterData.selectedDate);
            $("#filterMinAmount").val(filterData.amountMin);
            $("#filterMaxAmount").val(filterData.amountMax);
            if (filterData.Bankrule != '' && filterData.Bankrule.split(',').length > 0) {
                $('#bankrule').val(filterData.Bankrule.split(',')).trigger('change');
            }
            //$("#bankrule").val(filterData.Bankrule.split(','));
            if (filterData.TrackingCategory1 != null && filterData.TrackingCategory1 != '' && filterData.TrackingCategory1.length > 0) {
                $("#TrackingCategories").val(filterData.TrackingCategory1).trigger('change');
            }
            if (filterData.TrackingCategory2 != null && filterData.TrackingCategory2 != '' && filterData.TrackingCategory2.length > 0) {
                $("#TrackingCategories_1").val(filterData.TrackingCategory2).trigger('change');
            }
            if (filterData.FilterType == 0) {
                $('#rb0').prop('checked', true);
            }
            else if (filterData.FilterType == 1) {
                $('#rb1').prop('checked', true);
            }
            else if (filterData.FilterType == 2) {
                $('#rb2').prop('checked', true);
            }
            else if (filterData.FilterType == 3) {
                $('#rb3').prop('checked', true);
            }

        }

    }

    function XeroConnectionUpdate() {

        var XConstatus = $('#XeroConnectionStatus');
        $("#OnDemandData").show();
        $("#OnDemandDataAzureFunc").show();
        //if (XConstatus.val() == "True") {
        //    $("#OnDemandData").show();
        //}
        //else {
        //    $("#OnDemandData").hide()
        //}
    }

});


async function LoadPaginationContent(channelUniqueNameGuid, pageNo, pageSize) {

    getAjaxSync(apiurl + `Reconciliation/getcommentsOnreconcliationId?reconcliationId=${channelUniqueNameGuid}&&pageNo=${pageNo}&&pageSize=${pageSize}`, null, async function (responseComm) {

        await LoadAllComments(responseComm.resultData.reconciliationComments);

        if (scrollChatState.target) {

            setTimeout(_ => scrollChatState.target.scrollTop = 4, 150);
        }
        //setScrollPosition();
        hideChatContentLoader();
    });
}

function LoadOnDemandCommentsPagination(channelUniqueNameGuid, pageNo, pageSize) {
    showChatContentLoader();
    $participantsContainer = $("#chatParticipants");
    $participants = "";
    $channelName = $(".channelName");
    $channelParticipantEmail = $(".channelParticipantEmail");
    $channelReconciliationDescription = $(".channelReconciliationDescription");
    $channelReconciliationDescriptionSidebar = $(".channelReconciliationDescriptionSidebar");
    $channelReconciliationCompany = $(".channelReconciliationCompany");
    $channelReconciliationDate = $(".channelReconciliationDate");
    $channelReconciliationAmount = $(".channelReconciliationAmount");
    $messageBodyInput = $("#message-body-input");
    $chatEditorArea = $(".chat-editor-area .emojiarea");
    $messageBodyFileUploader = $("#chat-file-upload");
    $messageBodyFilePreviewerModal = $("#chat-file-previewer-modal");
    $btnSendMessage = $("#send-message");
    $channelMessages = $("#channel-messages");
    $chatSiderbarFilterButtons = $("#divChatSiderbarFilters > button");

    chat.channelUniqueNameGuid = channelUniqueNameGuid;
    getAjaxSync(apiurl + `Reconciliation/getreconciliationInfoOnId?reconcliationId=${channelUniqueNameGuid}`, null, async function (response) {

        setCommentsHeader(response.resultData);
        setTimeout(async function () {
            await LoadPaginationContent(channelUniqueNameGuid, pageNo, pageSize);
        }, 200);


    });


    $btnSendMessage.unbind().click(function () {

        addNewMessagetoChatwindow($('#message-body-input').val());

    });
    var addNewMessagetoChatwindow = async function (input) {

        if (input == "") {
            return;
        }
        addNewComment(input);
        $('#message-body-input').empty();
        $('.emojionearea-editor').empty();
        $('#message-body-input').val("");
        $('.emojionearea-editor').val("");


    }
    $chatEditorArea[0].emojioneArea.off("keydown");
    $chatEditorArea[0].emojioneArea.on("keydown", function ($editor, event) {
        if (event.keyCode === 13 && !event.shiftKey) {
            event.preventDefault();
            if (event.type == "keydown") {
                if ($('.mentions-autocomplete-list:visible li.active').length > 0) {
                    $('.mentions-autocomplete-list:visible li.active').trigger('mousedown');
                }
                else {
                    if ($editor[0].innerHTML != '')

                        addNewMessagetoChatwindow($editor[0].innerHTML);
                }

            }
            else
                activeChannel?.typing();
        }
        else
            activeChannel?.typing();
    });
    setTimeout(addMentionPlugin, 3000);

    $messageBodyFileUploader.off("change");
    $messageBodyFileUploader.on("change", function (e) {
        var files = $(this)[0].files;
        if (files.length === 0) {
            ShowAlertBoxError("File uploader", "Select atleast one file.");
            return;
        }

        if (files.length > 5) {
            ShowAlertBoxError("File uploader", "You can upload max 5 files at a time.");
            return;
        }

        //TODO//Add Functionality to Preview files before upload using fancy Box

        //Upload
        files.forEach(function (file) {
            var uploader = new Uploader(file);
            let size = uploader.getSizeInMB();
            if (size > 20) {
                ShowAlertBoxError("File size exceeded", `${uploader.getName()} file size is ${size} MB. Allowded file size is less than or equal to 20 MB`);
            }
            else {


                addMediaMessageLocalFolder(file);

            }
        })
    });
}

