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
    public class RoleService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public RoleMainModel GetRoles()
        {
            return GetAsync<RoleMainModel>("Role/GetAllRole").Result;
            //response = client.GetAsync("Role/GetAllRole").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<RoleMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public MasterFeaturesMainModel GetMasterFeatures()
        {
            return GetAsync<MasterFeaturesMainModel>("Role/GetMasterFeatures").Result;            
        }
        public UserTypeModel GetUserTypes()
        {
            return GetAsync<UserTypeModel>("Role/GetUserTypes").Result;            
        }

        public RoleMainModel GetAllRoleInvitation()
        {
            return GetAsync<RoleMainModel>("Role/GetAllRoleInvitation").Result;
            //response = client.GetAsync("Role/GetAllRoleInvitation").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<RoleMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public RolesViewModel GetRoleById(string id)
        {
            return GetAsync<RolesViewModel>("Role/GetRole?id=" + id, true).Result;
            //response = client.GetAsync("Role/GetRole?id=" + id).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;

            //    var val = JsonConvert.DeserializeObject<RolesViewModel>((JObject.Parse(_content)["resultData"]).ToString());
            //    //var val = JsonConvert.DeserializeObject<RoleMainModel>(_content);
            //    return val;
            //    // model = JsonConvert.DeserializeObject<List<RoleModel>>(_content.Result);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }


        public RolesViewModel GetUserRoleById(string id)
        {
            return GetAsync<RolesViewModel>("Role/GetUserRoleById?Id=" + id, true).Result;
            //response = client.GetAsync("Role/GetUserRoleById?Id=" + id).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;

            //    var val = JsonConvert.DeserializeObject<RolesViewModel>((JObject.Parse(_content)["resultData"]).ToString());
            //    //var val = JsonConvert.DeserializeObject<RoleMainModel>(_content);
            //    return val;
            //    // model = JsonConvert.DeserializeObject<List<RoleModel>>(_content.Result);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }


        public RolesViewModel GetRoleByName(string roleName)
        {
            return GetAsync<RolesViewModel>("Role/GetRoleByName?roleName=" + roleName, true).Result;
            //response = client.GetAsync("Role/GetRoleByName?roleName=" + roleName).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;

            //    var val = JsonConvert.DeserializeObject<RolesViewModel>((JObject.Parse(_content)["resultData"]).ToString());
            //    //var val = JsonConvert.DeserializeObject<RoleMainModel>(_content);
            //    return val;
            //    // model = JsonConvert.DeserializeObject<List<RoleModel>>(_content.Result);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public RolesViewModel AddRoles(string name, string status, string userid,string DisplayRoleName,int UserType)
        {

            Proven.Model.RolesViewModel model = new RolesViewModel();           
            model.Name = name;
            model.Status = status;
            model.ModifiedBy = userid;
            model.DisplayRoleName = DisplayRoleName;
            model.UserType = UserType;
            model.CreatedBy = userid;
            //content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<RolesViewModel, Proven.Model.RolesViewModel>("Role/CreateRole", model).Result;
            //response = client.PostAsync("Role/CreateRole", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<RolesViewModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public RolesViewModel UpdateRoles(string id, string name, string status, string userid,string DisplayRoleName,int UserType)
        {
            Proven.Model.RolesViewModel model = new RolesViewModel();
            model.Id = id;
            model.Name = name;
            model.Status = status;
            model.ModifiedBy = userid;
            model.DisplayRoleName = DisplayRoleName;
            model.UserType = UserType;
           // var form = new Dictionary<string, string>
           //{
           //    {"Id", id},
           //    {"name", name},
           //    {"Status",status },
           //    {"ModifiedBy", userid},
           //     {"DisplayRoleName",DisplayRoleName },
           //     { "UserType",UserType}
           //};
            //content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<RolesViewModel, Proven.Model.RolesViewModel>("Role/UpdateRole", model).Result;
            //response = client.PostAsync("Role/UpdateRole", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<RolesViewModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public RolesViewModel DeleteRoles(string id)
        {
            var form = new Dictionary<string, string>
           {
               {"Id", id}
           };
            //content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<RolesViewModel, Dictionary<string, string>>("Role/DeleteRole", form).Result;
            //response = client.PostAsync("Role/DeleteRole", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<RolesViewModel>(_content);
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
