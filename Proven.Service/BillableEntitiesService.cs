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
            return GetAsync<BillableEntitiesMainModel>("BillableEntities/GetAllBillableEntitiesList").Result;
            //response = client.GetAsync("BillableEntities/GetAllBillableEntitiesList").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<BillableEntitiesMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
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

        public BillableEntitiesVM AddBillableEntity(string EntityName, string ProvenCFOXeroContactID, string Clients, string status, string userid)
        {
            var form = new Dictionary<string, string>
            {
                {"EntityName", EntityName},
                {"ProvenCFOXeroContactID", ProvenCFOXeroContactID },
                {"Clients", Clients },
                {"Status", status },
                {"CreatedBy", userid}
            };
            //content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            content = PreparePostContent(form);
            return PostAsync<BillableEntitiesVM>("BillableEntities/CreateBillableEntity", content).Result;
            //response = client.PostAsync("BillableEntities/CreateBillableEntity", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<BillableEntitiesVM>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }

        public BillableEntitiesVM UpdateBillableEntity(string Id, string EntityName, string ProvenCFOXeroContactID, string Clients, string status, string IsDeleted, string userid)
        {
            var form = new Dictionary<string, object>
           {
               {"Id", Id},
               {"EntityName", EntityName},
               {"ProvenCFOXeroContactID",ProvenCFOXeroContactID },
                {"Clients", Clients },
                {"Status", status },
               {"ModifiedBy", userid},
               {"IsDeleted", (IsDeleted == null || IsDeleted == ""? false: true)}
           };
            //content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            content = PreparePostContent(form);
            return PostAsync<BillableEntitiesVM>("BillableEntities/UpdateBillableEntity", content).Result;
            //response = client.PostAsync("BillableEntities/UpdateBillableEntity", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<BillableEntitiesVM>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public BillableEntitiesVM DeleteBillableEntity(int Id)
        {
            //var form = new Dictionary<string, string>
            //{
            //   {"Id", Id.ToString()}
            //};
            string result = string.Format("BillableEntities/DeleteBillableEntity?Id={0}", Id);
            return PostAsync<BillableEntitiesVM>(result, null).Result;
            //HttpResponseMessage response = client.PostAsync(result, null).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<BillableEntitiesVM>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }

        public BillableEntitiesVM RetireBillableEntity(int Id)
        {
            //var form = new Dictionary<string, string>
            //{
            //    {"Id", Id.ToString()}
            //};
            string result = string.Format("BillableEntities/DeactivateBillableEntity?Id={0}", Id);
            return PostAsync<BillableEntitiesVM>(result, null).Result;
            //response = client.PostAsync(result, null).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<BillableEntitiesVM>(_content);
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

        public BillableEntitiesVM GetClientBillsAssociationById(int Id)
        {
            response = client.GetAsync("BillableEntities/GetClientBillsAssociationById?Id=" + Id).Result;
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
    }
}
