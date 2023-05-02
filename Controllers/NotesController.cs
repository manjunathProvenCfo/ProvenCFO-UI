﻿using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    [Exception_Filters]
    public class NotesController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Notes
        [CheckSession]
        public ActionResult GetNotesPage()
        {
            try
            {
                List<UserSecurityVM> _roleList = (List<UserSecurityVM>)Session["LoggedInUserUserSecurityModels"];
                if (_roleList.Where(x => x.FeatureCode != "NTS").ToList().Count == _roleList.Count())
                {
                    return RedirectToAction("AgencyHome", "AgencyService");
                }
                using (NotesService objNotes = new NotesService())
                {
                    using (ClientService objClient = new ClientService())
                    {
                        int AgencyID = 0;

                        ViewBag.IsEditMode = false;
                        ViewBag.IsDraggable = false;
                        var userType = Convert.ToString(Session["UserType"]);
                        var user = Convert.ToString(Session["UserType"]);
                        List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                        if (UserPref != null && UserPref.Count() > 0)
                        {
                            var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                            AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                        }
                        var Categories = objNotes.GetAllNotesCategories("Active", AgencyID).ResultData;
                        TempData["CategoriesAndNotes"] = Categories;
                        var Summary = objClient.GetClientById(AgencyID);
                        TempData["NotesSummary"] = Summary;
                        if (Summary.Summaryid_ref == null)
                        {
                            Summary.Summaryid_ref = 1;
                        }
                        var NoteSummaryData = objNotes.GetNotesStatus();
                        TempData["NotesSummarydata"] = NoteSummaryData.ResultData;
                        var NotesLabels = objNotes.GetAllNotesLabels(true);
                        TempData["NotesLabels"] = NotesLabels.ResultData;
                        ViewBag.selectSummaryStatusText = NoteSummaryData.ResultData.Where(x => x.Id == Summary.Summaryid_ref).FirstOrDefault().SummaryData;

                        if (userType != "" && userType == "1")
                        {
                            ViewBag.IsEditMode = true;
                        }
                        if (user == "1")
                        {
                            ViewBag.IsDraggable = true;
                        }
                    }
                    return View();
                }

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        public ActionResult OpenDescription()
        {
            try
            {
                NotesDescriptionModel note = new NotesDescriptionModel();
                note.ModifiedBy = Session["UserFullName"].ToString();
                //note.TaskType = "Task";

                return PartialView("OpenDescription", note);
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult OpenExistingDescription(int NotesDescriptionId)
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    var result = objNotes.GetNotesDescriptionById(NotesDescriptionId);
                    TempData["SelectedTitle"] = result.Title;
                    TempData["SelectedDescription"] = result.Description;
                    TempData["Selectedtag"] = result.Labels;
                    Session["SelectedDescriptionId"] = result.Id;
                    //Session["Selectedtag"] =  result.Labels;
                    return Json(new { Description = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        [HttpGet]
        public ActionResult UpdateNoteSummary()
        {
            try
            {
                ClientModel result = new ClientModel();
                result.SummaryCreatedBy = User.UserFullName;
                return PartialView("UpdateNoteSummary", result);
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }
        //[CheckSession]
        //[HttpGet]
        //public ActionResult UpdateClientRefId()
        //{
        //    try
        //    {
        //        ClientModel result = new ClientModel();

        //        return PartialView("UpdateClientRefId", result);
        //    }
        //    catch (Exception ex)
        //    {
        //         log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
        //        throw ex;
        //    }
        //}
        [HttpGet]
        public JsonResult UpdateClientRefId(int id, int clientId)
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    ClientModel clientModel = new ClientModel();
                    NotesSummaryVM note = new NotesSummaryVM();
                    clientModel.Summaryid_ref = id;

                    clientModel.Id = clientId;

                    var result = objNotes.UpdateClientRefId(clientModel);


                    return Json(new { Description = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }




        [CheckSession]
        [HttpGet]
        public ActionResult CreateNewNotes()
        {
            try
            {
                NotesDescriptionModel result = new NotesDescriptionModel();
                result.CreatedBy = Session["UserFullName"].ToString();
                return PartialView("CreateNewNotes", result);
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }


        [CheckSession]
        [HttpPost]
        public JsonResult CreateNewNotes(NotesDescriptionModel Notes)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    string html = Notes.Description;/*"<p>This is ,1st text</p>\n<p><img src=\"data:image/png;base64,xYz=\" alt=\"\" /></p><p>This is 2nd, text</p>\n<p><img src=\"data:image/png;base64,lMn=\" alt=\"\" /></p>";*/

                    string[] parts = html.Split(new string[] { "<img src=\"data:image/png;base64,", "\" alt=\"\" /></p>" }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < parts.Length; i++)
                    {
                        string base64Data = parts[i];

                        string pattern = "^[a-zA-Z0-9+/]*={0,2}$";
                        Regex regex = new Regex(pattern);
                        bool isBase64 = regex.IsMatch(base64Data);

                        if (isBase64)
                        {
                            byte[] bytes = Convert.FromBase64String(base64Data);

                            string filename = Guid.NewGuid().ToString() + "-" + "NotesImages" + ".png";
                            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);


                            string path = Server.MapPath("~/Upload/Profile/");

                            string PathfileName = Path.Combine(path, filename);

                            using (FileStream file = new FileStream(PathfileName, FileMode.Create))
                            {
                                file.Write(bytes, 0, bytes.Length);
                            }

                            string newSrc = "../Upload/Profile/" + filename;
                            html = html.Replace($"data:image/png;base64,{base64Data}", newSrc);
                        }
                    }


                    var LoginUserid = Session["UserId"].ToString();

                    if (Notes.Id != null)
                    {
                        Notes.ModifiedBy = LoginUserid;
                        Notes.ModifiedDate = DateTime.Now;
                    }
                    else
                    {
                        Notes.CreatedBy = LoginUserid;
                        Notes.IsDeleted = false;
                    }

                    using (NotesService objNotes = new NotesService())
                    {

                        Notes.Description = html;
                        var result = objNotes.CreateNewNotes(Notes);
                        if (result.Status == true)
                        {
                            ViewBag.ErrorMessage = "Created";
                            return Json(new { id = Notes.Id, Status = ViewBag.ErrorMessage, Message = result.Message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Label is required!";
                            return Json(new { id = Notes.Id, Status = ViewBag.ErrorMessage, Message = result.Message }, JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Label is required!";
                    return Json(new { id = Notes.Id, Status = ViewBag.ErrorMessage, Message = "Notes title has a required field and can't be empty." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult UpdateNotesDescription(NotesDescriptionModel Notes)
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    var LoginUserid = Session["UserId"].ToString();
                    var result = objNotes.UpdateNotesDescription(Notes.Id.Value, Notes.Title, Notes.Description, Notes.IsPublished, LoginUserid, Notes.Labels).resultData;
                    if (result == true)
                    {
                        return Json(new { Notes = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Notes = result, Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }
        [CheckSession]
        [HttpPost]
        public JsonResult UpdateNoteSummary(int Id, string Summary)
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    ClientModel client = new ClientModel();
                    client.Id = Id;
                    client.Summary = Summary;
                    client.SummaryCreatedBy = User.UserId;
                    client.SummaryCreatedDate = DateTime.Now;

                    var clientResult = objNotes.UpdateNoteSummary(client);
                    if (client != null)
                    {
                        return Json(new { Data = clientResult, Message = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }




        [CheckSession]
        public ActionResult PublishingNotes(int Id)
        {
            try
            {
                using (NotesService obj = new NotesService())
                {
                    var result = obj.PublishingNotes(Id);
                    if (result == null)
                        ViewBag.ErrorMessage = "Can not be  published";
                    return RedirectToAction("GetNotesPage");
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                ViewBag.ErrorMessage = "";
                return View();
            }
        }

        [CheckSession]
        public string DeleteNotesDescription(int Id)
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    var results = objNotes.GetNotesDescriptionById(Id);
                    if (results.IsPublished == "Published" || results.IsPublished == "Unpublished")
                    {
                        var result = objNotes.DeleteNotesDescription(Id);
                        return result.IsPublished;
                    }
                    return results.IsPublished;
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        public string ResolveNote(int Id)
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    var results = objNotes.GetNotesDescriptionById(Id);
                    if (results.IsResolved == false)
                    {
                        var resolve = objNotes.ResolveNote(Id);
                        return resolve.IsPublished;
                    }

                    return results.IsPublished;
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }


        [CheckSession]
        [HttpPost]
        public JsonResult DragAndDropNotesDescription(int[] Ids, int[] Positions)
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    var result = objNotes.DragAndDropNotesDescription(Ids, Positions);

                    if (result != null)
                    {
                        return Json(new { notes = result, message = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { notes = result, message = "error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
            //return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
        }

        //[CheckSession]
        //public JsonResult TotalNotesCountByAgencyId(string AgencyId)
        //{
        //    try
        //    {

        //        using (NotesService objNotes = new NotesService())
        //        {
        //            var objResult = objNotes.TotalNotesCountByAgencyId(Convert.ToString(AgencyId));
        //            return Json(objResult, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //         log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
        //        throw ex;
        //    }
        //}


        //[CheckSession]
        //[HttpGet]
        //public JsonResult NotesIndividualCountAndPercentageByAgencyId(string AgencyId)
        //{
        //    try
        //    {
        //        using (NotesService objNotes = new NotesService())
        //        {
        //            var objResult = objNotes.NotesIndividualCountAndPercentageByAgencyId(Convert.ToString(AgencyId));
        //            return Json(objResult, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //         log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
        //        throw ex;
        //    }
        //}
        public JsonResult EmailSend(string ClientName, string ClientId, string url, string totalNotes, string sentdate)
        {
            try
            {
                using (AccountService obj = new AccountService())
                {
                    List<InviteUserModel> user = new List<InviteUserModel>();
                    var usersListwithRecPermission = obj.GetRegisteredUsersByAgencyWithReqPermission(ClientId, "NTS");
                    var Userslist = usersListwithRecPermission.ResultData;
                    var Recipientssdata = Userslist.Where(x => x.IsRegistered == 1 && x.IsActive == 1.ToString()).Select(x => x.Email);
                    
                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath("~/assets/files/NotesEmailTemplate.xml"));

                    string xml = System.IO.File.ReadAllText(Server.MapPath("~/assets/files/NotesEmailTemplate.xml"));

                    var subject = doc.SelectNodes("EmailContent/subject")[0].InnerText;
                    var body = doc.SelectNodes("EmailContent/body")[0].InnerText;
                    var footer = doc.SelectNodes("EmailContent/footer")[0].InnerText;

                    subject = subject.Replace("{CompanyName}", ClientName);
                    subject = subject.Replace("{TodaysDate}", DateTime.Now.ToString("dd MMMM, yyyy", new System.Globalization.CultureInfo("en-US")));

                    body = body.Replace("{totalNotes}", totalNotes);
                    var notes = "https://" + url + "/Notes/GetNotesPage";
                    var dashboard = "https://" + url + "/Dashboard/Dashboard";
                    var report = "https://" + url + "/Reports/ReportsList";
                    var login = "https://" + url + "/Home/Login";
                    var reconcilation = "https://" + url + "/Reconciliation/ReconciliationMain";
                    var chat = "https://" + url + "/Communication/Chat";
                    body = body.Replace("{notes}", notes);
                    body = body.Replace("{dashboard}", dashboard);
                    body = body.Replace("{report}", report);
                    body = body.Replace("{login}", login);
                    body = body.Replace("{reconcilation}", reconcilation);
                    body = body.Replace("{chat}", chat);

                    //footer = footer.Replace("{LastSent}", sentdate);
                    
                    footer = sentdate != "null" ? footer.Replace("{LastSent}", sentdate) : "";

                    return Json(new { Subject = subject, Body = body, Recipients = Recipientssdata, Status = "Success", LastSent = footer }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {

                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}

