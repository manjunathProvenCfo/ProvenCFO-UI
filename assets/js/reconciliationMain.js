//Chat Code start
var currentChannelUniqueNameGuid = "";
var IsEnableAutomation = false;
$(document).ready(function () {
    hideParticipantsSidebar();
    bindEnableAutomation();
   

    $("#ichat").click(function () {
        //let elCheckbox = $(".checkbox-bulk-select-target:checked:first");
        let elCheckbox = $("table tr.bg-300 td:first .checkbox-bulk-select-target");
        if (elCheckbox.length === 0) {
            ShowAlertBoxWarning("Please select reconciliation row!");
        }
        else {
            let reconciliaitonId = elCheckbox.attr("id");
            showReconciliationChat(reconciliaitonId);
        }
    });
    $(document).on("click", "#tblreconcilation tr", function (e) {
        $("#tblreconcilation tr").removeClass("bg-200");
        $(this).addClass("bg-200");

        if ($('#divChat:visible').length > 0) {
            let elComment = $(this).find("#btnComment");
            showReconciliationChat(elComment.data().id);
        }
    });

    $(document).on("click", "button[id=btnComment]", function (e) {
        //let channelUniqueNameGuid = e.currentTarget.dataset.id;
        showReconciliationChat(e.currentTarget.dataset.id);
        //$('#divFilter').hide();
        //$('#divFilter').addClass('d-none');
        //$('#divChat').show();
        //$('#divChat').removeClass('d-none');
        //$('#divTable').addClass('col-md-8').removeClass('col-md-12');

        //if (currentChannelUniqueNameGuid != channelUniqueNameGuid) {
        //    currentChannelUniqueNameGuid = channelUniqueNameGuid;
        //    chat.publicChannelUniqueNameGuid = channelUniqueNameGuid;
        //    loadChatPage(true, 1);
        //}
    });
    var showReconciliationChat = function (channelUniqueNameGuid) {
        $('#divFilter').hide();
        $('#divFilter').addClass('d-none');
        $('#divBulkUpdate').hide();
        $('#divBulkUpdate').addClass('d-none');
        $('#divChat').show();
        $('#divChat').removeClass('d-none');
        $('#divTable').addClass('col-md-8').removeClass('col-md-12');

        if (currentChannelUniqueNameGuid != channelUniqueNameGuid) {
            currentChannelUniqueNameGuid = channelUniqueNameGuid;
            chat.publicChannelUniqueNameGuid = channelUniqueNameGuid;
            loadChatPage(true, 1, true);
        }
    }
});
var bindEnableAutomation = function () {
        getAjaxSync(`/Reconciliation/GetIsEnableAutomation?agencyId=${getClientId()}`, null, function (response) {
            if (response.Status === "Success") {

                IsEnableAutomation = response.Data;
                if (IsEnableAutomation === false) {
                    $("#OnDemandData").attr('disabled', true);
                    $("#OnDemandData").attr('title', 'Request on demand data has been disabled.');

                }
              
            }
        });
    }


$(document).ready(function () {

    bindNotInBooksAndBanksCount();

    bindNotInBooksAndBanksCount1();
   
    LoadFilterData();
   
   
    XeroConnectionUpdate();
    var type = sessionStorage.getItem('Type');
    $('#divFilter').hide();
    $('#divChat').hide();
    sessionStorage.removeItem('SelectedRecords');
    sessionStorage.removeItem('UnSelectedRecords');
    if (type != null) {

        if (type == "0") {
            $('#tabNotinBooks').addClass('tabselect');
            $('#tabNotinBanks').removeClass('tabselect');
        }
        else {
            $('#tabNotinBooks').removeClass('tabselect');
            $('#tabNotinBanks').addClass('tabselect');
        }
    }

    else {
        $('#tabNotinBooks').addClass('tabselect');
        $('#tabNotinBanks').removeClass('tabselect');
    }
    sessionStorage.clear();

    $('.checkbox-bulk-select-target').click(function () {
        
        if ($(this).is(":checked")) {
            
            $(this).closest('tr').addClass('bg-300');
        }
        else {
            $(this).closest("tr").removeClass('bg-300');
        }
    });
    /* checkbox - bulk - purchases - select*/
   
    $('.checkbox-bulk-select').change(function () {  
        if ($(this).is(":checked",true)) {

            $(this).closest('table').addClass('bg-300');
        }
        else {
            $(this).closest("table").removeClass('bg-300');
        }
        //$(".checkbox-bulk-select").attr('checked', false);
        //$(this).closest('tr').css("background-color", "#ff0000");
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

    $("#OnDemandData").click(function () {

        if (IsEnableAutomation === false) {
            $("#OnDemandData").attr('disabled', true);
            return;
        }
            
        
            $("#Loader").removeAttr("style");
            var ClientID = $("#ddlclient option:selected").val();
            var RequestType = "On Demand";
            var RequestedAtUTC = '';
            var CurrentStatus = "New";
            var RequestCompletedAtUTC = '';
            var Remark = '';

            var AgencyName = '';
            var CreatedBy = '';
            var CreatedDate = '';

            var pdata = { RequestType: RequestType, RequestedAtUTC: RequestedAtUTC, CurrentStatus: CurrentStatus, RequestCompletedAtUTC: RequestCompletedAtUTC, Remark: Remark, AgencyId: ClientID, AgencyName: AgencyName, CreatedBy: CreatedBy, CreatedDate: CreatedDate };
            postAjax('/Reconciliation/AddNewXeroOnDemandDataRequest', JSON.stringify(pdata), function (response) {
                if (response.Message == 'Success') {
                    setTimeout(() => {
                        reconcilationonstatusDemand(response.data.Id);
                    }, 1000);
                }

            });
      
    });


});