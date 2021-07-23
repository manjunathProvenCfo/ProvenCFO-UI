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
    public class AgencyServiceController : Controller, IDisposable
    {
        string errorMessage = string.Empty;
        string errorDescription = string.Empty;
        // GET: AgencyService
        [CheckSession]
        public ActionResult AgencyHome()
        {

            return View();
        }

        [CheckSession]
        [HttpGet]
        public JsonResult GetClientDetails(int id)
        {
            try
            {
                using (ClientService objClient = new ClientService())
                {
                    Utltity obj = new Utltity();
                    var objResult = objClient.GetClientById(id);
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Message= " + ex.Message.ToString() + ". Method= " + ex.TargetSite.Name.ToString();
                errorDescription = " StackTrace : " + ex.StackTrace.ToString() + " Source = " + ex.Source.ToString();
                Utltity.WriteMsg(errorMessage + " " + errorDescription);
                throw ex;
            }
        }


        [CheckSession]
        [ChildActionOnly]
        public ActionResult AgencySelection()
        {
            AgencyClient objAgy = new AgencyClient();
            using (ClientService objClient = new ClientService())
            {
                List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                if (TempData["ClientListActived"] == null)
                {
                    var objResult = new List<ClientModel>();
                    var LoginUserid = Session["UserId"].ToString();
                    if (Session["UserType"] != null && Session["UserType"].ToString().Trim() == "2")
                    {
                        objResult = objClient.GetClientListForAgecyUser(LoginUserid, true, false).ResultData;
                    }
                    else
                    {
                        objResult = objClient.GetClientListByStatus(true, false).ResultData;
                    }

                    TempData["ClientListActived"] = objResult;
                    objAgy.ClientList = objResult;
                }
                else
                {
                    objAgy.ClientList = (List<ClientModel>)TempData["ClientListActived"];
                }

                if (UserPref != null && UserPref.Count() > 0)
                {
                    var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    objAgy.SelectedClientNameID = Convert.ToInt32(selectedAgency.PreferanceValue);
                }
                else
                {
                    objAgy.SelectedClientNameID = objAgy.ClientList[0].Id;
                }
                return PartialView("_AgencySelection", objAgy);
            }
        }

        [CheckSession]
        [HttpPost]
        public JsonResult SetUserPreferences(int ClientId)
        {
            try
            {
                using (CommonService commSrv = new CommonService())
                {
                    UserPreferencesVM UserPref = new UserPreferencesVM();
                    Utltity obj = new Utltity();
                    var LoginUserid = Session["UserId"].ToString();
                    UserPref.PreferenceCategory = "Agency";
                    UserPref.Sub_Category = "ID";
                    UserPref.PreferanceValue = Convert.ToString(ClientId);
                    UserPref.UserID = LoginUserid;
                    UserPref.UserRole = "";
                    UserPref.CreatedBy = Convert.ToString(ClientId);
                    var objResult = commSrv.SetUserPreferences(UserPref, LoginUserid);
                    var objUserPref = commSrv.GetUserPreferences(LoginUserid);
                    Session["LoggedInUserPreferences"] = objUserPref;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Message= " + ex.Message.ToString() + ". Method= " + ex.TargetSite.Name.ToString();
                errorDescription = " StackTrace : " + ex.StackTrace.ToString() + " Source = " + ex.Source.ToString();
                Utltity.WriteMsg(errorMessage + " " + errorDescription);
                throw ex;
            }

        }
    }
}