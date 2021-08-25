using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;

namespace Proven.Model
{
    public class UserContext
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
        public string UserFullName { get; set; }
        public string UserType { get; set; }
        public UserContext(IPrincipal user)
        {
            if (user.Identity.IsAuthenticated)
            {
                var session = HttpContext.Current.Session;
                HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                string[] userData = ticket.Name.Split(',');
                session["UserId"] = UserId = userData[0];
                session["UserName"] = UserName = userData[1];
                session["LoginName"] = LoginName = userData[2];
                session["UserFullName"] = UserFullName = userData[3];
                session["UserType"] = UserType = userData[4];
            }
        }
    }
}
