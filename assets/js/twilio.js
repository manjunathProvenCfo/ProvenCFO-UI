var token;

var twilioClient;
var activeChannel;
var activeChannelPage;
var typingMembers = new Set();

const Chat_Page_Size = 30;

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

            twilioClient.on('channelInvited', updateChannels);
            twilioClient.on('channelAdded', updateChannels);
            twilioClient.on('channelUpdated', updateChannels);
            //twilioClient.on('channelLeft', leaveChannel);
            //twilioClient.on('channelRemoved', leaveChannel);
        });
}

var createOrJoinExistingChannel = function (friendlyName, uniqueName, isPrivate, attributes) {
    //check If Channel Existss
    twilioClient.getConversationByUniqueName(uniqueName).then(function (conv) {
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
    channel.join().catch(function (e) {
    });
}

var setActiveChannel = function (channel) {
    if (activeChannel) {
        activeChannel.removeListener('messageAdded', addMessage);
        activeChannel.removeListener('messageRemoved', removeMessage);
        activeChannel.removeListener('messageUpdated', updateMessage);
        //activeChannel.removeListener('updated', updateActiveChannel);
        activeChannel.removeListener('memberUpdated', updateMember);
    }

    activeChannel = channel;

    let participant = getParticipantByChannelIndex();

    addParticipant(activeChannel, participant.Email); //Replace this function;On page load create all conversation and participants

    $btnSendMessage.off('click');
    $btnSendMessage.on('click', function () {
        var body = $messageBodyInput.val();
        if (validateMessage()) {
            channel.sendMessage(body).then(function () {
                $messageBodyInput.val('').focus();
                $messageBodyInput.trigger('change');
                setScrollPosition();
                //TODO: $('#channel-messages li.last-read').removeClass('last-read');
            });
        }
    });

    //TODO
    //if (channel.status !== 'joined') {
    //    $('#channel').addClass('view-only');
    //    return;
    //} else {
    //    $('#channel').removeClass('view-only');
    //}

    channel.getMessages(Chat_Page_Size).then(function (page) {
        activeChannelPage = page;
        page.items.forEach(addMessage);

        channel.on('messageAdded', addMessage);
        channel.on('messageUpdated', updateMessage);
        channel.on('messageRemoved', removeMessage);

        //setScrollPosition();
        debugger
        var newestMessageIndex = page.items.length ? page.items[page.items.length - 1].index : 0;
        var lastIndex = channel.lastReadMessageIndex;
        if (!isEmpty(lastIndex) && lastIndex !== newestMessageIndex) {
            let $lastReadMessageDiv = $channelMessages.children(`div.media[data-index='${lastIndex}']`);
            $lastReadMessageDiv.before(`<div class="text-center fs--2 text-500 separator"><span>New Messages</span></div>`);
            $newMessagesDiv = $channelMessages.children('.separator');
            //var top = $newMessagesDiv.position() && $newMessagesDiv.position().top;
            //$channelMessages.scrollTop(top + $channelMessages.scrollTop());
            $newMessagesDiv[0].scrollIntoView(true);

        }
        else {
            setScrollPosition();
        }

        //if ($('#channel-messages ul').height() <= $('#channel-messages').height()) {
        //    channel.updateLastConsumedMessageIndex(newestMessageIndex).then(updateChannels);
        //}

        //return channel.getMembers();
    });
    channel.on('typingStarted', function (member) {
        member.getUser().then(user => {
            typingMembers.add(member.identity);
            updateTypingIndicator();
        });
    });

    channel.on('typingEnded', function (member) {
        member.getUser().then(user => {
            typingMembers.delete(member.identity);
            updateTypingIndicator();
        });
    });
}

var updateChannels = function updateChannels() {
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

var updateActiveChannel = function () {

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
    channel.getParticipantByIdentity(identity)
        .then(function (obj) {
        })
        .catch(function (e) {
            activeChannel.add(identity).then(function (member) {
            }).catch(function (err) {
            });
        });
}
//region start
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

var validateMessage = function () {
    let isValid = true;
    let body = $messageBodyInput.val();
    if (isEmptyOrBlank(body)) {
        //TODO//show error alert
        isValid = false;
    }
    return isValid;
}

var addTimestampRow = function (time) {
    let row = $(`#${moment(time).format("YYYYMMDD")}`);
    if (row.length < 1) {
        let rowHTML = `<div id="${moment(time).format("YYYYMMDD")}" class="text-center fs--2 text-500"><span>${moment(time).format("MMM DD, YYYY")}</span></div>`;
        $channelMessages.prepend(rowHTML);
    }
    row = $(`#${moment(time).format("YYYYMMDD")}`);
    return row;
}

var prepareMessageRow = function (message, timeStampRowId) {
    let msg = message.state;
    let timestamp = msg.timestamp;
    let time = moment(timestamp, "HH:mm:ss").format("hh:mm A");
    let msgHTML = "";
    let row = "";

    switch (msg.type) {
        case "text":
            msgHTML = msg.body;
            break;
        default:
    }
    if (msg.author.toLowerCase() == chat.userEmail) {
        row = `<div class='media p-3' id="${msg.sid}" data-index="${message.index}" data-sid="${message.sid}" data-timestamp="${timeStampRowId}">
                                <div class='media-body d-flex justify-content-end'>
                                    <div class='w-100 w-xxl-75'>
                                        <div class='hover-actions-trigger d-flex align-items-center justify-content-end'>
                                            <div class='bg-primary text-white p-2 rounded-soft chat-message'>
                                                ${msgHTML}
                                            </div>
                                        </div>
                                        <div class='text-400 fs--2 text-right'>
                                            ${time}<span class='fas fa-check ml-2 text-success'></span>
                                        </div>
                                    </div>
                                </div>
                            </div>`;
    } else {
        let participant = getParticipantByEmail(msg.author);
        row = `<div class='media p-3' id="${msg.sid}" data-index="${message.index}" data-sid="${message.sid}" data-timestamp="${timeStampRowId}">
        <div class='avatar avatar-l mr-2'>
            <img class='rounded-circle' src='`+ (isEmptyOrBlank(participant?.ProfileImage) == true ? Default_Profile_Image : participant?.ProfileImage) + `' alt='' />
        </div>
        <div class='media-body'>
            <div class='w-xxl-75'>
                <div class='hover-actions-trigger d-flex align-items-center'>
                    <div class='chat-message bg-200 p-2 rounded-soft'>
                    ${msgHTML}
                    </div>
                </div>
                <div class='text-400 fs--2'>
                    <span>${time}</span>
                </div>
            </div>
        </div>
    </div>`;
    }
    return row;
    //<ul class="hover-actions position-relative list-inline mb-0 text-400 ml-2">
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Forward"><span class="fas fa-share"></span></a></li>
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Archive"><span class="fas fa-archive"></span></a></li>
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Edit"><span class="fas fa-edit"></span></a></li>
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Remove"><span class="fas fa-trash-alt"></span></a></li>
    //</ul>
}

var addMessage = function (message) {
    let msg = message.state;
    var timestampRow = addTimestampRow(msg.timestamp);
    let timeStampRowId = timestampRow.attr("id");
    var messageRow = prepareMessageRow(message, timeStampRowId)
    let msgDiv = $channelMessages.find(`[data-timestamp='${timeStampRowId}']:last`);
    if (msgDiv.length > 0)
        $channelMessages.find(`[data-timestamp='${timeStampRowId}']:last`).after(messageRow)
    else {

    }
        timestampRow.after(messageRow);
}

var removeMessage = function (message) { }

var updateMessage = function (args) {

}
var createMessage = function (message, $el) {

}
//region end

var updateMember = function myfunction() {

}

function updateTypingIndicator() {
    var message = 'Typing: ';
    var names = Array.from(typingMembers).slice(0, 3);

    if (typingMembers.size) {
        message += names.join(', ');
    }

    if (typingMembers.size > 3) {
        message += ', and ' + (typingMembers.size - 3) + 'more';
    }

    if (typingMembers.size) {
        message += '...';
    } else {
        message = '';
    }
    //$typingIndicator.removeClass("d-none");
    //$typingIndicatorMessage.text(message);
    $('#typing-indicator span').text(message);
}