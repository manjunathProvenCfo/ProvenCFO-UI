var twilioClient;
var notificationChannels;
var notificationChannelParticipants;
var isNotificationsAlreadyFetched = false;
var $notifictionsDropDown = $("#navbarDropdownNotification");
var $notifictionsList = $("#navbarDropdownNotificationListGroup");
var $divNotificationsCard = $("#divNotificationsCard");
var $divNotificationsCardBody = $("#divNotificationsCard .card-body");
var notifications = [];
var addMessageProcessedGlobal = [];

const _audio = new Audio("/assets/audio/notification.mp3");
const chatPages = ["communication/chat"]//reconciliation
const timer = ms => new Promise(res => setTimeout(res, ms));


var Default_Profile_Image = "/assets/img/team/default-logo.png";
const Notification_Bell_Size = 2;

$(function () {
    //Twilio Chat
    twilioChatGlobal();
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

var twilioChatGlobal = function (isNotiAlreadyFetched) {
    if (!isEmptyOrBlank(isNotiAlreadyFetched))
        isNotificationsAlreadyFetched = false;

    if (userEmailAddress !== "") {
        var filteredPages = chatPages.filter(x => (window.location.href.toLowerCase().indexOf(x) > 0 ? false : true))
        if (filteredPages.length === chatPages.length)
            //setTimeout(async function () { await createTwilioClientGlobal(); }, 1);
            createTwilioClientGlobal();
    }
}

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
            try {
                twilioChatGlobal(false);
            } catch (e) { console.log("error in twilioChatGlobal,called from SetUserPreferencesForAgency(callback)"); }
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

                if (isNotificationsAlreadyFetched === false) {
                    isNotificationsAlreadyFetched = true;
                    showWaitMeLoader($notifictionsList);
                    showWaitMeLoader($divNotificationsCard);

                    await setNotificationMessageAddedListenerOnAllChannels();

                    var isNotificationPage = isLocationUrlContains("communication/notifications")
                    await bindNotificationMessages(isNotificationPage);
                }
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
        value.off('messageAdded', notificationMessageAddedAllChannels);
        value.on('messageAdded', notificationMessageAddedAllChannels);
    });
}

var notificationMessageAddedAllChannels = function (msg) {
    let msgState = msg.state;
    if (addMessageProcessedGlobal.indexOf(msgState.sid) > -1)
        return;
    addMessageProcessedGlobal.push(msgState.sid);

    var isNotificationPage = isLocationUrlContains("communication/notifications")
    if (userEmailAddress.toLowerCase() !== msgState.author.toLowerCase()) {
        let allMentionedMessages = [];

        if (msgState.type === "text") {
            let matches = _.flatten(findMentionInMessageBody(msgState.body));
            if (matches.length > 0 && matches.filter(x => { if (x.toLowerCase() === userEmailAddress.toLowerCase()) return true; return false; }).length > 0) {
                allMentionedMessages.push(msgState);
            }
            allMentionedMessages = _.flatten(allMentionedMessages);


            if (allMentionedMessages !== null && allMentionedMessages.length > 0) {
                for (var y = 0; y < allMentionedMessages.length; y++) {
                    let recData = notificationChannels.get(msg.conversation.sid);
                    notifications.push({ channelId: msg.conversation.sid, channelUniqueName: msg.conversation.channelState.uniqueName, messageId: allMentionedMessages[y].sid, messageBody: allMentionedMessages[y].body, messageAuthor: allMentionedMessages[y].author, messageDate: allMentionedMessages[y].dateUpdated, recAccNameWithDesc: `${recData.AccountName}(${recData.ReconciliationDescription})` });
                }
            }
        }
        if (isEmptyOrBlank(notificationChannelParticipants.get(notifications[notifications.length - 1].messageAuthor.toLowerCase()))) {
            let emails = _.uniq(_.map(notifications, (x) => x.messageAuthor.toLowerCase())).join(",");
            getAjaxSync(`/Communication/GetNotificationParticipantsByEmails?emails=${emails}`, null, function (response) {
                if (!isEmptyOrBlank(response)) {
                    notificationChannelParticipants = new Map(response.map(i => [i.Email.toLowerCase(), i]));
                }
            })
        }
        preapreAndBindNotifications([notifications[notifications.length - 1]], isNotificationPage, true);
        _audio.play();
    }
}

