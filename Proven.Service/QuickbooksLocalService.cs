using Newtonsoft.Json;
using Proven.Model;
using Proven.Service.AccountingPackage;
using QuickBooksSharp;
using QuickBooksSharp.Entities;
//using QuickBooksSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Linq;


namespace Proven.Service
{
    public class QuickbooksLocalService<T, V> : AccountingAppFatory<T, V>,  IDisposable
    {
        private bool isDisposed = false;
        string _ClientId;
        string _ClientSecret;
        string _scopes;
        string _callbackurl;
        private DataService _service;
        string _url;
        public QuickbooksLocalService(string ClientId, string ClientSecret, string scopes, string CallbackUrl = "")
        {
            _ClientId = ClientId;
            _ClientSecret = ClientSecret;
            _scopes = scopes;
            _callbackurl = CallbackUrl;
            _url = Convert.ToString(ConfigurationManager.AppSettings[""]);


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
            //Token = await this.ValidateToken(Token);
            bool IsProdEnviroment = false;
            var SandboxCompanyId = ConfigurationManager.AppSettings["QuickBooks_TestEnviroment_CompanyId"].ToString();
            if(TenentID == SandboxCompanyId) {
                IsProdEnviroment = true;
            }
            TokenResponse objToken = (TokenResponse)Convert.ChangeType(Token, typeof(TokenResponse));
            _service = new DataService(objToken.access_token,Convert.ToInt64(TenentID), IsProdEnviroment);
            var res = await _service.QueryAsync<Account>("SELECT * FROM Account");
            return (V)Convert.ChangeType(res.Response.Entities, typeof(V));
        }
        public override async Task<V> GetBankAccounts(T Token, string TenentID)
        {
            //Token = await this.ValidateToken(Token);
            bool IsProdEnviroment = IsProductionEnvirnoment(TenentID);            
            TokenResponse objToken = (TokenResponse)Convert.ChangeType(Token, typeof(TokenResponse));
            _service = new DataService(objToken.access_token, Convert.ToInt64(TenentID), IsProdEnviroment);
            
            var resBankAccount = await _service.QueryAsync<Account>("Select * from Account where AccountType = 'Bank'");
            var resCreditCard = await _service.QueryAsync<Account>("Select * from Account where AccountType = 'Credit Card'");                      
            QuickBooksSharp.Entities.Account[] CombinationOfAllAccount = new QuickBooksSharp.Entities.Account[resBankAccount.Response.Entities.Length + resCreditCard.Response.Entities.Length];
            resBankAccount.Response.Entities.CopyTo(CombinationOfAllAccount,0);
            resCreditCard.Response.Entities.CopyTo(CombinationOfAllAccount, resBankAccount.Response.Entities.Length);            
            return (V)Convert.ChangeType(CombinationOfAllAccount, typeof(V));
        }
        private bool IsProductionEnvirnoment(string TenentID)
        {
            bool IsProdEnviroment = false;
            var SandboxCompanyId = ConfigurationManager.AppSettings["QuickBooks_TestEnviroment_CompanyId"].ToString();
            if (TenentID == SandboxCompanyId)
            {
                IsProdEnviroment = true;
            }
            return IsProdEnviroment;

        }
        public override async Task<V> GetReportProfitAndLossAsync(T Token, string TenentID, DateTime? fromDate = null, DateTime? toDate = null, int? periods = null, string timeframe = null, string trackingCategoryID = null, string trackingOptionID = null, string trackingCategoryID2 = null, string trackingOptionID2 = null, bool? standardLayout = null, bool? paymentsOnly = null)
        {
            System.Collections.Generic.Dictionary<string, string> dic = new System. Collections.Generic.Dictionary<string, string>();
            bool IsProdEnviroment = IsProductionEnvirnoment(TenentID);

            if (fromDate != null) dic.Add("start_date", fromDate.Value.ToString("yyyy-MM-dd"));
            if(toDate != null) dic.Add("end_date", toDate.Value.ToString("yyyy-MM-dd"));
            if(timeframe != null) dic.Add("summarize_column_by", timeframe);

            TokenResponse objToken = (TokenResponse)Convert.ChangeType(Token, typeof(TokenResponse));
            _service = new DataService(objToken.access_token, Convert.ToInt64(TenentID), IsProdEnviroment);
            var res = await _service.GetReportAsync("ProfitAndLoss", dic);

            return (V)Convert.ChangeType(res, typeof(V));
            
        }

