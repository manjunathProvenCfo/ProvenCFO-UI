﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class NeedsService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;



        public KanbanTasksModel GetKanbanTaskByTid(int KanbanTaskid)
        {
            return GetAsync<KanbanTasksModel>("Needs/GetKanbanTaskByTid?KanbanTaskid=" + KanbanTaskid, true).Result;
            //response = client.GetAsync("Needs/GetKanbanTaskByTid?KanbanTaskid=" + KanbanTaskid).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    //var val = JsonConvert.DeserializeObject<RolesViewModel>((JObject.Parse(_content)["resultData"]).ToString());
            //    return JsonConvert.DeserializeObject<KanbanTasksModel>((JObject.Parse(_content)["resultData"]).ToString());
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public KanbanTasksMainModel GetAllKanbanTask(string Status)
        {
            return GetAsync<KanbanTasksMainModel>("Needs/GetAllKanbanTask?Status=" + Status).Result;
            //response = client.GetAsync("Needs/GetAllKanbanTask?Status=" + Status).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<KanbanTasksMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public TeamUserAssociationVMMainModel getTeamMembersList(string ClientID)
        {
            return GetAsync<TeamUserAssociationVMMainModel>("Teams/getTeamMembersList?ClientID=" + ClientID).Result;
            //response = client.GetAsync("Teams/getTeamMembersList?ClientID=" + ClientID).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<TeamUserAssociationVMMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public UserMainModel getAgencyMembersList(string ClientID)
        {
            return GetAsync<UserMainModel>("Teams/getAgencyMembersList?ClientID=" + ClientID).Result;
            //response = client.GetAsync("Teams/getAgencyMembersList?ClientID=" + ClientID).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<UserMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public kanbanSegmentTypesMainModel GetAllSegments(string Status, int AgencyID)
        {
            return GetAsync<kanbanSegmentTypesMainModel>("Needs/GetAllSegments?Status=" + Status + "&AgencyID=" + AgencyID).Result;
            //response = client.GetAsync("Needs/GetAllSegments?Status=" + Status + "&AgencyID=" + AgencyID).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<kanbanSegmentTypesMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }

        public KanbanTasksMainModel CreateNewTask(KanbanTasksModel Task, string LoginUserID)
        {
            // var form = new Dictionary<string, dynamic>
            //{
            //     {"TaskTitle", Task.TaskTitle},
            //     {"TaskDescription", Task.TaskDescription},
            //     {"SegmentTypeID_Ref", Convert.ToString(Task.SegmentTypeID_Ref)},
            //     {"StartDate", Convert.ToString(Task.StartDate)},
            //     {"EndDate", Convert.ToString(Task.EndDate)},
            //     {"DueDate", Convert.ToString(Task.DueDate)},
            //     {"Priority", Task.Priority},
            //     {"Reporter", Task.Reporter},
            //     {"Assignee", Task.Assignee},
            //     {"AgencyId_Ref",  Convert.ToString(Task.AgencyId_Ref)},
            //     {"Labels", Task.Labels},
            //     {"Status", Task.Status},
            //     {"CreatedBy", LoginUserID},
            //     {"IsDeleted", "false"},
            //     {"EstimatedHours", Convert.ToString(Task.EstimatedHours)},
            //     {"KanbanAttachments", Task.KanbanAttachments}
            //};

            KanbanTasksVM kbvm = new KanbanTasksVM();
            kbvm.TaskTitle = Task.TaskTitle;
            kbvm.TaskDescription = Task.TaskDescription;
            kbvm.SegmentTypeID_Ref = Task.SegmentTypeID_Ref;
            kbvm.StartDate = Task.StartDate;
            kbvm.EndDate = Task.EndDate;
            kbvm.DueDate = Task.DueDate;
            kbvm.Priority = Task.Priority;
            kbvm.Reporter = Task.Reporter;
            kbvm.Assignee = Task.Assignee;
            kbvm.AgencyId_Ref = Task.AgencyId_Ref;
            kbvm.Labels = Task.Labels;
            kbvm.CreatedBy = LoginUserID;
            kbvm.IsDeleted = false;
            kbvm.EstimatedHours = Task.EstimatedHours;
            kbvm.KanbanAttachments = Task.KanbanAttachments;
            //content = new StringContent(JsonConvert.SerializeObject(kbvm), Encoding.UTF8, "application/json");
            return PostAsync<KanbanTasksMainModel, KanbanTasksVM>("Needs/CreateKanbanTask", kbvm).Result;
            //response = client.PostAsync("Needs/CreateKanbanTask", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<KanbanTasksMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public ReturnModel addNewAssigneeToKanbanTask(KanbanTaskAssigneeAsssociationModel asssignee)
        {
            //content = new StringContent(JsonConvert.SerializeObject(asssignee), Encoding.UTF8, "application/json");
            return PostAsync<ReturnModel, KanbanTaskAssigneeAsssociationModel>("Needs/addNewAssigneeToKanbanTask?SingleMember=" + asssignee.SingleMember, asssignee).Result;
            //response = client.PostAsync("Needs/addNewAssigneeToKanbanTask?SingleMember=" + asssignee.SingleMember, content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ReturnModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public KanbanTaskCommentsVMMainModel AddComments(KanbanTaskCommentsModel comments)
        {
            //content = new StringContent(JsonConvert.SerializeObject(comments), Encoding.UTF8, "application/json");
            return PostAsync<KanbanTaskCommentsVMMainModel, KanbanTaskCommentsModel>("Needs/AddComments", comments).Result;
            //response = client.PostAsync("Needs/AddComments", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<KanbanTaskCommentsVMMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public KanbanTaskCommentsListMainModel GetCommentsByTaskId(int TaskId)
        {
            return GetAsync<KanbanTaskCommentsListMainModel>("Needs/GetCommentsByTaskId?TaskId=" + TaskId).Result;
            //response = client.GetAsync("Needs/GetCommentsByTaskId?TaskId=" + TaskId).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<KanbanTaskCommentsListMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }

        public ReturnModel RemoveAssigneeFromKanbanTask(int TaskId, string UserId)
        {
            string result = string.Format("Needs/RemoveAssigneeFromKanbanTask?TaskId={0}&UserId={1}", TaskId, UserId);
            return PostAsync<ReturnModel>(result).Result;
            //response = client.PostAsync(result, null).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ReturnModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public KanbanAttachmentsMainModel RemoveAttachmentFromKanbanTask(int TaskId, string FileName, bool IsTaskAttachment)
        {
            string result = string.Format("Needs/RemoveAttachmentFromKanbanTask?TaskId={0}&FileName={1}&IsTaskAttachment={2}", TaskId, FileName, IsTaskAttachment);
            return PostAsync<KanbanAttachmentsMainModel>(result, null).Result;
            //response = client.PostAsync(result, null).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<KanbanAttachmentsMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public ReturnModel UpdateTaskSegmentType(string TaskID, string SegmentType)
        {

            string result = string.Format("Needs/UpdateTaskSegmentType?TaskID={0}&SegmentType={1}", TaskID, SegmentType);
            return PostAsync<ReturnModel>(result).Result;
            //response = client.PostAsync(result, null).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ReturnModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public ReturnModel UpdateTaskDescription(int TaskID, string DescriptionText, string LoginUserID)
        {
            KanbanTasksVM kbvm = new KanbanTasksVM();
            kbvm.Id = TaskID;
            kbvm.TaskDescription = DescriptionText;
            kbvm.CreatedBy = LoginUserID;


            //content = new StringContent(JsonConvert.SerializeObject(kbvm), Encoding.UTF8, "application/json");
            return PostAsync<ReturnModel, KanbanTasksVM>("Needs/UpdateTaskDescription", kbvm).Result;
            //response = client.PostAsync("Needs/UpdateTaskDescription", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ReturnModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public ReturnModel UpdateLableForKanbanTask(int TaskID, string DescriptionText, string LoginUserID)
        {
            KanbanTasksVM kbvm = new KanbanTasksVM();
            kbvm.Id = TaskID;
            kbvm.Labels = DescriptionText;
            kbvm.ModifiedBy = LoginUserID;


            //content = new StringContent(JsonConvert.SerializeObject(kbvm), Encoding.UTF8, "application/json");
            return PostAsync<ReturnModel, KanbanTasksVM>("Needs/UpdateLableForKanbanTask", kbvm).Result;
            //response = client.PostAsync("Needs/UpdateLableForKanbanTask", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ReturnModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public KanbanAttachmentsListMainModel AddAttachmentTaskTask(List<KanbanAttachmentsVM> KanbanAttachments)
        {

            // var form = new Dictionary<string, string>
            //{
            //     {"TaskTitle", Task.TaskTitle},
            //     {"TaskDescription", Task.TaskDescription},
            //     {"SegmentTypeID_Ref", Convert.ToString(Task.SegmentTypeID_Ref)},
            //     {"StartDate", Convert.ToString(Task.StartDate)},
            //     {"EndDate", Convert.ToString(Task.EndDate)},
            //     {"DueDate", Convert.ToString(Task.DueDate)},
            //     {"Priority", Task.Priority},
            //     {"Reporter", Task.Reporter},
            //     {"Assignee", Task.Assignee},
            //     {"AgencyId_Ref",  Convert.ToString(Task.AgencyId_Ref)},
            //     {"Labels", Task.Labels},
            //     {"Status", Task.Status},
            //     {"CreatedBy", LoginUserID},
            //     {"IsDeleted", "false"},
            //     {"EstimatedHours", Convert.ToString(Task.EstimatedHours)}
            //};
            //content = new StringContent(JsonConvert.SerializeObject(KanbanAttachments), Encoding.UTF8, "application/json");
            return PostAsync<KanbanAttachmentsListMainModel, List<KanbanAttachmentsVM>>("Needs/AddAttachmentTaskTask", KanbanAttachments).Result;
            //response = client.PostAsync("Needs/AddAttachmentTaskTask", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<KanbanAttachmentsListMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}

        }

        //public KanbanCountWithIndividualPriorityMainModel KanbanCountWithIndividualPriority(string AgencyId)
        //{
        //    return GetAsync<KanbanCountWithIndividualPriorityMainModel>("Needs/KanbanCountWithIndividualPriority?AgencyId=" + AgencyId).Result;
        //}
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                if (response != null)
                    response.Dispose();
                if (content != null)
                {
                    content.Dispose();
                }
                // free managed resources               
            }
            isDisposed = true;
        }
        public KanbanTasksVM DeleteNeedsCard(int TaskId)
        {
            string result = string.Format("Needs/DeleteNeedsCard?TaskId={0}", TaskId);
            return PostAsync<KanbanTasksVM>(result).Result;
        }
    }
}
