var $participantsContainer;
var $participantFirst;
var $participants;
var $channelName;
var $channelParticipantEmail;
var $channelReconciliationDescription;
var $channelReconciliationDescriptionSidebar;
var $channelReconciliationCompany;
var $channelReconciliationDate;
var $channelReconciliationAmount;
var $chatSiderbarFilterButtons;
var $messageBodyInput;
var $chatEditorArea;
var $messageBodyFileUploader;
var $messageBodyFilePreviewerModal;
var $btnSendMessage;
var $channelMessages;
var $typingIndicator;
var $typingIndicatorMessage;
var $newMessagesDiv;
var addMessageProcessed = [];
const monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
var chat = {
    userId: "",
    userEmail: "test1@mailinator.com",
    twilioUserId: "",
    channels: [],
    participants: [],
    channelIndex: -1,
    publicChannelUniqueNameGuid: "",
    clientId: 0,
    type: 0,
    forReconciliationIconColor: false,
    selectedRecentParticipantOnce: false,
    isReconciliationIconColorChanged: false
};
var CommentHtmls = {
    ReconciliationHtml:`<div class="media chat-contact hover-actions-trigger w-100" id="{id}" data-email="" data-index="1" data-channelid="{id}" data-toggle="tab" data-target="#chat" role="tab" onclick="loadCommentsPage('{channelUniqueNameGuid}')">
                        <div class="avatar avatar-xl status-offline">
                            <img class="rounded-circle" src="/assets/img/team/default-logo.png" alt="">
                        </div>
                        <div class="media-body chat-contact-body ml-2 d-md-none d-lg-block">
                            <div class="d-flex justify-content-between">
                                <h6 class="mb-0 chat-contact-title" title="{account}">{account}</h6><span class="badge badge-primary fs--2" style="display:none" id="spanUnreadMsgCount">0</span>
                            </div>
                            <div class="min-w-0">
                                <div class="chat-contact-content pr-3">
                                    <span class="channelReconciliationDescriptionSidebar">{agencyName}/{description}</span>
                                </div>
                                <div class="position-absolute b-0 r-0 hover-hide">
                                </div>
                            </div>
                        </div>
                    </div>`,
    datehtml: '<div id="{id}" class="text-center fs--2 text-500 date-stamp"><span>{innerText}</span></div>',
    commenthtml: `<div class="media p-3" data-timestamp="{date}"><div class= "media-body d-flex justify-content-end">
                  <div class="w-100 w-xxl-75"><div class="hover-actions-trigger d-flex align-items-center justify-content-end">
                    <div class="bg-primary text-white p-2 rounded-soft chat-message">{innerText}</div></div>
                    <div class="text-400 fs--2 text-right">{time}<span class="ml-2 text-success" data-fa-i2svg=""><svg class="svg-inline--fa fa-check fa-w-16" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="check" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" data-fa-i2svg=""><path fill="currentColor" d="M173.898 439.404l-166.4-166.4c-9.997-9.997-9.997-26.206 0-36.204l36.203-36.204c9.997-9.998 26.207-9.998 36.204 0L192 312.69 432.095 72.596c9.997-9.997 26.207-9.997 36.204 0l36.203 36.204c9.997 9.997 9.997 26.206 0 36.204l-294.4 294.401c-9.998 9.997-26.207 9.997-36.204-.001z"></path></svg></span>
                </div></div></div></div>`,
    otherscCommentshtml: `<div class="media p-3" data-timestamp="{date}"><div class="avatar avatar-l mr-2">
            <img class="rounded-circle" src="{profileimgurl}" alt=""></div><div class="media-body"><div class="w-xxl-75">
                <div class="hover-actions-trigger d-flex align-items-center"><div class="chat-message bg-200 p-2 rounded-soft">
                    {text}
                    </div></div><div class="text-400 fs--2"><span class="font-weight-semi-bold mr-2">{userName}</span>
                    <span>{time}</span></div></div></div></div>`
}

