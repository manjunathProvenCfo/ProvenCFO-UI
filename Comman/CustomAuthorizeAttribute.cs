using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProvenCfoUI.Comman
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IPrincipal
    {
        private readonly string[] allowedUsertype;
        private List<UserSecurityVM> _roleList;
        public CustomAuthorizeAttribute(params string[] usertype)
        {
            this.allowedUsertype = usertype;
        }

        public IIdentity Identity => throw new NotImplementedException();      

        public bool IsInRole(string role)
        {
            if (_roleList == null && _roleList.Count > 0)
            {
                if (role.Length > 3)
                {
                    if (_roleList.Where(x => x.RoleName == role).ToList().Count > 0)
                    {
                        return true;
                    }
                }
                else if (role.Length == 3)
                {
                    if (_roleList.Where(x => x.FeatureCode == role).ToList().Count > 0 || _roleList.Where(x => x.ModuleCode == role).ToList().Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            if (_roleList == null)
            {
                _roleList = (List<UserSecurityVM>)httpContext.Session["LoggedInUserUserSecurityModels"];
            }

            if (_roleList != null && _roleList.Count > 0)
            {
                foreach (var utype in allowedUsertype)
                {
                    var matchrolse = _roleList.Where(x => x.UserTypeName == utype);
                    if (matchrolse != null && matchrolse.Count() > 0) return true;
                }
            }
            return authorize;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectToRouteResult(
               new RouteValueDictionary
               {
                    { "controller", "Home" },
                    { "action", "UnAuthorized" }
               });
        }
    }
}