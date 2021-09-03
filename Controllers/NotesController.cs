﻿using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class NotesController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Notes
        [CheckSession]
        public ActionResult GetNotesPage()
        {
            try
            {
                using (NotesService objNotes = new NotesService())
                {
                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    var Categories = objNotes.GetAllNotesCategories("Active", AgencyID).ResultData;
                    TempData["CategoriesAndNotes"] = Categories;
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                    Session["SelectedDescriptionId"] = result.Id;
                    return Json(new { Description = result, Message = "Success" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                    //int AgencyID = User.AgencyID;//LoggedInUserPreferenced Agency Id
                    //string CreatedBy = string.Empty;
                    var LoginUserid = Session["UserId"].ToString();
                    //NotesDescriptionModel NewNotesDescription = new NotesDescriptionModel();
                    //List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    //if (UserPref != null && UserPref.Count() > 0)
                    //{
                    //    var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    //    AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    //}
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
                    //Notes.IsPublished = "Published";
                    //NewNotesDescription.Title = Notes.Title;
                    //NewNotesDescription.Description = Notes.Description;
                    //NewNotesDescription.NoteCatId = Notes.NoteCatId;
                    //NewNotesDescription.IsPublished = Notes.IsPublished;
                    ////NewNotesDescription.Position = Notes.Position;
                    //NewNotesDescription.Status = Notes.Status;
                    //NewNotesDescription.CreatedBy = LoginUserid;
                    //NewNotesDescription.IsDeleted = false;
                    using (NotesService objNotes = new NotesService())
                    {
                        var result = objNotes.CreateNewNotes(Notes);
                        if (result.Status == true)
                        {
                            ViewBag.ErrorMessage = "Created";
                            return Json(new { id = Notes.Id, Status = ViewBag.ErrorMessage, Message = result.Message }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Exist";
                            return Json(new { id = Notes.Id, Status = ViewBag.ErrorMessage, Message = result.Message }, JsonRequestBehavior.AllowGet);
                        }
                    
                    }
                }
                else
                {
                    ViewBag.ErrorMessage = "Exist";
                    return Json(new { id = Notes.Id, Status = ViewBag.ErrorMessage, Message = "Notes title has a required field and can't be empty." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                    var result = objNotes.UpdateNotesDescription(Notes.Id.Value, Notes.Title, Notes.Description, Notes.IsPublished,  LoginUserid).resultData;
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

    }
}

