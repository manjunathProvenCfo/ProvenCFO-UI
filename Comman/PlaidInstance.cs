using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Comman
{
    public sealed class PlaidInstance
    {
        private static PlaidInstance _instance = null;
        private static readonly object _syncObject = new object();
        private PlaidInstance()
        {

        }
        public static PlaidInstance Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncObject)
                    {
                        if (_instance == null)
                        {
                            _instance = new PlaidInstance();
                        }
                    }
                }
                return _instance;
            }
        }
        public string ClientID { get;  } = ConfigurationManager.AppSettings["Plaid_ClientID"];
        public string ClientSecret { get;  } = ConfigurationManager.AppSettings["Plaid_ClientSecret"];
        public string link_token { get; set; }
        public string public_token { get; set; }
        public string access_token { get; set; }
        public string[] institution_id { get; set; }
        public string client_name { get; set; }
        public string[] country_codes { get;  } = new string[] { "US" };
        public string language { get;  } = "en";
        public string[] products { get; set; } = new string[] { "auth", "transactions" };
        public Acklann.Plaid.Environment Environment { get; set; } = Plaid.GetEnvironment();
        public PlaidUser PlaidUser { get; set; }
    }
    public class PlaidUser
    {
        public string client_user_id { get; set; }
    }
}