using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class BaseController : Controller
    {
        [CheckSession]
        // GET: Base
        public ActionResult Index()
        {
            return View();
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