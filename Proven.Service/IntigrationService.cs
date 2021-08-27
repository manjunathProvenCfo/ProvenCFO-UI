using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class IntigrationService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;

        public XeroTrackingMainCategoriesVM GetXeroTracking()
        {
            return GetAsync<XeroTrackingMainCategoriesVM>("Xero/GetXeroTracking").Result;
            //response = client.GetAsync("Invitation/GetALLInvitation").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<InviteUserMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public XeromainGlAccountVM GetXeroGlAccount()
        {
            return GetAsync<XeromainGlAccountVM>("Xero/GetXeroGlAccount").Result;
            //response = client.GetAsync("Invitation/GetALLInvitation").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<InviteUserMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public XeroGlAccountVM CreateXeroGlAccount(string AccountId, string Code, string Name, string Status, string Type, string TaxType, string Class, string EnablePaymentsToAccount, string ShowInExpenseClaims, string BankAccountNumber, string BankAccountType, DateTime? UpdatedDateUTC, string CurrencyCode, string ReportingCode, string HasAttachments, string AddToWatchlist,string AgencyId)
        {
            var form = new Dictionary<string, object>
            {
                {"AccountId", AccountId},
                {"Code",Code },
                {"Name",Name },
                {"Status",Status },
                {"Type",Type },
                {"TaxType",TaxType },
                {"Status",Status },
                {"Class",Class },
                {"EnablePaymentsToAccount",EnablePaymentsToAccount },
                 {"ShowInExpenseClaims",ShowInExpenseClaims },
                {"BankAccountNumber" ,BankAccountNumber},
                {"BankAccountType",BankAccountType },
                {"UpdatedDateUTC",UpdatedDateUTC },
                {"CurrencyCode",CurrencyCode },

                {"ReportingCode",ReportingCode },
                {"HasAttachments",HasAttachments },
                {"AddToWatchlist",AddToWatchlist },
                {"AgencyId",AgencyId }
            };

            //content = new StringContent(JsonConvert.SerializeObject(from), Encoding.UTF8, "application/json");
            return PostAsync<XeroGlAccountVM, Dictionary<string, object>>("Xero/CreateXeroGlAccount", form).Result;
            //response = client.PostAsync("Client/CreateClientUser", content).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    AccountService obj = new AccountService();
            //    //var result = obj.Register(Email, "admin", "admin", ClientName, "");

            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}

        }



        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                if (response != null)
                    response.Dispose();
                if (content != null)
                {
                    content.Dispose();
                }
                // free managed resources               
            }
            isDisposed = true;
        }
    }
}

