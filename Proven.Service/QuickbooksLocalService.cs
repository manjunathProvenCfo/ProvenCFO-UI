using Proven.Model;
using Proven.Service.AccountingPackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public override Task<V> GetGLAccounts(T Token, string TenentID)
        {
            throw new NotImplementedException();
        }

        public override Task<V> GetReportProfitAndLossAsync(T Token, string TenentID, DateTime? fromDate = null, DateTime? toDate = null, int? periods = null, string timeframe = null, string trackingCategoryID = null, string trackingOptionID = null, string trackingCategoryID2 = null, string trackingOptionID2 = null, bool? standardLayout = null, bool? paymentsOnly = null)
        {
            throw new NotImplementedException();
        }

        public override AppTokenInfoMain GetSavedToken(int AgencyID)
        {
            throw new NotImplementedException();
        }

        public override T getTokenFormat(V TokenInfo)
        {
            throw new NotImplementedException();
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

        public override Task<T> RefreshToken(T Token)
        {
            throw new NotImplementedException();
        }

        public override ReturnModel UpdateToken(V tokenInfoVM)
        {
            throw new NotImplementedException();
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
