﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public abstract class BankTransctionFatory<T, V> : BaseService
    {
        public abstract Task<V> GetBankTransactionsAsync(T Token, string TenentID, dynamic whereCause);
    }
}