var loadChatPage = async function (isPublicChatOnly, type, autoSelectParticipant) {
    showChatContentLoader();
    if (isEmptyOrBlank(isPublicChatOnly))
        isPublicChatOnly = false;
    if (isEmptyOrBlank(type))
        chat.type = 0;
    else
        chat.type = type;
    if (isEmptyOrBlank(autoSelectParticipant))
        chat.autoSelectParticipant = false;
    else
        chat.autoSelectParticipant = autoSelectParticipant;

    $participantsContainer = $("#chatParticipants");
    $participants = "";
    $channelName = $(".channelName");
    $channelParticipantEmail = $(".channelParticipantEmail");
    $channelReconciliationDescription = $(".channelReconciliationDescription");
    $channelReconciliationDescriptionSidebar = $(".channelReconciliationDescriptionSidebar");
    $channelReconciliationCompany = $(".channelReconciliationCompany");
    $channelReconciliationDate = $(".channelReconciliationDate");
    $channelReconciliationAmount = $(".channelReconciliationAmount");
    $messageBodyInput = $("#message-body-input");
    $chatEditorArea = $(".chat-editor-area .emojiarea");
    $messageBodyFileUploader = $("#chat-file-upload");
    $messageBodyFilePreviewerModal = $("#chat-file-previewer-modal");
    $btnSendMessage = $("#send-message");
    $channelMessages = $("#channel-messages");
    $chatSiderbarFilterButtons = $("#divChatSiderbarFilters > button");

    $channelMessages.empty();
    $messageBodyInput.val('').focus();
    $messageBodyInput.trigger('change');

    addChannelMessagesScrollEvent();
    $('.tab-pane .chat-content-header .col-auto').remove();

    chat.clientId = getClientId();
    if (isPublicChatOnly === true) {
        hideParticipantsSidebar();
        getPublicChatParticipants(chat.publicChannelUniqueNameGuid);
    }
    else {
        chat.autoSelectParticipant = true;
        getChatParticipants();
        createTwilioClient();
    }

    if (chat.channels.length > 0) {
        $participants = $("#chatParticipants div[id*='chat-link']");
        $participants.off('click');
        $participants.on('click', handleParticipantClick);
        $participantFirst = $("#chatParticipants .chat-contact:first");
    }
    $chatEditorArea[0].emojioneArea.off("keydown");
    $chatEditorArea[0].emojioneArea.on("keydown", function ($editor, event) {
        if (event.keyCode === 13 && !event.shiftKey) {
            event.preventDefault();
            if (event.type == "keydown") {
                if ($('.mentions-autocomplete-list:visible li.active').length > 0) {
                    $('.mentions-autocomplete-list:visible li.active').trigger('mousedown');
                }
                else {
                    if (chat.type == 0) {
                        $btnSendMessage[0].click();
                    }
                    else {
                        if ($editor[0].innerHTML != '')
                            addNewMessagetoChatwindow($editor[0].innerHTML);
                    }                    
                }                   
            }
            else
                activeChannel?.typing();
        }
        else
            activeChannel?.typing();
    });
    setTimeout(addMentionPlugin, 3000)

    $messageBodyFileUploader.off("change");
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

    //Notification Reconciliation chat selection
    if (isEmptyOrBlank(getParameterByName("isRecon")) === false && getParameterByName("isRecon") === "true") {
        $("#divChatSiderbarFilters > button[data-type=1]").click();
    }

}
var loadCommentsPage = async function (channelUniqueNameGuid) {

    showChatContentLoader();
    $participantsContainer = $("#chatParticipants");
    $participants = "";
    $channelName = $(".channelName");
    $channelParticipantEmail = $(".channelParticipantEmail");
    $channelReconciliationDescription = $(".channelReconciliationDescription");
    $channelReconciliationDescriptionSidebar = $(".channelReconciliationDescriptionSidebar");
    $channelReconciliationCompany = $(".channelReconciliationCompany");
    $channelReconciliationDate = $(".channelReconciliationDate");
    $channelReconciliationAmount = $(".channelReconciliationAmount");
    $messageBodyInput = $("#message-body-input");
    $chatEditorArea = $(".chat-editor-area .emojiarea");
    $messageBodyFileUploader = $("#chat-file-upload");
    $messageBodyFilePreviewerModal = $("#chat-file-previewer-modal");
    $btnSendMessage = $("#send-message");
    $channelMessages = $("#channel-messages");
    $chatSiderbarFilterButtons = $("#divChatSiderbarFilters > button");
    chat.channelUniqueNameGuid = channelUniqueNameGuid;

    $(document).on("click", "button[id=btnComment]", function (e) {        
        showReconciliationChat(e.currentTarget.dataset.id);       
    });


    getAjaxSync(apiurl + `Reconciliation/getcommentsOnreconcliationId?reconcliationId=${channelUniqueNameGuid}`, null, function (response) {
        setCommentsHeader(response.resultData.reconciliationdata);
        LoadAllComments(response.resultData.reconciliationComments);
        //setParticipants(response);
        //createTwilioClient();
        /*$participants.eq(0).click();*/
        setScrollPosition();
        hideChatContentLoader();
    });

    $btnSendMessage.unbind().click(function () {
        addNewMessagetoChatwindow($('#message-body-input').val());
    });
    var addNewMessagetoChatwindow = async function (input) {
        addNewComment(input);
        $('#message-body-input').empty();
        $('.emojionearea-editor').empty();
    }
    var addNewComment = function (inputText) {
        var CurrentDate = new Date();
        var CurrentDateString = CurrentDate.getFullYear() + '' + ('0' + (CurrentDate.getMonth() + 1)).slice(-2) + '' + ('0' + CurrentDate.getDate()).slice(-2);
        var CurrentDateStringForDisplay = monthNames[CurrentDate.getMonth()] + ' ' + ('0' + CurrentDate.getDate()).slice(-2) + ', ' + CurrentDate.getFullYear();
        var CurrentTimestring = getCurrentTime(new Date);
        var DateElement = $('#channel-messages #' + CurrentDateString);
        if (DateElement == null || DateElement == undefined || DateElement.length == 0) {
            var dhtml = CommentHtmls.datehtml.replace('{id}', CurrentDateString).replace('{innerText}', CurrentDateStringForDisplay);
            $channelMessages.append(dhtml);
        }
        var chtml = CommentHtmls.commenthtml.replace('{date}', CurrentDateString).replace('{innerText}', inputText).replace('{time}', CurrentTimestring);
        $channelMessages.append(chtml);
        SaveNewcommenttoDB(inputText, chat.channelUniqueNameGuid);
        setScrollPosition();
        $("button[data-id*='" + chat.channelUniqueNameGuid + "'] svg").removeClass('text-dark');
    }
    var SaveNewcommenttoDB = function (InputcommentText, ReconciliationId) {
        var currentdate = new Date();
        var datetime = getCurrentTime(currentdate); //new Date(currentdate.getFullYear(), (currentdate.getMonth() + 1), currentdate.getDate(), currentdate.getHours(), currentdate.getMinutes(), currentdate.getSeconds() );

        var input = {
            Id: 0,
            ReconciliationId_ref: ReconciliationId,
            CommentText: InputcommentText,
            CreatedBy: chat.userId,
            CreatedDate: currentdate,
            IsDeleted: false,
            AgencyId: chat.AgencyId
        }
        postAjaxSync(apiurl + `Reconciliation/InsertReconcilationComments`, JSON.stringify(input), function (response) {
            var r = response;
        });
    }
    $chatEditorArea[0].emojioneArea.off("keydown");
    $chatEditorArea[0].emojioneArea.on("keydown", function ($editor, event) {
        if (event.keyCode === 13 && !event.shiftKey) {
            event.preventDefault();
            if (event.type == "keydown") {
                if ($('.mentions-autocomplete-list:visible li.active').length > 0) {
                    $('.mentions-autocomplete-list:visible li.active').trigger('mousedown');
                }
                else {
                    if ($editor[0].innerHTML != '')
                        addNewMessagetoChatwindow($editor[0].innerHTML);
                }

            }
            else
                activeChannel?.typing();
        }
        else
            activeChannel?.typing();
    });
    setTimeout(addMentionPlugin, 3000)
}

