using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class ReconcilationService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;


        public ReconciliationMainModel GetReconciliation(int ClientId, string type, int Isreconciled)
        {
            return GetAsync<ReconciliationMainModel>("Reconciliation/GetReconciliation?ClientId=" + ClientId + "&type=" + type + "&Isreconciled=" + Isreconciled).Result;
        }

        public ReconciliationCountModel GetReconciliationDataCountAgencyId(string AgencyId)
        {
            return GetAsync<ReconciliationCountModel>("Reconciliation/GetReconciliationDataCountAgencyId?AgencyId=" + AgencyId).Result;
        }
        public ReconciliationCountModel GetReconciliationNegCountAgencyId(string AgencyId)
        {
            return GetAsync<ReconciliationCountModel>("Reconciliation/GetReconciliationNegCountAgencyId?AgencyId=" + AgencyId).Result;
        }
        public ReconciliationCountModel GetReconciliationCountAgencyId(string AgencyId)
        {
            return GetAsync<ReconciliationCountModel>("Reconciliation/GetReconciliationCountAgencyId?AgencyId=" + AgencyId).Result;
        }

        public ReturnStringModel GetDistinctAccount(int ClientId)
        {
            return GetAsync<ReturnStringModel>("Reconciliation/GetDistinctAccount?ClientId=" + ClientId).Result;
        }
        public ReturnModel UpdateReconciliation(int AgencyID, string id, int GLAccount, string BankRule, int TrackingCategory)
        {
            string result = string.Format("Reconciliation/UpdateReconciliation?AgencyID={0}&id={1}&GLAccount={2}&BankRule={3}&TrackingCategory={4}", AgencyID, id, GLAccount, BankRule, TrackingCategory);
            return PostAsync<ReturnModel>(result).Result;
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
