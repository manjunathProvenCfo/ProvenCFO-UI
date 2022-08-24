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
    public class BankTransactionRuleEngine : IBankTransactionRuleEngine, IDisposable
    {
        private bool isDisposed = false;
        public BankTransactionRuleEngine()
        {

        }
        public async Task<dynamic> GetReconciliationByPaidXero(ClientModel client)
        {
            PlaidResponceModel resultBooksRec = null;
            PlaidResponceModel resultBankRec = null;
            string Access_token = String.Empty;
            dynamic factory = null;
            Xero.NetStandard.OAuth2.Model.Accounting.BankTransactions XeroDataBothNotInBookNotInBanks = null;
            List<Xero.NetStandard.OAuth2.Model.Accounting.BankTransaction> XeroDataBothNotInBookNotInBanksList = null;

            Xero.NetStandard.OAuth2.Model.Accounting.BankTransactions XeroNotInBanks = null;
            List<Xero.NetStandard.OAuth2.Model.Accounting.BankTransaction> XeroNotInBanksList = null;
            try
            {


                if (client.Plaid_Enabled == false) return null;
                using (ClientService objclient = new ClientService())
                {
                    var ClientBankAccounts = objclient.GetClientXeroAcccountsByAgencyId(client.Id).ResultData.Where(x => x.access_token != null && x.access_token != string.Empty && x.PlaidConnectionStatus == true).ToList();
                    var PlaidAPIDetails = objclient.GetThirdPatyAPIDetails(Common.Plaid, true);

                    if (ClientBankAccounts == null) return null;
                    foreach (var accout in ClientBankAccounts)
                    {
                        if (accout.IsActive == true)
                        {
                            Access_token = SecurityCommon.DecryptToBytesUsingCBC(Convert.FromBase64String(accout.access_token.Replace(" ", "+"))).Replace("\t", "");
                            if (client.ThirdPartyAccountingApp_ref == 1)
                            {
                                factory = new XeroBankTransaction<IXeroToken, Xero.NetStandard.OAuth2.Model.Accounting.BankTransactions>(client.APIClientID, client.APIClientSecret, client.APIScope, "");
                                XeroDataBothNotInBookNotInBanks = await factory.GetBankTransactionsAsync(AccountingPackageInstance.Instance.XeroToken, AccountingPackageInstance.Instance.TenentID, @"IsReconciled==true&&Status!=""DELETED""");
                                XeroDataBothNotInBookNotInBanksList = XeroDataBothNotInBookNotInBanks._BankTransactions.Where(x => x.BankAccount.AccountID == Guid.Parse(accout.AccountID)).ToList();

                                XeroNotInBanks = await factory.GetBankTransactionsAsync(AccountingPackageInstance.Instance.XeroToken, AccountingPackageInstance.Instance.TenentID, @"IsReconciled==false&&Status!=""DELETED""");
                                XeroNotInBanksList = XeroNotInBanks._BankTransactions.Where(x => x.BankAccount.AccountID == Guid.Parse(accout.AccountID)).ToList();
                            }
                        }
                        else
                        {
                            factory = null;
                        }
                        PlaidRequestModelsVM plaidRequest = new PlaidRequestModelsVM();
                        plaidRequest.start_date = Common.PreviousMonth(DateTime.Now);
                        plaidRequest.end_date = DateTime.Now;
                        PlaidBankTransaction<string, Acklann.Plaid.Transactions.GetTransactionsResponse> plaidfactory = new PlaidBankTransaction<string, Acklann.Plaid.Transactions.GetTransactionsResponse>(PlaidAPIDetails.ClientId, PlaidAPIDetails.ClientSecret, "en", new string[] { "auth", "transactions" }, new string[] { "US" }, PlaidBankTransaction<string, dynamic>.GetEnvironment());
                        var PlaidData = await plaidfactory.GetBankTransactionsAsync(Access_token, null, plaidRequest);
                        List<company_reconciliationVM> NotInBooksRecords = new List<company_reconciliationVM>();
                        List<company_reconciliationVM> NotInBankRecords = new List<company_reconciliationVM>();
                        foreach (var xero in XeroDataBothNotInBookNotInBanksList)
                        {
                            //var GetTransaction = await factory.GetBankTransactionAsync(AccountingPackageInstance.Instance.XeroToken, AccountingPackageInstance.Instance.TenentID, xero.BankTransactionID.Value);
                            company_reconciliationVM reconciliationVM = new company_reconciliationVM();
                            reconciliationVM.id = Convert.ToString(xero.BankTransactionID);
                            reconciliationVM.account_name = xero.BankAccount.Name;
                            reconciliationVM.amount = xero.Total.HasValue == true ? xero.Total.Value : 0;
                            reconciliationVM.company = client.Name;
                            reconciliationVM.date = xero.Date.Value;
                            reconciliationVM.description = xero.LineItems.FirstOrDefault()?.Description;

                            reconciliationVM.reference = xero.Reference;
                            reconciliationVM.AgencyID = client.Id;
                            var MatchedRec = PlaidData.Transactions.Where(x => x.Date == xero.Date && x.Amount == xero.Total).FirstOrDefault();
                            if (MatchedRec != null)
                            {
                                reconciliationVM.type = "Unreconciled";
                                NotInBooksRecords.Add(reconciliationVM);
                            }
                            else
                            {
                                reconciliationVM.type = "Outstanding Payments";
                                NotInBankRecords.Add(reconciliationVM);
                            }
                        }
                        foreach (var xero in XeroNotInBanksList)
                        {
                            company_reconciliationVM reconciliationVM = new company_reconciliationVM();
                            reconciliationVM.id = Convert.ToString(xero.BankTransactionID);
                            reconciliationVM.account_name = xero.BankAccount.Name;
                            reconciliationVM.amount = xero.Total.HasValue == true ? xero.Total.Value : 0;
                            reconciliationVM.company = client.Name;
                            reconciliationVM.date = xero.Date.Value;
                            reconciliationVM.description = xero.LineItems.FirstOrDefault()?.Description;

                            reconciliationVM.reference = xero.Reference;
                            reconciliationVM.AgencyID = client.Id;
                            NotInBankRecords.Add(reconciliationVM);
                        }
                        using (ReconcilationService res = new ReconcilationService())
                        {

                            resultBooksRec = await res.CreatePlaidReconciliation(NotInBooksRecords);
                            resultBankRec = await res.CreatePlaidReconciliation(NotInBankRecords);
                        }
                    }
                }
                Tuple<PlaidResponceModel, PlaidResponceModel> result = Tuple.Create(resultBooksRec, resultBankRec);
                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }


        //throw new NotImplementedException();

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