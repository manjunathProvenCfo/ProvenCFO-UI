using log4net;
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
    public class NotesController : Controller
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
                    int AgencyID = 0;
                    string CreatedBy = string.Empty;
                    var LoginUserid = Session["UserId"].ToString();
                    NotesDescriptionModel NewNotesDescription = new NotesDescriptionModel();
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }

                    NewNotesDescription.Title = Notes.Title;
                    NewNotesDescription.Description = Notes.Description;
                    NewNotesDescription.NoteCatId = Notes.NoteCatId;
                    NewNotesDescription.IsPublished = Notes.IsPublished;
                    //NewNotesDescription.Position = Notes.Position;
                    NewNotesDescription.Status = Notes.Status;
                    NewNotesDescription.CreatedBy = LoginUserid;
                    NewNotesDescription.IsDeleted = false;
                    using (NotesService objNotes = new NotesService())
                    {
                        var result = objNotes.CreateNewNotes(Notes, LoginUserid);
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

    }
}

