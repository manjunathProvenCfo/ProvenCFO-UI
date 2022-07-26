using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProvenCfoUI.Comman
{
    interface IBankTransactionRuleEngine
    {
        Task<dynamic> GetReconciliationByPaidXero(ClientModel client);
    }
}