﻿var token;

var twilioClient;
var activeChannel;
var activeChannelPage;
var activeChannelMessages;
var allSubChannels = [];
var typingMembers = new Set();
var onlineOfflineMembers = new Object();
var subscribedChannelsLastMessage = [];
const Chat_Page_Size = 10;
const Chat_Find_Mention_Page_Size = 10;


var createTwilioClient = async function () {
    if (!isEmptyOrBlank(twilioClient)) {
        await connectionStateConnectedMethods();
        return;
    }
    getTwilioToken(chat.userEmail);
    //logLevel: 'info'
    Twilio.Conversations.Client.create(token, { logLevel: 'error'})//logLevel: 'error'
        .then(async function (createdClient) {

            twilioClient = createdClient;
            //twilioClient.getSubscribedConversations().then(updateChannels);


            twilioClient.on("connectionStateChanged", async function (state) {
                //await isTwilioClientConnected();
                ////Sort Sidebar channels, set reconsiliation icon color
                //updateChannels(chat.forReconciliationIconColor);
                //selectSidebarParticipant();

                ////addMessgeAdded Event Listener 
                //setMessageAddedListenerOnAllChannels();
                await connectionStateConnectedMethods();
            });

            twilioClient.on("channelJoined", function (channel) {
                channel.on("messageAdded", updateUnreadMessages);
                channel.on("messageAdded", updateChannels);
            });
            //click First Paticipant
            //setTimeout(function () { $participants.eq(0).click(); }, 3000);


            setTimeout(registerUserUpdatedEventHandlers, 200);

            twilioClient.on('tokenAboutToExpire', () => {
                getTwilioToken(chat.userEmail);
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
var connectionStateConnectedMethods = async function () {
    await isTwilioClientConnected();
    //Sort Sidebar channels, set reconsiliation icon color
    updateChannels(chat.forReconciliationIconColor);
    selectSidebarParticipant();

    //addMessgeAdded Event Listener 
    //setMessageAddedListenerOnAllChannels();
}

var createAllChannels = function () {
    let participantsToCreate = chat.channels.filter(x => isEmptyOrBlank(x.ChannelId));
    if (participantsToCreate.length == 0)
        return;
    participantsToCreate.forEach(function (participant) {
        let attributes = { "type": "private" }
        let channelName = getChannelUniqueName(chat.userEmail, participant.Email, chat.clientId);
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

var getChannelBySidAndJoin = async function (channelId) {
    let existingChannel = twilioClient.conversations.conversations.get(channelId);
    if (isEmptyOrBlank(existingChannel)) {
        twilioClient.getConversationBySid(channelId).then(function (conv) {

            setActiveChannel(conv);
        });
    } else {

        setActiveChannel(existingChannel);
    }
    isConversationSyncListFetched();
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

var addMediaMessage = function (file) {
    const formData = new FormData();
    formData.append('file', file);
    
    if (activeChannel?.channelState?.attributes?.type === "public reconciliation") {
        UpdateReconciliationHasStatus(activeChannel.channelState.uniqueName);
    }
    activeChannel.sendMessage(formData).then(function (msg) {
        setScrollPosition();
        if ($newMessagesDiv && $newMessagesDiv.length > 0)
            $newMessagesDiv.remove();
        let lastMessage = $channelMessages.children('.media:last')
        if (lastMessage && lastMessage.length > 0)
            activeChannel.updateLastReadMessageIndex(parseInt(lastMessage.attr('data-index'))).then(function () {

            });
    });
}

var setActiveChannel = function (channel) {
    if (activeChannel) {
        activeChannel.removeListener('messageAdded', addMessage);
        activeChannel.removeListener('messageRemoved', removeMessage);
        activeChannel.removeListener('messageUpdated', updateMessage);
        activeChannel.off('messageAdded', addMessage);
        activeChannel.off('messageRemoved', removeMessage);
        activeChannel.off('messageUpdated', updateMessage);
        //activeChannel.removeListener('updated', updateActiveChannel);
        //activeChannel.removeListener('memberUpdated', updateMember);
    }

    activeChannel = channel;

    let participant = getChannelParticipnatByChannelIndex();//TODO Public Chat

    addParticipant(participant.Email);

    $btnSendMessage.off('click');
    $btnSendMessage.on('click', function () {
        let body = $chatEditorArea[0].emojioneArea.getText();
        $messageBodyInput.val('').focus();
        $messageBodyInput.trigger('change');
        if (validateMessage(body)) {
            if (channel.channelState.attributes.type === "public reconciliation") {
                UpdateReconciliationHasStatus(channel.channelState.uniqueName);
            }
            channel.sendMessage(body).then(function () {
                //getTwilioToken();
                //twilioClient.updateToken(token);

                $messageBodyInput.val('').focus();
                $messageBodyInput.trigger('change');
                setScrollPosition();
                if ($newMessagesDiv && $newMessagesDiv.length > 0)
                    $newMessagesDiv.remove();
                let lastMessage = $channelMessages.children('.media:last')
                if (lastMessage && lastMessage.length > 0)
                    channel.updateLastReadMessageIndex(parseInt(lastMessage.attr('data-index')));
            });
        }
    });
    $channelMessages.empty();

    $messageBodyInput.val('').focus();
    $messageBodyInput.trigger('change');

    //TODO
    //if (channel.status !== 'joined') {
    //    $(s'#channel').addClass('view-only');
    //    return;
    //} else {
    //    $('#channel').removeClass('view-only');
    //}

    channel.getMessages(Chat_Page_Size).then(function (page) {
        hideChatContentLoader();
        activeChannelPage = page;
        activeChannelMessages = page.items;
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
            if ($newMessagesDiv && $newMessagesDiv.length > 0)
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
            if (member.identity.toLowerCase() != chat.userEmail.toLowerCase()) {
                typingMembers.add(member.identity);
                updateTypingIndicator();
            }
        });
    });

    channel.on('typingEnded', function (member) {
        member.getUser().then(user => {
            typingMembers.delete(member.identity);
            updateTypingIndicator();
        });
    });
}
var updateChannels = function (forReconciliationIconColor) {
    allSubscribedChannels = [];
    twilioClient.getSubscribedConversations({ limit: 100 }).then(page => {
        getAllSubscribedChannels(page).then(async allSubscribedChannels => {
            allSubscribedChannels = allSubscribedChannels.sort(function (a, b) {
                return a.friendlyName > b.friendlyName;
            });
            await isConversationSyncListFetched();
            joinAndSortSubscribedChannels(allSubscribedChannels, forReconciliationIconColor);

        });
    });
}

var getAllSubscribedChannels = function (page) {
    if (page?.hasOwnProperty("items") === true) {
        allSubChannels = _.union(allSubChannels, page.items);
    }

    return new Promise(function (resolve, reject) {
        if (page.hasNextPage === true) {
            page.nextPage().then(y => {
                resolve(getAllSubscribedChannels(y));
            });
        }
        else { resolve(allSubChannels) };
    });

}
var joinAndSortSubscribedChannels = async function (subscribedChannels, forReconciliationIconColor) {
    subscribedChannelsLastMessage = [];
    var subscribedChannelsByType = subscribedChannels.filter(function (channel) {
        let typeOfChannel = "private";
        if (chat.type === 0)
            typeOfChannel = "private";
        else
            typeOfChannel = "public reconciliation";
        if (channel.channelState.attributes.hasOwnProperty("type") && channel.channelState.attributes.type === typeOfChannel) {

            //channel.messagesEntity.messagesListPromise.then(function (e) {
            //    //console.log(e)
            //});
            subscribedChannelsLastMessage.push({
                channelId: channel.sid,
                channelUniqueName: channel.channelState.uniqueName,
                lastMessage: (channel.channelState.hasOwnProperty("lastMessage") === false ? null : channel.channelState.lastMessage),
                lastReadMessageIndex: channel.channelState.lastReadMessageIndex,
                isUnread: (channel.channelState.hasOwnProperty("lastMessage") === true && channel.channelState.lastMessage.index >= 0 && channel.channelState.lastMessage.index !== channel.channelState.lastReadMessageIndex ? true : false)
            });
            return true;
        }
        return false;
    });
    subscribedChannelsByType.forEach(function (channel) {
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

    //sort particiapnts start
    //let subscribedChannelsWithoutLastMessage = _.where(sorted, { lastMessage: null });
    let subscribedChannelsWithLastMessage = _.filter(subscribedChannelsLastMessage, x => x.lastMessage != null);
    let sorted = _.sortBy(subscribedChannelsWithLastMessage, function (o) { return o.lastMessage?.dateCreated; })
    sorted = sorted.reverse();
    
    let sortedParticipants = [];
    if (isEmptyOrBlank($participants))
        return;
    let remainingParticipants = $participants.toArray();
    sorted.forEach(function (sortedChannel) {
        var elParticipant = $participants.filter(function (i, obj) {
            if ((obj.dataset?.channelid ?? "") === sortedChannel.channelId) {
                remainingParticipants = removeSortedParticipantFromRemaningByChannelId(remainingParticipants, sortedChannel.channelId)
                return true;
            }
        });

        if (elParticipant.length > 0) {
            //add unread msg count
            let spanUnreadCount = elParticipant.find("#spanUnreadMsgCount");
            let lastMessageIndex = (sortedChannel.lastMessage?.index ?? -1) + 1;
            let lastMessageReadIndex = (sortedChannel.lastReadMessageIndex ?? -1) + 1;
            let unreadMsgCount = lastMessageIndex - lastMessageReadIndex;
            if (unreadMsgCount > 0) {
                spanUnreadCount.text(unreadMsgCount);
                spanUnreadCount.show();
            }
            else {
                spanUnreadCount.text("");
                spanUnreadCount.hide();
            }
            sortedParticipants.push(elParticipant);
        }
    });
    $participantsContainer.empty();
    $participantsContainer.html(sortedParticipants);
    remainingParticipants.forEach(function (rp) {
        if ($(`#chatParticipants div[id*='chat-link'][data-index='${rp.dataset.index}']`).length === 0) {
            $participantsContainer.append(rp);
        }
    });

    if (chat.channels.length > 0) {
        $participants = $("#chatParticipants div[id*='chat-link']");
        $participants.off('click');
        $participants.on('click', handleParticipantClick);
        $participantFirst = $("#chatParticipants .chat-contact:first");
    }

    //Sidebar Participant Selection Start
    if (chat.selectedRecentParticipantOnce === false) {
        chat.selectedRecentParticipantOnce = true;
        selectSidebarParticipant();
    }
    //Sidebar Participant Selection End
}

var setReconciliationIconColor = async function (channelFindMentionFirst, commentChannelUniqueName) {
    channelFindMentionFirst.getMessages(Chat_Find_Mention_Page_Size).then(function (page, mentionChannelUniqueName = commentChannelUniqueName) {
        let msgs = page.items;
        let allMatches = [];
        for (var i = 0; i < msgs.length; i++) {
            let msg = msgs[i].state;
            if (msg.type === "text") {
                let matches = findMentionInMessageBody(msg.body);
                if (matches.length > 0) {
                    allMatches.concat(matches);
                }
            }
        }
        allMatches = _.flatten(allMatches);
        if (allMatches.length > 0) {
            if (allMatches.filter(x => {
                if (x.toLowerCase() === chat.userEmail.toLowerCase()) return true; return false;
            }).length > 0) {
                let elComment = $(`#btnComment[data-id='${mentionChannelUniqueName}']`);
                elComment.children().remove();
                elComment.append(`<span class="fas fa-comment fs--1"></span>`);
                var dtReconciliationPageNumber = $dtReconciliation.page();
                $dtReconciliation.rows().invalidate().draw();
                $dtReconciliation.page(dtReconciliationPageNumber).draw(false);
            }
        }
    });
}

var setReconciliationMentionIconColor = async function (notifications) {
    if (!isLocationUrlContains("reconciliation"))
        return;
    let colorChannelsDictKeys = _.map(notifications, "channelUniqueName");
    var $dtReconciliation = $('#tblreconcilation').DataTable();
    $dtReconciliation.column($dtReconciliation.columns().header().length - 1).nodes().to$().each(async function (index, obj) {
        let elComment = $(obj).children("#btnComment");
        let commentChannelUniqueName = elComment.data().id;
        if (colorChannelsDictKeys.indexOf(commentChannelUniqueName) > -1) {
            elComment.children().remove();
            elComment.append(`<span class="fas fa-comment fs--1"></span>`);
        }
    });
    let dtReconciliationPageNumber = $dtReconciliation.page();
    $dtReconciliation.rows().invalidate().draw();
    $dtReconciliation.page(dtReconciliationPageNumber).draw(false);
}

var updateActiveChannel = function () {

}

var addJoinedChannel = function (channel) {
    //var updateableParticipants = chat.channels.filter(obj => isEmptyOrBlank(obj.ChannelUniqueName))
    var updateableParticipants = chat.channels;
    var privateUpdateableParticipants = updateableParticipants.filter(x => x.IsPrivate === true && isEmptyOrBlank(x.ChannelId));
    var publicUpdateableParticipants = updateableParticipants.filter(x => x.IsPrivate === false);
    if (channel?.channelState?.attributes?.type === "private") {
        for (var i = 0; i < privateUpdateableParticipants.length; i++) {
            if (channel?.uniqueName?.toLowerCase() == getChannelUniqueName(chat.userEmail, privateUpdateableParticipants[i].ChatParticipants[0].Email, chat.clientId)) {
                privateUpdateableParticipants[i].ChannelId = channel.sid;
                privateUpdateableParticipants[i].ChannelUniqueName = channel.uniqueName;
                $participants.eq(i).attr("data-channelId", channel.sid);

                //var objTwilioConversations = { ConversationId: channel.sid, ConversationUniqueName: channel.uniqueName, IsPrivate: channel.channelState.attributes?.type == "private", Status: channel.channelState.state.current }
                //insertUpdateTwilioConversation(objTwilioConversations);
            }
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

var validateMessage = function (body) {
    let isValid = true;

    body = body.replaceAll("\n", "");
    if (isEmptyOrBlank(body)) {
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

var prepareMessageRow = function (message, timeStampRowId, participantName) {
    let msg = message.state;
    let timestamp = msg.timestamp;
    let time = moment(timestamp, "HH:mm:ss").format("hh:mm A");
    let msgHTML = "";
    let row = "";

    switch (msg.type) {
        case "text":
            if (!isEmptyOrBlank(msg.body))
                msgHTML = msg.body.replaceAll("\n", "</br>");
            break;
        default:
    }
    if (msg.type.toLowerCase() == "text") {
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
                    <span class='font-weight-semi-bold mr-2'>${participantName}</span>
                    <span>${time}</span>
                </div>
            </div>
        </div>
    </div>`;
        }
    }
    else if (msg.type.toLowerCase() == "media") {
        if (msg.author.toLowerCase() == chat.userEmail.toLowerCase()) {
            row = `<div class='media p-3' id="${msg.sid}" data-index="${message.index}" data-sid="${message.sid}" data-timestamp="${timeStampRowId}" data-media-sid="${message.media.sid}">
                                <div class="media-body d-flex justify-content-end">
                                    <div class="w-100 w-xxl-75">
                                        <div class="hover-actions-trigger d-flex align-items-center justify-content-end">
                                            <div class="chat-message chat-gallery justify-content-end">
                                                <div class="row mx-n1 justify-content-end">
                                                    
                                                </div>
                                            </div>
                                        </div>
                                        <div class='text-400 fs--2 text-right'>
                                            ${time}<span class='fas fa-check ml-2 text-success'></span>
                                        </div>
                                    </div>
                                </div>
                            </div>`;
        }
        else {
            row = `<div class='media p-3' id="${msg.sid}" data-index="${message.index}" data-sid="${message.sid}" data-timestamp="${timeStampRowId}" data-media-sid="${message.media.sid}">
                                <div class="media-body">
                                    <div class="w-xxl-75">
                                        <div class="hover-actions-trigger d-flex align-items-center">
                                            <div class="chat-message chat-gallery">
                                                <div class="row mx-n1 justify-content-start">
                                                </div>
                                            </div>
                                        </div>
                                     <div class='text-400 fs--2'>
                                         <span>${time}</span>
                                     </div>
                                    </div>
                                </div>
                            </div>`;
        }
    }

    return row;
    //<ul class="hover-actions position-relative list-inline mb-0 text-400 ml-2">
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Forward"><span class="fas fa-share"></span></a></li>
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Archive"><span class="fas fa-archive"></span></a></li>
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Edit"><span class="fas fa-edit"></span></a></li>
    //<li class="list-inline-item"><a class="chat-option" href="#!" data-toggle="tooltip" data-placement="top" title="Remove"><span class="fas fa-trash-alt"></span></a></li>
    //</ul>
}

var prepareImageMessageBody = function (url, filename) {
    let messageBody = `<div class="col-6 col-md-4 px-1" style="min-width: 50px;"><a href="${url}" class="data-fancybox" data-fancybox="twilio-gallery" data-caption="${filename}"><img src="${url}" alt="" class="img-fluid rounded mb-2" onload="setScrollPosition();"></a></div>`;
    return messageBody;
}

var prepareDocMessageBody = function (url, filename) {
    if (isEmptyOrBlank(filename))
        filename = "Download Link";
    let messageBody = `<a href="${url}" target="_blank">${filename}</a>`;
    return messageBody;
}

var addMessagePrepand = function (message) {
    addMessage(message, true);
}
var addMessage = function (message) {
    //if (isEmptyOrBlank(prepand))
    //    prepand = false;
    let msg = message.state;
    if (addMessageProcessed.indexOf(msg.sid) > -1)
        return;
    addMessageProcessed.push(msg.sid);

    var timestampRow = addTimestampRow(msg.timestamp);
    let timeStampRowId = timestampRow.attr("id");

    let participantName = getParticipantNameByEmail(msg.author.toLowerCase());
    var messageRow = prepareMessageRow(message, timeStampRowId, participantName);
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
    //
    if (msg.type == "media") {
        let checkMediaMessage = activeChannelMessages.filter(x => x?.media?.sid == message.media.sid);
        if (checkMediaMessage.length == 0) {
            activeChannelMessages.push(message);
        }
        msg.media.getContentTemporaryUrl().then(function (mediaURL, msg) {
            // log media temporary URL
            if (!isEmptyOrBlank(mediaURL)) {
                let url = new URL(mediaURL);
                let urlPathnames = url.pathname.split('/')
                if (urlPathnames.length > 0) {
                    let mediaId = urlPathnames[urlPathnames.length - 1];
                    var elMsg = $channelMessages.children(`.media[data-media-sid=${mediaId}]`);
                    elMsg.find(".chat-message").addClass("chat-gallery");
                    let mediaMessage = activeChannelMessages.filter(x => x?.media?.sid == mediaId)
                    if (mediaMessage.length > 0) {
                        mediaMessage = mediaMessage[0]
                        if (mediaMessage) {
                            if (mediaMessage.media.state.contentType.indexOf("image") != -1) {

                                elMsg.find(".row").append(prepareImageMessageBody(mediaURL, mediaMessage.media.state.filename));
                            }
                            else {
                                elMsg.find(".chat-message").addClass("bg-primary text-white p-2 rounded-soft");
                                //elMsg.find(".row").remove();
                                //elMsg.html(prepareDocMessageBody(mediaURL, mediaMessage.media.state.filename));

                                elMsg.find(".row").append(prepareDocMessageBody(mediaURL, mediaMessage.media.state.filename));
                            }
                        }
                    }
                }
            }
        });
    }
    if (lastMsgDiv && lastMsgDiv.length > 0 && isElementInView(lastMsgDiv))
        setScrollPosition();

    if (isEmptyOrBlank(getParameterByName('msgId')) === false) {
        let elMediaMessage = $(`#${getParameterByName('msgId')}`);
        if (elMediaMessage.length > 0) {
            location.href = `#${getParameterByName('msgId')}`;
        }
    }
}

var removeMessage = function (message) { }

var updateMessage = function (args) {

}
var createMessage = function (message, $el) {

}
//TODO//ReTest this message
var addChannelMessagesScrollEvent = function () {
    var isUpdatingConsumption = false;
    $channelMessages.off('scroll');
    $channelMessages.on('scroll', function (e) {
        //if ($channelMessages.height() - 50 < $channelMessages.scrollTop() + $channelMessages.height()) {
        //    activeChannel.getMessages(Chat_Page_Size).then(messages => {
        //        
        //        messages.items.map(x => activeChannelMessages.push(x));
        //        var newestMessageIndex = messages.length ? messages[0].index : 0;
        //        if (!isUpdatingConsumption && activeChannel.lastConsumedMessageIndex !== newestMessageIndex) {
        //            isUpdatingConsumption = true;
        //            activeChannel.updateLastReadMessageIndex(newestMessageIndex).then(function () {
        //                isUpdatingConsumption = false;
        //            });
        //        }
        //    });
        //}
        if (activeChannel === null || activeChannel === undefined) return;
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
                    //console.log(e);
                }
            });

            if (lastIndex != maxReadMessageIndex) {
                activeChannel.updateLastReadMessageIndex(parseInt(maxReadMessageIndex)).then(function () {

                });
            }
        }

        if ($channelMessages.scrollTop() < 50 && activeChannelPage && activeChannelPage.hasPrevPage) {
            var lowestMessageIndex = $channelMessages.children('.media:first').attr('data-index');
            activeChannelPage.prevPage().then(page => {
                page.items.forEach(x => activeChannelMessages.push(x));
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
                    onlineOfflineMembers[user.state.identity.toLowerCase()] = (isEmptyOrBlank(user.state.online) ? false : user.state.online);
                });
            });
        });
    setOnlineOfflineStatus();
}

var setOnlineOfflineStatus = function () {
    Object.keys(onlineOfflineMembers).forEach(key => {
        key = key.toLowerCase()
        let participant = getParticipantByEmail(key);
        if (isEmptyOrBlank(participant) === false) {
            participant["Online"] = onlineOfflineMembers[key];
            var elParticipant = $participants.filter(function (index, obj) {
                return obj.attributes['data-email'].value.toLowerCase() === participant.Email.toLowerCase();
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
        }).catch(function (err) {
            //console.log(err);
        });

    });
}

var handleUserUpdate = function (user, updateReasons) {
    // loop over each reason and check for reachability change
    updateReasons.forEach(reason => {
        if (reason == 'reachabilityOnline') {
            let participant = getParticipantByEmail(user.state.identity);
            if (!isEmptyOrBlank(participant)) {
                participant["Online"] = isEmptyOrBlank(user?.state?.online) ? false : user?.state?.online;
                let elParticipant = $participantsContainer.children(`[data-email='${participant.Email.toLowerCase()}']`)
                if (participant["Online"])
                    elParticipant.children('.avatar').removeClass('status-offline').addClass('status-online');
                else
                    elParticipant.children('.avatar').removeClass('status-online').addClass('status-offline');
            }
        }
    });
}

var setMessageAddedListenerOnAllChannels = async function () {
    await isConversationSyncListFetched();

    twilioClient.conversations.conversations.forEach(function (value) {
        value.removeListener('messageAdded', messageAddedAllChannels);
        value.off('messageAdded', messageAddedAllChannels);
        value.on('messageAdded', messageAddedAllChannels);
    });
}
var messageAddedAllChannels = function (msg) {
    if (location.href.toLowerCase().indexOf("communication/chat") > -1) {
        if (msg.conversation.sid !== activeChannel.sid) {
            let elParticipant = $(`#chatParticipants div[data-channelId*='${msg.conversation.sid}']`);
            let spanUnreadCount = elParticipant.find("#spanUnreadMsgCount");
            let unreadCount = (isNaN(parseInt(spanUnreadCount.text())) ? 0 : parseInt(spanUnreadCount.text()));
            spanUnreadCount.text(unreadCount + 1);
            spanUnreadCount.show();
            _audio.play();
        }
    }
}
//EventHandler




