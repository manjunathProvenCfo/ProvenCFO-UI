using Newtonsoft.Json;
using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
            return GetAsync<LoginMainModel>("Account/Login?username=" + username + "&Password=" + password).Result;
            //response = client.GetAsync("Account/Login?username=" + username + "&Password=" + password).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<LoginMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public LoginModel Register(string email, string passwordhash, string confirmpassword, string firstname, string lastname, int? UserType, int? AgencyID)
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
            //content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<LoginModel, Dictionary<string, string>>("Account/Register/register", form).Result;
            //response = client.PostAsync("Account/Register/register", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<LoginModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        //public ReconciliationMainModel CreateReconciliation(string id, string account_name, string amount, string company, string date, string description, string gl_account, string reconciled, string reference, string rule, string type)
        //{
        //    reconciliationVM rv = new reconciliationVM();
        //    rv.id = id;
        //    rv.account_name = account_name;
        //    rv.amount = amount;
        //    rv.company = company;
        //    rv.date = date;
        //    rv.description = description;
        //    rv.gl_account = gl_account;
        //    rv.reconciled = reconciled;
        //    rv.reference = reference;
        //    rv.rule = rule;
        //   // rv.type = type;
        //    var form = new Dictionary<string, string>
        //   {
        //       {"plenadata__id", "0"},
        //       {"id", id},
        //       {"account_name", account_name},
        //       {"amount",  amount},
        //       {"company", company},
        //       {"date", date},
        //       {"description", description},
        //       {"gl_account", gl_account},
        //       {"reconciled", reconciled},
        //       {"reference", reference},
        //       {"rule", rule},
        //       {"type", type}
        //   };
        //    content = new StringContent(JsonConvert.SerializeObject(rv), Encoding.UTF8, "application/json");
        //    client.DefaultRequestHeaders.Authorization
        //                 = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJSZWNvbmNpbGlhdGlvblNlcnZpY2VBY2Nlc3NUb2tlbiIsImp0aSI6ImM5MDdlM2RmLWY2YzgtNDg5MC05Y2RlLThiZmM5Zjg2ZjQyZCIsIklkIjoiNDBjZTQxYmItNTkzNi00MGMxLTk3ZjktYjk4YTJmZmY2MDM3IiwiRmlyc3ROYW1lIjoicGxhbmEiLCJMYXN0TmFtZSI6ImRhdGEiLCJVc2VyTmFtZSI6ImluZm9AcGxlbmFkYXRhLmNvbSIsIkVtYWlsIjoiaW5mb0BwbGVuYWRhdGEuY29tIiwiZXhwIjoxNjI3NjQzNTk0LCJpc3MiOiJQcm92ZW5DRk9BdXRoZW50aWNhdGlvblNlcnZlciIsImF1ZCI6IlRoaXJkUGFydHlTZXJ2aWNlUG9zdG1hbkNsaWVudCJ9.Zurze_b-v8hSmWrSUDLd4R7vF7agchYMLfEZJTzPC7Q");
        //    response = client.PostAsync("Reconciliation/CreateReconciliation", content).Result;


        //    if (response.IsSuccessStatusCode)
        //    {
        //        var _content = response.Content.ReadAsStringAsync().Result;
        //        return JsonConvert.DeserializeObject<ReconciliationMainModel>(_content);
        //    }
        //    else
        //    {
        //        string msg = response.ReasonPhrase;
        //        throw new Exception(msg);

        //    }
        //}

        public LoginModel ForgotPassword(string email)
        {
            var form = new Dictionary<string, string>
           {

               {"Email", email},

           };
            //var content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<LoginModel, Dictionary<string, string>>("Account/ForgotPassword/ForgotPassword", form).Result;
            //response = client.PostAsync("Account/ForgotPassword/ForgotPassword", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<LoginModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
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
            //content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<LoginProfileMainModel, Dictionary<string, string>>("Account/ResetPassword/ResetPassword", form).Result;
      
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
            return PostAsync<LoginMainModel, Dictionary<string, string>>("Account/ResetPassword/ResetPassword", form).Result;
        
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
            return PostAsync<LoginModel, Dictionary<string, string>>("Account/SetPassword/SetPassword", form).Result; 
           
        }

        public UserMainModel RegisteredUserList()
        {
            return GetAsync<UserMainModel>("Account/GetRegisteredUsers").Result;
         
        }
        public InviteUserMainModel RegisteredUserListbyAgency(string selectedAgency)
        {
            return GetAsync<InviteUserMainModel>("Account/GetRegisteredUsersByAgency?AgencyId=" + selectedAgency).Result;           
        }
        public InviteUserMainModel GetRegisteredUsersByAgencyWithReqPermission(string selectedAgency,string RequiredPermissionCode)
        {
            return GetAsync<InviteUserMainModel>("Account/GetRegisteredUsersByAgencyWithReqPermission?AgencyId=" + selectedAgency + "&RequiredPermissionCode=" + RequiredPermissionCode).Result;
        }

        public UserDetailMainModel GetUserDetail(string id)
        {
            string result = string.Format("Account/GetUserDetail?id={0}", id);
            return GetAsync<UserDetailMainModel>(result).Result;
     
        }

        public UserDetailMainModel GetUserById(int Invite_id)
        {
            string result = string.Format("Invitation/GetUserById?id={0}", Invite_id);
            return GetAsync<UserDetailMainModel>(result).Result;
    
        }

        public ReturnModel IsInviteValied(int Invite_id)
        {
            string result = string.Format("Invitation/IsInviteValied?id={0}", Invite_id);
            return GetAsync<ReturnModel>(result).Result;
      
        }
        public ReturnModel IsInviteRegisterd(string Invite_email)
        {
            string result = string.Format("Invitation/IsInviteRegisterd?email={0}", Invite_email);
            return GetAsync<ReturnModel>(result).Result;
          
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
                {"LinkedInProfile", model.LinkedInProfile},
               {"Id", model.Id}
           };
            return PostAsync<UserDetailMainModel, Dictionary<string, string>>("Account/UpdateUserDetail", form).Result;
        
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

        public UserMainModel GetRegisteredStaffUserList()
        {
            return GetAsync<UserMainModel>("Account/GetRegisterdStaffUserList").Result;
        
        }
    }
}
