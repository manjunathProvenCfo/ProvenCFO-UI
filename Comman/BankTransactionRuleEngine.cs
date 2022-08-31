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
                        if (PlaidData.IsSuccessStatusCode == true)
                        {

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
                        else
                        {
                            resultBooksRec = GetPlaidErrorDetails(PlaidData);
                            resultBankRec = GetPlaidErrorDetails(PlaidData);
                            Tuple<PlaidResponceModel, PlaidResponceModel> errresult = Tuple.Create(resultBooksRec, resultBankRec);
                            return errresult;
                        }
                    }
                }
                Tuple<PlaidResponceModel, PlaidResponceModel> result = Tuple.Create(resultBooksRec, resultBankRec);
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public PlaidResponceModel GetPlaidErrorDetails(Acklann.Plaid.Transactions.GetTransactionsResponse Plaidresponse)
        {
            PlaidResponceModel resultBooksRec = new PlaidResponceModel();
            resultBooksRec.Status = Plaidresponse.IsSuccessStatusCode;
            switch (Plaidresponse.Exception.ErrorCode)
            {
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidCredentials:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidMfa:
                case Acklann.Plaid.Exceptions.ErrorCode.ItemLoginRequired:
                case Acklann.Plaid.Exceptions.ErrorCode.InsufficientCredentials:
                    resultBooksRec.Error = Plaidresponse.Exception.ErrorMessage;
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.ItemLocked:
                    resultBooksRec.Error = "Plaid Account has been locked, Please contact the administrator.";
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.UserSetupRequired:
                    resultBooksRec.Error = "Plaid Account User Setup Required, Please contact the administrator.";
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.MfaNotSupported:
                    resultBooksRec.Error = "Plaid Account multifactor factor authentication not supported , Please contact the administrator.";
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidSendMethod:
                case Acklann.Plaid.Exceptions.ErrorCode.NoAccounts:
                case Acklann.Plaid.Exceptions.ErrorCode.ItemNotSupported:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidUpdatedUsername:
                case Acklann.Plaid.Exceptions.ErrorCode.ItemNoError:
                    resultBooksRec.Error = "Error while Plaid API data request, Please contact the administrator.";
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.NoAuthAccounts:
                case Acklann.Plaid.Exceptions.ErrorCode.NoInvestmentAccounts:
                case Acklann.Plaid.Exceptions.ErrorCode.NoInvestmentAuthAccounts:
                    resultBooksRec.Error = "Plaid API authenction issue, Please contact the administrator.";
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.ProductsNotSupported:
                case Acklann.Plaid.Exceptions.ErrorCode.ItemNotFound:
                case Acklann.Plaid.Exceptions.ErrorCode.ProductNotReady:
                case Acklann.Plaid.Exceptions.ErrorCode.InternalServerError:
                    resultBooksRec.Error = "Error while Plaid API data request, Please contact the administrator.";
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.PlannedMaintenance:
                    resultBooksRec.Error = "Plaid API service under maintenance, Please contact the administrator.";
                    break;
                case Acklann.Plaid.Exceptions.ErrorCode.InstitutionDown:
                case Acklann.Plaid.Exceptions.ErrorCode.InstitutionNotResponding:
                case Acklann.Plaid.Exceptions.ErrorCode.InstitutionNotAvailable:
                case Acklann.Plaid.Exceptions.ErrorCode.InstitutionNoLongerSupported:
                case Acklann.Plaid.Exceptions.ErrorCode.MissingFields:
                case Acklann.Plaid.Exceptions.ErrorCode.UnknownFields:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidField:
                case Acklann.Plaid.Exceptions.ErrorCode.IncompatibleApiVersion:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidBody:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidHeaders:
                case Acklann.Plaid.Exceptions.ErrorCode.NotFound:
                case Acklann.Plaid.Exceptions.ErrorCode.NoLongerAvailable:
                case Acklann.Plaid.Exceptions.ErrorCode.SandboxOnly:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidAccountNumber:
                case Acklann.Plaid.Exceptions.ErrorCode.TooManyVerificationAttempts:
                case Acklann.Plaid.Exceptions.ErrorCode.IncorrectDepositAmounts:
                case Acklann.Plaid.Exceptions.ErrorCode.UnauthorizedEnvironment:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidProduct:
                case Acklann.Plaid.Exceptions.ErrorCode.UnauthorizedRouteAccess:
                case Acklann.Plaid.Exceptions.ErrorCode.DirectIntegrationNotEnabled:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidApiKeys:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidAccessToken:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidPublicToken:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidLinkToken:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidProcessorToken:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidAuditCopyToken:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidAccountId:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidInstitution:
                case Acklann.Plaid.Exceptions.ErrorCode.InvalidCredentialFields:
                case Acklann.Plaid.Exceptions.ErrorCode.AccountsLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.AdditionLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.AuthLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.BalanceLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.IdentityLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.ItemGetLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.RateLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.TransactionsLimit:
                case Acklann.Plaid.Exceptions.ErrorCode.RecaptchaRequired:
                case Acklann.Plaid.Exceptions.ErrorCode.RecaptchaBad:
                case Acklann.Plaid.Exceptions.ErrorCode.InstitutionPoorHealthError:
                case Acklann.Plaid.Exceptions.ErrorCode.IncorrectOauthNonce:
                case Acklann.Plaid.Exceptions.ErrorCode.OauthStateIdAlreadyProcessed:
                case Acklann.Plaid.Exceptions.ErrorCode.SandboxProductNotEnabled:
                case Acklann.Plaid.Exceptions.ErrorCode.SandboxWebhookInvalid:
                    resultBooksRec.Error = "Error while Plaid API data request, Please contact the administrator.";
                    break;
                default:
                    resultBooksRec.Error = "Error while Plaid API data request, Please contact the administrator.";
                    break;
            }
            resultBooksRec.ErrorDesctiption = Plaidresponse.Exception.ErrorMessage;
            resultBooksRec.ErrorType = "Plaid Error";
            return resultBooksRec; 
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