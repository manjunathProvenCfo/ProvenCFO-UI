using log4net;
using Proven.Model;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [Exception_Filters]
    public class BaseController : Controller
    {
        //private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //[CheckSession]
        // GET: Basess
        //test
        protected new UserContext User => new UserContext(HttpContext?.User);
        public BaseController()
        {

        }

        public void IsReadOnlyUser()
        {
            if (!string.IsNullOrEmpty(User.UserType))
            {
                if (User.UserType == "2")
                    ViewBag.IsReadonlyUser = true;
                else
                    ViewBag.IsReadonlyUser = false;
            }
        }

        [CheckSession]
        public ActionResult Download(string fileName)
        {
            string fullPath = System.IO.Path.Combine(Server.MapPath("~/ExportFile/"), fileName);
            byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
            System.IO.File.Delete(fullPath);
            return File(fileByteArray, "application/vnd.ms-excel", fileName);
        }
    }
}