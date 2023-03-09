using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Client;
using Xero.NetStandard.OAuth2.Config;
using Xero.NetStandard.OAuth2.Models;
using Xero.NetStandard.OAuth2.Token;
using Xero.NetStandard.OAuth2.Api;
using System.Net.Http;
using IdentityModel.Client;
using Newtonsoft.Json;
using Proven.Model;
using Proven.Service.AccountingPackage;
using Xero.NetStandard.OAuth2.Model.Accounting;

namespace Proven.Service
{
    public class XeroService<T, V> : AccountingAppFatory<T, V>, IDisposable
    {
        private bool isDisposed = false;
        private const string Vurl = @"https://appqa.provencfo.com/Integration/Callback";
        string _ClientId;
        string _ClientSecret;
        string _scopes;
        string _callbackurl;
        XeroClient XeroClient;
        private AccountingApi _accountinstance;
        XeroConfiguration xeroConfig;
        IXeroToken _XeroToken;
        public XeroService(string ClientId, string ClientSecret, string scopes, string AppName, string CallbackUrl = "")
        {
            _ClientId = ClientId;
            _ClientSecret = ClientSecret;
            _scopes = scopes;
            _callbackurl = CallbackUrl + "Integration/Callback";
            xeroConfig = new XeroConfiguration();
            xeroConfig.AppName = AppName;
            xeroConfig.CallbackUri = new Uri(CallbackUrl == "" ? Vurl : _callbackurl);
            xeroConfig.ClientId = _ClientId;
            xeroConfig.ClientSecret = _ClientSecret;
            xeroConfig.Scope = _scopes;
            xeroConfig.State = "123";
            XeroClient = new XeroClient(xeroConfig);
            _accountinstance = new AccountingApi();

        }
        public override async Task<T> LoginAccess()
        {
            try
            {
                var result = await XeroClient.RequestClientCredentialsTokenAsync();
                return (T)result;

            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

        }

        public override async Task<T> LoginXeroAccesswithCode(string code)
        {
            try
            {
                var result = await XeroClient.RequestAccessTokenAsync(code);
                return (T)result;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            
        }
        public string BuildLoginUri()
        {
            try
            {
                var XeroClient1 = new XeroClient(xeroConfig);
                var clientState = Guid.NewGuid().ToString();
                var url = XeroClient1.BuildLoginUri();
                return url;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

        }
        public override T getTokenFormat(V TokenInfoVM)
        {
            try
            {
                var Token = (TokenInfoVM)Convert.ChangeType(TokenInfoVM, typeof(TokenInfoVM));
                IXeroToken token = new XeroOAuth2Token
                {
                    AccessToken = Token.access_token,
                    RefreshToken = Token.refresh_token,
                    ExpiresAtUtc = Token.ModifiedDate.Value.AddSeconds(Token.expires_in.Value),
                    IdToken = Token.id_token,
                    // This is required for refresh logic down in GetCurrentValidTokenAsync.
                    //ExpiresAtUtc =  DateTime.MaxValue
                };
                if (DateTime.UtcNow >= token.ExpiresAtUtc)
                {
                    token = (IXeroToken)(T)RefreshToken((T)token).GetAwaiter().GetResult();
                }
                return (T)token;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

        }
        public override T getSavedTokenFormat(int AgencyId)
        {
            try
            {
                var SavedToken = GetSavedToken(AgencyId).ResultData;
                IXeroToken token = new XeroOAuth2Token
                {
                    AccessToken = SavedToken.access_token,
                    RefreshToken = SavedToken.refresh_token,

                    IdToken = SavedToken.id_token
                    // This is required for refresh logic down in GetCurrentValidTokenAsync.                    
                };
                return (T)token;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }
        public override async Task<T> RefreshToken(T xeroToken)
        {
            try
            {
                IXeroToken objToken = (IXeroToken)xeroToken;//(IXeroToken)Convert.ChangeType(xeroToken, typeof(IXeroToken));
                //return await XeroClient.RefreshAccessTokenAsync(xeroToken);
                var result = await XeroClient.RefreshAccessTokenAsync(objToken);
                return (T)result;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

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

        public override ReturnModel UpdateToken(V tokenInfoVM)
        {
            return ProdPostAsync<ReturnModel, V>("Xero/UpdateToken", tokenInfoVM).Result;

        }
        public override ReturnAsyncModel UpdateAccessToken(V tokenInfoVM)
        {
            return ProdPostAsync<ReturnAsyncModel, V>("Xero/UpdateAccessToken", tokenInfoVM).Result;

        }

        public override async Task<T> ConnnectApp(V Token)
        {
            //return await RequestClientCredentialsTokenAsync();
            var objToken = (IXeroToken)Convert.ChangeType(Token, typeof(IXeroToken));
            var result = await XeroClient.GetConnectionsAsync(objToken);
            return (T)Convert.ChangeType(result, typeof(List<T>));
        }
        public override async Task<V> GetConnections()
        {
            IXeroToken Token = (IXeroToken)LoginAccess().Result;
            var result = await XeroClient.GetConnectionsAsync(Token);
            return (V)Convert.ChangeType(result, typeof(List<V>));
        }

        public async Task<V> GetXeroConnections(T Token)
        {
            var result = await XeroClient.GetConnectionsAsync((IXeroToken)Token);
            return (V)Convert.ChangeType(result, typeof(V));
        }
        /// <summary>
        /// Requests a fully formed IXeroToken with list of tenants filled
        /// </summary>
        /// <returns></returns>
        //HttpClient _httpClient = new HttpClient();
        //public async Task<IXeroToken> RequestClientCredentialsTokenAsync()
        //{

        //    var response = await _httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
        //    {
        //        Address = "https://identity.xero.com/connect/token",
        //        ClientId = xeroConfig.ClientId,
        //        ClientSecret = xeroConfig.ClientSecret,
        //        Scope = xeroConfig.Scope
        //    });

        //    if (response.IsError)
        //    {
        //        throw new Exception(response.Error);
        //    }

        //    var xeroToken = new XeroOAuth2Token()
        //    {
        //        AccessToken = response.AccessToken,
        //        ExpiresAtUtc = DateTime.UtcNow.AddSeconds(response.ExpiresIn)
        //    };
        //    xeroToken.Tenants = await XeroClient.GetConnectionsAsync(xeroToken);
        //    return xeroToken;

        //}
        /// <summary>
        /// Get's a list of Tokens given the accesstoken
        /// </summary>
        /// <param name="xeroToken"></param>
        /// <returns>List of Tenants attached to accesstoken</returns>

        public override async Task<V> GetContacts(T xeroToken, string XeroTenentID = "")
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetContactsAsync(Token.AccessToken, XeroTenentID);
            return (V)Convert.ChangeType(result, typeof(V));
        }
        public override async Task<V> GetContact(T xeroToken, string XeroTenentID = "", Guid? ContactID = null)
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetContactAsync(Token.AccessToken, XeroTenentID, ContactID.Value);
            return (V)Convert.ChangeType(result, typeof(V));
        }

        public override async Task<T> GetBankSummary(V xeroToken, string XeroTenentID)
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetReportBankSummaryAsync(Token.AccessToken, XeroTenentID);
            return (T)Convert.ChangeType(result, typeof(T));
        }
        public override async Task<V> GetInvoices(T xeroToken, string XeroTenentID)
        {
            var freshToken = await RefreshToken(xeroToken);
            var freshedToken = (IXeroToken)Convert.ChangeType(freshToken, typeof(IXeroToken));
            var result = await _accountinstance.GetInvoicesAsync(freshedToken.AccessToken, XeroTenentID);
            return (V)Convert.ChangeType(result, typeof(V));
        }
        public override async Task<V> GetGLAccounts(T xeroToken, string XeroTenentID)
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetAccountsAsync(Token.AccessToken, XeroTenentID);
            return (V)Convert.ChangeType(result._Accounts, typeof(V));
        }
        public override async Task<V> GetBankAccounts(T Token, string TenentID)
        {
            var objToken = (IXeroToken)Token;

            var result = await _accountinstance.GetAccountsAsync(objToken.AccessToken, TenentID);
            var finalresult = result._Accounts.Where(x => x.Status.ToString() == "ACTIVE" && x.Type.ToString() == "BANK").ToList();
            return (V)Convert.ChangeType(finalresult, typeof(V));
        }
        public override async Task<V> GetTrackingCategories(T xeroToken, string XeroTenentID)
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetTrackingCategoriesAsync(Token.AccessToken, XeroTenentID);
            return (V)Convert.ChangeType(result, typeof(V));
        }
        public override async Task<V> GetReportProfitAndLossAsync(T xeroToken, string XeroTenentID, DateTime? fromDate = null, DateTime? toDate = null, int? periods = null, string timeframe = null, string trackingCategoryID = null, string trackingOptionID = null, string trackingCategoryID2 = null, string trackingOptionID2 = null, bool? standardLayout = null, bool? paymentsOnly = null)
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetReportProfitAndLossAsync(Token.AccessToken, XeroTenentID, fromDate, toDate, periods, timeframe, trackingCategoryID, trackingCategoryID2, trackingOptionID, trackingOptionID2, standardLayout, paymentsOnly);
            return (V)Convert.ChangeType(result, typeof(V));
        }
        public override async Task<DateTime> GetEndOfYearLockDate(T xToken, ClientModel client)
        {
            DateTime? EndOfYearLockDatey = null;
            dynamic XeroOrganization = null;
            using (XeroOrganization = new XeroBankTransaction<IXeroToken, Xero.NetStandard.OAuth2.Model.Accounting.Organisations>(client.APIClientID, client.APIClientSecret, client.APIScope, ""))
            {
                Organisations org = await XeroOrganization.GetOrganisationsAsync(xToken, client.XeroID);
                var result = org._Organisations.Where(x => x.OrganisationID == Guid.Parse(client.XeroID)).FirstOrDefault();
                if (result != null && result.EndOfYearLockDate != null)
                {
                    EndOfYearLockDatey = result.EndOfYearLockDate.Value;
                }

            }
            return (DateTime)EndOfYearLockDatey;

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

        public override async Task<V> GetOrganisationsId(T xeroToken, string XeroTenentID = "")
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetOrganisationsAsync(Token.AccessToken, XeroTenentID);
            return (V)Convert.ChangeType(result, typeof(V));
        }

        public override Task<T> ValidateToken(T xeroToken)
        {
            throw new NotImplementedException();
        }

        public override string GenerateAuthorizationPromptUrl()
        {
            var clientState = Guid.NewGuid().ToString();
            return XeroClient.BuildLoginUri(clientState);
        }

        public override Task<V> GetBankStatementsAsync(T xeroToken, string xeroTenantId, string bankAccountID = null, string dateFrom = null, string dateTo = null, string order = null, int? page = null, DateTime? ifModifiedSince = null, string status = null)
        {
            throw new NotImplementedException();
        }
    }
}
