using Proven.Service;
using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class BillableEntitiesController : Controller
    {
        string errorMessage = string.Empty;
        string errorDescription = string.Empty;
        // GET: BillableEntities

        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        public ActionResult GetAllBillableEntitiesList()
        {
            using (BillableEntitiesService obj = new BillableEntitiesService())
            {
                var objResult = obj.GetAllBillableEntitiesList();

                return View(objResult.ResultData);
            }
        }
    }
}