        public override AppTokenInfoMain GetSavedToken(int AgencyID)
        {
            var response = Prodclient.GetAsync("Xero/GetSavedToken?AgencyID=" + AgencyID).Result;
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
                objToken.expires_in = TimeSpan.FromSeconds(SavedToken.expires_in.Value);
                objToken.id_token = SavedToken.id_token;
                objToken.access_token = SavedToken.access_token;
                objToken.refresh_token = SavedToken.refresh_token;
                if (SavedToken.ModifiedDate == null) {
                    SavedToken.ModifiedDate = SavedToken.CreatedDate;       
                 }
                DateTime tokenExpairedAt = SavedToken.ModifiedDate.Value.AddSeconds(SavedToken.expires_in.Value);
                if (DateTime.UtcNow >= tokenExpairedAt)
                {
                    dynamic token = objToken;
                    dynamic t = RefreshToken((T)token).GetAwaiter().GetResult();
                    return (T)Convert.ChangeType(t, typeof(T));
                }

                return (T)Convert.ChangeType(objToken, typeof(T));
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public override T getSavedTokenFormat(int AgencyId)
        {
            try
            {
                var SavedToken = GetSavedToken(AgencyId).ResultData;
                TokenResponse objToken = new TokenResponse();                
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
            try
            {
                var quickbookToken = (TokenResponse)Convert.ChangeType(Token, typeof(TokenResponse));
                var res = await new AuthenticationService().RefreshOAuthTokenAsync(_ClientId, _ClientSecret, quickbookToken.refresh_token);

                //if (res.refresh_token != Convert.ToString(Token))
                //    TestHelper.PersistNewRefreshToken(res.refresh_token);
                return (T)Convert.ChangeType(res, typeof(T));
                //throw new NotImplementedException();
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }

        public override ReturnModel UpdateToken(V tokenInfoVM)
        {
            return ProdPostAsync<ReturnModel, V>("Xero/UpdateToken", tokenInfoVM).Result;
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

        public override Task<V> GetOrganisationsId(T xeroToken, string XeroTenentID)
        {
            throw new NotImplementedException();
        }

        public override async Task<T> ValidateToken(T Token)
        {
            var quickbookToken = (TokenResponse)Convert.ChangeType(Token, typeof(TokenResponse));
            var NewToken = await this.RefreshToken(Token);
            return (T)Convert.ChangeType(NewToken, typeof(T));
        }

        public override string GenerateAuthorizationPromptUrl()
        {
            IEnumerable<string> scopeslist = new string[] { "com.intuit.quickbooks.accounting", "openid", "email","profile" };
            var res = new AuthenticationService().GenerateAuthorizationPromptUrl(_ClientId, scopeslist, _callbackurl, "");
            return (string)res;
        }

        public override ReturnAsyncModel UpdateAccessToken(V tokenInfoVM)
        {
            throw new NotImplementedException();
        }

        public override Task<V> GetBankStatementsAsync(T xeroToken, string xeroTenantId, string bankAccountID = null, string dateFrom = null, string dateTo = null, string order = null, int? page = null, DateTime? ifModifiedSince = null, string status = null)
        {
            throw new NotImplementedException();
        }

        public override Task<DateTime> GetEndOfYearLockDate(T xToken, ClientModel client)
        {
            throw new NotImplementedException();
        }
    }
}
