using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class CommunicationController : Controller
    {
        // GET: Communication
        [CheckSession]
        public ActionResult CommunicationMain()
        {
            return View();
        }
    }
}