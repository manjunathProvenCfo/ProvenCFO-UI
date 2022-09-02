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

namespace Proven.Service
{
    public class XeroBankTransaction<T, V>: BankTransctionFatory<T, V>,IDisposable
    {
        private bool isDisposed = false;
        private const string Vurl = @"https://provencfoqa.codewarriorsllc.com/AgencyService/AgencyHome";
        string _ClientId;
        string _ClientSecret;
        string _scopes;
        string _callbackurl;
        XeroClient XeroClient;
        private AccountingApi _accountinstance;
        XeroConfiguration xeroConfig;
        IXeroToken _XeroToken;
        public XeroBankTransaction(string ClientId, string ClientSecret, string scopes, string AppName, string CallbackUrl = "")
        {
            _ClientId = ClientId;
            _ClientSecret = ClientSecret;
            _scopes = scopes;
            _callbackurl = CallbackUrl;
            xeroConfig = new XeroConfiguration();
            xeroConfig.AppName = AppName;
            xeroConfig.CallbackUri = new Uri(Vurl);
            xeroConfig.ClientId = _ClientId;
            xeroConfig.ClientSecret = _ClientSecret;
            xeroConfig.Scope = _scopes;
            xeroConfig.State = "123";
            XeroClient = new XeroClient(xeroConfig);
            _accountinstance = new AccountingApi();

        }
        public override async Task<V> GetBankTransactionsAsync(T xeroToken, string XeroTenentID, dynamic whereCause)
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetBankTransactionsAsync(Token.AccessToken, XeroTenentID, null,Convert.ToString(whereCause)); //"IsReconciled=false&&Status!=\"DELETED\""
            return (V)Convert.ChangeType(result, typeof(V));
        }
        public override async Task<V> GetBankTransactionAsync(T xeroToken, string XeroTenentID, Guid bankTransactionID)
        {
            var Token = (IXeroToken)xeroToken;
            var result = await _accountinstance.GetBankTransactionAsync(Token.AccessToken, XeroTenentID,bankTransactionID); //"IsReconciled=false&&Status!=\"DELETED\""
            return (V)Convert.ChangeType(result, typeof(V));
        }
        public override async Task<V> GetPaymentsAsync(T iToken, string TenentID, dynamic whereCause)
        {
            var Token = (IXeroToken)iToken;
            var result = await _accountinstance.GetPaymentsAsync(Token.AccessToken, TenentID,null, Convert.ToString(whereCause));
            return (V)Convert.ChangeType(result, typeof(V));
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
