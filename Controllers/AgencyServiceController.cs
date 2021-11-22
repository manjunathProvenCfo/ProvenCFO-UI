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
using static ProvenCfoUI.Comman.Common;

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
      
        public async Task<JsonResult> GetAccountOutStanding()
        {
            var returnData = new Dictionary<string, dynamic>();
            try
            {
                if (XeroInstance.Instance.XeroConnectionStatus == true && !string.IsNullOrEmpty(Convert.ToString(XeroInstance.Instance.XeroContactIDofProvenCfo)))
                {                   
                    var result = await XeroInstance.Instance.XeroService.GetContact(XeroInstance.Instance.XeroToken, XeroInstance.Instance.XeroTenentID, XeroInstance.Instance.XeroContactIDofProvenCfo);
                                        
                    if (result != null && result._Contacts.FirstOrDefault().Balances != null && result._Contacts.FirstOrDefault().Balances.AccountsPayable != null)
                    {
                        var Outstanding = result._Contacts.FirstOrDefault().Balances.AccountsPayable.Outstanding;
                        returnData.Add("data", "");
                        returnData.Add("Total", Outstanding);
                    }
                    else
                    {
                        returnData.Add("data", "");
                        returnData.Add("Total", 0);
                    }                                       
                }
                else
                {
                    returnData.Add("data", "");
                    returnData.Add("Total", 0);                   
                }                
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(returnData, JsonRequestBehavior.AllowGet);
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
                    var userPreferencesVMs = new List<UserPreferencesVM>() { objResult };
                    //var objUserPref = commSrv.GetUserPreferences(LoginUserid);
                    Session["LoggedInUserPreferences"] = userPreferencesVMs;
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }

        }
        [HttpGet]
        [CheckSession]
        public async Task<JsonResult> GetGrossRevenueData(ChartOptions Option, ChartType cType)
        {
            dynamic obj = null;
            dynamic Header = null;
            dynamic Xdata = null;
            dynamic Ydata = null;
            try
            { 
                if (XeroInstance.Instance.XeroConnectionStatus == true)
                {
                    var pval = Common.getChartOptionValues(Option);
                    var result = await XeroInstance.Instance.XeroService.GetReportProfitAndLossAsync(XeroInstance.Instance.XeroToken, XeroInstance.Instance.XeroTenentID, pval.StartDate, pval.EndDate, pval.periods, pval.timeframe);
                   
                    Header = result.Reports[0].Rows[0].Cells.Select(x => x.Value.Replace("30 ","").Replace("31 ","").Replace("28 ","").Replace("29 ","")).ToArray();
                    if (cType == ChartType.Revenue && result!= null && result.Reports.Count > 0)
                    {
                        obj = result.Reports[0].Rows.Where(x => x.Title == "Revenue").ToList()[0].Rows.LastOrDefault().Cells.ToArray().Select(x => x.Value).ToArray();
                        Ydata = obj;
                    }
                    else if (cType == ChartType.NetIncome && result != null && result.Reports.Count > 0)
                    {
                        foreach (var item in result.Reports[0].Rows)
                        {
                            if (item.Rows != null && item.Rows.Count > 0 && item.Rows[0].Cells.Count > 0 && item.Rows[0].Cells[0].Value == "Net Income")
                            {
                                Ydata = item.Rows[0].Cells.Select(x => x.Value).ToArray();
                                
                            }
                        }
                        ///obj = result.Reports[0].Rows.Where(x => x.Rows[0].Cells.Select(y => y.Value).Contains("Net Income")).ToList()[0].Rows.LastOrDefault().Cells;
                        
                    }
                                      
                    return Json(new { Xdata = Header, Ydata = Ydata,Status ="Success" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Xdata = Header, Ydata = Ydata, Status = "No Data" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new {Status = "Error" }, JsonRequestBehavior.AllowGet);
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
    }
}