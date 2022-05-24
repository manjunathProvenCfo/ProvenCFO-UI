using Newtonsoft.Json;
using Proven.Model;
using Proven.Service.AccountingPackage;
using QuickBooksSharp;
using QuickBooksSharp.Entities;
//using QuickBooksSharp;
using System;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class QuickbooksLocalService<T, V> : AccountingAppFatory<T, V>, IDisposable
    {
        private bool isDisposed = false;
        string _ClientId;
        string _ClientSecret;
        string _scopes;
        string _callbackurl;
        private DataService _service;
        public QuickbooksLocalService(string ClientId, string ClientSecret, string scopes, string CallbackUrl = "")
        {
            _ClientId = ClientId;
            _ClientSecret = ClientSecret;
            _scopes = scopes;
            _callbackurl = CallbackUrl;
            
            
        }

        public override Task<T> ConnnectApp(V Token)
        {
            throw new NotImplementedException();
        }

        public override Task<V> GetConnections()
        {
            throw new NotImplementedException();
        }

        public override Task<V> GetContact(T Token, string TenentID = "", Guid? ContactID = null)
        {
            throw new NotImplementedException();
        }

        public override Task<V> GetContacts(T Token, string TenentID = "")
        {
            throw new NotImplementedException();
        }

        public override async Task<V> GetGLAccounts(T Token, string TenentID)
        {
            TokenResponse objToken = (TokenResponse)Convert.ChangeType(Token, typeof(TokenResponse));
            _service = new DataService(objToken.access_token,Convert.ToInt64(TenentID), true);
            var res = await _service.QueryAsync<Account>("SELECT * FROM Customer");
            return (V)Convert.ChangeType(res, typeof(V));
        }

        public override Task<V> GetReportProfitAndLossAsync(T Token, string TenentID, DateTime? fromDate = null, DateTime? toDate = null, int? periods = null, string timeframe = null, string trackingCategoryID = null, string trackingOptionID = null, string trackingCategoryID2 = null, string trackingOptionID2 = null, bool? standardLayout = null, bool? paymentsOnly = null)
        {
            throw new NotImplementedException();
        }

        public override AppTokenInfoMain GetSavedToken(int AgencyID)
        {
            var response = Prodclient.GetAsync("Xero/GetXeroToken?AgencyID=" + AgencyID).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<AppTokenInfoMain>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public override T getTokenFormat(V TokenInfo)
        {
            try
            {
                TokenResponse objToken = new TokenResponse();
                TokenInfoVM SavedToken = (TokenInfoVM)Convert.ChangeType(TokenInfo, typeof(TokenInfoVM));
                objToken.id_token = SavedToken.id_token;
                objToken.access_token = SavedToken.access_token;
                objToken.refresh_token = SavedToken.refresh_token;                             
                return (T)Convert.ChangeType(objToken, typeof(T));
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public override Task<V> GetTrackingCategories(T Token, string TenentID)
        {
            throw new NotImplementedException();
        }

        public override Task<T> LoginAccess()
        {
            throw new NotImplementedException();
        }

        public override Task<T> LoginXeroAccesswithCode(string code)
        {
            throw new NotImplementedException();
        }

        public override async Task<T> RefreshToken(T Token)
        {
            var quickbookToken = (TokenResponse)Convert.ChangeType(Token, typeof(TokenResponse));
            var res = await new AuthenticationService().RefreshOAuthTokenAsync(_ClientId, _ClientSecret, quickbookToken.refresh_token);

            //if (res.refresh_token != Convert.ToString(Token))
            //    TestHelper.PersistNewRefreshToken(res.refresh_token);
            return (T)Convert.ChangeType(res, typeof(T));
            //throw new NotImplementedException();
        }

        public override ReturnModel UpdateToken(V tokenInfoVM)
        {
            return ProdPostAsync<ReturnModel, V>("Xero/UpdateXeroToken", tokenInfoVM).Result;
        }
        public override Task<T> GetBankSummary(V xeroToken, string XeroTenentID)
        {
            throw new NotImplementedException();
        }

        public override Task<V> GetInvoices(T xeroToken, string XeroTenentID)
        {
            throw new NotImplementedException();
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