var loadreconcilationcomments = function () {
    showChatContentLoader();
    $participantsContainer = $("#chatParticipants");
    $participants = "";
    $channelName = $(".channelName");
    $channelParticipantEmail = $(".channelParticipantEmail");
    $channelReconciliationDescription = $(".channelReconciliationDescription");
    $channelReconciliationDescriptionSidebar = $(".channelReconciliationDescriptionSidebar");
    $channelReconciliationCompany = $(".channelReconciliationCompany");
    $channelReconciliationDate = $(".channelReconciliationDate");
    $channelReconciliationAmount = $(".channelReconciliationAmount");
    $messageBodyInput = $("#message-body-input");
    $chatEditorArea = $(".chat-editor-area .emojiarea");
    $messageBodyFileUploader = $("#chat-file-upload");
    $messageBodyFilePreviewerModal = $("#chat-file-previewer-modal");
    $btnSendMessage = $("#send-message");
    $channelMessages = $("#channel-messages");
    $chatSiderbarFilterButtons = $("#divChatSiderbarFilters > button");

    $channelMessages.empty();
    $messageBodyInput.val('').focus();
    $messageBodyInput.trigger('change');
    getAjaxSync(apiurl + `Reconciliation/GetAllCommentedReconciliations?AgencyID=${chat.clientId}&MaxCount=${0}`, null, function (response) {
        var Reconciliationdata = response;
        if (Reconciliationdata.resultData && Reconciliationdata.resultData.length > 0) {
            $.each(Reconciliationdata.resultData, function (index, aReconciliation) {
                var recHtml = CommentHtmls.ReconciliationHtml.replaceAll(/{account}/g, aReconciliation.account_name).replace('{agencyName}', aReconciliation.company).replace('{description}', aReconciliation.description).replaceAll(/{id}/g, aReconciliation.id).replaceAll(/{channelUniqueNameGuid}/g,aReconciliation.id);
                $participantsContainer.append(recHtml);
            });
        } 
        $participantsContainer.children(0)[0].click();
        loadCommentsPage($participantsContainer.children(0)[0].id);
        hideChatContentLoader();
        /*$participants.eq(0).click();*/
    });

}
var setCommentsHeader = function (reconciliationdata) {
    $channelReconciliationDescription.html("");
    $channelReconciliationCompany.html("");
    $channelReconciliationDate.html("");
    $channelReconciliationAmount.html("");
    $channelReconciliationDescription.html(`${reconciliationdata.company}/${reconciliationdata.description}`);
    $channelReconciliationDate.html(`${formatDateMMDDYYYY(reconciliationdata.date)}`);
    $channelReconciliationAmount.html(formatAmount(reconciliationdata.amount, true));
    $channelName.text(reconciliationdata.account_name);
}
var LoadAllComments = function (ReconciliationComments) {
    $channelMessages.empty();
    if (ReconciliationComments != null && ReconciliationComments.length > 0) {

        // this gives an object with dates as keys
        const dategroups = ReconciliationComments.reduce((groups, game) => {
            const date = game.createdDate.split('T')[0];
            if (!groups[date]) {
                groups[date] = [];
            }
            groups[date].push(game);
            return groups;
        }, {});

        // Edit: to add it in the array format instead
        const commentsgroupArrays = Object.keys(dategroups).map((date) => {
            return {
                date,
                comments: dategroups[date]
            };
        });
        $.each(commentsgroupArrays, function (index, aDates) {
            var dtarray = aDates.date.split('-');
            var datestring = monthNames[parseInt(dtarray[1]) - 1] + ' ' + dtarray[2] + ', ' + dtarray[0];
            var dhtml = CommentHtmls.datehtml.replace('{id}', aDates.date.replace('-', '')).replace('{innerText}', datestring);
            $channelMessages.append(dhtml);
            $.each(aDates.comments, function (index, acomments) {

                var UTCdate = getUTCDateTime(new Date(acomments.createdDateUTC));
                var time = getCurrentTime(new Date(UTCdate));
                var profileimgurl = acomments.commentedUserProfileImageurl;
                var commentText = acomments.commentText;
                var userName = acomments.commentedUserName;
                if (acomments && acomments.createdBy == chat.userId) {
                    var commentshtml = CommentHtmls.commenthtml.replace('{date}', aDates.date.replace('-', '')).replace('{innerText}', commentText).replace('{time}', time);
                    $channelMessages.append(commentshtml);
                }
                else {
                    var Otherscommentshtml = CommentHtmls.otherscCommentshtml.replace('{profileimgurl}', profileimgurl).replace('{text}', commentText).replace('{userName}', userName).replace('{date}', aDates.date.replace('-', '')).replace('{time}', time);
                    $channelMessages.append(Otherscommentshtml);
                }
            });

        });
        var obj = 0;
    }

}
var resetChatPage = function () {
    $participantsContainer.empty();
    chat.twilioUserId = "";
    chat.channels = [];
    chat.participants = [];
    chat.channelIndex = -1;
    selectedRecentParticipantOnce = false;

    typingMembers = new Set();
    onlineOfflineMembers = new Object();
}
function getCurrentTime(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}
function getUTCDateTime(date) {

    var dt = date.getDate();
    var month = date.getMonth() + 1;
    var year = date.getFullYear();
    var seconds = date.getSeconds()
    var minutes = date.getMinutes();
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = month + '/' + dt + '/' + year + ' ' + hours + ':' + minutes + ' ' + ampm + ' UTC';
    return strTime;
}
var hideParticipantsSidebar = function () { $(".chat-sidebar").hide(); }
var getPublicChatParticipants = function (channelUniqueNameGuid) {
    resetChatPage();
    getAjaxSync(`/Communication/getPublicChat?userId=${chat.userId}&userEmail=${chat.userEmail}&type=1&channelUniqueNameGuid=${channelUniqueNameGuid}&clientId=${chat.clientId}`, null, function (response) {
        setParticipants(response);
        createTwilioClient();
        /*$participants.eq(0).click();*/
    });
}
var getChatParticipants = function () {
    let participantsURL = `/Communication/ChatParticipants?UserId=${chat.userId}&userEmail=${chat.userEmail}&clientId=${chat.clientId}`;
    if (chat.type === 1) {
        participantsURL = `/Communication/getPublicChat?userId=${chat.userId}&userEmail=${chat.userEmail}&type=1&channelUniqueNameGuid=&clientId=${chat.clientId}&onlyHasChatChannels=true`;
    }
    getAjaxSync(participantsURL, null, function (response) {
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
            if (chat.type === 0)
                ShowAlertBoxWarning("No person exists for chat");
            else if (chat.type === 1)
                ShowAlertBoxWarning("No reconciliation exists for chat");
        }
    });
}
var setParticipants = function (response, type) {
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
                                <h6 class="mb-0 chat-contact-title" title="${obj[i].ChannelName}">` + obj[i].ChannelName + `</h6><span class="badge badge-primary fs--2" style="display:none" id="spanUnreadMsgCount">0</span>
                            </div>
                            <div class="min-w-0">
                                <div class="chat-contact-content pr-3">
                                    <span class='channelReconciliationDescriptionSidebar'>${(obj[i].IsPrivate === true ? '' : obj[i].Company + '/' + obj[i].ReconciliationDescription)}</span>
                                </div>
                                <!--<div class="dropdown dropdown-active-trigger position-absolute b-0 r-0 hover-actions">
                                    <button class="btn btn-link btn-sm text-400 dropdown-toggle dropdown-caret-none p-0 fs-0" type="button" data-toggle="dropdown" data-boundary="viewport" aria-haspopup="true" aria-expanded="false"><span class="fas fa-cog" data-fa-transform="shrink-3 down-4"></span></button>
                                    <div class="dropdown-menu dropdown-menu-right border py-2 rounded-soft">
                                        <a class="dropdown-item" href="#!">Mute</a>
                                        <div class="dropdown-divider"></div><a class="dropdown-item" href="#!">Archive</a><a class="dropdown-item" href="#!">Delete</a>
                                        <div class="dropdown-divider"></div><a class="dropdown-item" href="#!">Mark as Unread</a><a class="dropdown-item" href="#!">Something's Wrong</a><a class="dropdown-item" href="#!">Ignore Messsages</a><a class="dropdown-item" href="#!">Block Messages</a>
                                    </div>
                                </div>-->
                                <div class="position-absolute b-0 r-0 hover-hide">
                                </div>
                            </div>
                        </div>
                    </div>`;
    }
    $participantsContainer.empty();
    $participantsContainer.html(participants);

}

var handleParticipantClick = async function (event) {
    
    let index = event.currentTarget.dataset.index;
    if (isEmpty(index)) {
        throw 'Channel Index not found.'
        return;
    }

    if (chat.channelIndex != index) {
        chat.channelIndex = index;

        showChatContentLoader();

        let channel = getChannelByChannelIndex();
        let participant = getChannelParticipnatByChannelIndex();
        $channelName.text(channel.ChannelName);
        $channelMessages.empty();

        addMessageProcessed = [];
        $messageBodyInput.val('').focus();
        $messageBodyInput.trigger('change');

        if (channel.IsPrivate === true) {
            $channelParticipantEmail.text(participant.Email);
            $channelReconciliationDescription.html("");
            $channelReconciliationCompany.html("");
            $channelReconciliationDate.html("");
            $channelReconciliationAmount.html("");
        }
        else {
            $channelParticipantEmail.html("");
            $channelReconciliationDescription.html(`${channel.Company}/${channel.ReconciliationDescription}`);
            $channelReconciliationDate.html(`${formatDateMMDDYYYY(channel.ReconciliationDate)}`);
            $channelReconciliationAmount.html(formatAmount(channel.ReconciliationAmount, true));
        }
        //Twilio
        if (isEmptyOrBlank(channel.ChannelId)) {
            if (channel.IsPrivate === true) {
                var attributes = { "type": "private" }
                var channelName = getChannelUniqueName(chat.userEmail, participant.Email, chat.clientId);
            } else if (channel.Type == 1) {
                var attributes = { "type": "public reconciliation" }
                var channelName = channel.ChannelUniqueName;
            }
            createOrJoinExistingChannel(channelName, channelName, channel.IsPrivate, attributes);
        }
        else {
            await getChannelBySidAndJoin(channel.ChannelId);
        }
    }
    $chatEditorArea[0].emojioneArea.setFocus()
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

var getChannelUniqueName = function (userEmail, participantEmail, clientId) {
    userEmail = userEmail.toLowerCase();
    participantEmail = participantEmail.toLowerCase();
    if (userEmail < participantEmail)
        return userEmail + "_" + participantEmail + "_" + clientId;
    else
        return participantEmail + "_" + userEmail + "_" + clientId;
}

var addTypingIndicatorDiv = function () {
    let typingDiv = `<div class='media px-3 d-none' id="typing-indicator"><div class="media-body"> <div class="w-xxl-75"> <div class="hover-actions-trigger d-flex align-items-center"> <div class="chat-message typing-message bg-200 p-2 rounded-soft"> typing </div> </div> </div> </div></div>`;
    $channelMessages.append(typingDiv);
    $typingIndicator = $("#typing-indicator");
    $typingIndicatorMessage = $("#typing-indicator .typing-message");
}

var setScrollPosition = function () {
    let scrollPositionSet = false;
    if (isEmptyOrBlank(getParameterByName('msgId')) === false) {
        let elMediaMessage = $(`#${getParameterByName('msgId')}`);
        if (elMediaMessage.length > 0) {
            scrollPositionSet = true;
            location.href = `#${getParameterByName('msgId')}`;
        }
    }
    if (scrollPositionSet === false) {
        let elSeparator = $('div.separator');
        if (!isEmptyOrBlank(elSeparator) && elSeparator.length > 0)
            elSeparator[0].scrollIntoView(true);
        else
            $channelMessages.scrollTop($channelMessages[0].scrollHeight);
    }
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



