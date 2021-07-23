using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class ReconciliationController : Controller
    {
        // GET: Reconciliation
        [CheckSession]
        public ActionResult ReconciliationMain()
        {
            return View();
        }
    }
}