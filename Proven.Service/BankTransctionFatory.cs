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
    }
}
