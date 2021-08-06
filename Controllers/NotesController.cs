using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class NotesController : Controller
    {
        string errorMessage = string.Empty;
        string errorDescription = string.Empty;
        // GET: Notes
        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        public ActionResult Index()
        {
            return View();
        }
    }
}