var bindNotificationMessages = async function (isNotificationPage) {
    await isConversationSyncListFetched();

    let sortedChannels = getSortedChannels();
    getAjaxSync(`/Communication/GetNotificationChannels?clientId=${parseInt(getClientId())}`, null, function (response) {
        if (!isEmptyOrBlank(response)) {
            notificationChannels = new Map(response.map(i => [i.ChannelId, i]));
        }
    })
    sortedChannels = _.filter(sortedChannels, function (channel) {
        if (!isEmptyOrBlank(notificationChannels.get(channel.sid))) return true;
        return false;
        //if (_.filter(notificationChannels, { ChannelId: channel.sid }GetNotificationParticipantsByEmails).length > 0) return true;
        //return false;
    });

    getNotifications(sortedChannels, isNotificationPage).then(function (notifications, isNotiPage = isNotificationPage) {
        let emails = _.uniq(_.map(notifications, (x) => x.messageAuthor.toLowerCase())).join(",");
        getAjaxSync(`/Communication/GetNotificationParticipantsByEmails?emails=${emails}`, null, function (response) {
            if (!isEmptyOrBlank(response)) {
                notificationChannelParticipants = new Map(response.map(i => [i.Email.toLowerCase(), i]));
            }
        })
        if (notifications.length > 0)
            notifications = notifications.reverse();
        preapreAndBindNotifications(notifications, isNotiPage);
    });


}

var getSortedChannels = function (typeOfChannel) {
    if (isEmptyOrBlank(typeOfChannel))
        typeOfChannel = "public reconciliation";

    let allChannels = Array.from(twilioClient.conversations.conversations.values());
    let channels = _.filter(allChannels, function (channel) {
        return (channel.channelState?.attributes?.hasOwnProperty("type")
            && channel.channelState.attributes.type === typeOfChannel
            && channel.channelState.hasOwnProperty("lastMessage"))
    });

    let sortedChannels = _.sortBy(channels, function (o) { return o.channelState.lastMessage?.dateCreated; });
    sortedChannels = sortedChannels.reverse();
    return sortedChannels;
}

var getNotifications = async function (sortedChannels, isNotificationPages) {

    let totalNotifications = 0;
    let counter = (isNotificationPages ? sortedChannels.length : Notification_Bell_Size);

    for (var i = 0; i < sortedChannels.length && notifications.length < counter; i++) {
        let mentionNotification = await findAndParseMentionInNotification(sortedChannels[i]);
        if (mentionNotification !== null && mentionNotification.length > 0) {
            for (var y = 0; y < mentionNotification.length; y++) {
                let recData = notificationChannels.get(sortedChannels[i].sid);
                notifications.push({ channelId: sortedChannels[i].sid, channelUniqueName: sortedChannels[i].channelState.uniqueName, messageId: mentionNotification[y].sid, messageBody: mentionNotification[y].body, messageAuthor: mentionNotification[y].author, messageDate: mentionNotification[y].dateUpdated, recAccNameWithDesc: `${recData.AccountName}(${recData.ReconciliationDescription})` });
            }
        }
    }
    notifications = _.flatten(notifications);

    return notifications;
}

var findAndParseMentionInNotification = async function (channel) {
    let lastReadMessageIndex = (channel.channelState.lastReadMessageIndex ?? 0);
    let lastMessageIndex = (channel.channelState.lastMessage.index ?? 0);

    if (lastMessageIndex === lastReadMessageIndex)
        return null;

    return await channel.getMessages(lastMessageIndex - lastReadMessageIndex).then(function (page) {
        let msgs = page.items;
        let allMentionedMessages = [];
        for (var i = 0; i < msgs.length; i++) {
            let msg = msgs[i].state;
            if (msg.type === "text") {
                let matches = _.flatten(findMentionInMessageBody(msg.body));
                if (matches.length > 0 && matches.filter(x => { if (x.toLowerCase() === userEmailAddress.toLowerCase()) return true; return false; }).length > 0) {
                    allMentionedMessages.push(msg);
                }
            }
        }
        allMentionedMessages = _.flatten(allMentionedMessages);

        return allMentionedMessages;
    });
}

var findMentionInMessageBody = function (msg) {
    let matches = [];

    msg.replace(/[^<]*(<a href="#!" data-email="([^"]+)" class="([^"]+)">([^<]+)<\/a>)/g, function () {
        matches.push(Array.prototype.slice.call(arguments, 2, 3));
    });
    return _.flatten(matches);
}

