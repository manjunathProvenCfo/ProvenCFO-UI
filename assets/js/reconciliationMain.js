//Chat Code start
var currentChannelUniqueNameGuid = "";
var IsEnableAutomation = false;
var IsSeletedAll = false;
$(document).ready(function () {
    hideParticipantsSidebar();
    bindEnableAutomation();
    bindEnablePlaid();
    EnableSelectedBulkUpdateButton();
    //bindIsSeletedAll();
    lastModify();

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
    $(document).on("click", "#tblreconcilation tr",async function (e) {
        $("#tblreconcilation tr").removeClass("bg-200");
        $(this).addClass("bg-200");

        if ($('#divChat:visible').length > 0 && e.target.nodeName != "svg") {
            let elComment = $(this).find("#btnComment");
            
           await showReconciliationChat(elComment.data().id);
        }
    });

    $(document).on("click", "button[id=btnComment]", async function (e) {

        //let channelUniqueNameGuid = e.currentTarget.dataset.id;
        
       await showReconciliationChat(e.currentTarget.dataset.id);
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

    var showReconciliationChat = async function (channelUniqueNameGuid) {
        
        $('#divFilter').hide();
        $('#divFilter').addClass('d-none');
        $('#divBulkUpdate').hide();
        $('#divBulkUpdate').addClass('d-none');
        $('#divChat').show();
        $('#divChat').removeClass('d-none');
        $('#divTable').addClass('col-md-8').removeClass('col-md-12');

        //if (currentChannelUniqueNameGuid != channelUniqueNameGuid) {
        //    currentChannelUniqueNameGuid = channelUniqueNameGuid;
        //    chat.publicChannelUniqueNameGuid = channelUniqueNameGuid;
        //    loadChatPage(true, 1, true);
        //}

       await loadCommentsPage(channelUniqueNameGuid);

    }

    function hideParticipantsSidebar() { $(".chat-sidebar").hide(); }
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





$(document).ready(function () {

    bindNotInBooksAndBanksCount();

    /*bindNotInBooksAndBanksCount1();*/

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
            sessionStorage.clear();
        }
        else {
            $('#tabNotinBooks').removeClass('tabselect');
            $('#tabNotinBanks').addClass('tabselect');
            sessionStorage.clear();
        }

    }

    else {
        $('#tabNotinBooks').addClass('tabselect');
        $('#tabNotinBanks').removeClass('tabselect');
        /* window.location.reload();*/
    }


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
        if ($(this).is(":checked", true)) {

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
    $("#OnDemandDataPlaid").click(function () {
         if (IsEnableAutomation === false) {
            $("#OnDemandData").attr('disabled', true);
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
        //if (XConstatus.val() == "True") {
        //    $("#OnDemandData").show();
        //}
        //else {
        //    $("#OnDemandData").hide()
        //}
    }

});