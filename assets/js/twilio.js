﻿var token;

var twilioClient;
var activeChannel;
var activeChannelPage;
var typingMembers = new Set();
var onlineOfflineMembers = new Object();
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

            twilioClient.getSubscribedConversations().then(updateChannels);

            //click First Paticipant
            setTimeout(function () { $participants.eq(0).click(); }, 1500);


            setTimeout(registerUserUpdatedEventHandlers, 200);

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


var createAllChannels = function () {
    let participantsToCreate = chat.participants.filter(x => isEmptyOrBlank(x.ChannelId));
    if (participantsToCreate.length == 0)
        return;
    participantsToCreate.forEach(function (participant) {
        let attributes = { "type": "private" }
        let channelName = getChannelUniqueName(chat.userEmail, participant.Email);
        setTimeout(function () {
            twilioClient.createConversation({
                friendlyName: channelName,
                isPrivate: true,
                uniqueName: channelName,
                attributes: attributes
            }).then(joinChannel).then(updateChannels);
        }, 1000);
    });
}

var getChannelBySidAndJoin = function (channelId) {
    let existingChannel = twilioClient.conversations.conversations.get(channelId);
    if (isEmptyOrBlank(existingChannel)) {
        twilioClient.getConversationBySid(channelId).then(function (conv) {
            setActiveChannel(conv);
        });
    } else {
        setActiveChannel(existingChannel);
    }
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
        }).then(joinChannel).catch(res => {
            location.reload();
        }).then(setActiveChannel);
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
        //activeChannel.removeListener('memberUpdated', updateMember);
    }

    activeChannel = channel;

    let participant = getParticipantByChannelIndex();

    addParticipant(participant.Email);

    $btnSendMessage.off('click');
    $btnSendMessage.on('click', function () {
        var body = $messageBodyInput.val();
        if (validateMessage()) {
            channel.sendMessage(body).then(function () {
                $messageBodyInput.val('').focus();
                $messageBodyInput.trigger('change');
                setScrollPosition();
                if ($newMessagesDiv && $newMessagesDiv.length > 0)
                    $newMessagesDiv.remove();
            });
        }
    });
    $channelMessages.empty();

    //TODO
    //if (channel.status !== 'joined') {
    //    $(s'#channel').addClass('view-only');
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

        var newestMessageIndex = page.items.length ? page.items[page.items.length - 1].index : 0;
        var lastIndex = channel.lastReadMessageIndex;
        if (!isEmpty(lastIndex) && lastIndex !== newestMessageIndex) {
            let $lastReadMessageDiv = $channelMessages.children(`div.media[data-index='${lastIndex}']`);
            if ($lastReadMessageDiv.length === 0)
                $lastReadMessageDiv = $channelMessages.children(`div.media:first`);
            $lastReadMessageDiv.before(`<div class="text-center fs--2 text-500 separator"><span>New Messages</span></div>`);
            $newMessagesDiv = $channelMessages.children('.separator');
            if ($newMessagesDiv)
                $newMessagesDiv[0].scrollIntoView();
        }
        else {
            setScrollPosition();
        }

        if (isElementInView($channelMessages.children('.media:last'), false)) {
            channel.updateLastReadMessageIndex(newestMessageIndex).then(updateChannels);
        }

        return channel.getParticipants()
    }).then(function (members) {
        updateParticipants();

        channel.on('participantJoined', updateParticipants);
        channel.on('participantLeft', updateParticipants);
        channel.on('participantUpdated', updateParticipants);
        //TODO
        //members.forEach(member => {
        //    member.getUser().then(user => {

        //        user.on('updated', () => {
        //            updateMember.bind(null, member, user);
        //            updateMembers();
        //        });
        //    });
        //});
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
                        addJoinedChannel(channel);
                        break;
                    case 'notParticipating':
                        joinChannel(channel);
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
            //createAllChannels();
        });
}

var updateActiveChannel = function () {

}

