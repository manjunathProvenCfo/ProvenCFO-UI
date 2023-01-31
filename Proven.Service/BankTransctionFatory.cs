using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public abstract class BankTransctionFatory<T, V> : BaseService
    {
        public abstract Task<V> GetBankTransactionsAsync(T Token, string TenentID, dynamic whereCause);
        public abstract Task<V> GetBankTransactionAsync(T xeroToken, string XeroTenentID, Guid bankTransactionID);
        public abstract Task<V> GetPaymentsAsync(T Token, string TenentID, dynamic whereCause);
        public abstract Task<V> GetBankStatementsAsync(T xeroToken, string xeroTenantId, string bankAccountID = null, string dateFrom = null, string dateTo = null, string where = null, string order = null, int? page = null, DateTime? ifModifiedSince = null, string status = null);
    }
}
