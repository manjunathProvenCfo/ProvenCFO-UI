var $participantsContainer;
var $participantFirst;
var $participants;
var $channelName;

var chat = {
    userId: "",
    userEmail: "test1@mailinator.com",
    twilioUserId: "",
    participants: [],
    channelIndex: -1
};

var loadPage = function () {
    $participantsContainer = $("#chatParticipants");
    $participants = "";
    $channelName = $(".channelName");

    getChatParticipants();
    createTwilioClient();
    if (chat.participants.length > 0) {
        $participants = $("#chatParticipants div[id*='chat-link']");
        $participants.on('click', handleParticipantClick);
        $participantFirst = $("#chatParticipants .chat-contact:first");
    }
}

var getChatParticipants = function () {
    getAjaxSync("/Communication/ChatParticipants?UserId=" + chat.userId, null, function (response) {
        chat.participants = response;

        renderParticipants();
    });
}
var renderParticipants = function () {
    let obj = chat.participants;
    var participants = '';
    for (var i = 0; i < obj.length; i++) {
        participants = participants + ` <div class="media chat-contact hover-actions-trigger w-100" id="chat-link-` + i + `" data-index="` + i + `" data-channelId="` + obj[i].ChannelId + `" data-toggle="tab" data-target="#chat" role="tab">
                        <div class="avatar avatar-xl status-online">
                            <img class="rounded-circle" src="`+ (isEmptyOrBlank(obj[i].ProfileImage) == true ? "https://randomuser.me/api/portraits/men/" + i + ".jpg" : "https://randomuser.me/api/portraits/men/" + i + ".jpg") + `" alt="" />

                        </div>
                        <div class="media-body chat-contact-body ml-2 d-md-none d-lg-block">
                            <div class="d-flex justify-content-between">
                                <h6 class="mb-0 chat-contact-title">`+ obj[i].FirstName + ' ' + obj[i].LastName + `</h6><span class="message-time fs--2">Tue</span>
                            </div>
                            <div class="min-w-0">
                                <div class="chat-contact-content pr-3">
                                    <!--Antony
                                    sent
                                    6 photos-->
                                </div>
                                <div class="dropdown dropdown-active-trigger position-absolute b-0 r-0 hover-actions">
                                    <button class="btn btn-link btn-sm text-400 dropdown-toggle dropdown-caret-none p-0 fs-0" type="button" data-toggle="dropdown" data-boundary="viewport" aria-haspopup="true" aria-expanded="false"><span class="fas fa-cog" data-fa-transform="shrink-3 down-4"></span></button>
                                    <div class="dropdown-menu dropdown-menu-right border py-2 rounded-soft">
                                        <a class="dropdown-item" href="#!">Mute</a>
                                        <div class="dropdown-divider"></div><a class="dropdown-item" href="#!">Archive</a><a class="dropdown-item" href="#!">Delete</a>
                                        <div class="dropdown-divider"></div><a class="dropdown-item" href="#!">Mark as Unread</a><a class="dropdown-item" href="#!">Something's Wrong</a><a class="dropdown-item" href="#!">Ignore Messsages</a><a class="dropdown-item" href="#!">Block Messages</a>
                                    </div>
                                </div>
                                <div class="position-absolute b-0 r-0 hover-hide">
                                </div>
                            </div>
                        </div>
                    </div>`;
    }
    $participantsContainer.empty();
    $participantsContainer.html(participants);

}

var handleParticipantClick = function (event) {
    let index = event.currentTarget.dataset.index;

    if (isEmpty(index)) {
        throw 'Participant Index not found.'
        return;
    }

    if (chat.channelIndex != index) {
        chat.channelIndex = index;
    }
    let participant = getParticipantByChannelIndex();
    $channelName.text(participant.FirstName + " " + participant.LastName)

    //Twilio
    if (isEmptyOrBlank(participant.ChannelId)) {
        let attributes = { "type": "private" }
        let channelName = getChannelUniqueName(chat.userEmail, participant.Email);
        createOrJoinExistingChannel(channelName, channelName, true, attributes);
    }
}
//Database Queries Start
var updateTwilioUserId = function (userId, twiliouserId) {
    postAjaxSync("/Twilio/UpdateTwilioUserId?userId=" + userId + "&twiliouserId=" + twiliouserId, null, function (response) {
        debugger
    })
}

var insertUpdateTwilioConversation = function (objTwilioConversations) {
    debugger
    postAjax("/Twilio/InsertUpdateTwilioConversation", JSON.stringify(objTwilioConversations), function (response) {
        debugger
    })
}



var getParticipantByChannelIndex = function () {
    return chat.participants[chat.channelIndex];
}
var getParticipantByChannelId = function (channelId) {
    return chat.participants(x => x.ChannelId == channelId);
}

var getChannelUniqueName = function (userEmail, participantEmail) {
    userEmail = userEmail.toLowerCase();
    participantEmail = participantEmail.toLowerCase();
    if (userEmail < participantEmail)
        return userEmail + "_" + participantEmail;
    else
        return participantEmail + "_" + userEmail;
}