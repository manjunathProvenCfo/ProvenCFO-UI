var twilioClient;
const chatPages = ["communication/chat", "reconciliation"]
const timer = ms => new Promise(res => setTimeout(res, ms));

$(function () {
    //Twilio Chat 
    if (userEmailAddress != "") {
        var filteredPages = chatPages.filter(x => (window.location.href.toLowerCase().indexOf(x) > 0 ? false : true))
        if (filteredPages.length == chatPages.length)
            setTimeout(async function () { await createTwilioClientGlobal();}, 100);
    }
    //Twilio Chat

    bindNotInBooksAndBanksCount();
    GetTotalNotesCount();

    if ((sessionStorage.getItem('SelectedMenu') == null || sessionStorage.getItem('SelectedMenu') == '') && (sessionStorage.getItem('SelectedSubMenu') == null || sessionStorage.getItem('SelectedSubMenu') == '')) {
        $("#home").addClass("show");
    }
    else {
        HighlightMenu();
    }

    $('#navbarVerticalCollapse li a').click(function (event) {
        //localStorage.setItem('SelectedSubMenu', event.currentTarget.name);
        if (event.currentTarget.parentNode.id != '' && event.currentTarget.parentNode.id.indexOf('submenu') != -1) {
            //sessionStorage.setItem('SelectedMenu', event.currentTarget.parentNode.id);
            sessionStorage.setItem('SelectedSubMenu', event.currentTarget.parentNode.id);
        }
        else {
            sessionStorage.setItem('SelectedMenu', event.currentTarget.parentNode.id);
            sessionStorage.setItem('SelectedSubMenu', '');
        }
        HighlightMenu();

    });

})


var getClientId = function () {
    return $("#ddlclient option:selected").val();
}

//Layout page
var AgencyDropdownPartialViewChange = function () {

}
var AgencyDropdownPartialViewChangeGlobalWithCallback = function (callback) {
    SetUserPreferencesForAgency(callback);
}
function SetUserPreferencesForAgency(callback) {
    $.ajax({
        url: '/AgencyService/SetUserPreferences?ClientId=' + getClientId(),
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            //Call Global function here
            if (isEmptyOrBlank(callback) === false) {
                callback();
            }
            GetTotalNotesCount();
        },
        error: function (d) {

        }
    });

}

function GetTotalNotesCount() {

    var ClientID = $("#ddlclient option:selected").val();
    getAjax(`/Notes/TotalNotesCountByAgencyId?AgencyId=${ClientID}`, null, function (response) {

        if (response.Message == "Success") {

            let data = response.ResultData;
            let TotalNotes = 0;

            for (var i = 0; i < data.length; i++) {
                TotalNotes = TotalNotes + Number(data[i].TotalNotes);
            }
            $("#lblTotalNotes").text(TotalNotes);
        }
    });
}



function bindNotInBooksAndBanksCount() {

    var ClientID = $("#ddlclient option:selected").val();

    getAjax(`/Reconciliation/GetReconciliationDataCountAgencyId?AgencyId=${ClientID}`, null, function (response) {

        if (response.Message == "Success") {
            let data = response.ResultData;
            let totalSum = 0;
            for (var i = 0; i < data.length; i++) {
                totalSum = totalSum + data[i].totalCount;
                if (data[i].type.toLowerCase() == "Outstanding Payments".toLowerCase()) {
                    $("#lblNotInBanksCount").text(data[i].totalCount);
                }
                else {
                    $("#lblNotInBooksCount").text(data[i].totalCount);
                }
            }


            $("#lblNotInCount").text(totalSum);

        }
    })
}


function HighlightMenu() {
    var activeTabs = $('.nav-item.active');
    if (activeTabs != null && activeTabs.length > 0) {
        $.each(activeTabs, function (key, value) {
            if (value != null) {
                $(value).removeClass('active');
            }
        });
    }
    var activeTabs = $('.nav-item.show');
    if (activeTabs != null && activeTabs.length > 0) {
        $.each(activeTabs, function (key, value) {
            if (value != null) {
                $(value).removeClass('show');
            }
        });
    }
    if (location.href.indexOf('AgencyHome') != -1 || (sessionStorage.getItem('SelectedMenu') == undefined && sessionStorage.getItem('SelectedSubMenu') == undefined)) {
        $('#menu_home').addClass('active');
    }
    else {
        if (sessionStorage.getItem('SelectedMenu') != undefined && sessionStorage.getItem('SelectedMenu') != null && sessionStorage.getItem('SelectedMenu') != '') {
            $("#" + sessionStorage.getItem('SelectedMenu')).addClass("show");
        }
        if (sessionStorage.getItem('SelectedSubMenu') != undefined && sessionStorage.getItem('SelectedSubMenu') != null && sessionStorage.getItem('SelectedSubMenu') != '') {
            $('#' + sessionStorage.getItem('SelectedSubMenu'))[0].className = $('#' + sessionStorage.getItem('SelectedSubMenu'))[0].className + ' active';
            $("#" + sessionStorage.getItem('SelectedSubMenu'))[0].parentElement.className = $("#" + sessionStorage.getItem('SelectedSubMenu'))[0].parentElement.className + ' show';

        }

    }

}
//Layout page

//Global Chat with Notifications Start
var getTwilioToken = function (email) {
    postAjaxSync("/twilio/token?identity=" + email, null, function (response) {
        token = response;
    });
}

var createTwilioClientGlobal = async function () {
    getTwilioToken(userEmailAddress);
    Twilio.Conversations.Client.create(token, { logLevel: 'error' })
        .then(function (createdClient) {
            twilioClient = createdClient;
            twilioClient.on("connectionStateChanged", async function (state) {
                await isTwilioClientConnected();

                await setNotificationMessageAddedListenerOnAllChannels();
                var isNotificationPage = isLocationUrlContains("communication/notifications")
                let notificationMessages = await getNotificationMessages(isNotificationPage);
            });

            createdClient.on('tokenAboutToExpire', () => {
                getTwilioToken(userEmailAddress);
                createdClient.updateToken(token);
            });
        });
}

var setNotificationMessageAddedListenerOnAllChannels = async function () {
    await isConversationSyncListFetched();
    twilioClient.conversations.conversations.forEach(function (value) {
        value.removeListener('messageAdded', notificationMessageAddedAllChannels);
        value.on('messageAdded', notificationMessageAddedAllChannels);
    });
}

var notificationMessageAddedAllChannels = function (msg) {

}

var getNotificationMessages = async function (isNotificationPage) {
    await isConversationSyncListFetched();

    getSortedChannels();
}

var getSortedChannels = function (typeOfChannel) {
    if (isEmptyOrBlank(typeOfChannel))
        typeOfChannel = "public reconciliation";
    debugger
    let channels = _.filter(twilioClient.conversations.conversations.toJson(), function (k,channel) {
        return (channel.channelState?.attributes?.hasOwnProperty("type")
            && channel.channelState.attributes.type === typeOfChannel
            && channel.channelState.hasOwnProperty("lastMessage"))
    });
    channels = _.pluck(channels, "value");
    let sortedChannels = _.sortBy(channels, function (o) { return o.channelState.lastMessage?.dateCreated; })
    console.log(sortedChannels);
}

async function isConversationSyncListFetched() {
    while (twilioClient.conversations.syncListFetched === false) {
        await timer(1000);
    }
}

async function isTwilioClientConnected() {
    while (twilioClient.connectionState !== "connected") {
        await timer(1000);
    }
}

function isLocationUrlContains(url) {
    return (location.href.toLowerCase().indexOf(url) > -1 ? true : false);
}
//Global Chat with Notifications End