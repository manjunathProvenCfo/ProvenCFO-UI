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
    public class IntegrationController : Controller
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Integration
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        [CheckSession]
        public ActionResult Integration()
        {
           try
            {
                using (IntigrationService objIntegration = new IntigrationService())
                {
                    var objResult = objIntegration.GetXeroGlAccount();                    
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

        public ActionResult GetXeroTracking()
        {
            try
            {
                using (IntigrationService objIntegration = new IntigrationService())
                {
                    var objResult = objIntegration.GetXeroTracking();
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