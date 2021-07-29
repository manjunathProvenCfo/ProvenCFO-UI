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
    public class RoleService : BaseService , IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public RoleMainModel GetRoles()
        {
            
          response = client.GetAsync("Role/GetAllRole").Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<RoleMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public RoleMainModel GetAllRoleInvitation()
        {

            response = client.GetAsync("Role/GetAllRoleInvitation").Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<RoleMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public RolesViewModel GetRoleById(string id)
        {
       response = client.GetAsync("Role/GetRole?id="+id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<RolesViewModel>((JObject.Parse(_content)["resultData"]).ToString());
                //var val = JsonConvert.DeserializeObject<RoleMainModel>(_content);
                return val;
                // model = JsonConvert.DeserializeObject<List<RoleModel>>(_content.Result);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }
        public RolesViewModel GetUserRoleById(string id)
        {
            response = client.GetAsync("Role/GetUserRoleById?id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<RolesViewModel>((JObject.Parse(_content)["resultData"]).ToString());
                //var val = JsonConvert.DeserializeObject<RoleMainModel>(_content);
                return val;
                // model = JsonConvert.DeserializeObject<List<RoleModel>>(_content.Result);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }
        public RolesViewModel GetRoleByName(string roleName)
        {
           response = client.GetAsync("Role/GetRoleByName?roleName=" + roleName).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<RolesViewModel>((JObject.Parse(_content)["resultData"]).ToString());
                //var val = JsonConvert.DeserializeObject<RoleMainModel>(_content);
                return val;
                // model = JsonConvert.DeserializeObject<List<RoleModel>>(_content.Result);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public RolesViewModel AddRoles(string name,string status,string userid)
        {
            
            
            var form = new Dictionary<string, string>
           {
               {"name", name},
               {"Status",status },
               {"CreatedBy", userid}               
           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Role/CreateRole", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<RolesViewModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public RolesViewModel UpdateRoles(string id,string name, string status, string userid)
        {
            var form = new Dictionary<string, string>
           {
               {"Id", id},
               {"name", name},
               {"Status",status },
               {"ModifiedBy", userid}
           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Role/UpdateRole", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<RolesViewModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }
        public RolesViewModel DeleteRoles(string id)
        {
            var form = new Dictionary<string, string>
           {
               {"Id", id}


           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Role/DeleteRole", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<RolesViewModel>(_content);
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