var addJoinedChannel = function (channel) {
    //var updateableParticipants = chat.participants.filter(obj => isEmptyOrBlank(obj.ChannelUniqueName))
    var updateableParticipants = chat.participants;
    for (var i = 0; i < updateableParticipants.length; i++) {
        if (channel?.uniqueName?.toLowerCase() == getChannelUniqueName(chat.userEmail, updateableParticipants[i].Email)) {
            updateableParticipants[i].ChannelId = channel.sid;
            updateableParticipants[i].ChannelUniqueName = channel.uniqueName;
            $participants.eq(i).attr("data-channelId", channel.sid);

            //var objTwilioConversations = { ConversationId: channel.sid, ConversationUniqueName: channel.uniqueName, IsPrivate: channel.channelState.attributes?.type == "private", Status: channel.channelState.state.current }
            //insertUpdateTwilioConversation(objTwilioConversations);
        }
    }
}

var addParticipant = function (identity) {
    activeChannel.getParticipantByIdentity(identity)
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
    let prepand = false;
    let rowTimeStamp = moment(time).format("YYYYMMDD");
    var firstRowTimeStamp = $channelMessages.children('.text-center:first')?.attr('id');
    var lastRowTimeStamp = $channelMessages.children('.text-center:last')?.attr('id');
    let row = $(`#${rowTimeStamp}`);
    if (row.length < 1) {
        let rowHTML = `<div id="${moment(time).format("YYYYMMDD")}" class="text-center fs--2 text-500 date-stamp"><span>${moment(time).format("MMM DD, YYYY")}</span></div>`;
        if (lastRowTimeStamp && lastRowTimeStamp < rowTimeStamp)
            prepand = false;
        else
            prepand = true;
        if (prepand)
            $channelMessages.prepend(rowHTML);
        else
            $channelMessages.append(rowHTML);
    }
    row = $(`#${rowTimeStamp}`);
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
    if (msg.author.toLowerCase() == chat.userEmail.toLowerCase()) {
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
var addMessagePrepand = function (message) {
    addMessage(message, true);
}
var addMessage = function (message) {
    //if (isEmptyOrBlank(prepand))
    //    prepand = false;

    let msg = message.state;
    var timestampRow = addTimestampRow(msg.timestamp);
    let timeStampRowId = timestampRow.attr("id");
    var messageRow = prepareMessageRow(message, timeStampRowId);
    let firstMsgDiv = $channelMessages.find(`[data-timestamp='${timeStampRowId}']:first`);
    let lastMsgDiv = $channelMessages.find(`[data-timestamp='${timeStampRowId}']:last`);
    if (lastMsgDiv.length > 0) {
        let firstMsgDivIndex = firstMsgDiv.attr('data-index');
        if (firstMsgDiv.attr('data-index') > message.index)
            firstMsgDiv.before(messageRow);
        else
            lastMsgDiv.after(messageRow);
    }
    else {
        timestampRow.after(messageRow);
    }

    if (lastMsgDiv && lastMsgDiv.length > 0 && isElementInView(lastMsgDiv))
        setScrollPosition();
}

var removeMessage = function (message) { }

var updateMessage = function (args) {

}
var createMessage = function (message, $el) {

}
//TODO//ReTest this message
var addChannelMessagesScrollEvent = function () {
    var isUpdatingConsumption = false;
    $channelMessages.on('scroll', function (e) {
        //if ($channelMessages.height() - 50 < $channelMessages.scrollTop() + $channelMessages.height()) {
        //    activeChannel.getMessages(Chat_Page_Size).then(messages => {
        //        var newestMessageIndex = messages.length ? messages[0].index : 0;
        //        if (!isUpdatingConsumption && activeChannel.lastConsumedMessageIndex !== newestMessageIndex) {
        //            isUpdatingConsumption = true;
        //            activeChannel.updateLastReadMessageIndex(newestMessageIndex).then(function () {
        //                isUpdatingConsumption = false;
        //            });
        //        }
        //    });
        //}
        let lastIndex = activeChannel.lastReadMessageIndex;
        lastIndex = 0;
        let newestMessageIndex = $channelMessages.children('.media:last').attr('data-index');

        if (!isEmpty(lastIndex) && lastIndex != newestMessageIndex) {
            let maxReadMessageIndex = lastIndex;
            let $messages;

            let lastIndexDiv = $channelMessages.children(`.media[data-index=${lastIndex}]`);

            if (lastIndexDiv == 0)
                $messages = $channelMessages.children('.media');
            else
                $messages = lastIndexDiv.nextAll('.media');

            $messages.each(function (i, obj) {
                let index = obj.attributes["data-index"].value;
                try {
                    if (index > lastIndex && isElementInView(obj)) {
                        maxReadMessageIndex = index;
                    }
                } catch (e) {

                }
            });

            if (lastIndex != maxReadMessageIndex) {
                activeChannel.updateLastReadMessageIndex(parseInt(maxReadMessageIndex));
            }
        }

        if ($channelMessages.scrollTop() < 50 && activeChannelPage && activeChannelPage.hasPrevPage) {
            var lowestMessageIndex = $channelMessages.children('.media:first').attr('data-index');
            activeChannelPage.prevPage().then(page => {
                page.items.reverse().forEach(addMessage);
                activeChannelPage = page;
                if (lowestMessageIndex) {
                    let scrollToDiv = $channelMessages.children(`.media[data-index=${lowestMessageIndex - 1}]`);
                    if (scrollToDiv)
                        scrollToDiv[0].scrollIntoView();
                }
            });
        }
    });
}

//region end

var updateParticipants = function () {
    let sortedParticipants = activeChannel.getParticipants()
        .then(function (members) {
            members.forEach(function (member) {
                member.getUser().then(user => {
                    onlineOfflineMembers[user.state.identity] = (isEmptyOrBlank(user.state.online) ? false : user.state.online);
                });
            });
        });
    setOnlineOfflineStatus();
}

var setOnlineOfflineStatus = function () {
    Object.keys(onlineOfflineMembers).forEach(key => {
        let participant = getParticipantByEmail(key);
        if (isEmptyOrBlank(participant) === false) {
            participant["Online"] = onlineOfflineMembers[key];
            var elParticipant = $participants.filter(function (index, obj) {
                return obj.attributes['data-index'].value == participant.Index;
            });
            if (participant.Online) {
                elParticipant.children('.avatar').removeClass('status-offline').addClass('status-online');
            }
            else {
                elParticipant.children('.avatar').removeClass('status-online').addClass('status-offline');
            }
        }
    });
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


var isElementInView = function (element, fullyInView) {
    var pageTop = $(window).scrollTop();
    var pageBottom = pageTop + $(window).height();
    var elementTop = $(element).offset()?.top;
    var elementBottom = elementTop + $(element).height();

    if (fullyInView === true) {
        return ((pageTop < elementTop) && (pageBottom > elementBottom));
    } else {
        return ((elementTop <= pageBottom) && (elementBottom >= pageTop));
    }
}

//EventHandler
var registerUserUpdatedEventHandlers = function registerEventHandlers() {
    Object.keys(onlineOfflineMembers).forEach(key => {
        //register Event
        twilioClient.getUser(key).then(user => {
            user.on('updated', event => handleUserUpdate(event.user, event.updateReasons));
        });

    });
}

var handleUserUpdate = function (user, updateReasons) {
    // loop over each reason and check for reachability change
    updateReasons.forEach(reason => {
        if (reason == 'reachabilityOnline') {
            debugger
            let participant = getParticipantByEmail(user.state.identity);
            if (!isEmptyOrBlank(participant)) {
                participant["Online"] = isEmptyOrBlank(user?.state?.online) ? false : user?.state?.online;
                let elParticipant = $participantsContainer.children(`[data-index=${participant.Index}]`)
                if (participant["Online"])
                    elParticipant.children('.avatar').removeClass('status-offline').addClass('status-online');
                else
                    elParticipant.children('.avatar').removeClass('status-online').addClass('status-offline');
            }
        }
    });
}

