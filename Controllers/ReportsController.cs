using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        [CheckSession]
        public ActionResult ReportMain()
        {
            return View();
        }
    }
}