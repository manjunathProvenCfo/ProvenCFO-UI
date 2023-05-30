var $channelName;
var $channelReconciliationDescription;
var $btnSendNMessage = $("#send-nmessage");
var $channelMessages = $("#channel-messages");
var $targetedCategoryId;
var nchat = {
    userId: "",
    userEmail: "test1@mailinator.com",
    twilioUserId: "",
    channels: [],
    participants: [],
    channelIndex: -1,
    publicChannelUniqueNameGuid: "",
    clientId: 0,
    type: 1,
    forReconciliationIconColor: false,
    selectedRecentParticipantOnce: false,
    isReconciliationIconColorChanged: false,
    channelUniqueNameGuid: ''
};

var CommentHtmls = {
    datehtml: '<div id="{id}" class="text-center fs--2 text-500 date-stamp"><span>{innerText}</span></div>',
    commenthtml: `<div class="media p-3" data-timestamp="{date}" id="msg_{commentId}">
    <div class="media-body d-flex justify-content-end">
        <div class="w-100 w-xxl-75">
            <div class="hover-actions-trigger d-flex align-items-center justify-content-end">
                <div class="bg-primary text-white p-2 rounded-soft chat-message">{innerText}</div>
                <ul class="hover-actions position-relative list-inline mb-0 text-400 ms-2">
                    <li class="list-inline-item d-none"><a class="chat-option" href="#!" data-bs-toggle="tooltip" data-bs-placement="top" title="" data-bs-original-title="Edit" onclick="CommentEdit('{commentId}');" aria-label="Edit" data-toggle="modal" data-target="#EditCommentModal"><svg class="svg-inline--fa fa-edit fa-w-18" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="edit" role="img" viewBox="0 0 576 512" data-fa-i2svg=""><path fill="currentColor" d="M402.6 83.2l90.2 90.2c3.8 3.8 3.8 10 0 13.8L274.4 405.6l-92.8 10.3c-12.4 1.4-22.9-9.1-21.5-21.5l10.3-92.8L388.8 83.2c3.8-3.8 10-3.8 13.8 0zm162-22.9l-48.8-48.8c-15.2-15.2-39.9-15.2-55.2 0l-35.4 35.4c-3.8 3.8-3.8 10 0 13.8l90.2 90.2c3.8 3.8 10 3.8 13.8 0l35.4-35.4c15.2-15.3 15.2-40 0-55.2zM384 346.2V448H64V128h229.8c3.2 0 6.2-1.3 8.5-3.5l40-40c7.6-7.6 2.2-20.5-8.5-20.5H48C21.5 64 0 85.5 0 112v352c0 26.5 21.5 48 48 48h352c26.5 0 48-21.5 48-48V306.2c0-10.7-12.9-16-20.5-8.5l-40 40c-2.2 2.3-3.5 5.3-3.5 8.5z"></path></svg></a></li>
                    <li class="list-inline-item"><a class="chat-option" href="#!" data-bs-toggle="tooltip" data-bs-placement="top" title="" data-bs-original-title="Remove" onclick="CommentDelete('{commentId}');" aria-label="Remove"><svg class="svg-inline--fa fa-trash-alt fa-w-14" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="trash-alt" role="img" viewBox="0 0 448 512" data-fa-i2svg=""><path fill="currentColor" d="M32 464a48 48 0 0 0 48 48h288a48 48 0 0 0 48-48V128H32zm272-256a16 16 0 0 1 32 0v224a16 16 0 0 1-32 0zm-96 0a16 16 0 0 1 32 0v224a16 16 0 0 1-32 0zm-96 0a16 16 0 0 1 32 0v224a16 16 0 0 1-32 0zM432 32H312l-9.4-18.7A24 24 0 0 0 281.1 0H166.8a23.72 23.72 0 0 0-21.4 13.3L136 32H16A16 16 0 0 0 0 48v32a16 16 0 0 0 16 16h416a16 16 0 0 0 16-16V48a16 16 0 0 0-16-16z"></path></svg></a></li>
                </ul>
            </div>
            <div class="text-400 fs--2 text-right">
                {time}<span class="ml-2 text-success" data-fa-i2svg=""><svg class="svg-inline--fa fa-check fa-w-16" aria-hidden="true" focusable="false" data-prefix="fas" data-icon="check" role="img" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 512 512" data-fa-i2svg=""><path fill="currentColor" d="M173.898 439.404l-166.4-166.4c-9.997-9.997-9.997-26.206 0-36.204l36.203-36.204c9.997-9.998 26.207-9.998 36.204 0L192 312.69 432.095 72.596c9.997-9.997 26.207-9.997 36.204 0l36.203 36.204c9.997 9.997 9.997 26.206 0 36.204l-294.4 294.401c-9.998 9.997-26.207 9.997-36.204-.001z"></path></svg></span>
            </div>
        </div>
    </div>
</div>`,
    otherscCommentshtml: `<div class="media p-3" data-timestamp="{date}"><div class="avatar avatar-l mr-2">
            <img class="rounded-circle" src="{profileimgurl}" alt="" onerror="imgError(this);"></div><div class="media-body"><div class="w-xxl-75">
                <div class="hover-actions-trigger d-flex align-items-center"><div class="chat-message bg-200 p-2 rounded-soft">
                    {text}
                    </div></div><div class="text-400 fs--2"><span class="font-weight-semi-bold mr-2">{userName}</span>
                    <span>{time}</span></div></div></div></div>`
}

