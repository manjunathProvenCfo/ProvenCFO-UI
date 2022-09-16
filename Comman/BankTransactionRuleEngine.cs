using Acklann.Plaid.Transactions;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xero.NetStandard.OAuth2.Model.Accounting;
using Xero.NetStandard.OAuth2.Token;

namespace ProvenCfoUI.Comman
{
    public class BankTransactionRuleEngine : IBankTransactionRuleEngine, IDisposable
    {
        private bool isDisposed = false;
        public BankTransactionRuleEngine()
        {

        }
        private async Task<Tuple<PlaidResponceModel, PlaidResponceModel>> CompilePlaidDatawithXero(ClientModel client, List<BankTransaction> XeroDataBothNotInBookNotInBanksList, List<BankTransaction> XeroNotInBanksList, ClientXeroAccountsVM account, Payments XeroPayments, ThirdPartyAPIDetailsVM PlaidAPIDetails, GetTransactionsResponse PlaidData)
        {
            PlaidResponceModel resultBooksRec = null;
            PlaidResponceModel resultBankRec = null;
            List<company_reconciliationVM> NotInBooksRecords = new List<company_reconciliationVM>();
            List<company_reconciliationVM> NotInBankRecords = new List<company_reconciliationVM>();
            foreach (var plaid in PlaidData.Transactions)
            {
                var MatchedRec = XeroDataBothNotInBookNotInBanksList.Where(x => x.Date == plaid.Date && x.Total == plaid.Amount).FirstOrDefault();
                if (MatchedRec == null)
                {
                    company_reconciliationVM reconciliationVM = new company_reconciliationVM();
                    reconciliationVM.id = Convert.ToString(plaid.TransactionId);
                    reconciliationVM.account_name = account.Name;
                    reconciliationVM.amount = plaid.Amount;
                    reconciliationVM.company = client.Name;
                    reconciliationVM.date = plaid.Date;
                    reconciliationVM.description = string.Join(",", plaid.Categories);
                    reconciliationVM.reference = plaid.Name;
                    reconciliationVM.AgencyID = client.Id;
                    reconciliationVM.type = "Unreconciled";
                    NotInBooksRecords.Add(reconciliationVM);
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
                reconciliationVM.type = "Outstanding Payments";
                NotInBankRecords.Add(reconciliationVM);
            }
            foreach (var payment in XeroPayments._Payments)
            {
                company_reconciliationVM reconciliationVM = new company_reconciliationVM();
                reconciliationVM.id = Convert.ToString(payment.PaymentID);
                reconciliationVM.account_name = account.Name;
                reconciliationVM.amount = payment.Amount;
                reconciliationVM.company = client.Name;
                reconciliationVM.date = payment.Date;
                reconciliationVM.description = payment.Invoice.Contact.Name;

                reconciliationVM.reference = payment.Reference;
                reconciliationVM.AgencyID = client.Id;
                reconciliationVM.type = "Outstanding Payments";
                NotInBankRecords.Add(reconciliationVM);
            }
            using (ReconcilationService res = new ReconcilationService())
            {
                resultBooksRec = await res.CreatePlaidReconciliation(NotInBooksRecords);
                resultBankRec = await res.CreatePlaidReconciliation(NotInBankRecords);
            }
            Tuple<PlaidResponceModel, PlaidResponceModel> result = Tuple.Create(resultBooksRec, resultBankRec);



            return result;
        }
        public async Task<dynamic> GetReconciliationByPaidWithAccountingPackage(ClientModel client)
        {
            PlaidResponceModel resultBooksRec = null;
            PlaidResponceModel resultBankRec = null;
            string Access_token = String.Empty;
            dynamic BankTransactionfactory = null;
            dynamic Paymentfactory = null;
            List<Tuple<PlaidResponceModel, PlaidResponceModel>> FinalResult = new List<Tuple<PlaidResponceModel, PlaidResponceModel>>();

            Xero.NetStandard.OAuth2.Model.Accounting.BankTransactions XeroDataBothNotInBookNotInBanks = null;
            List<Xero.NetStandard.OAuth2.Model.Accounting.BankTransaction> XeroDataBothNotInBookNotInBanksList = null;

            Xero.NetStandard.OAuth2.Model.Accounting.BankTransactions XeroNotInBanks = null;
            Xero.NetStandard.OAuth2.Model.Accounting.Payments XeroPayments = null;

            List<Xero.NetStandard.OAuth2.Model.Accounting.BankTransaction> XeroNotInBanksList = null;
            List<Xero.NetStandard.OAuth2.Model.Accounting.Payment> XeroPaymentsList = null;
            try
            {


                if (client.Plaid_Enabled == false) return null;
                using (ClientService objclient = new ClientService())
                {
                    var ClientBankAccounts = objclient.GetClientXeroAcccountsByAgencyId(client.Id).ResultData.ToList();
                    var PlaidAPIDetails = objclient.GetThirdPatyAPIDetails(Common.Plaid, true);

                    if (ClientBankAccounts == null)
                    {
                        resultBooksRec = GetPlaidErrorDetails(null, "Vailidation Error", "Please sync the bank accounts under setup menu.");
                        Tuple<PlaidResponceModel, PlaidResponceModel> errresult = Tuple.Create(resultBooksRec, resultBooksRec);
                        FinalResult.Add(errresult);
                        return FinalResult;
                    };
                    if (ClientBankAccounts.Where(x => x.IsActive == true).Count() == 0)
                    {
                        resultBooksRec = GetPlaidErrorDetails(null, "Vailidation Error", "No bank account has been active. Please enable at least one bank account to sync the data.");
                        Tuple<PlaidResponceModel, PlaidResponceModel> errresult = Tuple.Create(resultBooksRec, resultBooksRec);
                        FinalResult.Add(errresult);
                        return FinalResult;
                    }
                    if (ClientBankAccounts.Where(x => x.PlaidConnectionStatus == true).Count() == 0 || (ClientBankAccounts.Where(x => x.IsActive == true && x.PlaidConnectionStatus == true).Count() == 0))
                    {
                        resultBooksRec = GetPlaidErrorDetails(null, "Vailidation Error", "No bank account has been authenticated. Please authenticated at least one bank account to sync the data.");
                        Tuple<PlaidResponceModel, PlaidResponceModel> errresult = Tuple.Create(resultBooksRec, resultBooksRec);
                        FinalResult.Add(errresult);
                        return FinalResult;
                    }
                    using (var reconciliation = new ReconcilationService())
                    {
                       await reconciliation.UpdateasReconciledallrecordsofAgency(client.Id, 1);
                    }
                    
                    foreach (var account in ClientBankAccounts)
                    {
                        if (account.IsActive == true && !string.IsNullOrEmpty(account.access_token))
                        {
                            Access_token = SecurityCommon.DecryptToBytesUsingCBC(Convert.FromBase64String(account.access_token.Replace(" ", "+"))).Replace("\t", "").Replace("\n", ""); 
                            if (client.ThirdPartyAccountingApp_ref == 1)
                            {
                                BankTransactionfactory = new XeroBankTransaction<IXeroToken, Xero.NetStandard.OAuth2.Model.Accounting.BankTransactions>(client.APIClientID, client.APIClientSecret, client.APIScope, "");
                                Paymentfactory = new XeroBankTransaction<IXeroToken, Xero.NetStandard.OAuth2.Model.Accounting.Payments>(client.APIClientID, client.APIClientSecret, client.APIScope, "");

                                XeroDataBothNotInBookNotInBanks = await BankTransactionfactory.GetBankTransactionsAsync(AccountingPackageInstance.Instance.XeroToken, AccountingPackageInstance.Instance.TenentID, string.Format(@"IsReconciled==true&&Status!=""DELETED""&&BankAccount.Code==""{0}""", account.Code));
                                XeroDataBothNotInBookNotInBanksList = XeroDataBothNotInBookNotInBanks._BankTransactions.ToList();

                                XeroNotInBanks = await BankTransactionfactory.GetBankTransactionsAsync(AccountingPackageInstance.Instance.XeroToken, AccountingPackageInstance.Instance.TenentID, string.Format(@"IsReconciled==false&&Status!=""DELETED""&&BankAccount.Code==""{0}""", account.Code));
                                XeroNotInBanksList = XeroNotInBanks._BankTransactions.ToList();

                                XeroPayments = await Paymentfactory.GetPaymentsAsync(AccountingPackageInstance.Instance.XeroToken, AccountingPackageInstance.Instance.TenentID, string.Format(@"IsReconciled==false&&Status!=""DELETED""&&Account.Code==""{0}""", account.Code));

                                PlaidRequestModelsVM plaidRequest = new PlaidRequestModelsVM();
                                plaidRequest.start_date = Common.PreviousMonth(DateTime.Now, -2);
                                plaidRequest.end_date = DateTime.Now;
                                PlaidBankTransaction<string, Acklann.Plaid.Transactions.GetTransactionsResponse> plaidfactory = new PlaidBankTransaction<string, Acklann.Plaid.Transactions.GetTransactionsResponse>(PlaidAPIDetails.ClientId, PlaidAPIDetails.ClientSecret, "en", new string[] { "auth", "transactions" }, new string[] { "US" }, PlaidBankTransaction<string, dynamic>.GetEnvironment());
                                var PlaidData = await plaidfactory.GetBankTransactionsAsync(Access_token, null, plaidRequest);
                                if (PlaidData.IsSuccessStatusCode == true)
                                {
                                    Tuple<PlaidResponceModel, PlaidResponceModel> result = await CompilePlaidDatawithXero(client, XeroDataBothNotInBookNotInBanksList, XeroNotInBanksList, account, XeroPayments, PlaidAPIDetails, PlaidData); //Tuple.Create(resultBooksRec, resultBankRec);
                                    FinalResult.Add(result);
                                }
                                else
                                {
                                    resultBooksRec = GetPlaidErrorDetails(PlaidData, "Plaid Error", "Error while Plaid API data request, Please contact the administrator.");
                                    resultBankRec = GetPlaidErrorDetails(PlaidData, "Plaid Error", "Error while Plaid API data request, Please contact the administrator.");
                                    Tuple<PlaidResponceModel, PlaidResponceModel> errresult = Tuple.Create(resultBooksRec, resultBankRec);
                                    return errresult;
                                }
                            }
                            else
                            {
                                BankTransactionfactory = null;
                            }
                        }
                    }
                }

                return FinalResult;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public PlaidResponceModel GetPlaidErrorDetails(Acklann.Plaid.Transactions.GetTransactionsResponse Plaidresponse, string ErrorType, string ErrorMsg = "")
        {
            PlaidResponceModel resultBooksRec = new PlaidResponceModel();
            if (ErrorType == "Plaid Error")
            {
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
                        resultBooksRec.Error = ErrorMsg;
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
                        resultBooksRec.Error = ErrorMsg;
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
                        resultBooksRec.Error = ErrorMsg;
                        break;
                    default:
                        resultBooksRec.Error = ErrorMsg;
                        break;
                }
                resultBooksRec.ErrorDesctiption = Plaidresponse.Exception.ErrorMessage;
                resultBooksRec.ErrorType = ErrorType;
            }
            else if (ErrorType == "Vailidation Error")
            {
                resultBooksRec.ErrorDesctiption = "Valiation Failed.";
                resultBooksRec.ErrorType = ErrorType;
                resultBooksRec.Error = ErrorMsg;
            }


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