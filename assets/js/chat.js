﻿var $participantsContainer;
var $participantFirst;
var $participants;
var $channelName;
var $channelParticipantEmail;
var $messageBodyInput;
var $messageBodyFileUploader;
var $messageBodyFilePreviewerModal;
var $btnSendMessage;
var $channelMessages;
var $typingIndicator;
var $typingIndicatorMessage;
var $newMessagesDiv;

var Default_Profile_Image = "/assets/img/team/default-logo.png";

var chat = {
    userId: "",
    userEmail: "test1@mailinator.com",
    twilioUserId: "",
    channels: [],
    participants: [],
    channelIndex: -1,
    publicChannelUniqueNameGuid: ""
};

var loadChatPage = function (isPublicChatOnly, type) {
    if (isEmptyOrBlank(isPublicChatOnly))
        isPublicChatOnly = false;
    if (isEmptyOrBlank(type))
        type = 1;
    $participantsContainer = $("#chatParticipants");
    $participants = "";
    $channelName = $(".channelName");
    $channelParticipantEmail = $(".channelParticipantEmail");
    $messageBodyInput = $("#message-body-input");
    $messageBodyFileUploader = $("#chat-file-upload");
    $messageBodyFilePreviewerModal = $("#chat-file-previewer-modal");
    $btnSendMessage = $("#send-message");
    $channelMessages = $("#channel-messages");

    $channelMessages.empty();
    addChannelMessagesScrollEvent();
    if (isPublicChatOnly === true) {
        hideParticipantsSidebar();
        getPublicChatParticipants(chat.publicChannelUniqueNameGuid);
    }
    else {
        getChatParticipants();
        createTwilioClient();
    }

    if (chat.channels.length > 0) {
        $participants = $("#chatParticipants div[id*='chat-link']");
        $participants.on('click', handleParticipantClick);
        $participantFirst = $("#chatParticipants .chat-contact:first");
    }

    $messageBodyInput.emojioneArea({
        "placeholder": "Type your message", "emojiPlaceholder": ":smile_cat:", "search": true, "tones": false, "filtersPosition": "bottom", "recentEmojis": false, events: {
            keydown: function (editor, event) {
                if (event.keyCode === 13) {
                    $btnSendMessage.click();
                } else if (activeChannel) {
                    activeChannel.typing();
                }
            },
            keyup: function (editor, event) {
                let val = "";
                if (editor && editor.length > 0)
                    val = editor[0].innerText;

                if (isEmptyOrBlank(val))
                    $btnSendMessage.removeClass('text-primary');
                else
                    $btnSendMessage.addClass('text-primary');
            }
        }
    });

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
                addMediaMessage(file);
            }
        })
    });

}
var resetChatPage = function () {
    chat.twilioUserId = "";
    chat.channels = [];
    chat.participants = [];
    chat.channelIndex = -1;

    typingMembers = new Set();
    onlineOfflineMembers = new Object();
}
var hideParticipantsSidebar = function () { $(".chat-sidebar").hide(); }
var getPublicChatParticipants = function (channelUniqueNameGuid) {
    resetChatPage();
    getAjaxSync(`/Communication/getPublicChat?userId=${chat.userId}&userEmail=${chat.userEmail}&type=1&channelUniqueNameGuid=${channelUniqueNameGuid}`, null, function (response) {
        debugger
        setParticipants(response);
        createTwilioClient();
        /*$participants.eq(0).click();*/
    });
}
var getChatParticipants = function () {
    getAjaxSync(`/Communication/ChatParticipants?UserId=${chat.userId}&userEmail=${chat.userEmail}`, null, function (response) {
        if (response.length > 0) {
            chat.channels = response;
            let arrParticipants = Array.prototype.concat.apply([], chat.channels.map(x => x.ChatParticipants));
            chat.participants = [];
            arrParticipants.forEach(x => {
                if (chat.participants.findIndex(i => i.Email === x.Email) == -1) {
                    chat.participants.push(x);
                }
            });
            for (var i = 0; i < chat.channels.length; i++) {
                if (chat.channels[i].IsPrivate === true) {
                    chat.channels[i].ChannelImage = (isEmptyOrBlank(chat.channels[i].ChatParticipants[0].ProfileImage) === true ? Default_Profile_Image : chat.channels[i].ChatParticipants[0].ProfileImage);
                }
                else {
                    chat.channels[i].ChannelImage = Default_Profile_Image;
                }
            }

            setOnlineOfflineMembersArray();
            renderParticipants();
        }
        else {
            ShowAlertBoxWarning("No participant exists for chat");
        }
    });
}
var setParticipants = function (response) {
    if (response.length > 0) {
        chat.channels = response;
        let arrParticipants = Array.prototype.concat.apply([], chat.channels.map(x => x.ChatParticipants));
        chat.participants = [];
        arrParticipants.forEach(x => {
            if (chat.participants.findIndex(i => i.Email === x.Email) == -1) {
                chat.participants.push(x);
            }
        });
        for (var i = 0; i < chat.channels.length; i++) {
            if (chat.channels[i].IsPrivate === true) {
                chat.channels[i].ChannelImage = (isEmptyOrBlank(chat.channels[i].ChatParticipants[0].ProfileImage) === true ? Default_Profile_Image : chat.channels[i].ChatParticipants[0].ProfileImage);
            }
            else {
                chat.channels[i].ChannelImage = Default_Profile_Image;
            }
        }

        setOnlineOfflineMembersArray();
        renderParticipants();
    }
    else {
        ShowAlertBoxWarning("No participant exists for chat");
    }
}
var setOnlineOfflineMembersArray = function () {

    chat.participants.forEach(x => {
        if (isEmpty(onlineOfflineMembers[x.Email.toLowerCase()])) {
            onlineOfflineMembers[x.Email.toLowerCase()] = x.Online;
        }
    });
}
var renderParticipants = function () {
    let obj = chat.channels;
    var participants = '';
    for (var i = 0; i < obj.length; i++) {
        chat.channels[i]["Index"] = i;
        participants = participants + ` <div class="media chat-contact hover-actions-trigger w-100" id="chat-link-` + i + `" data-email="` + (obj[i].IsPrivate === false ? '' : obj[i].ChatParticipants[0].Email.toLowerCase()) + `" data-index="` + i + `" data-channelId="` + obj[i].ChannelId + `" data-toggle="tab" data-target="#chat" role="tab">
                        <div class="avatar avatar-xl status-offline">
                            <img class="rounded-circle" src="`+ (isEmptyOrBlank(obj[i].ChannelImage) == true ? Default_Profile_Image : obj[i].ChannelImage) + `" alt="" />

                        </div>
                        <div class="media-body chat-contact-body ml-2 d-md-none d-lg-block">
                            <div class="d-flex justify-content-between">
                                <h6 class="mb-0 chat-contact-title">`+ obj[i].ChannelName + `</h6><!--<span class="message-time fs--2">Tue</span>-->
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
        throw 'Channel Index not found.'
        return;
    }

    if (chat.channelIndex != index) {
        chat.channelIndex = index;

        let channel = getChannelByChannelIndex();
        let participant = getChannelParticipnatByChannelIndex();
        $channelName.text(channel.ChannelName);
        $channelMessages.empty();
        if (channel.IsPrivate === true) {
            $channelParticipantEmail.text(participant.Email);
        }
        else {
            $channelParticipantEmail.html("&nbsp");
        }
        //Twilio
        if (isEmptyOrBlank(channel.ChannelId)) {
            if (channel.IsPrivate === true) {
                var attributes = { "type": "private" }
                var channelName = getChannelUniqueName(chat.userEmail, participant.Email);
            } else if (channel.Type == 1) {
                var attributes = { "type": "public reconciliation" }
                var channelName = channel.ChannelUniqueName;
            }
            createOrJoinExistingChannel(channelName, channelName, channel.IsPrivate, attributes);
        }
        else {
            getChannelBySidAndJoin(channel.ChannelId);
        }
    }
}

//Database Queries Start
var updateTwilioUserId = function (userId, twiliouserId) {
    postAjaxSync("/Twilio/UpdateTwilioUserId?userId=" + userId + "&twiliouserId=" + twiliouserId, null, function (response) {

    })
}

var insertUpdateTwilioConversation = function (objTwilioConversations) {
    postAjax("/Twilio/InsertUpdateTwilioConversation", JSON.stringify(objTwilioConversations), function (response) {
    });
}


var getChannelByChannelIndex = function () {
    return chat.channels[chat.channelIndex];
}
var getChannelByChannelId = function (channelId) {
    return chat.channels(x => x.ChannelId == channelId);
}
var getChannelParticipnatByChannelIndex = function () {
    return getChannelByChannelIndex().ChatParticipants[0];
}
var getParticipantByEmail = function (email) {
    let participant = chat.participants.filter(x => x.Email.toLowerCase() == email.toLowerCase());
    if (participant.length > 0)
        return participant[0];
    else
        null;
}
var getParticipantNameByEmail = function (email) {
    let participant = getParticipantByEmail(email);
    return participant?.FirstName;
}
//var getParticipantByChannelId = function (channelId) {
//    return chat.channels(x => x.ChannelId == channelId);
//}

var getChannelUniqueName = function (userEmail, participantEmail) {
    userEmail = userEmail.toLowerCase();
    participantEmail = participantEmail.toLowerCase();
    if (userEmail < participantEmail)
        return userEmail + "_" + participantEmail;
    else
        return participantEmail + "_" + userEmail;
}

var addTypingIndicatorDiv = function () {
    let typingDiv = `<div class='media px-3 d-none' id="typing-indicator"><div class="media-body"> <div class="w-xxl-75"> <div class="hover-actions-trigger d-flex align-items-center"> <div class="chat-message typing-message bg-200 p-2 rounded-soft"> typing </div> </div> </div> </div></div>`;
    $channelMessages.append(typingDiv);
    $typingIndicator = $("#typing-indicator");
    $typingIndicatorMessage = $("#typing-indicator .typing-message");
}

var setScrollPosition = function () {
    $channelMessages.scrollTop($channelMessages[0].scrollHeight);
}

var Uploader = function (file) {
    this.file = file;
};

Uploader.prototype.getType = function () {
    return this.file.type;
};
Uploader.prototype.getSizeInMB = function () {
    return parseFloat(parseFloat(this.file.size / 1024) / 1024).toFixed(2);
};
Uploader.prototype.getName = function () {
    return this.file.name;
};