var preapreAndBindNotifications = function (notifications, isNotificationPage, prepand) {
    if (isEmptyOrBlank(prepand))
        prepand = false;
    if (notifications.length > 0)
        $notifictionsDropDown.removeClass("notification-indicator-primary").addClass("notification-indicator-success");
    else
        $notifictionsDropDown.removeClass("notification-indicator-success").addClass("notification-indicator-primary");
    if (prepand === false) {
        $notifictionsList.empty();
        $divNotificationsCardBody.empty();
    }

    if (isNotificationPage === false) {
        let counter = Notification_Bell_Size;
        for (var i = 0; i < notifications.length && i < counter; i++) {
            let userFullName = notifications[i].messageAuthor;
            let avatar = Default_Profile_Image;
            let notificationChannelParticipant = notificationChannelParticipants.get(notifications[i].messageAuthor.toLowerCase())
            if (!isEmptyOrBlank(notificationChannelParticipant)) {
                avatar = notificationChannelParticipant.ProfileImage;
                userFullName = notificationChannelParticipant.FirstName + (isEmptyOrBlank(notificationChannelParticipant.LastName) ? "" : " " + notificationChannelParticipant.LastName);
            }
            let template = `<div class="list-group-item">
                         <a class="notification notification-flush bg-200" data-msg-id="${notifications[i].messageId}" data-channel-id="${notifications[i].channelId}" data-channel-uniquename="${notifications[i].channelUniqueName}" href="/communication/chat?isRecon=true&reconChannelId=${notifications[i].channelId}&msgId=${notifications[i].messageId}">
                             <div class="notification-avatar">
                                 <div class="avatar avatar-2xl mr-3">
                                     <img class="rounded-circle" src="${avatar}" alt="" />

                                 </div>
                             </div>
                             <div class="notification-body">
                                 <p class="mb-1"><strong>${userFullName}</strong>&nbsp;Mentioned you in a <strong>${notifications[i].recAccNameWithDesc}</strong> : "${parseMessageHtmlAndResize(notifications[i].messageBody, 30)}"</p>
                                 <span class="notification-time"><span class="mr-1" role="img" aria-label="Emoji">💬</span>${moment(notifications[i].messageDate).format("MMM-DD-YYYY hh:mm a")}</span>
                             </div>
                         </a>
                    </div>`;
            if (prepand === false) {
                $notifictionsList.append(template);
            }
            else {
                if ($notifictionsList.find(".list-group-item:last").length >= 2)
                    $notifictionsList.find(".list-group-item:last").remove();
                $notifictionsList.prepend(template);
            }
            setReconciliationMentionIconColor(notifications);
        }
    }
    else {
        for (var i = 0; i < notifications.length; i++) {
            let userFullName = notifications[i].messageAuthor;
            let avatar = Default_Profile_Image;
            let notificationChannelParticipant = notificationChannelParticipants.get(notifications[i].messageAuthor.toLowerCase())
            if (!isEmptyOrBlank(notificationChannelParticipant)) {
                avatar = notificationChannelParticipant.ProfileImage;
                userFullName = notificationChannelParticipant.FirstName + (isEmptyOrBlank(notificationChannelParticipant.LastName) ? "" : " " + notificationChannelParticipant.LastName);
            }
            let template = `<a class="border-bottom-0 notification rounded-0 border-x-0 border-300" data-msg-id="${notifications[i].messageId}" data-channel-id="${notifications[i].channelId}" data-channel-uniquename="${notifications[i].channelUniqueName}" href="/communication/chat?isRecon=true&reconChannelId=${notifications[i].channelId}&msgId=${notifications[i].messageId}">
            <div class="notification-avatar">
                <div class="avatar avatar-xl mr-3">
                    <img class="rounded-circle" src="${avatar}" alt="">

                </div>
            </div>
            <div class="notification-body">
                <p class="mb-1"><strong>${userFullName}</strong>&nbsp;Mentioned you in a  <strong>${notifications[i].recAccNameWithDesc}</strong> : "${parseMessageHtmlAndResize(notifications[i].messageBody, 50)}"</p>
                <span class="notification-time"><span class="mr-1" role="img" aria-label="Emoji">💬</span>${moment(notifications[i].messageDate).format("MMM-DD-YYYY hh:mm a")}</span>
            </div>
        </a>`;
            if (prepand === false)
                $divNotificationsCardBody.append(template);
            else
                $divNotificationsCardBody.prepend(template);
        }
    }

    hideWaitMeLoader($notifictionsList);
    hideWaitMeLoader($divNotificationsCard);

}

var setUserOnlineOfflineStatus = function (state) {
    if (state === "connected")
        $notifictionsDropDown.removeClass("notification-indicator-primary").addClass("notification-indicator-success");
    else
        $notifictionsDropDown.removeClass("notification-indicator-success").addClass("notification-indicator-primary");
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
function parseMessageHtmlAndResize(message, resizeLength) {
    if (isEmptyOrBlank(resizeLength))
        resizeLength = 30;

    let tmp = $("<div>").attr("style", "display:none");
    let html_text = tmp.html(message).text();
    tmp.remove();
    if (html_text.length <= resizeLength)
        return html_text;
    else
        return html_text.slice(0, resizeLength) + "...";
}
//Global Chat with Notifications End