var removeSortedParticipantFromRemaningByChannelId = function (arr, channelId) {
    return arr.filter(function (rp) {
        return (rp.dataset.channelid ?? "") !== channelId;
    });
}
$("#divChatSiderbarFilters > button").click(function () {
    var el = $(this);
    $chatSiderbarFilterButtons.removeClass("btn-falcon-primary").addClass("btn-falcon-default");
    el.addClass("btn-falcon-primary");
    let type = el.data().type;
    if (chat.type !== type) {
        resetChatPage();
        if (type === 0) {
            chat.type = 0;
            setTimeout(function () {
                $(".chat-content-header span").text('');
                activeChannel = null;
                chat.selectedRecentParticipantOnce = false;
                addMessageProcessed = [];
                loadChatPage(false, chat.type);
            }, 0)
        }
        else if (type === 1) {
            chat.type = 1;

            setTimeout(function () {
                $(".chat-content-header span").text('');
                activeChannel = null;
                chat.selectedRecentParticipantOnce = false;
                addMessageProcessed = [];
                //loadChatPage(false, chat.type);
                loadreconcilationcomments();
            }, 0)
        }
    }
});

function AgencyDropdownPartialViewChange() {
   
    ShowlottieLoader();
    setTimeout(function () {
        resetChatPage();
        loadChatPage();
        $chatSiderbarFilterButtons.removeClass("btn-falcon-primary").addClass("btn-falcon-default");
        $chatSiderbarFilterButtons.eq(0).addClass("btn-falcon-primary");
    }, 1)
    SetUserPreferencesForAgency();
    window.location.reload();
}

