using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetAlert.Controllers
{
    public class SweetController : Controller
    {

        // GET: Sweet
        [CheckSession]
        public ActionResult Alert()
        {
            return View();
        }
    }
}