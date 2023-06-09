﻿using Proven.Model;
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

        public XeromainGlAccountVM GetXeroGlAccount(int AgencyID,string Status)
        {
            return GetAsync<XeromainGlAccountVM>("Xero/GetGlAccount?AgencyID=" + AgencyID + "&Status=" +  Status).Result;
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
        public ClientXeroAccountsMainModel GetXeroBankAccount(int AgencyID, string Status)
        {
           return GetAsync<ClientXeroAccountsMainModel>("Xero/GetBankAccount?AgencyID=" + AgencyID + "&Status=" + Status).Result;
            
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
            return PostAsync<XeroGlAccountVM, XeroGlAccountVM>("Xero/CreateGlAccount1", test).Result;
        }
        public XeroGlAccountVM CreateGlAccount(List<XeroGlAccountVM> glAccounts)
        {           
            return PostAsync<XeroGlAccountVM, List<XeroGlAccountVM>>("Xero/CreateGlAccount", glAccounts).Result;           
        }
        public ClientXeroAccountsVM CreateXeroBankAccount(List<ClientXeroAccountsVM> glAccounts)
        {           
            return PostAsync<ClientXeroAccountsVM, List<ClientXeroAccountsVM>>("Xero/CreateXeroBankAccount", glAccounts).Result;           
        }
        public XeroTrackingCategoriesVM CreateXeroTrackingCatogories(List<XeroTrackingCategoriesVM> TrackingCategories)
        {
            return PostAsync<XeroTrackingCategoriesVM, List<XeroTrackingCategoriesVM>>("Xero/CreateXeroTracking", TrackingCategories).Result;
        }
        public async Task<ReturnModel> UpdatePlaidBankAccountDetails(string AccountId,string access_token,bool PlaidConnectionStatus)
        {
            return await PostAsync<ReturnModel>($"Xero/UpdatePlaidBankAccountDetails?AccountId={AccountId}&access_token={access_token}&PlaidConnectionStatus={PlaidConnectionStatus}");
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

