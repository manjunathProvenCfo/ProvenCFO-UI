using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xero.NetStandard.OAuth2.Token;

namespace Proven.Service
{
    public interface IAccouningPackage<T,V>
    {
        Task<T> LoginAccess();
        Task<T> LoginXeroAccesswithCode(string code);
        T getTokenFormat(V TokenInfo);
        Task<T> RefreshToken(T Token);
        AppTokenInfoMain GetSavedToken(int AgencyID);
        ReturnModel UpdateToken(V tokenInfoVM);
        Task<T> ConnnectApp(V Token);
        Task<V> GetConnections();
        Task<V> GetContacts(T Token, string TenentID = "");
        Task<V> GetContact(T Token, string TenentID = "", Guid? ContactID = null);
        Task<V> GetGLAccounts(T Token, string TenentID);
        Task<V> GetTrackingCategories(T Token, string TenentID);
        Task<V> GetReportProfitAndLossAsync(T Token, string TenentID, DateTime? fromDate = null, DateTime? toDate = null, int? periods = null, string timeframe = null, string trackingCategoryID = null, string trackingOptionID = null, string trackingCategoryID2 = null, string trackingOptionID2 = null, bool? standardLayout = null, bool? paymentsOnly = null);
    }
}
