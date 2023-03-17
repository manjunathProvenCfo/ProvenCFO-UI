var glAccounts_ = document.getElementById("all-gl-accounts");
var glAccountsOpt = "<option value='-1'>Pick Account</option>";
HidelottieLoader();
Array.prototype.forEach.bind(glAccounts_.children)(ele => {
    glAccountsOpt += `<option value=${ele.value}>${ele.innerText}</option>`;
});

function RenderBankRule(r, r1, r3) {
    if (r == true) {
        return `<div class="row justify-content-between">
                                                        <div class="col-auto">
                                                            <input id="ruleCheck" name="ruleCheck" onchange="javascript:onChangeBankRules('${r3.id}',this.checked,this)" checked="true" type="checkbox" value="true">

                                                        </div>
                                                    </div>`;

    }
    return `<div class="row justify-content-between">
                                                        <div class="col-auto">
                                                            <input id="ruleCheck" name="ruleCheck" onchange="javascript:onChangeBankRules('${r3.id}',this.checked,this)"  type="checkbox" value="false">

                                                        </div>
                                                    </div>`;
}
function GLAccountsRender(r, r1, r3) {  //value,name,row

    var select = `<div class="col-auto lastmodified" id="Gltoggel" data-toggle="tooltip" data-html="true" utc="${r3.GlAccountModifiedDateUTC}" ModifiedBy="${r3.GlAccountModifiedBy}" title="No Modification yet.">
  <select class="select-picker gl-account"  utcdate=${r3.GlAccountModifiedDateUTC} ModifiedBy=${r3.GlAccountModifiedBy} data-reconciliationId=${r3.id} data-selectedValue=${r} style="width:90%;">${glAccountsOpt}</select></div>`;

    return select;
}

function onChangeBankRules(id, value, obj) {

    obj.value = obj.checked;

    var ClientID = $("#ddlclient option:selected").val();
    postAjax('/Reconciliation/UpdateReconciliation?AgencyID=' + ClientID + '&id=' + id + '&GLAccount=' + 0 + '&RuleNew=' + value + '&TrackingCategory=' + 0, null, function (response) {
        if (response.Message == 'Success') {

        }
        else {

        }
    })
}

//check
function onChangeglAccount(id, event) {

    var selectedValue = event.currentTarget.value;
    if (isEmptyOrBlank(selectedValue)) {
        selectedValue = -1;
    }
    var userId = $('#topProfilePicture').attr('userId');
    var ClientID = $("#ddlclient option:selected").val();
    postAjax('/Reconciliation/UpdateReconciliation?AgencyID=' + ClientID + '&id=' + id + '&GLAccount=' + selectedValue + '&BankRule=' + 0 + '&TrackingCategory=' + 0 + '&userId=' + userId, null, function (response) {
        if (response.Message == 'Success') {

        }
        else {

        }
    })
}

var lastModify = function () {
    $(".lastmodified").each(function () {
        var utctime = $(this).attr("utc");
        var ModifiedBy = $(this).attr("ModifiedBy");

      

        if (utctime != null && ModifiedBy != undefined && utctime != '' && ModifiedBy != '') {
            var mod = (utctime + "").match(/([0-9]+)/g);
            if (mod == null) {
                
                $(this).attr("data-original-title", "No Modification yet.");
            } else {

                let modified = mod.length > 0 ? mod[0] : "";
                var localtime = getLocalTime(new Date(parseInt(modified)));
             var msg = "Last Modified by <br> " + ModifiedBy + " <br> " + localtime;
             $(this).attr("data-original-title", msg);
           }
        }
        else {
            $(this).attr("data-original-title", "No Modification yet.");
        }
    });
}

