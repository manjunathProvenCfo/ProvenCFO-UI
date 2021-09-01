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

namespace Proven.Service
{
    public class XeroService : BaseService
    {
        string _ClientId;
        string _ClientSecret;
        string _scopes;
        string _callbackurl;
        XeroClient XeroClient;
        private AccountingApi _accountinstance;
        XeroConfiguration xeroConfig;
        IXeroToken _XeroToken;
        public XeroService(string ClientId, string ClientSecret, string scopes, string CallbackUrl = "")
        {
            _ClientId = ClientId;
            _ClientSecret = ClientSecret;
            _scopes = scopes;
            _callbackurl = CallbackUrl;
            xeroConfig = new XeroConfiguration();
            xeroConfig.ClientId = _ClientId;
            xeroConfig.ClientSecret = _ClientSecret;            
            xeroConfig.Scope = _scopes;
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
        public async Task<IXeroToken> RefreshToken(IXeroToken xeroToken)
        {
            try
            {
                return await XeroClient.RefreshAccessTokenAsync(xeroToken);
            }
            catch (Exception ex)
            {
                throw ex;

            }
            //return await RequestClientCredentialsTokenAsync();            

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
        
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Contacts> GetContacts(IXeroToken xeroToken)
        {
            return await _accountinstance.GetContactsAsync(xeroToken.AccessToken, xeroToken.Tenants[0].TenantId.ToString());
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.ReportWithRows> GetBankSummary(IXeroToken xeroToken)
        {
            var obj = await _accountinstance.GetReportBankSummaryAsync(xeroToken.AccessToken, xeroToken.Tenants[0].TenantId.ToString());
            return obj;
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Invoices> GetInvoices(IXeroToken xeroToken)
        {
            var obj = await _accountinstance.GetInvoicesAsync(xeroToken.AccessToken, xeroToken.Tenants[0].TenantId.ToString());
            return obj;
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.Accounts> GetGLAccounts(IXeroToken xeroToken)
        {
            var obj = await _accountinstance.GetAccountsAsync(xeroToken.AccessToken, xeroToken.Tenants[0].TenantId.ToString());
            return obj;
        }
        public async Task<Xero.NetStandard.OAuth2.Model.Accounting.TrackingCategories> GetTrackingCategories(IXeroToken xeroToken)
        {
            var obj = await _accountinstance.GetTrackingCategoriesAsync(xeroToken.AccessToken, xeroToken.Tenants[0].TenantId.ToString());
            return obj;
        }

    }
}
