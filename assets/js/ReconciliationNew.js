var glAccounts_ = document.getElementById("all-gl-accounts");
var glAccountsOpt = "<option value='-1'>Pick Account</option>";
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
    debugger;
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
function SelectAllClick(e) {
    if (e.checked == false) {
        sessionStorage.removeItem('SelectedRecords');
        sessionStorage.removeItem('UnSelectedRecords');
        $('.checkbox-bulk-select-target').closest("tr").removeClass('bg-300');

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

const resetScrollChatState = function (rec) {

    if (rec != "" && rec != null) {

        scrollChatState.reconciliationId = rec;

        scrollChatState.pageNo = 2;
        scrollChatState.size = 20;
    };
}



function InitEvents() {
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


$(document).ready(() => {

    //enable mention users in input message
    chat.type = 1;

 
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
            { data: "fdate", name: "date" },
            { data: "description_display", name: "who" },
            { data: "reference_display", name: "description" },
            { data: "amount", name: "amount" },
         
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