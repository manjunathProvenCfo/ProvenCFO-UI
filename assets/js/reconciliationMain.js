//Chat Code start
var currentChannelUniqueNameGuid = "";
$(document).ready(function () {
    hideParticipantsSidebar();
    $(document).on("click", "button[id=btnComment]", function (e) {
        let channelUniqueNameGuid = e.currentTarget.dataset.id;
        $('#divFilter').hide();
        $('#divFilter').addClass('d-none');
        $('#divChat').show();
        $('#divChat').removeClass('d-none');
        $('#divTable').addClass('col-md-8').removeClass('col-md-12');

        if (currentChannelUniqueNameGuid != channelUniqueNameGuid) {
            currentChannelUniqueNameGuid = channelUniqueNameGuid;
            chat.publicChannelUniqueNameGuid = channelUniqueNameGuid;
            loadChatPage(true, 1);
        }
    });
});
//Chat Code end

$(document).ready(function () {
    bindNotInBooksAndBanksCount();
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

    $('#tblreconcilation tbody tr').click(function () {
        $(this).addClass('bg-300').siblings().removeClass('bg-300');
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
        if ($("#divChat").is(":visible")) {
            $('#divChat').hide();
            $('#divChat').addClass('d-none');
            $('#divBulkUpdate').hide();
            $('#divBulkUpdate').addClass('d-none');
            $('#divFilter').show();
            $('#divFilter').removeClass('d-none');

        }
        else if ($("#divFilter").is(":visible") || $("#divBulkUpdate").is(":visible")) {
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
    $('#ichat').click(function () {
        if ($("#divChat").is(":visible")) {
            $('#divChat').hide();
            $('#divChat').addClass('d-none');
            $('#divFilter').hide();
            $('#divBulkUpdate').hide();
            $('#divBulkUpdate').addClass('d-none');
            $('#divFilter').addClass('d-none');
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');

        }
        else if ($("#divFilter").is(":visible") || $("#divBulkUpdate").is(":visible")) {
            $('#divChat').hide();
            $('#divChat').addClass('d-none');
            $('#divFilter').hide();
            $('#divFilter').addClass('d-none');
            $('#divBulkUpdate').hide();
            $('#divBulkUpdate').addClass('d-none');
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');
        }
        else {
            $('#divChat').show();
            $('#divChat').removeClass('d-none');
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
        }
        else {
            $('#divTable').addClass('col-md-8').removeClass('col-md-12');
            $('#divBulkUpdate').removeClass('d-none');
            $('#divBulkUpdate').show();
        }
    });
    $('.checkbox-bulk-select-target').change(function (e) {
        if (this.checked) {
            var SelectedItems = sessionStorage.getItem('SelectedRecords') != null ? sessionStorage.getItem('SelectedRecords') + ',' + e.target.id : e.target.id;
            sessionStorage.setItem('SelectedRecords', SelectedItems);
        }
        else {
            var UnSelectedItems = sessionStorage.getItem('UnSelectedRecords') != null ? sessionStorage.getItem('UnSelectedRecords') + ',' + e.target.id : e.target.id;
            sessionStorage.setItem('UnSelectedRecords', UnSelectedItems);
        }
    });

    $("#OnDemandData").click(function () {

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