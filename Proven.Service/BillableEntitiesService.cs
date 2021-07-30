using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class BillableEntitiesService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;

        public BillableEntitiesMainModel GetAllBillableEntitiesList()
        {
            response = client.GetAsync("BillableEntities/GetAllBillableEntitiesList").Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<BillableEntitiesMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public BillableEntitiesVM GetBillableEntitiesById(int Id)
        {
            response = client.GetAsync("BillableEntities/GetBillableEntitiesById?Id=" + Id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                var val = JsonConvert.DeserializeObject<BillableEntitiesVM>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }
        public BillableEntitiesVM GetBillableEntitiesByName(string EntityName)
        {
            response = client.GetAsync("BillableEntities/GetBillableEntitiesByName?EntityName=" + EntityName).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                var val = JsonConvert.DeserializeObject<BillableEntitiesVM>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
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
