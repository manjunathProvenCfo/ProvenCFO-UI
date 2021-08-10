using log4net;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class BaseController : Controller
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [CheckSession]
        // GET: Base
        public ActionResult Index()
        {
            return View();
        }

        [CheckSession]
        public ActionResult Download(string fileName)
        {
            try
            {
                string fullPath = System.IO.Path.Combine(Server.MapPath("~/ExportFile/"), fileName);
                byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                return File(fileByteArray, "application/vnd.ms-excel", fileName);

            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
    }
}