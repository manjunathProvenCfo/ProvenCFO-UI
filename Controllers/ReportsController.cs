using log4net;
using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class ReportsController : Controller
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Reports
        [CheckSession]
        public ActionResult ReportsList()
        {
            return View();
        }
    }
}