var addMentionPlugin = function () {
    if (chat.type === 0) {
        $(".mentions-autocomplete-list").remove();
        return;
    }
    $messageBodyInput.mentionsInput({
        onDataRequest: function (mode, query, callback) {
            getAjax(`/communication/FilterMentionUsers?searchUser=${query}&userEmail=${chat.userEmail}&chatType=${chat.type}&clientId=${getClientId()}`, null, function (responseData) {
                callback.call(this, responseData);
            });
        },
        onCaret: true
    });
}
var getMentions = function () {
    $('#message-body-input').mentionsInput('getMentions', function (data) {
        return (JSON.stringify(data));
    });
}
var selectSidebarParticipant = function () {
    if (isEmptyOrBlank(getParameterByName('WithTeamMember')) === true && isEmptyOrBlank(getParameterByName('reconChannelId')) === true) {
        if (chat.autoSelectParticipant === true) {
            /*$participants.eq(0).click();*/
        }
    }
    else if (isEmptyOrBlank(getParameterByName('WithTeamMember')) === false) {
        let qsEmail = getParameterByName('WithTeamMember');
        let qsParticipant = $participants.filter(function (i, obj) {
            return obj.dataset.email === qsEmail.toLowerCase();
        });
        if (!isEmptyOrBlank(qsParticipant) && qsParticipant.length > 0)
            qsParticipant[0].click();
        else {
            if (chat.autoSelectParticipant === true)
                $participants.eq(0).click();
        }
    }
    else if (isEmptyOrBlank(getParameterByName('reconChannelId')) === false) {
        let reconChannelId = getParameterByName('reconChannelId');
        let qsParticipant = $participants.filter(function (i, obj) {
            if ((obj.dataset?.channelid ?? "") === reconChannelId) {
                return true;
            }
            return false;
        });
        if (!isEmptyOrBlank(qsParticipant) && qsParticipant.length > 0) {
            qsParticipant[0].click();
        }
        else {
            if (chat.autoSelectParticipant === true)
                $participants.eq(0).click();
        }
    }
}

var UpdateReconciliationHasStatus = function (id) {
    postAjax(`/communication/UpdateReconciliationHasStatus?id=${id}`, null, function (res) {
    });
}