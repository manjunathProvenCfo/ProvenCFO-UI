using Proven.Model;
using Proven.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xero.NetStandard.OAuth2.Token;

namespace ProvenCfoUI.Comman
{
    public class BankTransactionRuleEngine : IBankTransactionRuleEngine
    {

        public BankTransactionRuleEngine()
        {
            
        }
        public Task<dynamic> GetReconciliationByPaidXero(ClientModel client)
        {
            dynamic factory = null;
            if (client.Plaid_Enabled == true)
            {
                factory = new XeroBankTransaction<IXeroToken, dynamic>(client.APIClientID, client.APIClientSecret, client.APIScope, "");
            }
            else
            {

            }
            return factory;

            //throw new NotImplementedException();
        }
    }
}