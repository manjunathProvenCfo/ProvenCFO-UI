using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ProvenCfoUI.Comman
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckXeroToken : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AgencyClient objAgy = new AgencyClient();
            Controller controller = filterContext.Controller as Controller;
            var session = HttpContext.Current.Session;
            if (AccountingPackageInstance.Instance.XeroToken == null)
            {
                using (ClientService objClient = new ClientService())
                {
                    List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)session["LoggedInUserPreferences"];
                    if (UserPref != null && UserPref.Count() > 0)
                    {
                        var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                        objAgy.SelectedClientNameID = Convert.ToInt32(selectedAgency.PreferanceValue);
                    }
                    else
                    {
                        objAgy.SelectedClientNameID = objAgy.ClientList[0].Id;
                    }
                    objAgy.ClientList = (List<ClientModel>)controller.TempData["ClientListActived"];
                    var client = objAgy.ClientList.Where(x => x.Id == objAgy.SelectedClientNameID).FirstOrDefault();
                    Common.ConnectClientAccoutingPackage(client);
                }
            }
        }
    }
}