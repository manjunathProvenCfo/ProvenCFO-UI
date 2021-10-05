using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Proven.Model;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class ClientService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public ClientMainModel GetClientList()
        {
            return GetAsync<ClientMainModel>("Client/GetClientList").Result;
            
        }
        public ClientMainModel GetClientListByStatus(bool IsActive, bool IsDeleted)
        {
            return GetAsync<ClientMainModel>("Client/GetClientListByStatus?IsActive=" + IsActive + "&IsDeleted=" + IsDeleted).Result;
            //response = client.GetAsync("Client/GetClientListByStatus?IsActive=" + IsActive + "&IsDeleted=" + IsDeleted).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public ClientMainModel GetClientListForAgecyUser(string UserID, bool IsActive, bool IsDeleted)
        {
            return GetAsync<ClientMainModel>("Client/GetClientListForAgecyUser?UserID=" + UserID + "&IsActive=" + IsActive + "&IsDeleted=" + IsDeleted).Result;
            //response = client.GetAsync("Client/GetClientListForAgecyUser?UserID=" + UserID + "&IsActive=" + IsActive + "&IsDeleted=" + IsDeleted).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }
        public StateMainModel GetAllStates()
        {
            return GetAsync<StateMainModel>("Client/GetAllStates").Result;
            //response = client.GetAsync("Client/GetAllStates").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<StateMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public ClientModel GetClientByName(string ClientName)
        {
            return GetAsync<ClientModel>("Client/GetClientByName?ClientName=" + ClientName.ToString().Replace("&", "%26"), true).Result;
            //response = client.GetAsync("Client/GetClientByName?ClientName=" + ClientName.ToString().Replace("&", "%26")).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientModel>((JObject.Parse(_content)["resultData"]).ToString());
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public ClientModel GetClientById(int ClientID)
        {
            return GetAsync<ClientModel>("Client/GetClientById?ClientId=" + ClientID, true).Result;
            //response = client.GetAsync("Client/GetClientById?ClientId=" + ClientID).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientModel>((JObject.Parse(_content)["resultData"]).ToString());
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }


        public ClientUserAssociatioMainModel GetClientUserAssociationList()
        {
            return GetAsync<ClientUserAssociatioMainModel>("Client/GetClientUserAssociation").Result;
            //response = client.GetAsync("Client/GetClientUserAssociation").Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientUserAssociatioMainModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}

        }

        public ClientModel CreateClient(string ClientName, string Email, string PhoneNumber, string Address, string ContactPersonName, string CityName, string State, string Status, string LoginUserid, string TeamId, string EntityId, DateTime? StartDate, string XeroID, string XeroScope, string XeroClientID, string XeroClientSecret, bool ReceiveQuarterlyReports, bool EnableAutomation, string XeroContactIDforProvenCfo)
        {
            var form = new Dictionary<string, object>
            {
                {"Name", ClientName},
                {"Email",Email },
                {"PhoneNumber",PhoneNumber },
                {"Address",Address },
                {"ContactPersonName",ContactPersonName },
                {"State",State },
                {"Status",Status },
                {"CityName",CityName },
                {"CreatedBy",LoginUserid },
                 {"TeamId",TeamId },
                {"EntityId" ,EntityId},
                {"StartDate",StartDate },
                {"XeroID",XeroID },
                {"XeroScope",XeroScope },

                {"XeroClientID",XeroClientID },
                {"XeroClientSecret",XeroClientSecret },
                {"ReceiveQuarterlyReports",ReceiveQuarterlyReports },
                { "EnableAutomation",EnableAutomation},
                {"XeroContactIDforProvenCfo",XeroContactIDforProvenCfo }
            };

            //content = new StringContent(JsonConvert.SerializeObject(from), Encoding.UTF8, "application/json");
            return PostAsync<ClientModel, Dictionary<string, object>>("Client/CreateClientUser", form).Result;
            //response = client.PostAsync("Client/CreateClientUser", content).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    AccountService obj = new AccountService();
            //    //var result = obj.Register(Email, "admin", "admin", ClientName, "");

            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}

        }

        public ClientModel UpdateClient(int id, string ClientName, string Email, string PhoneNumber, string Address, string ContactPersonName, string CityName, string State, string Status, string LoginUserid, string TeamId, string BillableEntityId, DateTime? StartDate, string XeroID, string XeroScope,/*string XeroScopeArray,*/ string XeroClientID, string XeroClientSecret,bool ReceiveQuarterlyReports, bool EnableAutomation,string XeroContactIDforProvenCfo)
        {
            var form = new Dictionary<string, object>
            {
                {"Id", Convert.ToString(id)},
                {"Name", ClientName},
                {"State",State },
                {"Status",Status },
                {"CityName",CityName },
                {"ModifiedBy",LoginUserid},
                {"TeamId",TeamId },
                {"EntityId",BillableEntityId },
                 {"StartDate", StartDate },
                {"XeroID", XeroID },
                 {"XeroScope",XeroScope },
                 {"XeroClientID",XeroClientID },
                {"XeroClientSecret",XeroClientSecret },
                {"ReceiveQuarterlyReports",ReceiveQuarterlyReports },
                {"EnableAutomation",EnableAutomation },
                {"XeroContactIDforProvenCfo",XeroContactIDforProvenCfo }

               
                //{"PhoneNumber",PhoneNumber },
                //{"Address",Address },
                //{"ContactPersonName",ContactPersonName },

            };

            //var content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<ClientModel, Dictionary<string, object>>("Client/UpdateClientAgency", form).Result;
            //response = client.PostAsync("Client/UpdateClientAgency", content).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    AccountService obj = new AccountService();
            //    //var result = obj.Register(Email, "admin", "admin", ClientName, "");

            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ClientModel>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}

        }

        #region Client Delete
        public ReturnModel DeleteClient(int id)
        {

            string result = string.Format("Client/SoftDeleteClient?id={0}", id);
            return PostAsync<ReturnModel>(result).Result;
            //response = client.PostAsync(result, null).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<ReturnModel>(_content);
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
        #endregion


        public ClientModel GetClientInvitationAssociationById(int id)
        {
            return GetAsync<ClientModel>("Client/GetClientInvitationAssociationById?Id=" + id, true).Result;
            //response = client.GetAsync("Client/GetClientInvitationAssociationById?Id=" + id).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    var val = JsonConvert.DeserializeObject<ClientModel>((JObject.Parse(_content)["resultData"]).ToString());
            //    return val;
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }
        public bool IsClientInvitationAssociationByIdExists(int id)
        {
            //return GetAsync("Client/IsClientInvitationAssociationByIdExists?Id=" + id).Result;
            response = client.GetAsync("Client/IsClientInvitationAssociationByIdExists?Id=" + id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                var val = Convert.ToBoolean(JObject.Parse(_content)["resultData"]);
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
