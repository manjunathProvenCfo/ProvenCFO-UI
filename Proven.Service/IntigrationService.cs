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

        public XeroTrackingMainCategoriesVM GetXeroTracking(int AgencyId)
        {
            return GetAsync<XeroTrackingMainCategoriesVM>("Xero/GetXeroTrackingById?AgencyID=" + AgencyId).Result;
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

        public XeromainGlAccountVM GetXeroGlAccount(int AgencyID,string XeroStatus)
        {
            return GetAsync<XeromainGlAccountVM>("Xero/GetXeroGlAccount?AgencyID=" + AgencyID + "&XeroStatus=" +  XeroStatus).Result;
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
        public XeroGlAccountVM create(XeroGlAccountVM test)
        {
            return PostAsync<XeroGlAccountVM, XeroGlAccountVM>("Xero/CreateXeroGlAccount1", test).Result;
        }
        public XeroGlAccountVM CreateXeroGlAccount(List<XeroGlAccountVM> glAccounts)
        {           
            return PostAsync<XeroGlAccountVM, List<XeroGlAccountVM>>("Xero/CreateXeroGlAccount", glAccounts).Result;           
        }
        public XeroTrackingCategoriesVM CreateXeroTrackingCatogories(List<XeroTrackingCategoriesVM> TrackingCategories)
        {
            return PostAsync<XeroTrackingCategoriesVM, List<XeroTrackingCategoriesVM>>("Xero/CreateXeroTracking", TrackingCategories).Result;
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

