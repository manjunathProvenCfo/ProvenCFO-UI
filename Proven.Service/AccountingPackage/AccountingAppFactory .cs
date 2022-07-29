using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service.AccountingPackage
{
    public abstract class AccountingAppFatory<T,V> :  BaseService
    {

        public abstract Task<T> ConnnectApp(V Token);
        public abstract Task<V> GetConnections();
        public abstract Task<V> GetContact(T Token, string TenentID = "", Guid? ContactID = null);
        public abstract Task<V> GetContacts(T Token, string TenentID = "");
        public abstract Task<V> GetGLAccounts(T Token, string TenentID);
        public abstract Task<V> GetBankAccounts(T Token, string TenentID);
        public abstract Task<V> GetReportProfitAndLossAsync(T Token, string TenentID, DateTime? fromDate = null, DateTime? toDate = null, int? periods = null, string timeframe = null, string trackingCategoryID = null, string trackingOptionID = null, string trackingCategoryID2 = null, string trackingOptionID2 = null, bool? standardLayout = null, bool? paymentsOnly = null);
        public abstract AppTokenInfoMain GetSavedToken(int AgencyID);
        public abstract T getTokenFormat(V TokenInfo);
        public abstract T getSavedTokenFormat(int AgencyId);
        public abstract Task<V> GetTrackingCategories(T Token, string TenentID);
        public abstract Task<T> LoginAccess();
        public abstract Task<T> LoginXeroAccesswithCode(string code);
        public abstract Task<T> RefreshToken(T Token);
        public abstract ReturnModel UpdateToken(V tokenInfoVM);
        public abstract Task<T> GetBankSummary(V xeroToken, string XeroTenentID);
        public abstract Task<V> GetInvoices(T xeroToken, string XeroTenentID);        
    }
}
