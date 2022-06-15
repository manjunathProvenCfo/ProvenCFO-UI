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
    public class CheckSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext context = HttpContext.Current;
            if (context.Session != null)
            {
                if (context.Session.IsNewSession)
                {
                    if (HttpContext.Current.Request.IsAuthenticated)
                    {
                        FormsAuthentication.SignOut();
                        string redirectTo = "~/Home/LoginSessionExpired";
                        if (!string.IsNullOrEmpty(context.Request.RawUrl))
                        {
                            redirectTo = string.Format("~/Home/LoginSessionExpired", HttpUtility.UrlEncode(context.Request.RawUrl));
                            filterContext.Result = new RedirectResult(redirectTo);
                            return;
                        }

                    }
                }
                if (HttpContext.Current.Request.IsAuthenticated == false)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }
                else
                {
                    var session = HttpContext.Current.Session;
                    HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    string[] userData = ticket.Name.Split(',');
                    session["UserId"] = userData[0];
                    session["UserName"] = userData[1];
                    session["LoginName"] = userData[2];
                    session["UserFullName"] = userData[3];
                    session["UserType"] = userData[4];
                    session["SessionTimeout"] = session.Timeout;
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }
}