function noteChatsSec() {

    //$('#notesSection').addClass('col-md-8').removeClass('col-9');
    $('#divNChat').show();
    $('.chat-sidebar').addClass('d-none');
    $('#divNChat').removeClass('d-none');
}
noteChatsSec();

// On open of chat.
$('.openNotesChat').on("click", function (e) {

    //$('#accordionExample2').removeAttr('hidden');
    $('#collapseTwo').addClass('show');
    $('#collapseOne').removeClass('show');
    //$("html, body").animate({ scrollTop: "0" }, 1500);

    var targetCategoryType = e.currentTarget.dataset.notescategory;
    $targetedCategoryId = e.currentTarget.dataset.id;

    var CategoryName = document.querySelector('h3[data-categoryid="' + targetCategoryType + '"]');
    var Title = document.querySelector('h6[data-id="' + $targetedCategoryId + '"]');

    $channelName = $(".notesCategoryType");
    $channelName[0].innerText = CategoryName.innerText;

    $channelReconciliationDescription = $(".notesTitle");
    $channelReconciliationDescription[0].innerText = Title.innerText;

    resetScrollNotesChatState($targetedCategoryId);
    loadNotesCommentsPage($targetedCategoryId);
});

// On close of chat.
$('#closeNChat').on("click", function () {

    $('#collapseTwo').removeClass('show');
    $('#collapseOne').addClass('show');
    //$('#accordionExample2')[0].setAttribute('hidden', true);
});

// ---- New code v1 ---
const resetScrollNotesChatState = function (rec) {

    if (rec != "" && rec != null) {

        scrollNotesChatState.notesDescriptionId = rec;

        scrollNotesChatState.pageNo = 2;
        scrollNotesChatState.size = 20;
    };
}

const scrollNotesChatState = {
    size: 20,
    pageNo: 2,
    notesDescriptionId: '',
    target: null
};

