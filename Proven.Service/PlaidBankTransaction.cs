using Acklann.Plaid;
using Proven.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Proven.Service
{
    
    public class PlaidBankTransaction<T, V> : BankTransctionFatory<T, V>, IDisposable
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
        public PlaidBankTransaction(string ClientId, string ClientSecret, string Language, string[] Products, string[] CountryCodes, Acklann.Plaid.Environment environment, string CallbackUrl = "")
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
        public async Task<V> getLinkToken(T ClientName)
        {
            try
            {
                // Arrange

                var response = await _plaidclient.CreateLinkToken(new Acklann.Plaid.Management.CreateLinkTokenRequest
                {
                    ClientName = (string)Convert.ChangeType(ClientName, typeof(string)) ,
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
                    return (V)Convert.ChangeType(response.LinkToken.ToString(), typeof(V));
                }
                return (V)Convert.ChangeType(String.Empty, typeof(V));
               

            }
            catch (Exception e)
            {
                throw;
                return (V)Convert.ChangeType(String.Empty, typeof(V));
            }

        }
        public async Task<V> ExchangeTokenAsync(T public_token)
        {
            try
            {
                var response = await _plaidclient.ExchangeTokenAsync(new Acklann.Plaid.Management.ExchangeTokenRequest
                {
                    ClientId = _ClientId,
                    Secret = _ClientSecret,
                    PublicToken = (String)Convert.ChangeType(public_token, typeof(string))

                });
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return (V)Convert.ChangeType(response.AccessToken.ToString(), typeof(V));
                }
                return (V)Convert.ChangeType(String.Empty, typeof(V));
            }
            catch (Exception)
            {
                throw;
                return (V)Convert.ChangeType(String.Empty, typeof(V));
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
        public override async Task<V> GetBankTransactionsAsync(T Token, string TenentID, dynamic whereCause)
        {
            //var request = new Transactions.GetTransactionsRequest()
            var queryParameters = (PlaidRequestModelsVM)Convert.ChangeType(whereCause, typeof(PlaidRequestModelsVM));
            var response = await _plaidclient.FetchTransactionsAsync(new Acklann.Plaid.Transactions.GetTransactionsRequest {
                AccessToken = (String)Convert.ChangeType(Token, typeof(string)),
                ClientId = _ClientId,
                Secret = _ClientSecret,
                StartDate = queryParameters.start_date.Value,
                EndDate = queryParameters.end_date.Value, 
            });
            return (V)Convert.ChangeType(response, typeof(V)); ;
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
