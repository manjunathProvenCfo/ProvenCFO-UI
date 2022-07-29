using log4net;
using Newtonsoft.Json;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using QuickBooksSharp;
using QuickBooksSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xero.NetStandard.OAuth2.Token;

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
                    TempData["GlAccountReview"] = JsonConvert.SerializeObject(getGlAccountReview());
                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    if (AgencyID != 0)
                    {
                        ViewBag.XeroConnectionStatus = AccountingPackageInstance.Instance.ConnectionStatus;
                        ViewBag.XeroStatusMessage = AccountingPackageInstance.Instance.ConnectionMessage;
                        ViewBag.AccountingPackageId = AccountingPackageInstance.Instance.ClientModel.ThirdPartyAccountingApp_ref;
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

                        ViewBag.XeroConnectionStatus = AccountingPackageInstance.Instance.ConnectionStatus;
                        ViewBag.XeroStatusMessage = AccountingPackageInstance.Instance.ConnectionMessage;
                        ViewBag.AccountingPackageId = AccountingPackageInstance.Instance.ClientModel.ThirdPartyAccountingApp_ref;
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
                ViewBag.XeroConnectionStatus = AccountingPackageInstance.Instance.ConnectionStatus;
                ViewBag.XeroStatusMessage = AccountingPackageInstance.Instance.ConnectionMessage;
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
                if (AccountingPackageInstance.Instance.ConnectionStatus == true)
                {
                    using (ClientService objClient = new ClientService())
                    {
                        var client = objClient.GetClientById(ClientID);
                        using (XeroService<IXeroToken, TrackingCategories> AccountingPackageService = new XeroService<IXeroToken, TrackingCategories>(client.APIClientID, client.APIClientSecret, client.APIScope, AccountingPackageInstance.Instance.XeroAppName))
                        {
                            var result = await AccountingPackageService.GetTrackingCategories(AccountingPackageInstance.Instance.XeroToken, client.XeroID);
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
                        }
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
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
        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> GetXeroGLAccountSync(int ClientID)
        {
            try
            {
                string TenentID = string.Empty;

                if (AccountingPackageInstance.Instance.ConnectionStatus == true)
                {
                    using (ClientService objClient = new ClientService())
                    {
                        var client = objClient.GetClientById(ClientID);
                        dynamic AppPackage = null;
                        dynamic Token = null;
                        switch (client.ThirdPartyAccountingApp_ref.Value)
                        {
                            case 1:
                                AppPackage = new XeroService<IXeroToken, List<Xero.NetStandard.OAuth2.Model.Accounting.Account>>(client.APIClientID, client.APIClientSecret, client.APIScope, AccountingPackageInstance.Instance.XeroAppName);
                                Token = AccountingPackageInstance.Instance.XeroToken;
                                TenentID = client.XeroID;
                                break;
                            case 2:
                                AppPackage = new QuickbooksLocalService<TokenResponse, QuickBooksSharp.Entities.Account[]>(client.APIClientID, client.APIClientSecret, client.APIScope, AccountingPackageInstance.Instance.XeroAppName);
                                Token = AccountingPackageInstance.Instance.QuickBooksToken;
                                 TenentID = Convert.ToString(client.QuickBooksCompanyId);
                                break;
                            default:
                                break;
                        }
                        //using (XeroService<IXeroToken, Accounts> AccountingPackageService = new XeroService<IXeroToken, Accounts>(AccountingPackageInstance.Instance.ClientID, AccountingPackageInstance.Instance.XeroTenentID, AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName))
                        //{

                        var result = await AppPackage.GetGLAccounts(Token, TenentID);
                        if (result != null)
                        {
                            List<XeroGlAccountVM> gl = new List<XeroGlAccountVM>();
                            switch (client.ThirdPartyAccountingApp_ref.Value)
                            {
                                case 1:
                                    if (result != null)
                                    {
                                        foreach (var item in result)
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
                                    }
                                    break;
                                case 2:
                                    QuickBooksSharp.Entities.Account[] accounts = (QuickBooksSharp.Entities.Account[])result;
                                    foreach (var item in accounts)
                                    {
                                        XeroGlAccountVM account = new XeroGlAccountVM();
                                        account.AccountId = Convert.ToString(item.Id);
                                        account.Name = item.Name;
                                        account.Status = Convert.ToString(item.Active == true ? "ACTIVE" : "INACTIVE");
                                        account.AgencyId = ClientID;
                                        account.UpdatedDateUTC = DateTime.UtcNow;
                                        account.Class = Convert.ToString(item.Classification);
                                        account.Type = Convert.ToString(item.AccountType);
                                        account.BankAccountNumber = Convert.ToString("");
                                        account.BankAccountType = Convert.ToString(item.AccountType);
                                        account.CurrencyCode = Convert.ToString(item.CurrencyRef.value);
                                        account.Id = 0;
                                        gl.Add(account);

                                    }

                                    break;
                                default:
                                    break;
                            }

                            using (IntigrationService objInt = new IntigrationService())
                            {
                                objInt.CreateGlAccount(gl);
                            }

                            TempData["GlAccountReview"] = JsonConvert.SerializeObject(getGlAccountReview());
                        }
                        else
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                    }

                    //}
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetXeroBankAccountSync(int ClientID)
        {
            List<ClientXeroAccountsVM> gl = new List<ClientXeroAccountsVM>();
            try
            {
                string TenentID = string.Empty;
                if (AccountingPackageInstance.Instance.ConnectionStatus == true)
                {
                    using (ClientService objClient = new ClientService())
                    {
                        var client = objClient.GetClientById(ClientID);

                        //using (XeroService<IXeroToken, Accounts> AccountingPackageService = new XeroService<IXeroToken, Accounts>(AccountingPackageInstance.Instance.ClientID, AccountingPackageInstance.Instance.TenentID, AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName))
                        //{
                        dynamic AppPackage = null;
                        dynamic Token = null;
                        switch (client.ThirdPartyAccountingApp_ref.Value)
                        {
                            case 1:
                                AppPackage = new XeroService<IXeroToken, List<Xero.NetStandard.OAuth2.Model.Accounting.Account>>(client.APIClientID, client.APIClientSecret, client.APIScope, AccountingPackageInstance.Instance.XeroAppName);
                                Token = AccountingPackageInstance.Instance.XeroToken;
                                TenentID = client.XeroID;
                                break;
                            case 2:
                                AppPackage = new QuickbooksLocalService<TokenResponse, QuickBooksSharp.Entities.Account[]>(client.APIClientID, client.APIClientSecret, client.APIScope, AccountingPackageInstance.Instance.XeroAppName);
                                Token = AccountingPackageInstance.Instance.QuickBooksToken;
                                TenentID = Convert.ToString(client.QuickBooksCompanyId);
                                break;
                            default:    
                                break;
                        }

                        if (client != null)
                        {
                            var result = await AppPackage.GetBankAccounts(Token, TenentID);
                            if (result != null)
                            {
                                switch (client.ThirdPartyAccountingApp_ref.Value)
                                {
                                    case 1:
                                        if (result != null)
                                        {
                                            List<Xero.NetStandard.OAuth2.Model.Accounting.Account> XeroAccounts = result;
                                            var xaccounts = XeroAccounts;
                                            foreach (var item in XeroAccounts)
                                            {
                                                ClientXeroAccountsVM account = new ClientXeroAccountsVM();
                                                account.AccountID = Convert.ToString(item.AccountID);
                                                account.Code = item.Code;
                                                account.Name = item.Name;
                                                account.Status = Convert.ToString(item.Status);
                                                account.AgencyId_ref = ClientID;
                                                account.Class = Convert.ToString(item.Class);
                                                account.Type = Convert.ToString(item.Type);
                                                account.TaxType = Convert.ToString(item.TaxType);
                                                account.BankAccountNumber = Convert.ToString(item.BankAccountNumber);
                                                account.BankAccountType = Convert.ToString(item.BankAccountType);
                                                account.CurrencyCode = Convert.ToString(item.CurrencyCode);
                                                account.ReportingCode = Convert.ToString(item.ReportingCode);
                                                account.HasAttachments = item.HasAttachments;
                                                account.Id = 0;
                                                gl.Add(account);
                                            }
                                        }
                                        else
                                        {
                                            return Json(false, JsonRequestBehavior.AllowGet);
                                        }
                                        break;
                                    case 2:
                                        QuickBooksSharp.Entities.Account[] Qaccounts = (QuickBooksSharp.Entities.Account[])result;
                                        foreach (var item in Qaccounts)
                                        {
                                            ClientXeroAccountsVM acct = new ClientXeroAccountsVM();
                                            acct.AccountID = Convert.ToString(item.Id);
                                            acct.Name = item.Name;
                                            acct.Status = Convert.ToString(item.Active == true ? "ACTIVE" : "INACTIVE");
                                            acct.AgencyId_ref = ClientID;
                                            acct.Class = Convert.ToString(item.Classification);
                                            acct.Type = Convert.ToString(item.AccountType);
                                            acct.BankAccountNumber = Convert.ToString("");
                                            acct.BankAccountType = Convert.ToString(item.AccountType);
                                            acct.CurrencyCode = Convert.ToString(item.CurrencyRef.value);
                                            acct.Id = 0;
                                            acct.IsActive = true;
                                            acct.IsDeleted = false;
                                            gl.Add(acct);
                                        }
                                        break;
                                    default:
                                        break;
                                }
                                using (IntigrationService objInt = new IntigrationService())
                                {
                                    objInt.CreateXeroBankAccount(gl);
                                }

                            }
                        }
                    }
                    //}
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> UpdateXeroConnectionStatus()
        {
            ViewBag.XeroConnectionStatus = AccountingPackageInstance.Instance.ConnectionStatus;
            ViewBag.XeroStatusMessage = AccountingPackageInstance.Instance.ConnectionMessage;
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> GetLinkToken(string ClientName)
        {
            try
            {
                using (PlaidBankTransaction<string, string> plaid = new PlaidBankTransaction<string, string>(PlaidInstance.Instance.ClientID, PlaidInstance.Instance.ClientSecret, PlaidInstance.Instance.language, PlaidInstance.Instance.products, PlaidInstance.Instance.country_codes, PlaidInstance.Instance.Environment))
                {
                    var result = await plaid.getLinkToken(ClientName);
                    if (result != String.Empty)
                    {

                        return Json(new { link_token = result, Status = "Success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { link_token = "", Status = "Error" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                return Json(new { link_token = "", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> PostPublictoken(int ClientId, string AccountID, string public_token)
        {
            try
            {
                using (Plaid plaid = new Plaid(PlaidInstance.Instance.ClientID, PlaidInstance.Instance.ClientSecret, PlaidInstance.Instance.language, PlaidInstance.Instance.products, PlaidInstance.Instance.country_codes, PlaidInstance.Instance.Environment))
                {
                    var access_token = await plaid.ExchangeTokenAsync(public_token);
                    if (access_token != null)
                    {
                        var access_token_enc = SecurityCommon.EncryptString(Convert.ToString(access_token));
                        using (IntigrationService service = new IntigrationService())
                        {
                            var res = await service.UpdatePlaidBankAccountDetails(AccountID, access_token_enc, true);
                            if (res.resultData == true)
                            {
                                return Json(new { Status = "Success" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public static List<SelectListItem> getGlAccountReview()
        {

            List<SelectListItem> listItem = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "Perpetual";
            item.Value = "Perpetual";
            listItem.Add(item);
            SelectListItem item1 = new SelectListItem();
            item1.Text = "Monthly";
            item1.Value = "Monthly";
            listItem.Add(item1);
            SelectListItem item2 = new SelectListItem();
            item2.Text = "Quarterly";
            item2.Value = "Quarterly";
            listItem.Add(item2);
            SelectListItem item3 = new SelectListItem();
            item3.Text = "Semi-Annual";
            item3.Value = "Semi-Annual";
            listItem.Add(item3);
            SelectListItem item4 = new SelectListItem();
            item4.Text = "Annual";
            item4.Value = "Annual";
            listItem.Add(item4);
            SelectListItem item5 = new SelectListItem();
            item5.Text = "Never";
            item5.Value = "Never";
            listItem.Add(item5);
            return listItem;
        }
    }
}