$("#channel-messages")[0].addEventListener("scroll", function (e) {
    e.preventDefault();
    if (e.target.scrollTop == 0) {

        LoadOnDemandCommentsPagination(scrollNotesChatState.notesDescriptionId,
            1, scrollNotesChatState.pageNo * scrollNotesChatState.size);

        scrollNotesChatState.pageNo++;

        scrollNotesChatState.target = e.target;
    }
});
function SetupDynamicLoaderOnNotesScroll() {
    scrollNotesChatState.pageNo = 2;
    scrollNotesChatState.size = 10;
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

// ---> To do.
var CommentDelete = function (CommentId) {
    $('#msg_' + CommentId).remove();
    if (CommentId > 0) { DeleteNotesComment(CommentId) };
}

var DeleteNotesComment = function (commentId) {

    postAjaxSync(apiurl + `Notes/DeleteNotesComments?CommentId=` + commentId, null, function (response) {
        var r = response;
        if (response.resultData == true) {

            toastr.success("Message has been removed successfully!");
        }
        else {
            toastr.warning("Error while removing of message.");
        }
    });
}
// ---> To do end here.

var SaveNoteCommentsToDB = function (InputcommentText, NoteDesId) {

    var currentdate = new Date();
    var datetime = getCurrentTime(currentdate);
    var AgencyId = parseInt(nchat.clientId == undefined || nchat.clientId == null ? $("#ddlclient option:selected").val() : nchat.clientId);
    var CreatedBy = $('#topProfilePicture').attr('userId');
    var input = {
        Id: 0,
        NotesDescriptionId_ref: String(NoteDesId),
        CommentText: InputcommentText,
        CreatedBy: CreatedBy,
        CreatedDate: currentdate,
        IsDeleted: false,
        AgencyId: AgencyId
    }
    if (input.CreatedBy != null && input.CreatedBy != '' && input.AgencyId != null && input.AgencyId != '') {
        postAjaxSync(apiurl + `Notes/InsertNotesComments`, JSON.stringify(input), function (response) {
            var r = response;
            if (response.resultData != null) {
                var id = "msg_" + response.resultData;
                $('#msg_0').attr("id", id);
                if ($('#' + id + ' a').length > 0) {
                    $('#' + id + ' a:first').attr("onclick", "CommentEdit('" + response.resultData + "')")
                };
                if ($('#' + id + ' a').length > 1) {
                    $('#' + id + ' a').attr("onclick", "CommentDelete('" + response.resultData + "')");
                }
            }
        });
    }
}

var addNoteComments = function (inputText) {

    var CurrentDate = new Date();
    var CurrentDateString = CurrentDate.getFullYear() + '' + ('0' + (CurrentDate.getMonth() + 1)).slice(-2) + '' + ('0' + CurrentDate.getDate()).slice(-2);
    var CurrentDateStringForDisplay = monthNames[CurrentDate.getMonth()] + ' ' + ('0' + CurrentDate.getDate()).slice(-2) + ', ' + CurrentDate.getFullYear();
    var CurrentTimestring = getCurrentTime(new Date);
    var DateElement = $('#channel-messages #' + CurrentDateString);
    if (DateElement == null || DateElement == undefined || DateElement.length == 0) {
        var dhtml = CommentHtmls.datehtml.replace('{id}', CurrentDateString).replace('{innerText}', CurrentDateStringForDisplay);
        $channelMessages.append(dhtml);
    }
    var chtml = CommentHtmls.commenthtml.replace('{date}', CurrentDateString).replace('{innerText}', inputText).replace('{time}', CurrentTimestring).replace(/{commentId}/g, 0);
    $channelMessages.append(chtml);

    SaveNoteCommentsToDB(inputText, $targetedCategoryId);
    setScrollPosition();
}

function filterTextMessage(e) {
    var chatMessage = Array.prototype.filter.bind($(e));
    var textMessage = chatMessage(chat => chat.innerHTML != "")[0].innerHTML;

    const result = textMessage.replace(/[\r\n]/gm, '');
    return result;
}
// --- New code v1 end here ---


// -- New code v2 starting here --
var loadNotesCommentsPage = async function (channelUniqueNameGuid) {

    showChatContentLoader();
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

    getAjaxSync(apiurl + `Notes/GetNotesCommentsOnNotesDescriptionId?NotesDescriptionId=${channelUniqueNameGuid}&&AgencyId=${nchat.clientId}`, null, async function (responseComm) {
        //setCommentsHeader(responseComm.resultData.reconciliationdata);
        setTimeout(async function () {
            await LoadAllNotesComments(responseComm.resultData.notesComments);
            setScrollPosition();
            hideChatContentLoader();
        }, 100);
    });


    $btnSendNMessage.unbind().click(function () {

        var result = filterTextMessage(".emojionearea-editor")
        if (result == '') {
            $('#send-nmessage').attr("disable", true);
        } else {

            $('#send-nmessage').attr("disable", false);

            addNoteMessagetoChatwindow($('#message-body-input').val());
        }
    });
    var addNoteMessagetoChatwindow = async function (input) {

        if (input == "") {
            return;
        }
        addNoteComments(input);
        $('#message-body-input').empty();
        $('.emojionearea-editor').empty();
        $('#message-body-input').val("");
        $('.emojionearea-editor').val("");


    }

    //// ---> To do.
    $chatEditorArea[0].emojioneArea.off("keydown");
    $chatEditorArea[0].emojioneArea.on("keydown", function ($editor, event) {
        if (event.keyCode === 13 && !event.shiftKey) {

            event.preventDefault();
            if (event.type == "keydown") {
                if ($('.mentions-autocomplete-list:visible li.active').length > 0) {
                    $('.mentions-autocomplete-list:visible li.active').trigger('mousedown');
                }
                else {
                    var result = filterTextMessage(".emojionearea-editor")
                    if (result != '')

                        addNoteMessagetoChatwindow($editor[0].innerHTML);
                }
            }
            else
                activeChannel?.typing();
        }
        else
            activeChannel?.typing();
    });


    // //---> To do.
    setTimeout(addNMentionPlugin, 3000);


    // //---> To do.
    //$messageBodyFileUploader.off("change");
    //$messageBodyFileUploader.on("change", function (e) {
    //    var files = $(this)[0].files;
    //    if (files.length === 0) {
    //        ShowAlertBoxError("File uploader", "Select atleast one file.");
    //        return;
    //    }

    //    if (files.length > 5) {
    //        ShowAlertBoxError("File uploader", "You can upload max 5 files at a time.");
    //        return;
    //    }

    //    //Upload
    //    files.forEach(function (file) {
    //        var uploader = new Uploader(file);
    //        let size = uploader.getSizeInMB();
    //        if (size > 20) {
    //            ShowAlertBoxError("File size exceeded", `${uploader.getName()} file size is ${size} MB. Allowded file size is less than or equal to 20 MB`);
    //        }
    //        else {
    //            addMediaMessageLocalFolder(file);
    //        }
    //    })
    //});
}

var addNMentionPlugin = function () {

    $chatEditorArea = [$(".chat-editor-area .emojiarea")[0]];
    $("#message-body-input")[0].parentElement.id = "message-body-div";
    $messageBodyInput.mentionsInput({
        onDataRequest: function (mode, query, callback) {

            getAjax(`/communication/FilterMentionUsers?searchUser=${query}&userEmail=${nchat.userEmail}&chatType=${nchat.type}&clientId=${nchat.clientId}`, null, function (responseData) {
                callback.call(this, responseData);
            });
        },
        onCaret: true
    });

}

var LoadAllNotesComments = async function (ReconciliationComments) {

    showChatContentLoader();
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
            var key = date;
            let utcDate = dategroups[date][0].createdDateUTC;

            var localTime = UtcDateToLocalTime(utcDate);

            date = `${localTime.getFullYear()}-${localTime.getMonth() + 1}-${localTime.getDate()}`;
            return {
                date: date,
                comments: dategroups[key]
            };
        });
        await $.each(commentsgroupArrays, function (index, aDates) {
            var dtarray = aDates.date.split('-');
            var datestring = monthNames[parseInt(dtarray[1]) - 1] + ' ' + dtarray[2] + ', ' + dtarray[0];
            var dhtml = CommentHtmls.datehtml.replace('{id}', aDates.date.replace('-', '')).replace('{innerText}', datestring);
            $channelMessages.append(dhtml);
            $.each(aDates.comments, function (index, acomments) {

                var UTCdate = getUTCDateTime(new Date(acomments.createdDateUTC));
                var time = getCurrentTime(new Date(UTCdate));
                var profileimgurl = acomments.commentedUserProfileImageurl;
                var commentText = acomments.commentText;
                var commentId = acomments.id;
                var userName = acomments.commentedUserName;
                if (acomments && (acomments.isAttachment == null || acomments.isAttachment == false)) {
                    if (acomments && acomments.createdBy == nchat.userId) {
                        var commentshtml = CommentHtmls.commenthtml.replace('{date}', aDates.date.replace('-', '')).replace('{innerText}', commentText).replace('{time}', time).replace(/{commentId}/g, commentId);
                        $channelMessages.append(commentshtml);
                    }
                    else {
                        var Otherscommentshtml = CommentHtmls.otherscCommentshtml.replace('{profileimgurl}', profileimgurl).replace('{text}', commentText).replace('{userName}', userName).replace('{date}', aDates.date.replace('-', '')).replace('{time}', time);
                        $channelMessages.append(Otherscommentshtml);
                    }
                }
                else {
                    if (acomments.fileType != null) {
                        //
                        var FileName = acomments.fileName;
                        var FileScrPath = acomments.fileAttachmentPath;
                        var CommentId = acomments.id;
                        var FileExtention = acomments.fileType.replace(".", "");

                        switch (FileExtention.toLowerCase()) {
                            case 'jpg':
                            case 'jpeg':
                            case 'png':
                            case 'gif':
                            case 'jfif':
                                var Imagehtml = '';
                                if (acomments && acomments.createdBy == nchat.userId) {
                                    Imagehtml = CommentHtmls.SelfAttachmentImageHtml.replace(/{commentId}/g, CommentId).replace(/{time}/g, time).replace(/{FileScrPath}/g, FileScrPath).replace('{userName}', userName).replace('{profileimgurl}', profileimgurl);
                                }
                                else {
                                    Imagehtml = CommentHtmls.otherscAttachmentImageHtml.replace(/{commentId}/g, CommentId).replace(/{time}/g, time).replace(/{FileScrPath}/g, FileScrPath).replace('{userName}', userName).replace('{profileimgurl}', profileimgurl);
                                }
                                $channelMessages.append(Imagehtml);
                                break;
                            case 'zip':
                            case '7z':
                            case 'rar':
                            case 'pdf':
                            case 'txt':
                            case 'xls':
                            case 'xlsx':
                            case 'csv':
                            case 'doc':
                            case 'docx':
                                var Dochtml = '';
                                if (acomments && acomments.createdBy == nchat.userId) {
                                    Dochtml = CommentHtmls.SelfAttachmentDocumentHtml.replace(/{commentId}/g, CommentId).replace(/{time}/g, time).replace(/{FileScrPath}/g, FileScrPath).replace(/{FileName}/g, FileName).replace('{userName}', userName).replace('{profileimgurl}', profileimgurl);
                                }
                                else {
                                    Dochtml = CommentHtmls.otherscAttachmentDocumentsHtml.replace(/{commentId}/g, CommentId).replace(/{time}/g, time).replace(/{FileScrPath}/g, FileScrPath).replace(/{FileName}/g, FileName).replace('{userName}', userName).replace('{profileimgurl}', profileimgurl);
                                }
                                $channelMessages.append(Dochtml);
                                //window.open(filepath, '_blank');
                                //setTimeout(function () { $('.fancybox-button--close').click(); }, 500);

                                break;
                            default:
                                break;
                        }
                    }
                }
            });
        });
    }
}

