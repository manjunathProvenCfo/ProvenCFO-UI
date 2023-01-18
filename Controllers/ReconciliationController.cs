using CFO.Model.ViewModels;
using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ProvenCfoUI.Controllers
{
    [Exception_Filters]
    public class ReconciliationController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public const string NotInBank = "Outstanding Payments";
        public const string NotInBooks = "Unreconciled";

        // GET: Reconciliation
        [CheckSession]
        public ActionResult GetReconcilation(string Type)
        {
            string RecordsType = NotInBooks;
            try
            {
                if (Type == "Not in Banks")
                {
                    RecordsType = NotInBank;
                }
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    ViewBag.XeroConnectionStatus = AccountingPackageInstance.Instance.ConnectionStatus;
                    ViewBag.XeroStatusMessage = AccountingPackageInstance.Instance.ConnectionMessage;
                    ViewBag.AzureFunctionReconUrl = Convert.ToString(ConfigurationManager.AppSettings["AzureFunctionReconUrl"]);
                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    var userType = Convert.ToString(Session["UserType"]);
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    if (userType == "1")
                    {
                        var getallAction = objReConcilation.GetAllReconcilationAction().ResultData;
                        getallAction.ForEach(x => x.ActionName = x.ActionName);

                        if (Type == "Not in Banks")
                        {
                            ViewBag.isvisibleGlAccount = true;
                            TempData["Action"] = getallAction;
                        }
                        else
                        {
                            ViewBag.isvisibleGlAccount = false;
                        }
                    }
                    using (ClientService objClientService = new ClientService())
                    {
                        var ThirdpartyAccount = objClientService.GetClientById(AgencyID);
                        ViewBag.AccountingPackage = ThirdpartyAccount.ThirdPartyAccountingApp_ref;
                        var AccountingPackage = objClientService.GetClientXeroAcccountsByAgencyId(AgencyID).ResultData;
                        TempData["NotInBank"] = AccountingPackage;
                    }
                    using (IntigrationService objIntegration = new IntigrationService())
                    {
                        var glAccountList = objIntegration.GetXeroGlAccount(AgencyID, "ACTIVE").ResultData;
                        glAccountList.ForEach(x => x.Name = x.AgencyId != null? $"{x.Code} - {x.Name}": $"{x.Name}");
                        TempData["GLAccounts"] = glAccountList;
                        List<XeroTrackingCategoriesVM> objTCList = objIntegration.GetXeroTracking(AgencyID).ResultData;
                        if (objTCList != null && objTCList.Count > 0)
                        {
                            List<XeroTrackingOptionGroupVM> TCgroup = (from p in objTCList
                                                                       group p by p.Name into g
                                                                       select new XeroTrackingOptionGroupVM { Name = g.Key, Options = g.ToList() }).ToList();
                            TempData["TrackingCategories"] = TCgroup;

                        }

                        if (userType == "1")
                        {
                            ViewBag.IsStaffUser = true;
                            ViewBag.IsBankRuleVisible = true;
                            TempData["BankRule"] = getBankRule();

                            TempData["ReconciledStatus"] = getReconciledStatus();
                        }
                        else if (userType == "2" && Type == "Not in Banks")
                        {
                            var getallAction = objReConcilation.GetAllReconcilationAction().ResultData;
                            getallAction.ForEach(x => x.ActionName = x.ActionName);
                            ViewBag.isvisibleGlAccount = true;
                            TempData["Action"] = getallAction;
                        }
                        else
                        {
                            ViewBag.IsBankRuleVisible = false;
                            ViewBag.isvisibleGlAccount = false;
                        }

                        TempData["DistinctAccount"] = getDistincAccount(AgencyID, RecordsType);
                    }
                    var objResult = objReConcilation.GetReconciliation(AgencyID, RecordsType, 0, User.UserId, User.LoginName);

                    ViewBag.UserId = User.UserId;
                    ViewBag.UserEmail = User.LoginName;
                    TempData["ReconcilationData"] = objResult.ResultData;
                    Session["ReconcilationData"] = objResult.ResultData;
                    return View("ReconciliationMain", objResult.ResultData);

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        //[HttpPost]
        public ActionResult GetFilteredReconcilationdata(string accounts, DateTime? dateRangeFrom, DateTime? dateRangeTo, decimal? amountMin, decimal? amountMax, string Bankrule, string TrackingCategory1, string TrackingCategory2, reconcilationType? FilterType, int? AgencyID, string Type, bool? RuleNew)
        {

            ViewBag.IsStaffUser = false;

            using (ReconcilationService objReConcilation = new ReconcilationService())
            {
                ReconciliationfilterModel Filter = new ReconciliationfilterModel();
                Filter.accounts = accounts;
                Filter.dateRangeFrom = dateRangeFrom;
                Filter.dateRangeTo = dateRangeTo;
                Filter.amountMin = amountMin;
                Filter.amountMax = amountMax;
                Filter.Bankrule = Bankrule;
                Filter.TrackingCategory1 = TrackingCategory1;
                Filter.TrackingCategory2 = TrackingCategory2;
                Filter.FilterType = FilterType;
                Filter.AgencyID = AgencyID;
                Filter.Type = Type;
                Filter.UserID = User.UserId;
                Filter.RuleNew = RuleNew;

                var userType = Convert.ToString(Session["UserType"]);
                var objResult = objReConcilation.GetFilteredReconcilation(Filter).ResultData;
                using (IntigrationService objIntegration = new IntigrationService())
                {
                    var glAccountList = objIntegration.GetXeroGlAccount(Filter.AgencyID.Value, "ACTIVE").ResultData;
                    glAccountList.ForEach(x => x.Name = x.AgencyId != null? $"{x.Code} - {x.Name}" : $"{x.Name}");
                    TempData["GLAccounts"] = glAccountList;
                    //TempData["GLAccounts"] = objIntegration.GetXeroGlAccount(Filter.AgencyID.Value, "ACTIVE").ResultData;
                    if (userType == "1")
                    {
                        ViewBag.IsStaffUser = true;
                        ViewBag.IsBankRuleVisible = true;
                        List<XeroTrackingCategoriesVM> objTCList = objIntegration.GetXeroTracking(Filter.AgencyID.Value).ResultData;
                        if (objTCList != null && objTCList.Count > 0)
                        {
                            List<XeroTrackingOptionGroupVM> TCgroup = (from p in objTCList
                                                                       group p by p.Name into g
                                                                       select new XeroTrackingOptionGroupVM { Name = g.Key, Options = g.ToList() }).ToList();
                            TempData["TrackingCategories"] = TCgroup;
                        }
                        TempData["BankRule"] = getBankRule();
                        TempData["ReconciledStatus"] = getReconciledStatus();
                        var getallAction = objReConcilation.GetAllReconcilationAction().ResultData;
                        getallAction.ForEach(x => x.ActionName = x.ActionName);

                        if (Type == "Outstanding Payments")
                        {
                            ViewBag.isvisibleGlAccount = true;
                            TempData["Action"] = getallAction;
                        }
                        else
                        {
                            ViewBag.isvisibleGlAccount = false;
                        }
                    }
                    else
                    {
                        ViewBag.IsBankRuleVisible = false;
                    }
                    TempData["DistinctAccount"] = getDistincAccount(Filter.AgencyID.Value, Filter.Type);
                }
                TempData["ReconcilationData"] = objResult;
                Session["ReconcilationData"] = objResult;
                return View("ReconciliationMain", objResult);
            }
        }
        [CheckSession]
        public JsonResult ReconcilationBuilAction(BulkActionParametersVM BPParameter)
        {
            try
            {
                List<string> Ids;
                if (BPParameter.IsAllSelected == true)
                {
                    var UnselectedIds = !string.IsNullOrEmpty(BPParameter.UnSelectedRecords) ? BPParameter.UnSelectedRecords.Split(',') : new string[0];
                    var result = (List<Proven.Model.reconciliationVM>)Session["ReconcilationData"];
                    if (result == null) return Json(new { Message = "Error", UpdatedCount = 0 }, JsonRequestBehavior.AllowGet);
                    Ids = result.Select(x => x.id).ToList();
                    foreach (var item in UnselectedIds)
                    {
                        Ids.Remove(item);
                    }
                }
                else
                {
                    var UnselectedIds = !string.IsNullOrEmpty(BPParameter.UnSelectedRecords) ? BPParameter.UnSelectedRecords.Split(',') : new string[0];
                    Ids = BPParameter.SelectedItems.Split(',').ToList();
                    foreach (var item in UnselectedIds)
                    {
                        Ids.Remove(item);
                    }
                }
                if (Ids != null && Ids.Count > 0)
                {
                    BPParameter.Ids = Ids.ToArray();
                    using (ReconcilationService service = new ReconcilationService())
                    {
                        var returnVale = service.BulkUpdateReconcilation(BPParameter).resultData;
                        if (returnVale == true)
                            return Json(new { Message = "Success", UpdatedCount = Ids.Count() }, JsonRequestBehavior.AllowGet);
                        else
                            return Json(new { Message = "Error", UpdatedCount = 0 }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { Message = "NoRecords", UpdatedCount = 0 }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }

        }
        [CheckSession]
        public JsonResult GetReconciliationDataCountAgencyId(string AgencyId)
        {
            try
            {

                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    var objResult = objReConcilation.GetReconciliationDataCountAgencyId(AgencyId);
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }


        [CheckSession]
        public JsonResult GetReconciliationDashboardDataAgencyId(string AgencyID, string type)
        {
            string RecordsType = NotInBooks; 
            try
            {
                if (RecordsType == "Not in Banks")
                {
                    RecordsType = NotInBank;
                }
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    var objResult = objReConcilation.GetReconciliationDashboardDataAgencyId(AgencyID, type);

                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }
        [CheckSession]
        public JsonResult GetReconciliationNegCountAgencyId(string AgencyId)
        {
            try
            {

                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    var objResult = objReConcilation.GetReconciliationNegCountAgencyId(AgencyId);
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult GetReconciliationCountAgencyId(string AgencyId)
        {
            try
            {

                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    var objResult = objReConcilation.GetReconciliationCountAgencyId(AgencyId);
                    return Json(objResult, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }
        public static List<SelectListItem> getBankRule()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "BANK";
            item.Value = "BANK";
            listItem.Add(item);
            SelectListItem item1 = new SelectListItem();
            item1.Text = "BANK<$300";
            item1.Value = "BANK<$300";
            listItem.Add(item1);
            SelectListItem item2 = new SelectListItem();
            item2.Text = "BANK=$";
            item2.Value = "BANK=$";
            listItem.Add(item2);
            SelectListItem item3 = new SelectListItem();
            item3.Text = "BANK = SPECIAL ACCOUNT";
            item3.Value = "BANK = SPECIAL ACCOUNT";
            listItem.Add(item3);
            SelectListItem item4 = new SelectListItem();
            item4.Text = "CHECK";
            item4.Value = "CHECK";
            listItem.Add(item4);
            return listItem;
        }
        public static List<SelectListItem> getReconciledStatus()
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            SelectListItem item = new SelectListItem();
            item.Text = "Reconciled";
            item.Value = "1";
            listItem.Add(item);
            SelectListItem item1 = new SelectListItem();
            item1.Text = "Not Reconciled";
            item1.Value = "0";
            listItem.Add(item1);
            return listItem;
        }
        public static List<SelectListItem> getDistincAccount(int AgencyID, string Type)
        {
            List<SelectListItem> listItem = new List<SelectListItem>();
            using (ReconcilationService objReConcilation = new ReconcilationService())
            {
                var objaccount = objReConcilation.GetDistinctAccount(AgencyID, Type).resultData;
                foreach (var account in objaccount)
                {
                    SelectListItem item = new SelectListItem();
                    item.Text = account;
                    item.Value = account;
                    listItem.Add(item);
                }
            }
            return listItem;
        }
        [CheckSession]
        public ActionResult ReconciliationMain()
        {
            try
            {
                ClientModel ThirdpartyAccount = null;
                List<UserSecurityVM> _roleList = (List<UserSecurityVM>)Session["LoggedInUserUserSecurityModels"];
                if (_roleList.Where(x => x.FeatureCode != "RCN").ToList().Count == _roleList.Count())
                {
                    return RedirectToAction("AgencyHome", "AgencyService");
                }

                string RecordsType = NotInBooks;
                ViewBag.XeroConnectionStatus = AccountingPackageInstance.Instance.ConnectionStatus;
                ViewBag.XeroStatusMessage = AccountingPackageInstance.Instance.ConnectionMessage;
                ViewBag.AzureFunctionReconUrl = Convert.ToString(ConfigurationManager.AppSettings["AzureFunctionReconUrl"]);
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    int AgencyID = 0;  
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    var userType = Convert.ToString(Session["UserType"]);
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    ViewBag.IsStaffUser = false;
                    var getallAction = objReConcilation.GetAllReconcilationAction().ResultData;
                    getallAction.ForEach(x => x.ActionName = x.ActionName);

                    if (RecordsType == "Not in Banks")
                    {
                        ViewBag.isvisibleGlAccount = true;
                        TempData["Action"] = getallAction;
                    }
                    else
                    {
                        ViewBag.isvisibleGlAccount = false;
                    }
                    using (ClientService objClientService = new ClientService())
                    {
                       ThirdpartyAccount = objClientService.GetClientById(AgencyID);
                        ViewBag.AccountingPackage = ThirdpartyAccount.ThirdPartyAccountingApp_ref;

                        Session["DOMO_Last_batchrun_time"] = ThirdpartyAccount.DOMO_Last_batchrun_time; 
                        var AccountingPackage = objClientService.GetClientXeroAcccountsByAgencyId(AgencyID).ResultData;
                        TempData["NotInBank"] = AccountingPackage;
                    }
                    using (IntigrationService objIntegration = new IntigrationService())
                    {
                        var glAccountList = objIntegration.GetXeroGlAccount(AgencyID, "ACTIVE").ResultData;
                        glAccountList.ForEach(x => x.Name = x.AgencyId != null ? $"{x.Code} - {x.Name}" : $"{x.Name}");
                        TempData["GLAccounts"] = glAccountList;
                        List<XeroTrackingCategoriesVM> objTCList = objIntegration.GetXeroTracking(AgencyID).ResultData;
                        if (objTCList != null && objTCList.Count > 0)
                        {
                            List<XeroTrackingOptionGroupVM> TCgroup = (from p in objTCList
                                                                       group p by p.Name into g
                                                                       select new XeroTrackingOptionGroupVM { Name = g.Key, Options = g.ToList() }).ToList();
                            TempData["TrackingCategories"] = TCgroup;
                        }
                        if (userType == "1")
                        {
                            ViewBag.IsStaffUser = true;
                            ViewBag.IsBankRuleVisible = true;
                            TempData["BankRule"] = getBankRule();
                            TempData["ReconciledStatus"] = getReconciledStatus();
                        }
                        else
                        {
                            ViewBag.IsBankRuleVisible = false;
                        }
                        TempData["DistinctAccount"] = getDistincAccount(AgencyID, RecordsType);
                    }
                    var objResult = objReConcilation.GetReconciliation(AgencyID, RecordsType, 0, User.UserId, User.LoginName);

                    //var objResult1 = objReConcilation.GetReconciliationList(0, 10, "account_name asc", AgencyID.ToString(), RecordsType, "0", "", User.UserId);

                    ViewBag.UserId = User.UserId;


                    ViewBag.UserEmail = User.LoginName;
                    if (ThirdpartyAccount != null)
                    {

                        ViewBag.IsDomoEnabled=ThirdpartyAccount.IsDomoEnabled==null?false:ThirdpartyAccount.IsDomoEnabled;
                    }
                    TempData["ReconcilationData"] = objResult.ResultData;
                    TempData["PaginationData"] = objResult.ResultData;
                    Session["ReconcilationData"] = objResult.ResultData;
                    return View(objResult.ResultData);
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
        public JsonResult UpdateReconciliation(int AgencyID, string id, int GLAccount, string BankRule, int TrackingCategory, string UserId, int TrackingCategoryAdditional = 0, int reconciliationActionId = 0, bool RuleNew = false)
        {
            //BankRule = BankRule.Replace("0", "");
            using (ReconcilationService objReConcilation = new ReconcilationService())
            {
                var objResult = objReConcilation.UpdateReconciliation(AgencyID, id, GLAccount, BankRule, TrackingCategory, TrackingCategoryAdditional, reconciliationActionId, UserId, RuleNew);
                return Json(new { Message = objResult.message }, JsonRequestBehavior.AllowGet);
            }
             
        }
        [CheckSession]
        [HttpPost]
        public async Task<JsonResult> OnDemandDataRequestFromPlaid(int AgencyId)
        {
            try
            {
                using (ClientService objClientService = new ClientService())
                {
                    var client = objClientService.GetClientById(AgencyId);
                    using (BankTransactionRuleEngine BankData = new BankTransactionRuleEngine())
                    {
                        List<Tuple<PlaidResponceModel, PlaidResponceModel>> result = await BankData.GetReconciliationByPaidWithAccountingPackage(client);
                        if (result.Count > 0)
                        {
                            var TotalNotinBooksInsertedRecords = result.Sum(X => X.Item1.TotalInsertedRecords);
                            var TotalNotinBooksUpdatedRecords = result.Sum(X => X.Item1.TotalUpdatedRecords);
                            var TotalNotinBanksInsertedRecords = result.Sum(X => X.Item2.TotalInsertedRecords);
                            var TotalNotinBanksUpdatedRecords = result.Sum(X => X.Item2.TotalUpdatedRecords);
                            if (result[0].Item1.Status == true && result[0].Item2.Status == true)
                            {
                                string MsgString = "Sucessfully sync the data. Total Not in books records extracted: " + TotalNotinBooksInsertedRecords + " and Total Not in banks records extracted:" + TotalNotinBanksInsertedRecords;
                                return Json(new { data = MsgString, Status = true, Message = MsgString }, JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                if (result[0].Item1.ErrorType == "Plaid Error")
                                {
                                    log.Info("Plaid API Error :" + result[0].Item1.Error + " : " + result[0].Item1.ErrorDesctiption);
                                    return Json(new { data = result[0].Item1.Error, Status = false, Message = result[0].Item1.ErrorType != null ? "Error" : result[0].Item1.ErrorType }, JsonRequestBehavior.AllowGet);
                                }
                                else
                                {
                                    log.Info("Plaid data Syc validation failed :" + result[0].Item1.Error + " : " + result[0].Item1.ErrorDesctiption);
                                    return Json(new { data = result[0].Item1.Error, Status = false, Message = result[0].Item1.ErrorType != null ? "Error" : result[0].Item1.ErrorType }, JsonRequestBehavior.AllowGet);
                                }
                            }
                        }                        
                    }
                }
                return Json(new { data = "Error while data sync.", Status = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { data = "Error while data sync.", Status = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
            }                        
        }
        [CheckSession]
        [HttpPost]
        public async Task<JsonResult> XeroOnDemandDataRequest(XeroReconcilationDataOnDemandRequestVM model)
        {
            using (ClientService objClientService = new ClientService())
            {
                var client = objClientService.GetClientById(model.AgencyId.Value);
                using (BankTransactionRuleEngine BankData = new BankTransactionRuleEngine())
                {
                   var result = await BankData.GetReconciliationFromXero(client);
                }
            }
            return Json(new { data = "Error while data sync.", Status = false, Message = "Error" }, JsonRequestBehavior.AllowGet);
        }

        [CheckSession]
        [HttpPost]
        public JsonResult AddNewXeroOnDemandDataRequest(XeroReconcilationDataOnDemandRequestVM model)
        {
            try
            {

                int AgencyID = Convert.ToInt32(AccountingPackageInstance.Instance.ClientModel.Id);
                var LoginUserid = Convert.ToString(Session["UserId"].ToString());
                XeroReconcilationDataOnDemandRequestVM xero = new XeroReconcilationDataOnDemandRequestVM();
                model.RequestType = "On Demand";
                model.RequestedAtUTC = xero.RequestedAtUTC;
                model.CurrentStatus = "New";
                model.RequestCompletedAtUTC = xero.RequestCompletedAtUTC;
                model.Remark = xero.Remark;
                model.AgencyId = AgencyID;
                model.AgencyName = xero.AgencyName;
                model.CreatedBy = LoginUserid;
                model.CreatedDate = xero.CreatedDate;
                using (ReconcilationService objReconcilliation = new ReconcilationService())
                {
                    var result = objReconcilliation.AddNewXeroOnDemandDataRequest(model).ResultData;
                    if (result != null)
                    {
                        ViewBag.ErrorMessage = "Created";
                        return Json(new { data = result, Status = ViewBag.ErrorMessage, Message = "Success" }, JsonRequestBehavior.AllowGet);
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
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }


        [CheckSession]
        [HttpGet]
        public JsonResult GetXeroOnDemandRequestStatus(int AgencyId, string CurrentStatus)
        {
            try
            {
                var LoginUserid = Session["UserId"].ToString();
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    var objResult = objReConcilation.GetXeroOnDemandRequestStatus(AgencyId, CurrentStatus, LoginUserid).ResultData;
                    return Json(new { data = "", Status = objResult.CurrentStatus, Message = objResult.CurrentStatus }, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetXeroOnDemandRequestStatusById(int RequestID)
        {
            try
            {
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    var objResult = objReConcilation.GetXeroOnDemandRequestStatusById(RequestID).ResultData;
                    return Json(new { data = "", Status = objResult.CurrentStatus, Message = objResult.CurrentStatus }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }
        [CheckSession]
        public JsonResult GetIsEnableAutomation(int agencyId)
        {
            try
            {
                var IsEnableAutomation = false;
                using (ClientService objClientService = new ClientService())
                {
                    CreateClientVM Clientvm = new CreateClientVM();
                    var client = objClientService.GetClientById(agencyId);
                    IsEnableAutomation = client.EnableAutomation.HasValue ? client.EnableAutomation.Value : false;
                }
                return Json(new { Data = IsEnableAutomation, Status = "Success" }, JsonRequestBehavior.AllowGet);
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
        [CheckSession]
        public JsonResult GetIsEnablePlaid(int agencyId)
        {
            try
            {
                var IsEnablePlaid = false;
                using (ClientService objClientService = new ClientService())
                {
                    CreateClientVM Clientvm = new CreateClientVM();
                    var client = objClientService.GetClientById(agencyId);
                    IsEnablePlaid = client.Plaid_Enabled.HasValue ? client.Plaid_Enabled.Value : false;
                }
                return Json(new { Data = IsEnablePlaid, Status = "Success" }, JsonRequestBehavior.AllowGet);
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
        public JsonResult EmailSend(string ClientName, string ClientId, string NotInBankUnreconciledItemsCount, string url, string sentdate)
        {
            try
            {
                using (AccountService obj = new AccountService())
                {
                    List<InviteUserModel> user = new List<InviteUserModel>();
                    var usersListwithRecPermission = obj.GetRegisteredUsersByAgencyWithReqPermission(ClientId, "RCN");
                    var Userslist = usersListwithRecPermission.ResultData;
                    var Recipientssdata = Userslist.Where(x => x.IsRegistered == 1 && x.IsActive == 1.ToString()).Select(x => x.Email);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath("~/assets/files/ReconcilationEmailTemplate.xml"));

                    string xml = System.IO.File.ReadAllText(Server.MapPath("~/assets/files/ReconcilationEmailTemplate.xml"));
                    var subject = doc.SelectNodes("EmailContent/subject")[0].InnerText;
                    var body = doc.SelectNodes("EmailContent/body")[0].InnerText;
                    var footer = doc.SelectNodes("EmailContent/footer")[0].InnerText;
                    subject = subject.Replace("{CompanyName}", ClientName);
                    subject = subject.Replace("{TodaysDate}", DateTime.Now.ToString("dd MMMM, yyyy", new System.Globalization.CultureInfo("en-US")));
                    body = body.Replace("{NotInBankUnreconciledItemsCount}", NotInBankUnreconciledItemsCount);
                    body = body.Replace("{url}", url);
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

        [CheckSession]
        [HttpPost]
        public async Task<JsonResult> UploadReconcilationReportsAsync(HttpPostedFileBase file, int agencyId, string agencyName)
        {
            ReconciliationInputModel input = new ReconciliationInputModel();
            var fileName = file.FileName;
            try
            {
                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] binData = b.ReadBytes(file.ContentLength);


                string HTMLresult = System.Text.Encoding.UTF8.GetString(binData);
                using (ReconcilationService service = new ReconcilationService())
                {



                    input.HtmlorCsvString = HTMLresult.Trim();
                    input.CompanyName = agencyName;
                    input.CompanyId = agencyId;
                    using (var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(50000)))
                    {
                        var response = await service.XeroExtractionofManualImportedDatafromHtml(input, tokenSource.Token).ConfigureAwait(false); //  await httpClient.GetAsync(uri, tokenSource.Token);                                           
                        return Json(new { Status = "Success", result = response.ResultData, FileName = fileName }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                ReconciliationOutputModel objOutput = new ReconciliationOutputModel();
                objOutput.Status = true;
                objOutput.ValidationStatus = "RequestCancelation";
                objOutput.ValidationMessage = "The process takes a longer time to complete. Please check the result after a few minutes.";
                return Json(new
                {
                    File = "",
                    Status = "Inprogress",
                    result = objOutput,
                    FileName = fileName
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "A task was canceled.")
                {
                    ReconciliationOutputModel objOutput = new ReconciliationOutputModel();
                    objOutput.Status = true;
                    objOutput.ValidationStatus = "RequestCancelation";
                    objOutput.ValidationMessage = "The process takes a longer time to complete. Please check the result after a few minutes.";
                    return Json(new
                    {
                        File = "",
                        Status = "Inprogress",
                        result = objOutput,
                        FileName = fileName
                    }, JsonRequestBehavior.AllowGet);
                }
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSession]
        [HttpPost]
        public async Task<JsonResult> UploadReconcilationQuickBookReportsAsync(HttpPostedFileBase file, int agencyId, string agencyName, string accountName)
        {
            ReconciliationInputModel input = new ReconciliationInputModel();
            var fileName = file.FileName;
            try
            {
                BinaryReader b = new BinaryReader(file.InputStream);
                byte[] binData = b.ReadBytes(file.ContentLength);


                string HTMLresult = System.Text.Encoding.UTF8.GetString(binData);


                using (ReconcilationService service = new ReconcilationService())
                {



                    input.HtmlorCsvString = HTMLresult.Trim();
                    input.CompanyName = agencyName;
                    input.CompanyId = agencyId;
                    input.Type = "Unreconciled";
                    input.AccountName = accountName;
                    //var result =  service.XeroExtractionofManualImportedDatafromHtml(input).ResultData;
                    using (var tokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(50000)))
                    {
                        var response = await service.QuickBooksExtractionofManualImportedDatafromCSV(input, tokenSource.Token).ConfigureAwait(false); //  await httpClient.GetAsync(uri, tokenSource.Token);                                           
                        return Json(new { Status = "Success", result = response.ResultData, FileName = fileName }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                ReconciliationOutputModel objOutput = new ReconciliationOutputModel();
                objOutput.Status = true;
                objOutput.ValidationStatus = "RequestCancelation";
                objOutput.ValidationMessage = "The process takes a longer time to complete. Please check the result after a few minutes.";
                return Json(new
                {
                    File = "",
                    Status = "Inprogress",
                    result = objOutput,
                    FileName = fileName
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message == "A task was canceled.")
                {
                    ReconciliationOutputModel objOutput = new ReconciliationOutputModel();
                    objOutput.Status = true;
                    objOutput.ValidationStatus = "RequestCancelation";
                    objOutput.ValidationMessage = "The process takes a longer time to complete. Please check the result after a few minutes.";
                    return Json(new
                    {
                        File = "",
                        Status = "Inprogress",
                        result = objOutput,
                        FileName = fileName
                    }, JsonRequestBehavior.AllowGet);
                }
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [CheckSession]
        [HttpPost]
        public async Task<JsonResult> UploadReconcilationAttachmentAsync(HttpPostedFileBase file, string ReconciliationId, int AgencyId)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var LoginUserid = Session["UserId"].ToString();
                    int ReconciliationCommentId;
                    using (ReconcilationService service = new ReconcilationService())
                    {
                        ReconciliationComments objComment = new ReconciliationComments();
                        objComment.IsAttachment = true;
                        objComment.IsDeleted = false;
                        objComment.CreatedBy = LoginUserid;
                        objComment.ReconciliationId_ref = ReconciliationId;
                        objComment.AgencyId = AgencyId;
                        ReconciliationCommentId = service.InsertReconcilationCommentsDetailsForAttachments(objComment).resultData;
                    }
                    ReconciliationCommentAttachments newfile = new ReconciliationCommentAttachments();
                    List<ReconciliationCommentAttachments> newfiles = new List<ReconciliationCommentAttachments>();
                    Guid objGuid = Guid.NewGuid();

                    string _FileName = Path.GetFileName(file.FileName);
                    string File_path = Path.Combine(Server.MapPath("~/UploadedFiles/ReconciliationCommentsAttachments/" + ReconciliationId + "/" + ReconciliationCommentId), _FileName);
                    string Folder_path = Server.MapPath("~/UploadedFiles/ReconciliationCommentsAttachments/" + ReconciliationId + "/" + ReconciliationCommentId);
                    string File_path_scr = "../UploadedFiles/ReconciliationCommentsAttachments/" + ReconciliationId + "/" + ReconciliationCommentId + "/" + _FileName;
                    FileInfo fi = new FileInfo(_FileName);
                    newfile.ReconciliationId_ref = ReconciliationId;
                    newfile.FileType = fi.Extension;
                    newfile.FileName = _FileName;
                    newfile.FilePath = File_path_scr;
                    newfile.ReconciliationCommentId_ref = ReconciliationCommentId;
                    newfile.CreatedBy = LoginUserid;
                    newfile.CreatedDateUTC = DateTime.UtcNow;
                    newfiles.Add(newfile);
                    using (ReconcilationService service = new ReconcilationService())
                    {
                        await service.InsertReconciliationCommentAttachmentDetails(newfiles);
                    }
                    Common.SaveStreamAsFile(Folder_path, file.InputStream, _FileName);

                    return Json(new { FilePath = File_path_scr, FileName = _FileName, FileType = newfile.FileType, Status = "Success", Message = "File attaced successfully.", CommentId = ReconciliationCommentId }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { File = "", Status = "Failur", Message = "No files are attached.." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {

                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = "Error while sending attachment."
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [CheckSession]
        [HttpPost]
        public Task<JsonResult> DeleteReconciliationAttachment(int CommentId, int AgencyId)
        {
            try
            {
                using (ReconcilationService service = new ReconcilationService())
                {
                    var result = service.getReconciliationCommentAttachments(CommentId);
                    if (result.Status == true)
                    {
                        var attachmentData = result.ResultData;

                        if (attachmentData != null && !string.IsNullOrEmpty(attachmentData.FilePath))
                        {
                            Common.DeleteFile(attachmentData.FilePath);
                        }
                        var IsDeleted = service.DeleteReconciliationCommentAttachment(CommentId).resultData;
                        if (IsDeleted == true)
                        {
                            return Task.FromResult(Json(new { File = "", Status = "Success", Message = "Attachment has been deleted.", CommentId = attachmentData.ReconciliationCommentId_ref }, JsonRequestBehavior.AllowGet));
                        }

                    }
                }
                return Task.FromResult(Json(new { File = "", Status = "Success", Message = "Attachment has been deleted." }, JsonRequestBehavior.AllowGet));
            }
            catch (Exception ex)
            {

                return Task.FromResult(Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = "Error while sending attachment."
                }, JsonRequestBehavior.AllowGet));
            }

        }
    }
}