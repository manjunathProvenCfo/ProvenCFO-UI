using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class AgencyServiceController : BaseController, IDisposable
    {
        string errorMessage = string.Empty;
        string errorDescription = string.Empty;
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: AgencyService
        [CheckSession]
        public ActionResult AgencyHome()
        {

            return View();
        }
        [CheckSession]
        public ActionResult AgencyXeroHome(string code, string scope, string state)
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
                    var objResultClient = objClient.GetClientById(id);
                    Common.ConnectXeroClient(objResultClient);
                    return Json(objResultClient, JsonRequestBehavior.AllowGet);
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
        public JsonResult XeroSwitchOrganization(int ClientID)
        {
            try
            {
                using (ClientService objClient = new ClientService())
                {
                    Utltity obj = new Utltity();
                    var objResultClient = objClient.GetClientById(ClientID);
                    var url = Common.getXeroLoginUrl(objResultClient);
                    return Json(new { URL = url }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        [CheckSession]
      
        public async Task<JsonResult> GetBankSummary()
        {
            var returnData = new Dictionary<string, dynamic>();
            try
            {
                if (XeroInstance.Instance.XeroConnectionStatus == true)
                {
                    var result = await XeroInstance.Instance.XeroService.GetInvoices(XeroInstance.Instance.XeroToken, XeroInstance.Instance.XeroTenentID);
                    var Total = result._Invoices.Sum(x => x.Total);
                    returnData.Add("data", "");
                    returnData.Add("Total", Total);
                }
                else
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }

                return Json(returnData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        [CheckSession]
        [ChildActionOnly]
        public ActionResult AgencySelection()
        {
            AgencyClient objAgy = new AgencyClient();
            try
            {
                using (ClientService objClient = new ClientService())
                {
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (TempData["ClientListActived"] == null)
                    {
                        
                        var objResult = new List<ClientModel>();
                        var LoginUserid = Convert.ToString(User.UserId);
                        if (LoginUserid != "")
                        {
                            if (Session["UserType"] != null && Convert.ToString( Session["UserType"]).Trim() == "2")
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
                            return RedirectToAction("Login");
                        }
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

                    var client = objAgy.ClientList.Where(x => x.Id == objAgy.SelectedClientNameID).FirstOrDefault();
                    Common.ConnectXeroClient(client);
                    //if (client != null && !string.IsNullOrEmpty(client.XeroScope) && !string.IsNullOrEmpty(client.XeroClientID) && !string.IsNullOrEmpty(client.XeroClientSecret))
                    //{
                    //    XeroInstance.Instance.XeroScope = client.XeroScope; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                    //    XeroInstance.Instance.XeroClientID = client.XeroClientID;
                    //    XeroInstance.Instance.XeroClientSecret = client.XeroClientSecret;
                    //    //XeroService Xero = new XeroService("8CED4A15FB7149198DB6260147780F6D", "MHr607yAVALE1EX6QrhwOYYeCrQePcrRAfofw056YTK6qWg8", scope);
                    //    XeroService Xero = new XeroService(XeroInstance.Instance.XeroClientID, XeroInstance.Instance.XeroClientSecret, XeroInstance.Instance.XeroScope);
                    //    XeroInstance.Instance.XeroService = Xero;
                    //    Task.Run(async () =>
                    //    {
                    //        var token = await Xero.LoginXeroAccess();
                    //        XeroInstance.Instance.XeroToken = token;
                    //    });
                    //}


                    return PartialView("_AgencySelection", objAgy);
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }

        }
    }
}