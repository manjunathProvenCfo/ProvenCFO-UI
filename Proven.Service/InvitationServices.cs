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
    public class InvitationServices : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public InviteUserMainModel GetInvitation()
        {
            response = client.GetAsync("Invitation/GetALLInvitation").Result;
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

        public InviteUserVMId GetInvitationById(int id)
        {
            response = client.GetAsync("Invitation/GetuserById?Id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<InviteUserVMId>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public InviteUserVMId GetInvitationByEmail(string Email)
        {
            response = client.GetAsync("Invitation/GetInvitationByEmail?Email=" + Email).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<InviteUserVMId>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }
        public InviteUserVMId GetAgencyUserInvitationByEmail(string Email)
        {
            response = client.GetAsync("Invitation/GetAgencyUserInvitationByEmail?Email=" + Email).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<InviteUserVMId>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }

        public InviteUserModel AddInvitation(string firstname, string lastname, string email, string roleid, string jobid, string sessiontimeout, string LoginUserid)
        {
            var form = new Dictionary<string, string>
           {

               {"FirstName", firstname},
               {"LastName" ,lastname},
               {"Email", email},
               {"RoleId", roleid },
               {"JobId",jobid },
               {"sessiontimeout", sessiontimeout},
               //{"UserId", LoginUserid},
               {"CreatedBy", LoginUserid}

           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");

            response = client.PostAsync("Invitation/SendInvitation/SendInvitation", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InviteUserModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public InviteUserModel UpdateInvitationAgency(string firstname, string lastname, string email, string status, string LoginUserid, string agencyid)
        {
            var form = new Dictionary<string, string>
           {
               {"FirstName", firstname},
               {"LastName" ,lastname},
               {"Email", email},
               {"AgencyID", agencyid },
                {"IsActive",status },
               {"ModifiedBy", LoginUserid}
           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Invitation/UpdateInvitatedAgencyUser", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InviteUserModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }
        public InviteUserModel AddInvitationAgency(string firstname, string lastname, string email, string sessiontimeout, string agencyid, string LoginUserid)
        {
            var form = new Dictionary<string, string>
           {

               {"FirstName", firstname},
               {"LastName" ,lastname},
               {"Email", email},
               {"sessiontimeout", sessiontimeout},
               {"UserId", LoginUserid},
               {"AgencyID",agencyid }

           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");


            response = client.PostAsync("Invitation/SendInvitationAgency", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InviteUserModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public InviteUserModel DeactivateInvitation(int id)
        {
            var form = new Dictionary<string, string>
           {

               {"id", id.ToString()}
           };
            string result = string.Format("Invitation/DeactivateInvitation?id={0}", id);
            response = client.PostAsync(result, null).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InviteUserModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }


        public InviteUserModel UpdateInvite(string id, string firstname, string lastname, string rolename, string jobtitle, string status, string LoginUserid)

        {
            var form = new Dictionary<string, string>
           {
                 {"Id", id.ToString()},
               {"FirstName", firstname},
                {"LastName" ,lastname},
              {"RoleName",rolename },
                {"JobTitle",jobtitle },
                {"IsActive",status.ToString()},
                {"UserId",LoginUserid },
                {"ModifiedBy",LoginUserid}
           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Invitation/UpdateUserInvite", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InviteUserModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public InviteUserVMId GetInvitationForVlidation(int InvitationId, int AgencyId, Guid ActivationCode)
        {
            response = client.GetAsync("Invitation/GetInvitationForVlidation?InvitationId=" + InvitationId + "&AgencyId=" + AgencyId + "&ActivationCode=" + ActivationCode).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<InviteUserVMId>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public InviteUserVMId GetInvitationForVlidationStaff(int InvitationId, Guid ActivationCode)
        {
            response = client.GetAsync("Invitation/GetInvitationForVlidationStaff?InvitationId=" + InvitationId + "&ActivationCode=" + ActivationCode).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<InviteUserVMId>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        #region "UserAgencyLink"

        public ReturnModel ExistingAgencyUserInvitation(string InvitationId, string AgencyId, string ActiveCode)
        {
            var form = new Dictionary<string, string>
            {
               {"Id", InvitationId},
               {"AgencyId" ,AgencyId},
               {"ActivationCode", ActiveCode}

            };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");

            response = client.PostAsync("Invitation/ExistingAgencyUserInvitation", content).Result;

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




        #endregion



        #region User
        public ReturnModel DeleteUser(string id)
        {

            string result = string.Format("Account/DeleteUser?id={0}", id);
            response = client.PostAsync(result, null).Result;
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


        #endregion

        #region Invite Delete
        public ReturnModel DeleteInvite(int id)
        {

            string result = string.Format("Invitation/DeleteInvite?id={0}", id);
            response = client.PostAsync(result, null).Result;
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


        #endregion
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

        public InviteUserMainModel GetRegisteredStaffUserList()
        {
            response = client.GetAsync("Invitation/GetRegisterdStaffUserList").Result;
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<InviteUserMainModel>(content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }

    }
}
