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

        [CheckSession]
        [HttpPost]
        public ActionResult CreateXeroGlAccount(XeroGlAccountVM xeroGlAccount)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (IntigrationService objRole = new IntigrationService())
                    {
                       XeroGlAccountVM RoleVM = new XeroGlAccountVM();
                        //var LoginUserid = Session["UserId"].ToString();
                        //var result = new Proven.Model.RolesViewModel();

                        //if (Role.id == null)
                        //{
                        //var Existresult = objRole.GetRoleByName(Role.name);
                        //if (Existresult != null)
                        //{
                        //    ViewBag.ErrorMessage = "Exist";
                            return View("AddXeroGlAccount", RoleVM);
                        //}

                        //result = objRole.CreateXeroGlAccount();
                        ViewBag.ErrorMessage = "Created";
                        //}
                    }
                }
                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    throw ex;
                }
            }
            return View();
        }

        //public ActionResult Integration1()
        //{
        //    try
        //    {
        //        using (IntigrationService objIntegration = new IntigrationService())
        //        {
        //            var objResult = objIntegration.GetXeroTracking();
        //            return View(objResult.ResultData);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error(Utltity.Log4NetExceptionLog(ex));
        //        throw ex;
        //    }
        //}
    }
}