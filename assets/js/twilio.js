var token;

var twilioClient;
var activeChannel;

var getToken = function () {
    postAjaxSync("/twilio/token?identity=" + chat.userEmail, null, function (response) {
        token = response;
    })
}

var createTwilioClient = function () {
    getToken();
    Twilio.Conversations.Client.create(token, { logLevel: 'info' })
        .then(function (createdClient) {

            twilioClient = createdClient;
            //twilioClient.getUser(chat.userEmail).then(function (user) { console.log(user) });

            //click First Paticipant
            $participants.eq(0).click();

            twilioClient.getSubscribedConversations().then(updateChannels);

            twilioClient.on('tokenAboutToExpire', () => {
                getToken();
                twilioClient.updateToken(token);
            });

            twilioClient.on('channelJoined', function (channel) {
                channel.on('messageAdded', updateUnreadMessages);
                channel.on('messageAdded', updateChannels);
                updateChannels();
            });

            //twilioClient.on('channelInvited', updateChannels);
            //twilioClient.on('channelAdded', updateChannels);
            //twilioClient.on('channelUpdated', updateChannels);
            //twilioClient.on('channelLeft', leaveChannel);
            //twilioClient.on('channelRemoved', leaveChannel);
        });
}

var createOrJoinExistingChannel = function (friendlyName, uniqueName, isPrivate, attributes) {
    //check If Channel Existss
    twilioClient.getConversationByUniqueName(uniqueName).then(function (conv) {
        joinChannel(conv);
        setActiveChannel(conv);
    }).catch(function (e) {
        twilioClient.createConversation({
            friendlyName: friendlyName,
            isPrivate: isPrivate,
            uniqueName: uniqueName,
            attributes: attributes
        }).then(joinChannel).then(setActiveChannel);
    })

}

var joinChannel = function (channel) {
    //channel.getParticipantByIdentity("test1@mailinator.com")
    //    .then(function (obj) {
    //        debugger
    //        console.log(obj)
    //    })
    //    .catch(function (e) {
    //        debugger
    //    })
    channel.join().catch(function (e) {
        console.log(e);
    });
}

var setActiveChannel = function (channel) {
    if (activeChannel) {
        //activeChannel.removeListener('messageAdded', addMessage);
        //activeChannel.removeListener('messageRemoved', removeMessage);
        //activeChannel.removeListener('messageUpdated', updateMessage);
        activeChannel.removeListener('updated', updateActiveChannel);
        activeChannel.removeListener('memberUpdated', updateMember);
    }

    activeChannel = channel;

    let participant = getParticipantByChannelIndex();
    addParticipant(activeChannel, participant.Email); //Replace this function;On page load create all conversation and participants

    $('#send-message').off('click');
    $('#send-message').on('click', function () {
        var body = $('#message-body-input').val();
        channel.sendMessage(body).then(function () {
            $('#message-body-input').val('').focus();
            $('#channel-messages').scrollTop($('#channel-messages ul').height());
            $('#channel-messages li.last-read').removeClass('last-read');
        });
    });

    activeChannel.on('updated', updateActiveChannel);
}

function updateChannels() {
    twilioClient.getSubscribedConversations()
        .then(page => {
            subscribedChannels = page.items.sort(function (a, b) {
                return a.friendlyName > b.friendlyName;
            });
            subscribedChannels.forEach(function (channel) {
                switch (channel.status) {
                    case 'joined':
                        addJoinedChannel(channel);
                        break;
                    //case 'invited':
                    //    addInvitedChannel(channel);
                    //    break;
                    //default:
                    //    addKnownChannel(channel);
                    //    break;
                }
            });
        });
}

var addJoinedChannel = function (channel) {
    var updateableParticipants = chat.participants.filter(obj => isEmptyOrBlank(obj.ChannelUniqueName))
    for (var i = 0; i < updateableParticipants.length; i++) {
        if (channel?.uniqueName?.toLowerCase() == getChannelUniqueName(chat.userEmail, updateableParticipants[i].Email)) {
            updateableParticipants[i].ChannelId = channel.sid;
            updateableParticipants[i].ChannelUniqueName = channel.uniqueName;
            $participants.eq(i).attr("data-channelId", channel.sid);

            //debugger
            //var objTwilioConversations = { ConversationId: channel.sid, ConversationUniqueName: channel.uniqueName, IsPrivate: channel.channelState.attributes?.type == "private", Status: channel.channelState.state.current }
            //insertUpdateTwilioConversation(objTwilioConversations);
        }
    }
}

var addParticipant = function (channel, identity) {
    debugger
    channel.getParticipantByIdentity(identity)
        .then(function (obj) {
            debugger
            console.log(obj)
        })
        .catch(function (e) {
            channel.add(identity).then(function (member) {
                debugger
                console.log(member);
            }).catch(function (err) {
                debugger
                console.log(err);
            })
        })
}

var updateUnreadMessages = function updateUnreadMessages(message) {
    var channel = message.channel;
    if (channel !== activeChannel) {
        let contact = $("#chatParticipants div[data-channelId*='" + channel.sid + "']");
        let unreadMessagesDiv = contact.find(".chat-contact-content");
        contact.addClass("unread-message");
        switch (switch_on) {
            case "text":
                unreadMessagesDiv.html(message?.state?.body);
            default:
        }
    }
}