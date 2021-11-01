﻿using CFO.Model.ViewModels;
using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
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

                    //var objResult = objReConcilation.GetReconciliationDataCountAgencyId(AgencyId);
                    //return View(objResult.ResultData);
                    
                    int AgencyID = 0;
                    ViewBag.XeroConnectionStatus = XeroInstance.Instance.XeroConnectionStatus;
                    ViewBag.XeroStatusMessage = XeroInstance.Instance.XeroConnectionMessage;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    var userType = Convert.ToString(Session["UserType"]);
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    
                    using (IntigrationService objIntegration = new IntigrationService())
                    {

                        TempData["GLAccounts"] = objIntegration.GetXeroGlAccount(AgencyID).ResultData;
                        if (userType == "1")
                        {
                            ViewBag.IsBankRuleVisible = true;
                            List<XeroTrackingCategoriesVM> objTCList = objIntegration.GetXeroTracking(AgencyID).ResultData;
                            if (objTCList != null && objTCList.Count > 0)
                            {
                                List<XeroTrackingOptionGroupVM> TCgroup = (from p in objTCList
                                                                           group p by p.Name into g
                                                                           select new XeroTrackingOptionGroupVM { Name = g.Key, Options = g.ToList() }).ToList();
                                TempData["TrackingCategories"] = TCgroup;

                            }
                            TempData["BankRule"] = getBankRule();
                        }
                        else
                        {
                            ViewBag.IsBankRuleVisible = false;
                        }
                        TempData["DistinctAccount"] = getDistincAccount(AgencyID, RecordsType);
                    }
                    var objResult = objReConcilation.GetReconciliation(AgencyID, RecordsType, 0);

                    ViewBag.UserId = User.UserId;
                    ViewBag.UserEmail = User.LoginName;
                    TempData["ReconcilationData"] = objResult.ResultData;
                    Session["ReconcilationData"] = objResult.ResultData;
                    return View("ReconciliationMain", objResult.ResultData);

                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        //[HttpPost]
        public ActionResult GetFilteredReconcilation(string accounts, DateTime? dateRangeFrom, DateTime? dateRangeTo, decimal? amountMin, decimal? amountMax, string Bankrule, string TrackingCategory1, string TrackingCategory2, reconcilationType? FilterType, int? AgencyID, string Type)
        {


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


                var userType = Convert.ToString(Session["UserType"]);
                var objResult = objReConcilation.GetFilteredReconcilation(Filter).ResultData;
                using (IntigrationService objIntegration = new IntigrationService())
                {
                    TempData["GLAccounts"] = objIntegration.GetXeroGlAccount(Filter.AgencyID.Value).ResultData;
                    if (userType == "1")
                    {
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
                    }
                    else
                    {
                        ViewBag.IsBankRuleVisible = false;
                    }
                    TempData["DistinctAccount"] = getDistincAccount(Filter.AgencyID.Value, Filter.Type);
                }
                //TempData["BankRule"] = getBankRule();
                //TempData["DistinctAccount"] = getDistincAccount(Filter.AgencyID.Value, Filter.Type);
                TempData["ReconcilationData"] = objResult;
                Session["ReconcilationData"] = objResult;

                return View("ReconciliationMain", objResult);
                // return View("ReconciliationMain", objResult.ResultData);
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
                    var UnselectedIds = BPParameter.UnSelectedRecords.Split(',');
                    var result = (List<Proven.Model.reconciliationVM>)Session["ReconcilationData"];
                    if(result == null) return Json(new { Message = "Error", UpdatedCount = 0 }, JsonRequestBehavior.AllowGet);
                    Ids = result.Select(x => x.id).ToList();
                    foreach (var item in UnselectedIds)
                    {
                        Ids.Remove(item);
                    }
                }
                else
                {
                    var UnselectedIds = !string.IsNullOrEmpty(BPParameter.UnSelectedRecords)? BPParameter.UnSelectedRecords.Split(','): new string[0];
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        //public ActionResult ReconciliationMain()
        //{
        //    try
        //    {

        //        using (ReconcilationService objReConcilation = new ReconcilationService())
        //        {
        //            int AgencyID = 0;
        //            List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
        //            if (UserPref != null && UserPref.Count() > 0)
        //            {
        //                var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
        //                AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
        //            }
        //            var objResult = objReConcilation.GetReconciliation(AgencyID, NotInBank, 0);
        //            return View();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(Utltity.Log4NetExceptionLog(ex));
        //        throw ex;
        //    }
        //}
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
        public ActionResult ReconciliationMain()
        {
            try
            {
                string RecordsType = NotInBooks;
                ViewBag.XeroConnectionStatus = XeroInstance.Instance.XeroConnectionStatus;
                ViewBag.XeroStatusMessage = XeroInstance.Instance.XeroConnectionMessage;

                //  return View();
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    int AgencyID = 0;
                    ViewBag.IsEmailVisible = false;
                    var userType = Convert.ToString(Session["UserType"]);
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];

                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    using (IntigrationService objIntegration = new IntigrationService())
                    {
                       var glAccountList = objIntegration.GetXeroGlAccount(AgencyID).ResultData;
                        glAccountList.ForEach(x => x.Name = $"{x.Code } - {x.Name}");
                        TempData["GLAccounts"] = glAccountList;

                        if (userType == "1")
                        {
                            ViewBag.IsEmailVisible = true;
                            ViewBag.IsBankRuleVisible = true;
                            List<XeroTrackingCategoriesVM> objTCList = objIntegration.GetXeroTracking(AgencyID).ResultData;
                            if (objTCList != null && objTCList.Count > 0)
                            {
                                List<XeroTrackingOptionGroupVM> TCgroup = (from p in objTCList
                                                                           group p by p.Name into g
                                                                           select new XeroTrackingOptionGroupVM { Name = g.Key, Options = g.ToList() }).ToList();
                                TempData["TrackingCategories"] = TCgroup;
                            }
                            TempData["BankRule"] = getBankRule();
                        }
                        else
                        {
                            ViewBag.IsBankRuleVisible = false;
                        }
                        TempData["DistinctAccount"] = getDistincAccount(AgencyID, RecordsType);
                    }
                    var objResult = objReConcilation.GetReconciliation(AgencyID, RecordsType, 0);

                    ViewBag.UserId = User.UserId;
                    ViewBag.UserEmail = User.LoginName;
                    TempData["ReconcilationData"] = objResult.ResultData;
                    Session["ReconcilationData"] = objResult.ResultData;
                    return View(objResult.ResultData);
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
        public JsonResult UpdateReconciliation(int AgencyID, string id, int GLAccount, string BankRule, int TrackingCategory, int TrackingCategoryAdditional = 0)
        {
            //BankRule = BankRule.Replace("0", "");
            using (ReconcilationService objReConcilation = new ReconcilationService())
            {
                var objResult = objReConcilation.UpdateReconciliation(AgencyID, id, GLAccount, BankRule, TrackingCategory, TrackingCategoryAdditional);
                return Json(new { Message = objResult.message }, JsonRequestBehavior.AllowGet);
            }

        }

        [CheckSession]
        [HttpPost]
        public JsonResult AddNewXeroOnDemandDataRequest(XeroReconcilationDataOnDemandRequestVM model)
        {
            try
            {
                var AgencyID = 0;
                var LoginUserid = Convert.ToString(Session["UserId"].ToString());
                List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                if (UserPref != null && UserPref.Count() > 0)
                {
                    var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                }
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        //[CheckSession]
        //public JsonResult GetXeroOnDemandRequestStatus(string CurrentStatus)
        //{
        //    try
        //    {

        //        using (ReconcilationService objReConcilation = new ReconcilationService())
        //        {
        //            var objResult = objReConcilation.GetXeroOnDemandRequestStatus(CurrentStatus);
        //            return Json(objResult, JsonRequestBehavior.AllowGet);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(Utltity.Log4NetExceptionLog(ex));
        //        throw ex;
        //    }
        //}

        [CheckSession]
        [HttpGet]
        public JsonResult GetXeroOnDemandRequestStatus(int AgencyId, string CurrentStatus)
        {
            try
            {
                var LoginUserid = Session["UserId"].ToString();
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    var objResult = objReConcilation.GetXeroOnDemandRequestStatus(AgencyId,CurrentStatus, LoginUserid).ResultData;
                    return Json(new { data = "", Status = objResult.CurrentStatus, Message = objResult.CurrentStatus }, JsonRequestBehavior.AllowGet);
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

    }
}