using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ProvenCfoUI.Comman
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class CheckSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null)
            {
                if (context.Session.IsNewSession)
                {
                    string sessionCookie = context.Request.Headers["Cookie"];

                    if ((sessionCookie != null) && (sessionCookie.IndexOf("ASP.NET_SessionId") >= 0 ))
                    {
                        FormsAuthentication.SignOut();
                        string redirectTo = "~/Home/LoginSessionExpaired";
                        if (!string.IsNullOrEmpty(context.Request.RawUrl))
                        {
                            redirectTo = string.Format("~/Home/LoginSessionExpaired", HttpUtility.UrlEncode(context.Request.RawUrl));
                            filterContext.Result = new RedirectResult(redirectTo);
                            return;
                        }

                    }
                }
                if (context.Session["UserId"] == null || Convert.ToString(context.Session["UserId"]) == string.Empty)
                {
                    filterContext.Result = new RedirectResult("~/Home/Login");
                    return;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}