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
                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    using (IntigrationService objIntegration = new IntigrationService())
                    {
                        ViewBag.GLAccounts = objIntegration.GetXeroGlAccount(AgencyID);
                        ViewBag.TrackingCategories = objIntegration.GetXeroTracking(AgencyID);
                        
                    }
                    var objResult = objReConcilation.GetReconciliation(AgencyID, RecordsType, 0);
                    return View("ReconciliationMain", objResult.ResultData);
                    
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
        public ActionResult ReconciliationMain()
        {
            try
            {
                string RecordsType = NotInBooks;
                //  return View();
                using (ReconcilationService objReConcilation = new ReconcilationService())
                {
                    int AgencyID = 0;
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    using (IntigrationService objIntegration = new IntigrationService())
                    {
                        ViewBag.GLAccounts = objIntegration.GetXeroGlAccount(AgencyID).ResultData;
                        ViewBag.TrackingCategories = objIntegration.GetXeroTracking(AgencyID).ResultData;

                    }
                    var objResult = objReConcilation.GetReconciliation(AgencyID, RecordsType, 0);
                    return View(objResult.ResultData);
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