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
    [Exception_Filters]
    public class IntegrationController : Controller
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Integration
        [CustomAuthorize("Staff User")]
        [CheckSession]
        public ActionResult Integration()
        {
            try
            {
                using (IntigrationService objIntegration = new IntigrationService())
                {

                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    if (AgencyID != 0)
                    {
                        ViewBag.XeroConnectionStatus = XeroInstance.Instance.XeroConnectionStatus;
                        ViewBag.XeroStatusMessage = XeroInstance.Instance.XeroConnectionMessage;
                        var objResult = objIntegration.GetXeroGlAccount(AgencyID, "ACTIVE,ARCHIVED");
                        return View(objResult.ResultData);
                    }
                    return View();
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
        public ActionResult GetBankAccounts()
        {
            try
            {
                using (IntigrationService objIntegration = new IntigrationService())
                {
                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    if (AgencyID != 0)
                    {

                        ViewBag.XeroConnectionStatus = XeroInstance.Instance.XeroConnectionStatus;
                        ViewBag.XeroStatusMessage = XeroInstance.Instance.XeroConnectionMessage;
                        var objResult = objIntegration.GetXeroBankAccount(AgencyID, "ACTIVE");
                        return View(objResult.ResultData);
                    }
                    return View();
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
        public ActionResult AddXeroGlAccount()
        {
            try
            {
                XeroGlAccountVM result = new XeroGlAccountVM();
                return View(result);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        [CheckSession]
        public ActionResult GetXeroTracking()
        {
            try
            {
                int AgencyID = 0;
                List<XeroTrackingCategoriesVM> objXeroTC = new List<XeroTrackingCategoriesVM>();
                ViewBag.XeroConnectionStatus = XeroInstance.Instance.XeroConnectionStatus;
                ViewBag.XeroStatusMessage = XeroInstance.Instance.XeroConnectionMessage;
                List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                if (UserPref != null && UserPref.Count() > 0)
                {
                    var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                }
                if (AgencyID != 0)
                {
                    using (IntigrationService objIntegration = new IntigrationService())
                    {
                        var objResult = objIntegration.GetXeroTracking(AgencyID);
                        return View(objResult.ResultData);
                    }
                }

                return View(objXeroTC);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> GetXeroTrackingCategorySync(int ClientID)
        {
            try
            {
                if (XeroInstance.Instance.XeroConnectionStatus == true)
                {
                    var result = await XeroInstance.Instance.XeroService.GetTrackingCategories(XeroInstance.Instance.XeroToken, XeroInstance.Instance.XeroTenentID);
                    if (result._TrackingCategories != null)
                    {
                        List<XeroTrackingCategoriesVM> tcList = new List<XeroTrackingCategoriesVM>();
                        foreach (var item in result._TrackingCategories)
                        {
                            foreach (var Option in item.Options)
                            {
                                XeroTrackingCategoriesVM tc = new XeroTrackingCategoriesVM();
                                tc.Name = item.Name;
                                tc.Option = Option.Name;
                                tc.AgencyId = ClientID;
                                tc.Status = Convert.ToString(item.Status);
                                tcList.Add(tc);
                            }
                        }
                        using (IntigrationService objInt = new IntigrationService())
                        {
                            objInt.CreateXeroTrackingCatogories(tcList);
                        }
                    }
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        [CheckSession]
        public async Task<JsonResult> GetXeroGLAccountSync(int ClientID)
        {
            try
            {
                if (XeroInstance.Instance.XeroConnectionStatus == true)
                {
                    var result = await XeroInstance.Instance.XeroService.GetGLAccounts(XeroInstance.Instance.XeroToken, XeroInstance.Instance.XeroTenentID);
                    if (result._Accounts != null)
                    {
                        List<XeroGlAccountVM> gl = new List<XeroGlAccountVM>();
                        foreach (var item in result._Accounts)
                        {
                            XeroGlAccountVM account = new XeroGlAccountVM();
                            account.AccountId = Convert.ToString(item.AccountID);
                            account.Code = item.Code;
                            account.Name = item.Name;
                            account.Status = Convert.ToString(item.Status);
                            account.AgencyId = ClientID;
                            account.UpdatedDateUTC = DateTime.UtcNow;
                            account.Class = Convert.ToString(item.Class);
                            account.Type = Convert.ToString(item.Type);
                            account.TaxType = Convert.ToString(item.TaxType);
                            account.EnablePaymentsToAccount = Convert.ToString(item.EnablePaymentsToAccount);
                            account.ShowInExpenseClaims = Convert.ToString(item.ShowInExpenseClaims);
                            account.BankAccountNumber = Convert.ToString(item.BankAccountNumber);
                            account.BankAccountType = Convert.ToString(item.BankAccountType);
                            account.CurrencyCode = Convert.ToString(item.CurrencyCode);
                            account.ReportingCode = Convert.ToString(item.ReportingCode);
                            account.HasAttachments = Convert.ToString(item.HasAttachments);
                            account.AddToWatchlist = Convert.ToString(item.AddToWatchlist);
                            account.Id = 0;
                            gl.Add(account);

                        }
                        using (IntigrationService objInt = new IntigrationService())
                        {
                            objInt.CreateXeroGlAccount(gl);
                        }
                    }

                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        public async Task<JsonResult> GetXeroBankAccountSync(int ClientID)
        {
            try
            {
                if (XeroInstance.Instance.XeroConnectionStatus == true)
                {
                    var result = await XeroInstance.Instance.XeroService.GetGLAccounts(XeroInstance.Instance.XeroToken, XeroInstance.Instance.XeroTenentID);
                    if (result._Accounts != null)
                    {
                        var accounts = result._Accounts.Where(x => x.Status.ToString() == "ACTIVE" && x.Type.ToString() == "BANK" );
                        List<ClientXeroAccountsVM> gl = new List<ClientXeroAccountsVM>();
                        foreach (var item in accounts)
                        {
                            ClientXeroAccountsVM account = new ClientXeroAccountsVM();
                            account.AccountID = Convert.ToString(item.AccountID);
                            account.Code = item.Code;
                            account.Name = item.Name;
                            account.Status = Convert.ToString(item.Status);
                            account.AgencyId_ref = ClientID;
                            //account.UpdatedDateUTC = DateTime.UtcNow;
                            account.Class = Convert.ToString(item.Class);
                            account.Type = Convert.ToString(item.Type);
                            account.TaxType = Convert.ToString(item.TaxType);
                            //account.EnablePaymentsToAccount = Convert.ToString(item.EnablePaymentsToAccount);
                            //account.ShowInExpenseClaims = Convert.ToString(item.ShowInExpenseClaims);
                            account.BankAccountNumber = Convert.ToString(item.BankAccountNumber);
                            account.BankAccountType = Convert.ToString(item.BankAccountType);
                            account.CurrencyCode = Convert.ToString(item.CurrencyCode);
                            account.ReportingCode = Convert.ToString(item.ReportingCode);
                            account.HasAttachments = item.HasAttachments;
                            //account.AddToWatchlist = item.AddToWatchlist;
                            account.Id = 0;
                            gl.Add(account);


                        }
                        using (IntigrationService objInt = new IntigrationService())
                        {
                            objInt.CreateXeroBankAccount(gl);
                        }
                    }

                }
                else
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }


        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> UpdateXeroConnectionStatus()
        {
            ViewBag.XeroConnectionStatus = XeroInstance.Instance.XeroConnectionStatus;
            ViewBag.XeroStatusMessage = XeroInstance.Instance.XeroConnectionMessage;
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}