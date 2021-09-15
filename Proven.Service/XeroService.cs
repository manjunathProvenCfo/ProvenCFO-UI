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

namespace Proven.Service
{
    public class XeroService : BaseService
    {
        private const string V = @"https://provencfoqa.codewarriorsllc.com/AgencyService/AgencyHome";
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
            _callbackurl = CallbackUrl;
            xeroConfig = new XeroConfiguration();
            xeroConfig.AppName = AppName;
            xeroConfig.CallbackUri =  new Uri(V);
            xeroConfig.ClientId = _ClientId;
            xeroConfig.ClientSecret = _ClientSecret;
            xeroConfig.Scope = _scopes;
            xeroConfig.State = "123";
            XeroClient = new XeroClient(xeroConfig);
            _accountinstance = new AccountingApi();

        }
        public async Task<IXeroToken> LoginXeroAccess()
        {
            try
            {               
                return await XeroClient.RequestClientCredentialsTokenAsync();
                    
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

        }
        
        public async Task<IXeroToken> LoginXeroAccesswithCode(string code)
        {
            try
            {
                return await XeroClient.RequestAccessTokenAsync(code);
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
               var  XeroClient1 = new XeroClient(xeroConfig);
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
        public IXeroToken getTokenFormat(XeroTokenInfoVM xeroTokenInfo)
        {
            try
            {
                IXeroToken token = new XeroOAuth2Token
                {
                    AccessToken = xeroTokenInfo.access_token,
                    RefreshToken = xeroTokenInfo.refresh_token,
                    IdToken = xeroTokenInfo.id_token
                    // This is required for refresh logic down in GetCurrentValidTokenAsync.
                    //ExpiresAtUtc =  DateTime.MaxValue
                };
                return token;
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

        }
        public async Task<IXeroToken> RefreshToken(IXeroToken xeroToken)
        {
            try
            {
                //return await XeroClient.RefreshAccessTokenAsync(xeroToken);
                return await XeroClient.RefreshAccessTokenAsync(xeroToken);
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

        }
        public XeromainTokenInfo GetSavedXeroToken(int AgencyID)
        {
            return GetAsync<XeromainTokenInfo>("Xero/GetXeroToken?AgencyID=" + AgencyID).Result;
            
        }
        public ReturnModel UpdateXeroToken(XeroTokenInfoVM tokenInfoVM)
        {
            return PostAsync<ReturnModel, XeroTokenInfoVM>("Xero/UpdateXeroToken", tokenInfoVM).Result;

        }
        public async Task<List<Tenant>> ConnnectApp(IXeroToken xeroToken)
        {
            //return await RequestClientCredentialsTokenAsync();            
            return await XeroClient.GetConnectionsAsync(xeroToken);
        }
        public async Task<List<Tenant>> GetConnections()
        {
            var Token = LoginXeroAccess();
            return await XeroClient.GetConnectionsAsync(Token.Result);
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

        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Contacts> GetContacts(IXeroToken xeroToken, string XeroTenentID)
        {
            return await _accountinstance.GetContactsAsync(xeroToken.AccessToken, XeroTenentID);
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.ReportWithRows> GetBankSummary(IXeroToken xeroToken, string XeroTenentID)
        {
            var obj = await _accountinstance.GetReportBankSummaryAsync(xeroToken.AccessToken, XeroTenentID);
            return obj;
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Invoices> GetInvoices(IXeroToken xeroToken, string XeroTenentID)
        {
            var freshToken = await RefreshToken(xeroToken);
            var obj = await _accountinstance.GetInvoicesAsync(freshToken.AccessToken, XeroTenentID);
            return obj;
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Accounts> GetGLAccounts(IXeroToken xeroToken, string XeroTenentID)
        {
            var obj = await _accountinstance.GetAccountsAsync(xeroToken.AccessToken, XeroTenentID);
            return obj;
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.TrackingCategories> GetTrackingCategories(IXeroToken xeroToken, string XeroTenentID)
        {
            var obj = await _accountinstance.GetTrackingCategoriesAsync(xeroToken.AccessToken, XeroTenentID);
            return obj;
        }

    }
}
