using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class NeedsController : Controller
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Needs
        [CheckSession]
        public ActionResult NeedsMain()
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    var SegmentTasks = objNeeds.GetAllSegments("Active", AgencyID).ResultData;
                    TempData["SegmentsAndTasks"] = SegmentTasks;
                }
                return View();
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        [CheckSession]
        public ActionResult CreateNewTask()
        {
            try
            {
                KanbanTasksViewModel task = new KanbanTasksViewModel();
                task.ReporterName = Session["UserFullName"].ToString();
                task.TaskType = "Task";
                task.Assignee = "Automatic";
                task.Priority = "Medium";
                return PartialView("_CreateNewTask", task);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        [HttpPost]
        public JsonResult CreatNewTask(KanbanTasksViewModel Task)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    int AgencyID = 0;
                    string Assignee = string.Empty;
                    var LoginUserid = Session["UserId"].ToString();
                    KanbanTasksModel NewTasks = new KanbanTasksModel();
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }



                    NewTasks.TaskTitle = Task.TaskTitle;
                    NewTasks.TaskDescription = Task.TaskDescription;
                    NewTasks.SegmentTypeID_Ref = 1;
                    if (Task.dpStartDate != null && Task.dpStartDate != "")
                    {
                        NewTasks.StartDate = Convert.ToDateTime(DateTime.ParseExact(Task.dpStartDate, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (Task.dpEndDate != null && Task.dpEndDate != "")
                    {
                        NewTasks.EndDate = Convert.ToDateTime(DateTime.ParseExact(Task.dpEndDate, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                    }
                    if (Task.dpDueDate != null && Task.dpDueDate != "")
                    {
                        NewTasks.DueDate = Convert.ToDateTime(DateTime.ParseExact(Task.dpDueDate, "MM/dd/yyyy", CultureInfo.InvariantCulture));
                    }
                    NewTasks.Priority = Task.Priority;
                    NewTasks.Reporter = LoginUserid;
                    NewTasks.Assignee = Task.Assignee;
                    NewTasks.AgencyId_Ref = AgencyID;
                    NewTasks.Labels = Task.Labels;
                    NewTasks.Status = "Active";
                    NewTasks.CreatedBy = LoginUserid;
                    NewTasks.IsDeleted = false;
                    NewTasks.EstimatedHours = Task.EstimatedHours;
                    NewTasks.TaskType = Task.TaskType;
                    var _AddedFiles = TempData["FileAttaching"] as List<KanbanAttachmentsVM>;
                    NewTasks.KanbanAttachments = _AddedFiles;
                    using (NeedsService objNeeds = new NeedsService())
                    {
                        var result = objNeeds.CreateNewTask(NewTasks, LoginUserid);
                        if (result.Status == true)
                        {
                            if (result.ResultData != null && result.ResultData.Count > 0)
                            {
                                var _AddedFilesStream = TempData["FilesStream"] as List<KanbanAttachmentsStream>;
                                //var TaskId = result.ResultData[0].Id;
                                //foreach (var item in _AddedFiles)
                                //{
                                //    var filestream = _AddedFilesStream.Where(x => x.AttachedFileName == item.AttachedFileName).FirstOrDefault();
                                //    if (filestream != null)
                                //    { 
                                //        item.FilePath = item.FilePath + @"\" + TaskId + @"\" + item.AttachedFileName;

                                //    }

                                //}

                            }
                            ViewBag.ErrorMessage = "Created";
                            return Json(new { id = Task.Id, Status = ViewBag.ErrorMessage, Message = result.Message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Exist";
                            return Json(new { id = Task.Id, Status = ViewBag.ErrorMessage, Message = result.Message }, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Exist";
                    return Json(new { id = Task.Id, Status = ViewBag.ErrorMessage, Message = "Task title has a required field and can't be empty." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public ActionResult OpenTask()
        {
            try
            {
                KanbanTasksViewModel task = new KanbanTasksViewModel();
                task.ReporterName = Session["UserFullName"].ToString();
                task.TaskType = "Task";
                task.Assignee = "Automatic";
                task.Priority = "Medium";
                return PartialView("_OpenTask", task);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult OpenExistingTask(int TaskID)
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    var result = objNeeds.GetKanbanTaskByTid(TaskID);


                    foreach (var item in result.KanbanAssigneesList)
                    {
                        if (item.ProfileImage != null && item.ProfileImage != "")
                        {
                            var absolutePath = HttpContext.Server.MapPath(item.ProfileImage);
                            if (!System.IO.File.Exists(absolutePath))
                            {
                                item.ProfileImage = "../assets/img/team/avatar.png";
                            }
                        }
                        else
                        {
                            item.ProfileImage = "../assets/img/team/avatar.png";
                        }
                    }
                    TempData["SelectedTasks"] = result;
                    Session["SelectedTaskId"] = result.Id;
                    return Json(new { Task = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult UpdateTaskSegmentType(string TaskID, string SegmentType)
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    var result = objNeeds.UpdateTaskSegmentType(TaskID, SegmentType).resultData;
                    if (result == true)
                    {
                        return Json(new { Task = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Task = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        [CheckSession]
        public JsonResult UpdateTaskDescription(KanbanTasksModel Task)
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    var LoginUserid = Session["UserId"].ToString();
                    var result = objNeeds.UpdateTaskDescription(Task.Id.Value, Task.TaskDescription, LoginUserid).resultData;
                    if (result == true)
                    {
                        return Json(new { Task = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Task = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult UpdateLableForKanbanTask(KanbanTasksModel Task)
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    var LoginUserid = Session["UserId"].ToString();
                    var result = objNeeds.UpdateLableForKanbanTask(Task.Id.Value, Task.Labels, LoginUserid).resultData;
                    if (result == true)
                    {
                        return Json(new { Task = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Task = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult addNewAssigneeToKanbanTask(Proven.Model.KanbanTaskAssigneeAsssociationModel Assignee)
        {
            try
            {

                using (NeedsService objNeeds = new NeedsService())
                {
                    var result = objNeeds.addNewAssigneeToKanbanTask(Assignee).resultData;
                    if (result == true)
                    {
                        return Json(new { Task = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Task = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult RemoveAssigneeFromKanbanTask(int TaskId, string UserId)
        {
            try
            {


                using (NeedsService objNeeds = new NeedsService())
                {
                    var result = objNeeds.RemoveAssigneeFromKanbanTask(TaskId, UserId).resultData;
                    if (result == true)
                    {
                        return Json(new { Task = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Task = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        [CheckSession]
        public JsonResult RemoveAttachmentFromKanbanTask(int TaskId, string FileName, bool IsTaskAttachment)
        {
            try
            {


                using (NeedsService objNeeds = new NeedsService())
                {
                    var result = objNeeds.RemoveAttachmentFromKanbanTask(TaskId, FileName, IsTaskAttachment).ResultData;
                    if (result != null)
                    {
                        string Folder_path = Server.MapPath(result.FilePath.Replace("../..", "~"));
                        Common.DeleteFile(Folder_path);
                        return Json(new { Task = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Task = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }


        [CheckSession]
        public JsonResult getTeamMembersList(string ClientId)
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    var result = objNeeds.getTeamMembersList(ClientId).ResultData;
                    if (result != null)
                    {
                        TempData["TeamMembersList"] = result;
                        return Json(new { TeamMembers = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["TeamMembersList"] = new List<TeamUserAssociationVM>();
                        return Json(new { TeamMembers = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }


        [CheckSession]
        public JsonResult getAgencyMembersList(string ClientId)
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    var result = objNeeds.getAgencyMembersList(ClientId).resultData;
                    if (result != null)
                    {
                        foreach (var item in result)
                        {
                            if (item.ProfileImage != null && item.ProfileImage != "")
                            {
                                var absolutePath = HttpContext.Server.MapPath(item.ProfileImage);
                                if (!System.IO.File.Exists(absolutePath))
                                {
                                    item.ProfileImage = "../assets/img/team/avatar.png";
                                }
                            }
                            else
                            {
                                item.ProfileImage = "../assets/img/team/avatar.png";
                            }
                        }
                        TempData["AgencyMembersList"] = result;
                        return Json(new { TeamMembers = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        TempData["AgencyMembersList"] = new List<UserModel>();
                        return Json(new { TeamMembers = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                    //return PartialView("_OpenTask", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }


        [CheckSession]
        [HttpPost]
        public JsonResult AddComments(KanbanTaskCommentsModel Comment)
        {
            try
            {
                using (NeedsService objNeeds = new NeedsService())
                {
                    var LoginUserid = Session["UserId"].ToString();
                    var UserFullName = Session["UserFullName"].ToString();
                    Comment.CreatedBy = LoginUserid;
                    var commentResult = objNeeds.AddComments(Comment).ResultData;
                    if (commentResult != null && commentResult.Id > 0)
                    {
                        ViewBag.ErrorMessage = "Created";
                        return Json(new { data = commentResult, Status = ViewBag.ErrorMessage, Message = "Success", UserFullName = UserFullName }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error";
                        return Json(new { data = "", Status = ViewBag.ErrorMessage, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        
        [HttpPost]
        public JsonResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                KanbanAttachmentsVM newfile = new KanbanAttachmentsVM();
                KanbanAttachmentsStream filestream = new KanbanAttachmentsStream();
                if (file.ContentLength > 0)
                {

                    var LoginUserid = Session["UserId"].ToString();
                    string _FileName = Path.GetFileName(file.FileName);
                    string File_path = Path.Combine(Server.MapPath("~/UploadedFiles/Tasks"), _FileName);
                    string Folder_path = Server.MapPath("~/UploadedFiles/Tasks");
                    FileInfo fi = new FileInfo(_FileName);
                    Guid objGuid = Guid.NewGuid();


                    newfile.AttachedFileName = file.FileName;
                    newfile.FileExtention = fi.Extension;
                    newfile.FileType = newfile.FileExtention.Replace(".", "");
                    newfile.FilePath = "../../UploadedFiles/Tasks/" + objGuid.ToString() + "/" + _FileName;

                    newfile.IsTaskAttachement = true;
                    newfile.IsCommentAttachment = false;
                    newfile.CreatedBy = LoginUserid;
                    filestream.AttachedFileName = file.FileName;
                    filestream.InputStream = file.InputStream;

                    var _AddedFiles = TempData["FileAttaching"] as List<KanbanAttachmentsVM>;
                    //var _AddedFilesStream = TempData["FilesStream"] as List<KanbanAttachmentsStream>;
                    if (_AddedFiles != null && _AddedFiles.Count > 0)
                    {
                        _AddedFiles.Add(newfile);
                        //_AddedFilesStream.Add(filestream);
                        TempData["FileAttaching"] = _AddedFiles;
                        //TempData["FilesStream"] = _AddedFilesStream;
                    }
                    else
                    {
                        List<KanbanAttachmentsVM> attachments = new List<KanbanAttachmentsVM>();
                        attachments.Add(newfile);
                        //_AddedFilesStream.Add(filestream);
                        TempData["FileAttaching"] = attachments;
                        //TempData["FilesStream"] = _AddedFilesStream;
                    }
                    Common.SaveStreamAsFile(Folder_path + "/" + objGuid.ToString(), file.InputStream, _FileName);
                    //Common.SaveStreamAsFile(Folder_path, file.InputStream, _FileName);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                ViewBag.ErrorMessage = "Success";
                return Json(new { File = newfile, Status = ViewBag.ErrorMessage, Message = ViewBag.Message }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                ViewBag.Message = "File upload failed!!";
                ViewBag.ErrorMessage = "Error";
                return Json(new { File = "", Status = ViewBag.ErrorMessage, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UploadFileAndSave(HttpPostedFileBase file, string TaskID)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var LoginUserid = Session["UserId"].ToString();
                    string _FileName = Path.GetFileName(file.FileName);
                    string File_path = Path.Combine(Server.MapPath("~/UploadedFiles/Tasks"), _FileName);
                    string Folder_path = Server.MapPath("~/UploadedFiles/Tasks");
                    FileInfo fi = new FileInfo(_FileName);
                    Guid objGuid = Guid.NewGuid();

                    KanbanAttachmentsVM newfile = new KanbanAttachmentsVM();
                    newfile.AttachedFileName = file.FileName;
                    newfile.FileExtention = fi.Extension;
                    newfile.FileType = newfile.FileExtention.Replace(".", "");
                    newfile.FilePath = "../../UploadedFiles/Tasks/" + objGuid.ToString() + "/" + _FileName;
                    if (Session["SelectedTaskId"] != null)
                    {
                        newfile.TaskId_Ref = Session["SelectedTaskId"] != null && Convert.ToString(Session["SelectedTaskId"]) != "" ? Convert.ToInt32(Session["SelectedTaskId"]) : 0;
                        newfile.IsTaskAttachement = true;
                        newfile.IsCommentAttachment = false;
                        newfile.CreatedBy = LoginUserid;

                        List<KanbanAttachmentsVM> attachments = new List<KanbanAttachmentsVM>();
                        List<KanbanAttachmentsVM> Resultattachments = new List<KanbanAttachmentsVM>();
                        attachments.Add(newfile);
                        using (NeedsService NeedServ = new NeedsService())
                        {
                            Resultattachments = NeedServ.AddAttachmentTaskTask(attachments).ResultData;
                            newfile.Id = Resultattachments != null && Resultattachments.Count > 0 ? Resultattachments[0].Id : 0;
                            Common.SaveStreamAsFile(Folder_path + "/" + objGuid.ToString(), file.InputStream, _FileName);
                            ViewBag.ErrorMessage = "Success";
                        }
                        return Json(new { File = newfile, Status = ViewBag.ErrorMessage, Message = "" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                ViewBag.ErrorMessage = "Error";
                return Json(new { File = "", Status = ViewBag.ErrorMessage, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { File = "", Status = ViewBag.ErrorMessage, Message = "File upload failed!!" }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RemoveFile(string fileName)
        {
            try
            {
                var _AddedFiles = TempData["FileAttaching"] as List<KanbanAttachmentsVM>;
                if (_AddedFiles != null && _AddedFiles.Count > 0)
                {
                    var filetoremove = _AddedFiles.Where(x => x.AttachedFileName == fileName).FirstOrDefault();
                    _AddedFiles.Remove(filetoremove);
                    string Folder_path = Server.MapPath(filetoremove.FilePath.Replace("../..", "~"));
                    Common.DeleteFile(Folder_path);
                    TempData["FileAttaching"] = _AddedFiles;
                    ViewBag.Message = "File remove Successfully!!";
                }
                return Json(new { Message = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                ViewBag.Message = "File remove failed!!";
                return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}