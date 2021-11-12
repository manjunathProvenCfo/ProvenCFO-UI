using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Dashboard()
        {
            ClientModel objClient = new ClientModel();
            List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
            if (UserPref != null && UserPref.Count() > 0)
            {
                using (ClientService obj = new ClientService())
                {
                    var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    var objResult = obj.GetClientById(Convert.ToInt32(selectedAgency.PreferanceValue));
                    if (!string.IsNullOrEmpty(objResult.DashboardURLId) && objResult.DashboardId != null && objResult.DashboardId != 0)
                    {
                        return View(objResult);
                    }
                    else
                    {
                        return RedirectToAction("DashboardError", "Dashboard");
                    }
                }
            }
            return RedirectToAction("DashboardError", "Dashboard");
        }

        public ActionResult DashboardError()
        {
            return PartialView("DashboardError");
        }
    }
}