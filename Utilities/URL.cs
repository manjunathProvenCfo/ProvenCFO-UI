using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Utilities
{
    public  static class URL
    {

        public static string BaseApi = ConfigurationManager.AppSettings["provencfoapi"].ToString(); 
        //public static string tokenUrl = "https://localhost:44341/Token";
        //public static string Register = "https://localhost:44341/api/account/register";
        //public static string forgotPassword = "https://localhost:44341/api/Account/ChangePassword";
        //public static string logout = "https://localhost:44341/api/Account/Logout";

    }
}