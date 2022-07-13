using Acklann.Plaid;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProvenCfoUI.Comman
{
    public class Plaid : IDisposable
    {
        private bool isDisposed = false;
        string _ClientId;
        string _ClientSecret;
        string[] _products;
        string _callbackurl;
        string _language;
        string[] _countryCodes;
        public Acklann.Plaid.Environment _env;
        PlaidClient _plaidclient;
        public Plaid(string ClientId, string ClientSecret, string Language, string[] Products,string[] CountryCodes, Acklann.Plaid.Environment environment, string CallbackUrl = "")
        {
            _ClientId = ClientId;
            _ClientSecret = ClientSecret;
            _products = Products;
            _callbackurl = CallbackUrl;
            _language = Language;
            _countryCodes = CountryCodes;
            _env = environment;
            _plaidclient = new PlaidClient(_ClientId, _ClientSecret, null, _env);
        }
        public async Task<string> getLinkToken(string ClientName)
        {
            try
            {
                // Arrange
                
                var response = await _plaidclient.CreateLinkToken(new Acklann.Plaid.Management.CreateLinkTokenRequest
                {
                    ClientName = ClientName,
                    ClientId = _ClientId,
                    Secret = _ClientSecret,
                    CountryCodes = _countryCodes,
                    Products = _products,
                    User = new Acklann.Plaid.Management.CreateLinkTokenRequest.UserInfo
                    {
                        ClientUserId = Guid.NewGuid().ToString()
                    }
                });
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.LinkToken.ToString();
                }
                return String.Empty;

            }
            catch (Exception e)
            {
                throw;
                return String.Empty;
            }
            
        }
        public async Task<string> ExchangeTokenAsync(string public_token)
        {
            try
            {
                var response = await _plaidclient.ExchangeTokenAsync(new Acklann.Plaid.Management.ExchangeTokenRequest
                {
                    ClientId = _ClientId,
                    Secret = _ClientSecret,
                    PublicToken = public_token

                });
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return response.AccessToken.ToString();
                }
                return String.Empty;
            }
            catch (Exception)
            {
                throw;
                return String.Empty;
            }
            
        }
        
        /// <summary>
        /// Function to return the paid Environment 
        /// </summary>
        /// <returns></returns>
        public static Acklann.Plaid.Environment GetEnvironment()
        {
            Acklann.Plaid.Environment Env;
            var environmentstring = ConfigurationManager.AppSettings["Plaid_Environment"];
            switch (environmentstring)
            {
                case "development":
                    Env = Acklann.Plaid.Environment.Development;
                    break;
                case "dev":
                    Env = Acklann.Plaid.Environment.Development;
                    break;
                case "production":
                    Env = Acklann.Plaid.Environment.Production;
                    break;
                case "prod":
                    Env = Acklann.Plaid.Environment.Production;
                    break;
                default:
                    Env = Acklann.Plaid.Environment.Sandbox;
                    break;
            }
            return Env;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }
            this.isDisposed = true;
        }
    }
}