async function LoadPaginationNotesContent(channelUniqueNameGuid, pageNo, pageSize) {
    getAjaxSync(apiurl + `Notes/GetNotesCommentsOnNotesDescriptionId?NotesDescriptionId=${channelUniqueNameGuid}&&AgencyId=${nchat.clientId}&&pageNo=${pageNo}&&pageSize=${pageSize}`, null, async function (responseComm) {

        await LoadAllNotesComments(responseComm.resultData.notesComments);

        if (scrollNotesChatState.target) {

            setTimeout(_ => scrollNotesChatState.target.scrollTop = 4, 150);
        }
        //setScrollPosition();
        hideChatContentLoader();
    });
}

function LoadOnDemandCommentsPagination(channelUniqueNameGuid, pageNo, pageSize) {

    showChatContentLoader();
    setTimeout(async function () {
        await LoadPaginationNotesContent(channelUniqueNameGuid, pageNo, pageSize);
    }, 200);
}
// -- New code v2 end here --

window.onload = function () {

    if ($('.openNotesChat').length > 0) {
        $('#collapseOne').on('shown.bs.collapse', function () {

            $('#collapseTwo').removeClass('show');
        });
        $('#collapseOne').on('hidden.bs.collapse', function () {
            $('#collapseTwo').addClass('show');
        });


        $('#accordionExample2').removeAttr('hidden');


        $('#collapseTwo').on('shown.bs.collapse', function () {
            $('#collapseOne').removeClass('show');
        });
        $('#collapseTwo').on('hidden.bs.collapse', function () {
            $('#collapseOne').addClass('show');
        });

        var targetCategoryType = $('.openNotesChat')[0].getAttribute('data-notesCategory');
        var CategoryName = document.querySelector('h3[data-categoryid="' + targetCategoryType + '"]');
        $channelName = $(".notesCategoryType");
        $channelName[0].innerText = CategoryName.innerText;

        var targetCategory = $('.openNotesChat')[0].getAttribute('data-id');
        var Title = document.querySelector('h6[data-id="' + targetCategory + '"]');
        $channelReconciliationDescription = $(".notesTitle");
        $channelReconciliationDescription[0].innerText = Title.innerText;

        $targetedCategoryId = targetCategory;
        resetScrollNotesChatState($targetedCategoryId);
        loadNotesCommentsPage($targetedCategoryId);
    }
}
