using CFO.Model.ViewModels;
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
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    using (IntigrationService objIntegration = new IntigrationService())
                    {



                        TempData["GLAccounts"] = objIntegration.GetXeroGlAccount(AgencyID).ResultData;
                        List<XeroTrackingCategoriesVM> objTCList = objIntegration.GetXeroTracking(AgencyID).ResultData;
                        if (objTCList != null && objTCList.Count > 0)
                        {
                            List<XeroTrackingOptionGroupVM> TCgroup = (from p in objTCList
                                                                       group p by p.Name into g
                                                                       select new XeroTrackingOptionGroupVM { Name = g.Key, Options = g.ToList() }).ToList();
                            TempData["TrackingCategories"] = TCgroup;


                        }
                        TempData["BankRule"] = getBankRule();
                        TempData["DistinctAccount"] = getDistincAccount(AgencyID, RecordsType);
                    }
                    var objResult = objReConcilation.GetReconciliation(AgencyID, RecordsType, 0);
                    ViewBag.ReconcilationData = objResult.ResultData;
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
                ViewBag.ReconcilationData = objResult;

                return View("ReconciliationMain", objResult);
                // return View("ReconciliationMain", objResult.ResultData);
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
            //type = NotInBooks;
            //type = NotInBooks;
            //type = NotInBank;
            try
            {
                if (RecordsType == "Not in Banks")
                {
                    RecordsType = NotInBank;
                }
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                   
                    var objResult = objReConcilation.GetReconciliationDashboardDataAgencyId(AgencyID , RecordsType);
                   
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
                

                    //  return View();
                    using (ReconcilationService objReConcilation = new ReconcilationService())
                    {
                        int AgencyID = 0;
                        var userType = Convert.ToString(Session["UserType"]);
                        List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                       
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
                        ViewBag.ReconcilationData = objResult.ResultData;
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
    }
}