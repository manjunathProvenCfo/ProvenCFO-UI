﻿using log4net;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Controllers;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SweetAlert.Controllers
{
    [Exception_Filters]
    public class SweetController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Sweet
        [CheckSession]
        public ActionResult Alert()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
            
        }
    }
}