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
    public class BaseController : Controller
    {
        //private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //[CheckSession]
        // GET: Basess
        protected new UserContext User => new UserContext(HttpContext?.User);
        public BaseController()
        {
            var userId = User.UserId;
        }
    }
}