function ChatRender(r, r1, r2) {

    let IsMentioned = r2.Iscurrent_user_mentioned;
    let dataId = r2.id;
    let has_conversation = r2.has_twilio_conversation;
    
    if (IsMentioned == true) {
        return `      <button id="btnComment" data-id="${dataId}" class="btn btn-link btn-sm btn-reveal mr-3" type="button" data-boundary="html" aria-expanded="false">
                                                <span class="fas fa-comment fs--1"></span>
                                            </button>`;
    } else {

       var str = `<button id="btnComment" data-id="${dataId}" class="btn btn-link btn-sm btn-reveal mr-3" type="button" data-boundary="html" aria-expanded="false">`

                                                if(has_conversation == true)
                                                {
                                                    str +=`<span class="far fa-comment fs--1"></span>`;

                                                }
                                                else
                                                {
                                                    str+=`<span class="far fa-comment fs--1 text-dark"></span>`;
                                                }

        str += `</button>`;
        return str;
    }

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


function SelectAllClick(e) {
    if (e.checked == false) {
        sessionStorage.removeItem('SelectedRecords');
        sessionStorage.removeItem('UnSelectedRecords');
        $('.checkbox-bulk-select-target').closest("tr").removeClass('bg-300');
      
        $(".checkbox-bulk-select-target").trigger("click");
    } else if (e.checked==true){
    
        $(".checkbox-bulk-select-target").trigger("click");
    }

    EnableSelectedBulkUpdateButton();

}
function SelectClick(e) {
    
    if (e.checked) {
        var UnSelectedRecords = isEmptyOrBlank(sessionStorage.getItem('UnSelectedRecords')) === false ? sessionStorage.getItem('UnSelectedRecords')?.split(',') : [];


        UnSelectedRecords = UnSelectedRecords.filter(function (x) {
            return x != e.id
        });
        sessionStorage.setItem('UnSelectedRecords', UnSelectedRecords);

        var SelectedItems = isEmptyOrBlank(sessionStorage.getItem('SelectedRecords')) === false ? (sessionStorage.getItem('SelectedRecords') + ',' + e.id) : e.id;
        sessionStorage.setItem('SelectedRecords', SelectedItems);

    }
    else {

        var SelectedItems = isEmptyOrBlank(sessionStorage.getItem('SelectedRecords')) === false ? sessionStorage.getItem('SelectedRecords')?.split(',') : [];
        SelectedItems = SelectedItems.filter(function (x) {
            return x != e.id
        });

        sessionStorage.setItem('SelectedRecords', SelectedItems);

        var UnSelectedItems = isEmptyOrBlank(sessionStorage.getItem('UnSelectedRecords')) === false ? (sessionStorage.getItem('UnSelectedRecords') + ',' + e.id) : e.id;
        sessionStorage.setItem('UnSelectedRecords', UnSelectedItems);


    }
    EnableSelectedBulkUpdateButton();
}
const scrollChatState = {
    size: 20,
    pageNo: 2,
    reconciliatioId: '',
    target: null
};



function ResetCheckBoxOnPageChange() {

    var IsAllSelected = $('#checkbox-bulk-purchases-select')[0].checked;

    if (IsAllSelected) {
        $(".checkbox-bulk-select-target").trigger("click");
        return;
    } else {

       
        
       let SelectedItems = isEmptyOrBlank(sessionStorage.getItem('SelectedRecords')) === false ? sessionStorage.getItem('SelectedRecords')?.split(',') : [];

        let checkboxs = $(".checkbox-bulk-select-target");
        SelectedItems.forEach(id => {

            Array.prototype.forEach.bind(checkboxs)(chkBox => {
                if (chkBox.getAttribute("id") == id) {
                    chkBox.checked = true;
                } else {
                    chk.checked = false;
                }
            });
        });

         

        $(".checkbox-bulk-select-target").trigger("change");



        
    }
}
const resetScrollChatState = function (rec) {

    if (rec != "" && rec != null) {

        scrollChatState.reconciliationId = rec;

        scrollChatState.pageNo = 2;
        scrollChatState.size = 20;
    };
}
var IsEnableAutomation = false;

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

function InitEvents() {



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
                debugger;
                getAjax(AzureFunctionReconUrl + `?AgencyId=${getClientId()}&NotInbooks=${IsNotinBooks}&NotInbanks=${IsNotinBanks}`, null, function (Azureresponse) {
                    Azureresponse = JSON.parse(Azureresponse);
                    UpdateXeroonDemandDatarequestStatus(Azureresponse, RequestID);
                    if (Azureresponse.status === true && Azureresponse.statusCode == 200) {
                        sessionStorage.removeItem("NotInBooksData");
                        sessionStorage.removeItem("NotInBanksData");

                        //uncampatible on safari.
                        let finalAzureMessage = ((((Azureresponse.message.replace("Success : ", "")).replace(" =", "=")).replace(/=/g, ": ")).replace(/(?=Not)|(?=In)/g, " ")).replace(/b/g, "B");
                        debugger;
                     
                        ShowAlertBoxSuccess("Success!", "Successfully synced with Xero. \n" + finalAzureMessage, function () { window.location.reload(); });
                    }
                    else if (Azureresponse.status === false && Azureresponse.statusCode != 500) {
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


    $("#CancelBulkupdate").click(function () {

        if ($("#divBulkUpdate").is(":visible")) {

            $('#divBulkUpdate').hide();
            $('#divBulkUpdate').addClass('d-none');
            $('#divTable').addClass('col-md-12').removeClass('col-md-8');

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
    $('#Cancel1').click(function () {
        $("#Filter").trigger("click");
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
            $("#example").DataTable().search("").draw();
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





   
}
var agencyId = "";
function UtcDateToLocalTime(utcDate) {

    if (utcDate != null) {
        var localTime;
        let timeZoneOffset = new Date().getTimezoneOffset();
        let utcServerDateTime = new Date(utcDate);
        let utcTimeInMilliseconds = utcServerDateTime.getTime();


        /*
            timeZoneOffset :minutes
         */
        switch ((timeZoneOffset > 0)) {
            case true:
                localTime = new Date(utcTimeInMilliseconds - (timeZoneOffset * 60000)); //in one minutes there is 60,000 milliseconds
                break;
            case false:
                localTime = new Date(utcTimeInMilliseconds + ((-1 * timeZoneOffset) * 60000));
                break;
        }

        return localTime;


    }

    return utcDate;
}
function LoadLastRunDate() {
 
    $.ajax({
        url: '/Reconciliation/GetEndYearLockDate?id=' + agencyId,
        type: "GET",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            if (data.DOMO_Last_batchrun_time != null) {
                $('#domoLastBatchRun').show();
                let roughDate = data.DOMO_Last_batchrun_time;
                let dateTimeMill = Number(roughDate.match(/\d+/)[0]);

                let utcDateTime = new Date(dateTimeMill);
                var localTime = utcDateTime.toLocaleString();
             
                 localTime = UtcDateToLocalTime(localTime)
                var _date = `${localTime.getMonth() + 1}/${localTime.getDate()}/${localTime.getFullYear()} ${localTime.getHours()}:${localTime.getMinutes()} ${(localTime.getHours() >= 12 ? "PM" : "AM")}`;

                $('#domoLastBatchRunTime')[0].innerText = _date;


            } else {
                $('#domoLastBatchRun').remove();
            }

            if (data.End_Of_YearLockDate != null) {

                $('#endOfYearLock').show();
                let roughEndDate = data.End_Of_YearLockDate;
                let endDateTimeMill = Number(roughEndDate.match(/\d+/)[0]);

                let utcDateTime = new Date(endDateTimeMill);
                var localEndDateTime = utcDateTime.toLocaleDateString();
                $('#endOfYearLockDate')[0].innerText = localEndDateTime;

            } else {
                $('#endOfYearLock').remove();
            }
        },
        error: function (error) {
            console.log(error);
            $('#domoLastBatchRun').remove();
            $('#endOfYearLock').remove();
        }
    })

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


function BulkActionReconcilation(CommentText) {
    $(".apply-btn").prop("disabled", true);
    $(".blk-cancel").prop("disabled", true);

    var GLaccount = $('#BA_filterGLaccounts').val();

    var CreatedBy = $('#topProfilePicture').attr('userId');
    var Action = $('#BA_filterAction').val();
    var ClientID = $("#ddlclient option:selected").val();
    var bankrule = $('#BA_bankrules').val();
    var ruleNewStr = $('#bulkRule_New').val();

    if (ruleNewStr != "") {
        var ruleNew = "true" === ruleNewStr;
    }
    var reconcilationStatus = $('#BA_Status').val();
    var TrackingCategories = $("#BA_TrackingCategories").val() != undefined ? $("#BA_TrackingCategories").val() : 0;
    var AditionalTrackingCategories = $("#BA_TrackingCategories_1").val() != undefined ? $("#BA_TrackingCategories_1").val() : 0;
    
    var IsAllSelected = $('#checkbox-bulk-purchases-select')[0].checked;
    var SelectedItems = sessionStorage.getItem('SelectedRecords');
    var UnSelectedRecords = sessionStorage.getItem('UnSelectedRecords');
    //IsAllSelected = $("#checkbox-bulk-purchases-select")[0].getAttribute("isselected") == "true";

    if (
        ((SelectedItems == null && IsAllSelected != true) || (IsAllSelected != true && SelectedItems == ''))) {
        ShowAlertBoxWarning("No records are selected to perform bulk action.");
        return;
    }

    if (GLaccount == '' && bankrule == '' && reconcilationStatus == '' && (TrackingCategories == '' || TrackingCategories == 0) && (AditionalTrackingCategories == '' && AditionalTrackingCategories == 0 && Action == '')) {
        ShowAlertBoxWarning("No Option are selected to perform bulk action.");
        return;
    }
    var type = 'Outstanding Payments';
    var IsNotinBooks = $('#tabNotinBooks').hasClass('tabselect');
    if (IsNotinBooks == true) {
        type = 'Unreconciled';
        sessionStorage.setItem('Type', 0);
    }
    else {
        sessionStorage.setItem('Type', 1);
    }

    let pdata = {
        GLaccount: GLaccount, TrackingCategory: TrackingCategories,
        AditionalTrackingCategory: AditionalTrackingCategories, BankRule: bankrule,
        AgencyID: ClientID,
        IsAllSelected: IsAllSelected,
        SelectedItems: SelectedItems,
        UnSelectedRecords: UnSelectedRecords,
        reconcilationStatus: reconcilationStatus,
        Action: Action,
        CommentText: CommentText,
        CreatedBy: CreatedBy,
        RuleNew: ruleNew
    };

    ShowlottieLoader();
    postAjax('/Reconciliation/ReconcilationBuilAction', JSON.stringify(pdata), function (response) {
        if (response.Message == 'Success') {
            sessionStorage.removeItem('SelectedRecords');
            sessionStorage.removeItem('UnSelectedRecords');
            ShowAlertBoxSuccess("Success!", "Successfully updated " + response.UpdatedCount + " records.", function () {
                sessionStorage.clear();

                HidelottieLoader();
                location.reload(true);

            });
        }
        else if (response.Message == 'NoRecords') {
            ShowAlertBoxWarning("No records are selected to perform bulk action.");
            return;
        }
        else {
            ShowAlertBoxError("Error", "Error while updating records.");
            return;
        }
    })
}

function replaceAll(string, search, replace) {
    return string.split(search).join(replace);
}



$(document).ready(() => {

    //enable mention users in input message
    agencyId =  $("#ddlclient option:selected").val();
    LoadLastRunDate();
    chat.type = 1;
    bindEnableAutomation();
    sessionStorage.removeItem('SelectedRecords');
    sessionStorage.removeItem('UnSelectedRecords');
    HidelottieLoader();
    sendEmail();
 
    switch (RecordType) {
   
        case "Unreconciled":
            $('#tabNotinBooks').addClass('tabselect');
            break;
        case "Outstanding Payments":
            $('#tabNotinBanks').addClass('tabselect');
            break;
    } 

    InitEvents();
   
    const configureColumn = function () {

        var column = [
            {
                data: "id", name: "id", sortable: false, render: (r1, r2, r3) => {

                    return `<div class="custom-control custom-checkbox">
                                            <input class="custom-control-input checkbox-bulk-select-target" onclick="SelectClick(this);" type="checkbox" id="${r1}" value="${r1}" />

                                            <label class="custom-control-label" for="${r1}"></label>
                                        </div>
                                    `;
                },
                orderable: false,
                searchable: false
            },
            { data: "account_name", name: "account_name" },
            {
                data: "fdate", name: "date", render: function (r1, r2, r3) {


                    return ("" + r1).replace(/-/g,'/');
                }
            },
            { data: "description_display", name: "who" },
            { data: "reference_display", name: "description" },
            {
                data: "amount", name: "amount", render: function (r1, r2, r3) {

                    
                    let num = r1;
                    
                    if (0>r1) {


                        let res = (r1 + "").split('').reverse();
                        res.pop();

                        res = res.reverse();
                        res = res.join('');

                        num = `<span style="color:red">$(${res})</span>`;

                    }
                    return  num  ;
                }
            },
         
        ];

        const GLColumn = [
            { data: "gl_account_ref", name: "gl_account_ref", render: GLAccountsRender },
           ];
        const BankRule = [{
            data: "RuleNew", name: "RuleNew", render: RenderBankRule
        }];

        if (GlAccountVisible == "True") {
            column = [...column, ...GLColumn];
        }
        if (IsBankRuleVisible == "True") {
            column = [...column, ...BankRule];
        }

        //Add Chat column
        column.push({
            orderable: false,
            searchable: false,
            render: ChatRender
        });

        return column;
    };
    var showReconciliationChat = async function (channelUniqueNameGuid) {
      
        resetScrollChatState(channelUniqueNameGuid);
        $('#divFilter').hide();
        $('#divFilter').addClass('d-none');
        $('#divBulkUpdate').hide();
        $('#divBulkUpdate').addClass('d-none');
        $('#divChat').show();
        $('.chat-sidebar').addClass('d-none');
        $('#divChat').removeClass('d-none');
        $('#divTable').addClass('col-md-8').removeClass('col-md-12');

        await loadCommentsPage(channelUniqueNameGuid);

    }



    $("#example").DataTable({
        paging: true,
        serverSide: true,
        pageLength: 10,
        drawCallback: function (a, b) {

            ResetCheckBoxOnPageChange();

            HidelottieLoader();
             Array.prototype.forEach.bind($(".gl-account"))(selectEle => {
                let index = 0;
                let value = selectEle.getAttribute("data-selectedValue");
                Array.prototype.forEach.bind($(selectEle.children))(opt => {
                    index++;
                    if (opt.value == value) {
                        selectEle.selectedIndex = index - 1;
                    }
                });

            });
            $(".gl-account").trigger("change");

            $(".gl-account").on("change", function (e) {
                var self = e.currentTarget;
                let Id = self.getAttribute("data-reconciliationId");

                onChangeglAccount(Id, e);
            })

            $(".select-picker").select2();
            $('.lastmodified').tooltip();
            $("table").on("click", "button[id=btnComment]", async function (e) {
                resetScrollChatState(e.currentTarget.dataset.id);
                await showReconciliationChat(e.currentTarget.dataset.id);
            });
             setTimeout(()=>lastModify(),0);
        },
        ajax: {
            url: window.location.origin+"/Reconciliation/ReconcilationPaginiation",
            type: "POST"
        },
        processing: true,
        columns: configureColumn()
    }

    );

});


function sendEmail() {
    var chat = "/Communication/Chat";
    let enchat = encodeURIComponent(chat);
    $('.text-danger').empty();
    $("#sendbutton").attr("disabled", false);
    var ClientName = $("#ddlclient option:selected").text();
    var ClientId = $("#ddlclient option:selected").val();
    getClientDate(ClientName);
    ClientName = encodeURIComponent(ClientName);
    getAjaxSync(apiurl + `Reconciliation/GetAllCommentedReconciliations?AgencyID=${ClientId}&MaxCount=${0}`, null, function (response) {
        let CommentsCount = response.resultData.length;
        /*var Reconciliationdata = response.resultData.slice;*/
        let count = CommentsCount;
        var NotInBankUnreconciledItemsCount = count;
        var LastMailSent = sessionStorage.getItem("LastMailSent");
        getAjax(`/Reconciliation/EmailSend?ClientName=${ClientName}&ClientId=${ClientId}&NotInBankUnreconciledItemsCount=${NotInBankUnreconciledItemsCount}&url=${enchat}&sentdate=${LastMailSent}`, null, function (response) {
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
}

/*Clientdata*/


function getClientDate(ClientName) {
    getAjaxSync(apiurl + `Reconciliation/getLastSentDate?ClientName=${ClientName}`, null, function (response) {
        sessionStorage.setItem("LastMailSent", response.resultData);
    });
}
$("#email-to").on("focus", function (e) {
    e.preventDefault();
    e.target.removeAttribute("readonly");

});
$("#email-to").on("blur", function (e) {
    e.preventDefault();
    e.target.setAttribute("readonly", "");
    /*validateMultipleEmails($('#email-to').val());*/
});

function validateMultipleEmails(emailinput) {
    var emails = emailinput;
    var invalidEmails = [];
    if (emails == "") {
        $('.text-danger').empty();
        $('.emailvalidation').append('<span class="text-danger">Please Enter Email </span>');
        $("#sendbutton").attr("disabled", true);
        return;
    }
    emails = emails.split(',');
    var regexs = /^([A-Za-z0-9_\-\.])+\@@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;;



    for (var i = 0; i < emails.length; i++) {
        //Trimming the whitespace from emails if any
        emails[i] = emails[i].trim();
        //Cheking the invalid emails and push them into an array
        if (!regexs.test(emails[i])) {
            invalidEmails.push(emails[i]);
        }
    }
    if (invalidEmails.length != 0) {
        $('.text-danger').empty();
        $('.emailvalidation').append('<span class="text-danger">Invalid emails: ' + invalidEmails.join(',') + '</span>');
        $("#sendbutton").attr("disabled", true);
    }
    else if (invalidEmails.length == 0) {
        $('.emailvalidation .text-danger').remove();
        $("#sendbutton").attr("disabled", false);
    }
}

function sendMail() {
    var recip = $("#email-to").val();
    var subject = $("#email-subject").val();
    var body = $("#ibody").html();
    var pdata = {
        Recipents: recip,
        Subject: subject,
        Body: body,
    };
    postAjaxSync(apiurl + `Reconciliation/sendReconcilationEmail`, JSON.stringify(pdata), function (response) {
        ShowAlertBoxSuccess("", "Email has been sent ", function () { window.location.reload(); });
    });
}

var hideParticipantsSidebar = function () { $(".chat-sidebar").hide(); }

var onChangeAditinalBA = function (e) {
    var BankAccount = $("#QBO_AccountsFilter option:selected").text();
    if (BankAccount == "Select Bank Account") {
        $("#btnDropzoneUpload").attr('disabled', true);
        $('#btnDropzoneUpload').css('cursor', 'not-allowed');
        $("#btnDropzoneUpload").attr('title', 'Please select a bank account.');
    }
    else {
        $("#btnDropzoneUpload").attr('disabled', false);
        $('#btnDropzoneUpload').css('cursor', '');
        $("#btnDropzoneUpload").attr('title', '');
    }
}
$("#email-subject").on("focus", function (e) {
    e.preventDefault();
    e.target.removeAttribute("readonly");
});
$("#email-subject").on("blur", function (e) {
    e.preventDefault();
    e.target.setAttribute("readonly", "");
});

/*Impote*/

var myParam = location.search.split('yes=')[1];

if (myParam == "1") {
    $("#email").show();
    $("#btnImportReconcilition").show();
}
