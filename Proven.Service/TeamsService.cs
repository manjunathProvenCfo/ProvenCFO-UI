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
    public class TeamsService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public TeamsMainModel GetTeamsList()
        {

            response = client.GetAsync("Teams/GetAllTeams").Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TeamsMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public TeamsVM GetTeamsById(int Id)
        {
            response = client.GetAsync("Teams/GetTeamsById?Id=" + Id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                var val = JsonConvert.DeserializeObject<TeamsVM>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }
        public TeamsVM GetTeamsByName(string TeamName)
        {
            response = client.GetAsync("Teams/GetTeamByName?TeamName=" + TeamName).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                var val = JsonConvert.DeserializeObject<TeamsVM>((JObject.Parse(_content)["resultData"]).ToString());
                return val;
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }

        public TeamsVM AddTeam(string TeamName, string status, string TeamMemberId1, string TeamMemberId2, string TeamMemberId3, string userid)
        {
            var form = new Dictionary<string, string>
            {
                {"TeamName", TeamName}, 
                {"Status", status },
                {"TeamMemberId1", TeamMemberId1 },
                {"TeamMemberId2", TeamMemberId2 },
                {"TeamMemberId3", TeamMemberId3 },
                {"CreatedBy", userid}
            };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Teams/CreateTeam", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TeamsVM>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }
        public TeamsVM UpdateTeam(string Id, string TeamName, string TeamMemberId1, string TeamMemberId2, string TeamMemberId3, string status, string IsDeleted, string userid)
        {
            var form = new Dictionary<string, string>
           {
               {"Id", Id},
               {"TeamName", TeamName},
               {"Status",status },
                {"TeamMemberId1", TeamMemberId1 },
                {"TeamMemberId2", TeamMemberId2 },
                {"TeamMemberId3", TeamMemberId3 },
               {"ModifiedBy", userid},
               {"IsDeleted", (IsDeleted == null || IsDeleted == ""? false: true).ToString()}

           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Teams/UpdateTeam", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TeamsVM>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public TeamsVM DeleteTeams(int Id)
        {
            var form = new Dictionary<string, string>
           {
               {"Id", Id.ToString()}
           };
            string result = string.Format("Teams/DeleteTeam?Id={0}", Id);
            HttpResponseMessage response = client.PostAsync(result, null).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TeamsVM>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }

        public TeamUserAssociationVM DeleteTeamUserAssociation(int Id)
        {
            var form = new Dictionary<string, string>
           {
               {"Id", Id.ToString()}
           };
            string result = string.Format("Teams/DeleteTeamUserAssociation?Id={0}", Id);
            HttpResponseMessage response = client.PostAsync(result, null).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TeamUserAssociationVM>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }

        public TeamsVM RetireTeams(int Id)
        {
            var form = new Dictionary<string, string>
            {

                {"Id", Id.ToString()}
            };
            string result = string.Format("Teams/DeactivateTeams?Id={0}", Id);
            response = client.PostAsync(result, null).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<TeamsVM>(_content);
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

        public TeamsVM GetTeamClientById(int Id)
        {
            response = client.GetAsync("Teams/GetTeamClientById?Id=" + Id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                var val = JsonConvert.DeserializeObject<TeamsVM>((JObject.Parse(_content)["resultData"]).ToString());
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


