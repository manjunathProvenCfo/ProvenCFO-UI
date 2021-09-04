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
