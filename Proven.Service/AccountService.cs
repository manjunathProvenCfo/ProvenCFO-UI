using Newtonsoft.Json;
using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class AccountService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public LoginMainModel LoginAccess(string username, string password)
        {
            response = client.GetAsync("Account/Login?username=" + username + "&Password=" + password).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<LoginMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public LoginModel Register(string email, string passwordhash, string confirmpassword, string firstname, string lastname,int? UserType,int? AgencyID)
        {
            var form = new Dictionary<string, string>
           {
               {"Email", email},
               {"PasswordHash", passwordhash},
               {"confirmpassword", confirmpassword},
               {"FirstName", firstname},
               {"LastName", lastname},
               {"UserType", UserType.HasValue == true? Convert.ToString(UserType.Value): ""},
               {"AgencyID", AgencyID.HasValue == true? Convert.ToString(AgencyID.Value): ""},
           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Account/Register/register", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<LoginModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public LoginModel ForgotPassword(string email)
        {
            var form = new Dictionary<string, string>
           {

               {"Email", email},

           };
            var content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Account/ForgotPassword/ForgotPassword", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<LoginModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public LoginProfileMainModel ChangePasswordForProfile(string oldpassword, string newpassword, string confirmpassword, string id)
        {
            var form = new Dictionary<string, string>
           {
               {"oldpassword", oldpassword},
               {"newpassword", newpassword},
               {"ConfirmedPassword", confirmpassword},
                {"Id", id}

           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Account/ResetPassword/ResetPassword", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<LoginProfileMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public LoginMainModel ChangePassword(string oldpassword, string newpassword, string confirmpassword, string id)
        {
            var form = new Dictionary<string, string>
           {
               {"oldpassword", oldpassword},
               {"newpassword", newpassword},
               {"ConfirmedPassword", confirmpassword},
                {"Id", id}

           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Account/ResetPassword/ResetPassword", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<LoginMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public LoginModel SetPassword(string id, string newpassword, string confirmpassword, string ActivationCode)
        {
            var form = new Dictionary<string, string>
           {
                 {"Id", id},
               {"ActivationCode", ActivationCode},
               {"newpassword", newpassword},
               {"confirmpassword", confirmpassword}

           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Account/SetPassword/SetPassword", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<LoginModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public UserMainModel RegisteredUserList()
        {
            response = client.GetAsync("Account/GetRegisteredUsers").Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<UserMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }
        public InviteUserMainModel RegisteredUserListbyAgency(string selectedAgency)
        {
            response = client.GetAsync("Account/GetRegisteredUsersByAgency?AgencyId=" + selectedAgency).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InviteUserMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }


        public UserDetailMainModel GetUserDetail(string id)
        {
            string result = string.Format("Account/GetUserDetail?id={0}", id);
            response = client.GetAsync(result).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<UserDetailMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public UserDetailMainModel GetUserById(int Invite_id)
        {
            string result = string.Format("Invitation/GetUserById?id={0}", Invite_id);
            response = client.GetAsync(result ).Result;
            //HttpResponseMessage response = client.GetAsync("Account/Login?username=" + username + "&Password=" + password).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<UserDetailMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public ReturnModel IsInviteValied(int Invite_id)
        {
            string result = string.Format("Invitation/IsInviteValied?id={0}", Invite_id);
            response = client.GetAsync(result).Result;
            //HttpResponseMessage response = client.GetAsync("Account/Login?username=" + username + "&Password=" + password).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ReturnModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }
        public ReturnModel IsInviteRegisterd(string Invite_email)
        {
            string result = string.Format("Invitation/IsInviteRegisterd?email={0}", Invite_email);
            response = client.GetAsync(result).Result;
            //HttpResponseMessage response = client.GetAsync("Account/Login?username=" + username + "&Password=" + password).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<ReturnModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }


        public UserDetailMainModel UpdateUserDetail(UserModel model)
        {
            var form = new Dictionary<string, string>
           {
               {"Email", model.Email},
               {"FirstName", model.FirstName},
               {"LastName", model.LastName},
               {"AboutUs", model.AboutUs},
               {"CoverImage", model.CoverImage},
               {"ProfileImage", model.ProfileImage},
                {"PhoneNumber", model.PhoneNumber},
               {"Id", model.Id}
           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Account/UpdateUserDetail", content).Result;


            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<UserDetailMainModel>(_content);
            }
            else
            {
                string msg = response.Content.ReadAsStringAsync().Result;
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
                if(response != null)
                response.Dispose();
                if (content != null)
                {
                    content.Dispose();
                }

                // free managed resources               
            }
            isDisposed = true;
        }

        public UserMainModel GetRegisteredStaffUserList()
        {
            response = client.GetAsync("Account/GetRegisterdStaffUserList").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<UserMainModel>(content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }
    }
}
