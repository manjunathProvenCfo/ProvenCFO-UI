
$('divClientDetials').addClass('d-none');
var selectedTask = '';
var selectedColumnsid = '';
var gCurrentViewTaskId = '';




$(document).ready(function () {
    $('#divEditDescription').hide();
    // Dropzone.autoDiscover = false;
    //view Page
    var previewNode_view = document.querySelector("#template_view");
    previewNode_view.id = "";
    var previewTemplate_view = previewNode_view.parentNode.innerHTML;
    previewNode_view.parentNode.removeChild(previewNode_view);
    //view Page
    var myDropzone_view = new Dropzone("#kanban-modal-open", { // Make the whole body a dropzone
        url: "/Needs/UploadFileAndSave", // Set the url
        thumbnailWidth: 80,
        thumbnailHeight: 80,
        parallelUploads: 20,
        previewTemplate: previewTemplate_view,
        autoQueue: false, // Make sure the files aren't queued until manually added
        previewsContainer: "#previews_view", // Define the container to display the previews
        clickable: ".btnselectfile_view", // Define the element that should be used as click trigger to select files.
        success: function (file, response) {
            if (response != null && response.Status == 'Success') {
                addAttachmentOnviewLoad([response.File], false, true);

                $.each($('#previews_view .file-row '), function (key, value) {
                    if (value != null && value.children[2] != undefined && value.children[2].innerText == file.name) {
                        value.remove();
                    }
                });
                // $('#previews_view').empty();                      
            }
        }
    });
    //view Page
    myDropzone_view.on("addedfile", function (file) {
        file.previewElement.querySelector(".start").onclick = function () {

            var IsCanAddfiles = true;
            var filesList = $('#attachmentContainer h6');

            $.each(filesList, function (key, item) {
                if (item != null && item.innerText == file.name) {
                    IsCanAddfiles = false;
                }
            });
            if (IsCanAddfiles == true) {
                myDropzone_view.enqueueFile(file);
                var AttachmentCount = $('#attCount_' + gCurrentViewTaskId + ' span')[0].innerText.trim();
                if (AttachmentCount != undefined && AttachmentCount != '') {
                    $('#attCount_' + gCurrentViewTaskId + ' span')[0].innerText = parseInt(AttachmentCount) + 1;
                }
            }
            else {
                ShowAlertBox('Exist!', 'Selected file is already attached.', 'warning');
            }

            //var att = '<div class="media align-items-center mb-3" id="att_' + 0 + '"><a class="text-decoration-none mr-3" href="' + item.FilePath + '" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(' + item.FilePath + ');"></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" data-fancybox="attachment-title">' + item.AttachedFileName + '</a></h6><span class="mx-1"></span><button data-dz-remove class=" cancel" style="border: none; background: transparent; font-size: 12px;" onclick="Removeattachment_view(' + item.Id + ',' + singlCode + item.AttachedFileName + singlCode + ',' + true + ')"><i class="glyphicon glyphicon-ban-circle"></i><span>Remove</span></button><p class="mb-0">Uploaded at ' + item.CreatedDateForDisplay + '</p></div></div>';
            //$('#attachmentContainer').prepend(att);
        };
    });

    //view Page
    myDropzone_view.on("sending", function (file) {
        file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
    });

    //Create Page
    var previewNode = document.querySelector("#template");
    previewNode.id = "";
    var previewTemplate = previewNode.parentNode.innerHTML;
    previewNode.parentNode.removeChild(previewNode);

    //Create Page
    var myDropzone = new Dropzone("#kanban-modal-new", { // Make the whole body a dropzone
        url: "/Needs/UploadFile", // Set the url
        thumbnailWidth: 80,
        thumbnailHeight: 80,
        parallelUploads: 20,
        previewTemplate: previewTemplate,
        autoQueue: false, // Make sure the files aren't queued until manually added
        previewsContainer: "#previews", // Define the container to display the previews
        clickable: ".btnselectfile", // Define the element that should be used as click trigger to select files.
        success: function (file, response) {
            if (response != null && response.Status == 'Success') {
                addAttachmentOnviewLoad([response.File], false, false);

                $.each($('#previews .file-row '), function (key, value) {
                    if (value != null && value.children[2] != undefined && value.children[1].innerText == file.name) {
                        value.remove();
                    }
                });

            }
        }
    });


    //Create Page
    myDropzone.on("addedfile", function (file) {
        file.previewElement.querySelector(".start").onclick = function () {
            var IsCanAddfiles = true;
            var filesList = $('#attachmentContainer_Add h6');

            $.each(filesList, function (key, item) {
                if (item != null && item.innerText == file.name) {
                    IsCanAddfiles = false;
                }
            });
            if (IsCanAddfiles == true) {
                myDropzone.enqueueFile(file);

            }
            else {
                ShowAlertBox('Exist!', 'Selected file is already attached.', 'warning');
            }

        };
    });

    //Create Page
    myDropzone.on("sending", function (file) {
        file.previewElement.querySelector(".start").setAttribute("disabled", "disabled");
    });

    $('.delete ').click(function (e) {
        alert('Hi');
    })

    $(".glyphicon-trash").on('click', function (event) {
        alert('delere');
    });

    //AgencyDropdownPartialViewChange();
    var ClientID = $("#ddlclient option:selected").val();
    getAgencyMembersList(ClientID);

    $("#btnCreateNewTicket").click(function () {

        var TaskType = '';//$('#ddlTaskType').val();
        var TaskTitle = $('#txtTaskTitle').val();
        var Description = tinyMCE.activeEditor.getContent();
        //var Assignee = '';//$('#ddlAssignee').val();
        var Priority = 'medium';//$('#ddlPriority').val();
        var dpStartDate = '';//$('#dpStartDate').val();
        var dpEndDate = '';//$('#dpEndDate').val();
        var dpDueDate = '';//$('#dpDueDate').val();
        var EstimatedHours = '';//$('#txtEstimatedHours').val();
        var Labels = $('#divTag span').map(function (i, opt) {
            return $(opt) != null && $(opt).length > 0 ? $(opt)[0].innerText : '';
        }).toArray().join(', ');
        var Assignee = $('#ulAddedMembers li').map(function (i, opt) {
            return $(opt) != null && $(opt).length > 0 && $(opt)[0].id != '' && $(opt)[0].id.indexOf('li_') != -1 ? $(opt)[0].id : '';
        }).toArray().join(', ');

        var pdata = { TaskTitle: TaskTitle, TaskDescription: Description, Assignee: Assignee, Priority: Priority, dpStartDate: dpStartDate, dpEndDate: dpEndDate, dpDueDate: dpDueDate, EstimatedHours: EstimatedHours, TaskType: TaskType, Labels: Labels };

        $.ajax({
            type: "POST",
            url: "/Needs/CreatNewTask",
            data: JSON.stringify({ Task: pdata }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.Message == 'Success') {
                    window.location.reload();
                    $('.close-circle').click();
                    $('.modal-backdrop').remove();
                }
                else {
                    if (response.Message == 'Exist') {
                        ShowAlertBox('Required!', response.Message, 'warning');
                    }
                    else {
                        ShowAlertBox('Exist!', response.Message, 'warning');

                    }
                }
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });

    });
    $('#btnCancelComments').click(function (e) {
        $('#txtComments').val('');
    });
    $('#btnEditDescription').click(function (e) {

        $('#divEditDescription').show();
        $('#divViewDescription').hide();
        var content = $('#kanban-modal-label-description').html();
        tinymce.get("txtDescription").setContent(content);
    });
    $('#btnSaveDescription').click(function (e) {
        var TaskId = gCurrentViewTaskId;
        var Description = tinyMCE.activeEditor.getContent();
        UpdateTaskDescription(TaskId, Description);
        $('#divViewDescription').show();
        $('#divEditDescription').hide()
        $('#kanban-modal-label-description').html(tinyMCE.activeEditor.getContent());
    });
    $('#btnCancelDescription').click(function (e) {
        $('#divViewDescription').show();
        $('#divEditDescription').hide();
    });
    $('#kanband-create-new-btn').click(function (e) {
        tinymce.get("txtDescription").getContent()
        tinymce.get("txtDescription").setContent("");
        tinyMCE.activeEditor.setContent("");
        gCurrentViewTaskId = '';
        $('#divTagView ul').empty();
        $('#divTag ul').empty();
        $.each($('#ulAddedMembers li'), function (key, assigne) {
            if (assigne != null && assigne.attributes['id'] != undefined && assigne.attributes['id'].value != undefined && assigne.attributes['id'].value != '') {
                if (assigne.attributes['id'].value.indexOf('li_') != -1) {
                    assigne.remove();
                }                
            }
        });
        $('#txtTaskTitle').val('');
        $.each($('#attachmentContainer_Add .media'), function (key, attachment) {           
            attachment.remove();
            
        });
        $('#txtTaskTitle').val('');
        $('#previews').empty();

    });
    $('.kanban-item-card').click(function (e) {

        var TaskID = 0;

        var elements = e.currentTarget.children[0].children;
        ClearViewPage();
        $.each(elements, function (key, value) {
            if (value != null && value.nodeName != undefined && value.nodeName == 'INPUT') {
                TaskID = String(value.id.split("_")[1]);
            }
        });
        gCurrentViewTaskId = TaskID;
        $.ajax({
            type: "POST",
            url: "/Needs/OpenExistingTask?TaskID=" + TaskID,
            // data: JSON.stringify({ TaskID: pdata }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.Message == 'Success') {
                    if (response.Task != null) {
                        $('#kanban-modal-label-title').html(response.Task.TaskTitle);
                        // setTimeout(function () {
                        if (response.Task.TaskDescription != null) {
                            $('#kanban-modal-label-description').html(response.Task.TaskDescription);
                            tinyMCE.activeEditor.setContent(response.Task.TaskDescription);
                        }
                        else {
                            $('#kanban-modal-label-description').html('');
                        }

                        //}, 1000); 
                        $('#Reporter').html(response.Task.ReporterName);
                        addMembersOnviewLoad(response.Task.KanbanAssigneesList);
                        addAttachmentOnviewLoad(response.Task.KanbanAttachments, true, true);
                        addcommentsList(response.Task.KanbanComments);
                        addTagOnView(response.Task.Labels);
                        $('#previews').empty();
                        $('#previews_view').empty();
                    }
                }
                else {
                    if (response.Message == 'Exist') {
                        ShowAlertBox('Required!', response.Message, 'warning');
                    }
                    else {
                        ShowAlertBox('Exist!', response.Message, 'warning');
                    }
                }



            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    })

    $('#btnSaveComments').click(function (e) {
        var commentText = $('#txtComments').val();
        if (commentText != null && commentText.trim() != '') {
            var TaskId = gCurrentViewTaskId;
            var ComType = 'text';

            var pdata = {
                CommentText: commentText, CommentType: ComType, TaskId_Ref: TaskId, ParentCommentID_Ref: 0
            }

            $.ajax({
                type: "POST",
                url: "/Needs/AddComments",
                data: JSON.stringify({ Comment: pdata }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.Message == 'Success') {
                        addComment(response.data, response.UserFullName);
                    }
                    else if (response.Message == 'Error') {
                        ShowAlertBox("Error", 'Error while adding new comment.', 'error');
                    }
                },
                failure: function (response) {
                    alert(response.responseText);
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
        else {
            ShowAlertBox('Required!', 'Please enter the text in comment box to save the comments.', 'warning');
        }

    })


});
function ClearViewPage() {
    $('#divCommantsList').empty();
}
function RemoveFile(e) {

    var fileToRemove = e.currentTarget.children[1].innerText;
    $.ajax({
        type: "POST",
        url: "/Needs/RemoveFile?fileName=" + fileToRemove,
        // data: JSON.stringify({ TaskID: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {

            }
            else {
                alert("Error while upload");
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

function RemoveAttchment(attachmentId) {
    $('#att_' + attachmentId)[0].remove();

}
function RemovveFileOnCreate(attachmentId) {
    RemoveAttchment(attachmentId);
    var fileToRemove = $('#att_' + attachmentId + ' h6')[0].innerText;

    $.ajax({
        type: "POST",
        url: "/Needs/RemoveFile?fileName=" + fileToRemove,
        // data: JSON.stringify({ TaskID: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {

            }
            else {
                alert("Error while upload");
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

function RemoveMember(UserID) {
    $('#li_' + UserID)[0].remove();

}
function Removeattachment_view(attachmentId, FileName, IsTaskAttachment) {
    $('#att_' + attachmentId)[0].remove();
    var TaskId = gCurrentViewTaskId;
    $.ajax({
        type: "POST",
        url: "/Needs/RemoveAttachmentFromKanbanTask?TaskId=" + TaskId + "&FileName=" + FileName + '&IsTaskAttachment=' + IsTaskAttachment,
        // data: JSON.stringify({ TaskID: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {

                var AttachmentCount = $('#attCount_' + gCurrentViewTaskId + ' span')[0].innerText.trim();
                if (AttachmentCount != undefined && AttachmentCount != '') {
                    $('#attCount_' + gCurrentViewTaskId + ' span')[0].innerText = parseInt(AttachmentCount) - 1;
                }
                return true;
            }
            else {
                ShowAlertBox("Error", response.Message, 'error');
            }
        },
        failure: function (response) {
            ShowAlertBox("Error", response.Message, 'error');
        },
        error: function (response) {
            ShowAlertBox("Error", response.Message, 'error');
        }
    });
}
function RemoveMember_view(UserID) {
    $('#li_' + UserID).remove();
    $('#ulAddedMembers_view #li_' + UserID).remove();

    var TaskId = gCurrentViewTaskId;
    $.ajax({
        type: "POST",
        url: "/Needs/RemoveAssigneeFromKanbanTask?TaskId=" + TaskId + "&UserId=" + UserID,
        // data: JSON.stringify({ TaskID: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {
                return true;
            }
            else {
                ShowAlertBox("Error", response.Message, 'error');
            }
        },
        failure: function (response) {
            ShowAlertBox("Error", response.Message, 'error');
        },
        error: function (response) {
            ShowAlertBox("Error", response.Message, 'error');
        }
    });
    $('#' + gCurrentViewTaskId + ' #li_' + UserID).remove();
}

function allowDrop(ev) {
    ev.preventDefault();
}

function drag(ev) {
    var count = parseInt($('#' + $('#' + ev.target.id).parent().parent()[0].id + ' h5 span')[0].innerText.replace('(', '').replace(')', ''));
    selectedTask = ev.target.id;
    //count = count - 1;
    selectedColumnsid = $('#' + ev.target.id).parent().parent()[0].id;
    // $('#' + $('#' + ev.target.id).parent().parent()[0].id + ' h5 span')[0].innerText = '(' + count + ')';
    ev.dataTransfer.setData("text", ev.target.id);
}

function drop(ev) {
    ev.preventDefault();
    //var count = parseInt($('#' + $('#' + ev.target.id).parent()[0].id + ' h5 span')[0].innerText.replace('(', '').replace(')', ''));
    //var fromCount = parseInt($('#' + selectedColumnsid + ' h5 span')[0].innerText.replace('(', '').replace(')', ''));
    var segmentType1 = '';
    var data = ev.dataTransfer.getData("text");
    if (ev.target.className.indexOf('kanban-items-container-new') == -1) {
        $('#' + data).insertBefore($('#' + ev.target.closest(".kanban-item").id));
        segmentType1 = ev.target.closest(".kanban-items-container-new").attributes['segmenttype'].value;
    } else {
        ev.target.appendChild(document.getElementById(data));
        segmentType1 = ev.target.attributes['segmenttype'].value;
    }
    if (segmentType1 != '') {
        UpdateTaskSegmentType(selectedTask, segmentType1);
        updateKanbanColumnsCount();
    }
}
function updateKanbanColumnsCount() {
    var columns = $('.kanban-container .kanban-column');
    if (columns != null && columns.length > 0) {
        $.each(columns, function (key, col) {
            if (col != null && col.attributes['id'] != undefined) {
                var cardscount = $('#' + col.attributes['id'].value + ' .kanban-item');
                $('#' + col.attributes['id'].value + ' .kanban-column-header span')[0].innerText = '(' + cardscount.length + ')';
            }
        });
    }
}

function AgencyDropdownPartialViewChange() {
    var ClientID = $("#ddlclient option:selected").val();
    //getTeamMembersList(ClientID);
    if (ClientID != null && ClientID != undefined && ClientID != '') {
        getAgencyMembersList(ClientID);
        $.ajax({
            url: '/AgencyService/GetClientDetails?id=' + ClientID,
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $('#kanband-create-new-btn').show();
                    // Write here what should happend once client selected

                    SetUserPreferencesForAgency();
                    window.location.reload();
                }
                else {
                    // Write here what should happend once selected client is null
                }
            },
            error: function () {
                // Write here what should happend when action result is errored.
            }
        });
    }
    else {
        ClearAllTickets();
        //window.location.reload();
    }
}
function ClearAllTickets() {
    var columns = $('.kanban-container .kanban-column');
    if (columns != null && columns.length > 0) {
        $.each(columns, function (key, col) {
            if (col != null && col.attributes['id'] != undefined) {
                var cardscount = $('#' + col.attributes['id'].value + ' .kanban-item');
                $.each(columns, function (key1, item) {
                    item.remove();
                });
            }
        });
        $('#kanband-create-new-btn').hide();
    }


}
function UpdateTaskSegmentType(TaskID, SegmentType) {
    $.ajax({
        type: "POST",
        url: "/Needs/UpdateTaskSegmentType?TaskID=" + TaskID + "&SegmentType=" + SegmentType,
        // data: JSON.stringify({ TaskID: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {
                return true;
            }
            else {
                return false;
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}
function UpdateTaskDescription(TaskID, DescriptionText) {

    var pdata = {
        Id: TaskID, TaskDescription: DescriptionText
    }
    $.ajax({
        type: "POST",
        url: "/Needs/UpdateTaskDescription",
        data: JSON.stringify({ Task: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {
                return true;
            }
            else {
                return false;
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}
function UpdateLableForKanbanTask(TaskID, LableOrTag) {

    var pdata = {
        Id: TaskID, Labels: LableOrTag
    }
    $.ajax({
        type: "POST",
        url: "/Needs/UpdateLableForKanbanTask",
        data: JSON.stringify({ Task: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {
                return true;
            }
            else {
                return false;
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}
function getAgencyMembersList(ClientId) {
    $.ajax({
        type: "POST",
        url: "/Needs/getAgencyMembersList?ClientId=" + ClientId,
        // data: JSON.stringify({ TaskID: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {
                sessionStorage.setItem('getAgencyMembersList', JSON.stringify(response.TeamMembers));
                response.TeamMembers.forEach(function (item) {
                    $('#ulTeamMembersList').append('<li onclick="addMembers(event);" id=' + item.Id + '><a class="media align-items-center text-decoration-none hover-200 py-1 px-3" href="#"><div class="avatar avatar-xl mr-2"><img class="rounded-circle" src="' + item.ProfileImage + '" alt="" /></div><div class="media-body"><h6 class="mb-0">' + item.FirstName + ' ' + item.LastName + '</h6></div></a></li>');
                    $('#ulTeamMembersList_view').append('<li onclick="addMembers_view(event);" id=' + item.Id + '><a class="media align-items-center text-decoration-none hover-200 py-1 px-3" href="#"><div class="avatar avatar-xl mr-2"><img class="rounded-circle" src="' + item.ProfileImage + '" alt="" /></div><div class="media-body"><h6 class="mb-0">' + item.FirstName + ' ' + item.LastName + '</h6></div></a></li>');
                });
                return true;
            }
            else {
                return false;
            }
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}
function addMembersOnviewLoad(AssigneeList) {
    var assigneelist = $('#ulAddedMembers_view');

    $.each(assigneelist.children(), function (key, item) {
        if (item != null && item.attributes['id'] != undefined) {
            item.remove();
        }
    });
    if (AssigneeList != null && AssigneeList.length > 0) {
        var singlCode = "'";
        
        $.each(AssigneeList, function (key, item) {
            if (item != null) {

                if ($('#ulAddedMembers_view #li_' + item.UserId_Ref).length == 0) {
                    $('#ulAddedMembers_view').prepend('<li class="nav-item dropdown"  id="li_' + item.UserId_Ref + '"><a class="nav-link p-0 dropdown-toggle dropdown-caret-none" href="#!" data-toggle="dropdown"><div class="avatar avatar-xl"><img class="rounded-circle" src="' + item.ProfileImage + '" alt="" /></div></a><div class="dropdown-menu dropdown-md px-0 py-3"><div class="media align-items-center px-3"><div class="avatar avatar-2xl mr-2"><img class="rounded-circle" src="' + item.ProfileImage + '" alt="" /></div><div onclick="RemoveMember_view(' + singlCode + item.UserId_Ref + singlCode + ');" class="media-body"><h6 class="mb-0"><a class="stretched-link text-900" href="#">' + item.UserName + '</a></h6><p  class="mb-0 fs--2">' + item.UserName + '</p></div></div><hr class="my-2" /><span onclick="EnterComments(event);" userid=' + item.UserId_Ref + ' class="input-group-btn"><a class="dropdown-item" href="#!">.</a><div class="dropdown-item text-danger" id=REM_' + item.UserId_Ref + ' onclick="EnterComments();" userid=' + item.UserId_Ref + '>Remove Member</div></span></div></li>');
                }
            }
        });
    }
}
function addAttachmentOnviewLoad(attachmentsList, IsClearExisting, IsViewMode) {
    var singlCode = "'";
    var attachmentContainer = IsViewMode == true ? $('#attachmentContainer') : $('#attachmentContainer_Add');
    if (IsClearExisting == true) {
        $.each(attachmentContainer.children(), function (key, item) {
            if (item != null && item.attributes['id'] != undefined) {
                item.remove();
            }
        });
    }
    if (attachmentsList != null && attachmentsList.length > 0) {


        $.each(attachmentsList, function (key, item) {
            if (item != null) {
                var bgimage = item.FilePath;
                var Openimage = item.FilePath;
                var d = new Date($.now());
                var currentdatetime = (d.getMonth() + 1) + '/' + d.getDate() + '/' + d.getFullYear() + ' ' + formatAMPM(d);
               
                var uploadedTime = item.CreatedDateForDisplay == null ? currentdatetime : item.CreatedDateForDisplay;
                if (item.FileType.toUpperCase() == 'PDF') {
                    bgimage = '../../assets/img/kanban/I_PDF.png';
                }
                else if (item.FileType.toUpperCase() == 'DOCX' || item.FileType.toUpperCase() == 'DOC') {
                    bgimage = '../../assets/img/kanban/I_Doc.png';
                }
                else if (item.FileType.toUpperCase() == 'RAR' || item.FileType.toUpperCase() == 'ZIP') {
                    bgimage = '../../assets/img/kanban/I_Zip.png';
                }
                else if (item.FileType.toUpperCase() == 'XLSX' || item.FileType.toUpperCase() == 'XLS') {
                    bgimage = '../../assets/img/kanban/I_XLS.jpg';
                }
                else if (item.FileType.toUpperCase() == 'TXT' || item.FileType.toUpperCase() == 'txt') {
                    bgimage = '../../assets/img/kanban/I_TXT.png';
                }
                else if (item.FileType.toUpperCase() == 'CSV' || item.FileType.toUpperCase() == 'csv') {
                    bgimage = '../../assets/img/kanban/I_CSV.png';
                }
                if (IsViewMode == true && $('#attachmentContainer #att_' + item.Id).length == 0) {
                    var singlCode = "'";
                    var att = '<div class="media align-items-center mb-3" id="att_' + item.Id + '"><a class="text-decoration-none mr-3" href="' + bgimage + '" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(' + bgimage + ');background-size:115px 60px" onclick="openModal(' + singlCode + Openimage + singlCode + ', false);"></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" onclick="openModal(' + singlCode + Openimage + singlCode + ', true);" data-fancybox="attachment-title">' + item.AttachedFileName + '</a></h6><span class="mx-1"></span><button data-dz-remove class=" cancel" style="border: none; background: transparent; font-size: 12px;" onclick="Removeattachment_view(' + item.Id + ',' + singlCode + item.AttachedFileName + singlCode + ',' + true + ')"><i class="glyphicon glyphicon-ban-circle"></i><span>Remove</span></button><p class="mb-0">Uploaded at ' + uploadedTime + '</p></div></div>';
                    $('#attachmentContainer').prepend(att);
                }
                else {
                    var singlCode = "'";
                    var TempAttchmentID = ($('#attachmentContainer_Add media').length) + 1;
                    var att = '<div class="media align-items-center mb-3" id="att_' + TempAttchmentID + '"><a class="text-decoration-none mr-3" href="' + bgimage + '" data-fancybox="attachment-bg"><div class="bg-attachment"><div class="bg-holder rounded" style="background-image:url(' + bgimage + ');background-size:115px 60px" onclick="openModal(' + singlCode + Openimage + singlCode + ', false);"></div></div></a><div class="media-body fs--2"><h6 class="mb-1"><a class="text-decoration-none" href="~/assets/img/kanban/3.jpg" onclick="openModal(' + singlCode + Openimage + singlCode + ', true);" data-fancybox="attachment-title">' + item.AttachedFileName + '</a></h6><span class="mx-1"></span><button data-dz-remove class=" cancel" style="border: none; background: transparent; font-size: 12px;" onclick="RemovveFileOnCreate(' + TempAttchmentID + ')"><i class="glyphicon glyphicon-ban-circle"></i><span>Remove</span></button><p class="mb-0">Uploaded at ' + uploadedTime + '</p></div></div>';
                    $('#attachmentContainer_Add').prepend(att);
                }

            }
        });
    }
}
function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}

function addNewAssigneeToKanbanTask(UserID) {

    var TaskId = gCurrentViewTaskId;
    var pdata = { TaskId_Ref: TaskId, UserId_Ref: UserID };
    $.ajax({
        type: "POST",
        url: "/Needs/addNewAssigneeToKanbanTask",
        data: JSON.stringify({ Assignee: pdata }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.Message == 'Success') {
                return true;
            }
            else {
                ShowAlertBox("Error", response.Message, 'error');
            }
        },
        failure: function (response) {
            ShowAlertBox("Error", response.Message, 'error');
        },
        error: function (response) {
            ShowAlertBox("Error", response.Message, 'error');
        }
    });

}
function addcommentsList(data) {
    if (data && data.length > 0) {
        $.each(data, function (key, value) {
            if (value != null) {
                //var profilePic = data.ProfileImage != undefined && data.ProfileImage != null && data.ProfileImage != '' ? data.ProfileImage : '../assets/img/team/avatar.png';
                //var commentHTMLelement = '<div class="media mb-3"> <a href="#"><div class="avatar avatar-l"><img class="rounded-circle" src="' + profilePic + '" alt="" /></div></a><div class="media-body ml-2 fs--1"><p class="mb-1 bg-200 rounded-soft p-2"><a class="font-weight-semi-bold" href="#">' + value.CreatedByName + ' </a>' + value.CommentText + '</p><a href="#!">Like</a> &bull;  &bull;' + value.CommentDuration + '</div></div>';
                //commentobj.prepend(commentHTMLelement);
                //$('#txtComments').val('');
                addComment(value, value.CreatedByName);
            }
        });
    }
}
function addComment(data, UserFullName) {
    var commentobj = $('#divCommantsList');
    if (commentobj != null) {
        var profilePic = data.UserProfilePic != undefined && data.UserProfilePic != null && data.UserProfilePic != '' ? data.UserProfilePic : '../assets/img/team/avatar.png';
        var commentHTMLelement = '<div class="media mb-3"> <a href="#"><div class="avatar avatar-l"><img class="rounded-circle" src="' + profilePic + '" alt="" /></div></a><div class="media-body ml-2 fs--1"><p class="mb-1 bg-200 rounded-soft p-2"><a class="font-weight-semi-bold" href="#">' + UserFullName + '  : </a>' + data.CommentText + '</p><a href="#!">Like</a> &bull; ' + data.CommentDuration + ' </div></div>';
        commentobj.prepend(commentHTMLelement);
        $('#txtComments').val('');
    }
}
function addMembers(e) {

    var element = $('#' + e.currentTarget.id + ' h6');
    var IsExistelement = $('#ulAddedMembers #li_' + e.currentTarget.id);
    if (IsExistelement.length > 0) {
        ShowAlertBox('Exist!', 'Selected member is already added.', 'warning');
        return;
    }
    var singlCode = "'";
    var getAgencyMembersList = JSON.parse(sessionStorage.getItem('getAgencyMembersList'));
    var SelectMember = getAgencyMembersList.find(x => x.Id === e.currentTarget.id);

    $('#ulAddedMembers').prepend('<li class="nav-item dropdown"  id="li_' + SelectMember.Id + '"><a class="nav-link p-0 dropdown-toggle dropdown-caret-none" href="#!" data-toggle="dropdown"><div class="avatar avatar-xl"><img class="rounded-circle" src="' + SelectMember.ProfileImage + '" alt="" /></div></a><div class="dropdown-menu dropdown-md px-0 py-3"><div class="media align-items-center px-3"><div class="avatar avatar-2xl mr-2"><img class="rounded-circle" src="' + SelectMember.ProfileImage + '" alt="" /></div><div onclick="RemoveMember(' + singlCode + SelectMember.Id + singlCode + ');" class="media-body"><h6 class="mb-0"><a class="stretched-link text-900" href="#">' + SelectMember.FirstName + ' ' + SelectMember.LastName + '</a></h6><p  class="mb-0 fs--2">' + SelectMember.FirstName + ' ' + SelectMember.LastName + '</p></div></div><hr class="my-2" /><span onclick="EnterComments(event);" userid=' + SelectMember.Id + ' class="input-group-btn"><a class="dropdown-item" href="#!">.</a><div class="dropdown-item text-danger" id=REM_' + SelectMember.Id + ' onclick="EnterComments();" userid=' + SelectMember.Id + '>Remove Member</div></span></div></li>');
}
function addMembers_view(e) {
    var element = $('#' + e.currentTarget.id + ' h6');
    var MainPageelement = $('#' + gCurrentViewTaskId + ' ul')
    var singlCode = "'";
    var IsExistelement = $('#ulAddedMembers_view #li_' + e.currentTarget.id);
    if (IsExistelement.length > 0) {
        ShowAlertBox('Exist!', 'Selected member is already added.', 'warning');
        return;
    }
    var getAgencyMembersList = JSON.parse(sessionStorage.getItem('getAgencyMembersList'));
    var SelectMember = getAgencyMembersList.find(x => x.Id === e.currentTarget.id);

    $('#ulAddedMembers_view').prepend('<li class="nav-item dropdown"  id="li_' + SelectMember.Id + '"><a class="nav-link p-0 dropdown-toggle dropdown-caret-none" href="#!" data-toggle="dropdown"><div class="avatar avatar-xl"><img class="rounded-circle" src="' + SelectMember.ProfileImage + '" alt="" /></div></a><div class="dropdown-menu dropdown-md px-0 py-3"><div class="media align-items-center px-3"><div class="avatar avatar-2xl mr-2"><img class="rounded-circle" src="' + SelectMember.ProfileImage + '" alt="" /></div><div onclick="RemoveMember_view(' + singlCode + SelectMember.Id + singlCode + ');" class="media-body"><h6 class="mb-0"><a class="stretched-link text-900" href="#">' + SelectMember.FirstName + ' ' + SelectMember.LastName + '</a></h6><p  class="mb-0 fs--2">' + SelectMember.FirstName + ' ' + SelectMember.LastName + '</p></div></div><hr class="my-2" /><span onclick="EnterComments(event);" userid=' + SelectMember.Id + ' class="input-group-btn"><a class="dropdown-item" href="#!">.</a><div class="dropdown-item text-danger" id=REM_' + SelectMember.Id + ' onclick="EnterComments();" userid=' + SelectMember.Id + '>Remove Member</div></span></div></li>');
    MainPageelement.prepend('<li class="nav-item dropdown"  id="li_' + SelectMember.Id + '"><a class="nav-link p-0 dropdown-toggle dropdown-caret-none" href="#!" data-toggle="dropdown"><div class="avatar avatar-xl"><img class="rounded-circle" src="' + SelectMember.ProfileImage + '" alt="" /></div></a><div class="dropdown-menu dropdown-md px-0 py-3"><div class="media align-items-center px-3"><div class="avatar avatar-2xl mr-2"><img class="rounded-circle" src="' + SelectMember.ProfileImage + '" alt="" /></div></div><hr class="my-2" /><span onclick="EnterComments(event);" userid=' + SelectMember.Id + ' class="input-group-btn"><a class="dropdown-item" href="#!">.</a><div class="dropdown-item text-danger" id=REM_' + SelectMember.Id + ' onclick="EnterComments();" userid=' + SelectMember.Id + '></div></span></div></li>');
    addNewAssigneeToKanbanTask(SelectMember.Id);
}

function addTagOnView(TagNames) {  
    $('#divTag ul').empty();
    $('#divTagView ul').empty();
    if (TagNames != null) {

        var tagName = TagNames.split(',');
        if (tagName.length > 0) {
            tagName = tagName[0];
            switch (tagName) {
                case 'Urgent':
                    addTag('badge-soft-success bg-red', 'Urgent', true);
                    break;
                case 'High':
                    addTag('badge-soft-primary bg-orange', 'High', true);
                    break;
                case 'Medium':
                    addTag('badge-soft-info bg-light-blue', 'Medium', true);
                    break;
                case 'Low':
                    addTag('badge-soft-danger bg-green', 'Low', true);
                    break;
                default:
            }
        }
    }
}
function addTag(className, TagName, Isview = false) {
    var TaskId = gCurrentViewTaskId;
    var tags = $('#divTag span');
    $('#divTagView ul').empty();
    $('#divTag ul').empty();
    var IsNotDuplicate = false
    //$.each(tags, function (key, value) {
    //    if (value != undefined && value.innerText == TagName) {
    //        IsNotDuplicate = true;
    //    }
    //});
    //if (IsNotDuplicate == false) {
        //$('#divTag').prepend('<span class="badge mr-1 py-2 ' + className + '" data-toggle="dropdown" aria-expanded="true">' + TagName + '</span> <div class="dropdown-menu dropdown-md px-0 py-3 show" style="position: absolute; transform: translate3d(0px, 28px, 0px); top: 0px; left: 0px; will-change: transform;" x-placement="bottom-start"><span class="input-group-btn"><a class="dropdown-item" href="#!"></a><div class="dropdown-item text-danger" onclick="EnterComments();">Remove Member</div></span></div>');
    
   // }
    if (TaskId != null && TaskId !== "" && Isview == false) {
        $('#divTagView').prepend('<ul class="nav avatar-group mb-0"><li class="nav-item dropdown"><a aria-expanded="false" data-bs-toggle="dropdown" role="button" href="#" class="nav-link p-0 dropdown-toggle dropdown-caret-none ms-n1"><span class="badge mr-1 py-2 ' + className + '" data-toggle="dropdown" aria-expanded="true" onclick="taggaleRemoveOption()">' + TagName + '</span></a><div class="dropdown-menu dropdown-md px-0 py-3" style="" id="divShowRemove"><a class="dropdown-item text-danger" href="#!" onclick="RemoveTags();">Remove Tag</a></div></li></ul>');
        UpdateLableForKanbanTask(TaskId, TagName);
        $('#' + TaskId + ' .card-body .badge').empty();
        $('#' + TaskId + ' .card-body').prepend('<div class="mb-2"><span class="badge py-2 me-1 mb-1 ' + className + '">' + TagName + '</span></div>')
    } else if (TaskId != null && TaskId !== "" && Isview == true) {

        $('#divTagView').prepend('<ul class="nav avatar-group mb-0"><li class="nav-item dropdown"><a aria-expanded="false" data-bs-toggle="dropdown" role="button" href="#" class="nav-link p-0 dropdown-toggle dropdown-caret-none ms-n1"><span class="badge mr-1 py-2 ' + className + '" data-toggle="dropdown" aria-expanded="true" onclick="taggaleRemoveOption()">' + TagName + '</span></a><div class="dropdown-menu dropdown-md px-0 py-3" style="" id="divShowRemove"><a class="dropdown-item text-danger" href="#!" onclick="RemoveTags();">Remove Tag</a></div></li></ul>');
    }
    else {

        $('#divTag').prepend('<ul class="nav avatar-group mb-0"><li class="nav-item dropdown"><a aria-expanded="false" data-bs-toggle="dropdown" role="button" href="#" class="nav-link p-0 dropdown-toggle dropdown-caret-none ms-n1"><span class="badge mr-1 py-2 ' + className + '" data-toggle="dropdown" aria-expanded="true" onclick="taggaleRemoveOption()">' + TagName + '</span></a><div class="dropdown-menu dropdown-md px-0 py-3" style="" id="divShowRemove"><a class="dropdown-item text-danger" href="#!" onclick="RemoveTags();">Remove Tag</a></div></li></ul>');
    }
    

}
function taggaleRemoveOption() {
   
    $('#divShowRemove').show();
    var TaskId = gCurrentViewTaskId;
    if (TaskId != null && TaskId !== "" ) {
        UpdateLableForKanbanTask(TaskId, '');
        $('#' + TaskId + ' .card-body .badge').empty();
    }
    
}
function RemoveTags() {
    $('#divTag ul').empty();
    $('#divTagView ul').empty();
}

function ShowAlertBox(title, text, type) {
    sweetAlert
        ({
            title: title, //"Exist!",
            text: text,//"This User Role is Exist !",
            type: type //"warning"
        },
            function () {
                //window.location.href = '/Role/Role'
            });
}


function openModal(filepath, IsShow) {


    var extension = filepath.substr((filepath.lastIndexOf('.') + 1));
    switch (extension.toLowerCase()) {
        case 'jpg':
        case 'jpeg':
        case 'png':
        case 'gif':
        case 'jfif':
            if (IsShow != undefined && IsShow == true) {
                $("#myModal").css("display", "block");
                $(".mySlides").css("display", "block");
                $("#imgPreview").attr("src", filepath);
                setTimeout(function () { $('.fancybox-button--close').click(); }, 100);
            }
            break;
        case 'zip':
        case 'rar':
        case 'pdf':
        case 'txt':
        case 'xls':
        case 'xlsx':
        case 'csv':
        case 'doc':
        case 'docx':

            window.open(filepath, '_blank');
            setTimeout(function () { $('.fancybox-button--close').click(); }, 500);

            break;
        default:
            break;
    }




}

function closeModal() {
    $("#myModal").css("display", "none");
    $(".mySlides").css("display", "none");
    $("#imgPreview").attr("src", "");
    //document.getElementById("myModal").style.display = "none";
}