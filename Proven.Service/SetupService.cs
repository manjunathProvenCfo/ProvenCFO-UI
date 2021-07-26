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
    public class SetupService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public JobTitleModel GetJobTitleById(int Id)
        {
            response = client.GetAsync("Setup/GetJobTitleById?id=" + Id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<JobTitleModel>((JObject.Parse(_content)["resultData"]).ToString());
               
                return val;
               
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }
        public JobTitleModel GetJobTitleByTitle(string jobTitle)
        {
            response = client.GetAsync("Setup/GetJobTitleByTitle?jobTitle=" + jobTitle).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<JobTitleModel>((JObject.Parse(_content)["resultData"]).ToString());

                return val;

            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }
        public JobTitleMainModel GetJobTitleList()
        {
            response = client.GetAsync("Setup/GetJobTitleList").Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JobTitleMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public JobTitleMainModel GetJobTitleListInvitation()
        {
            response = client.GetAsync("Setup/GetJobTitleListInvitation").Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JobTitleMainModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public JobTitleModel SaveJobTitle(JobTitleModel model)
        {

            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            response = client.PostAsync("Setup/AddNewJobTitle", content).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JobTitleModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public JobTitleModel AddJobTitle(string title, string jobCode, string status,  string userid)
        {
            var form = new Dictionary<string, string>
           {
               {"Title", title},
               {"JobCode", jobCode},
               {"Status",status },
               {"CreatedBy", userid}
           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Setup/CreateJobTitles", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JobTitleModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public JobTitleModel UpdateJobTitle(string Id, string title, string jobCode, string status, string IsDeleted, string loginuser)
        {
            var form = new Dictionary<string, string>
           {
               {"Id", Id},
               {"Title", title},
               {"JobCode",jobCode},
               {"Status", status},
               {"ModifiedBy",loginuser},
               {"IsDeleted", (IsDeleted == null || IsDeleted == ""? false: true).ToString()}

           };
            content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            response = client.PostAsync("Setup/UpdateJobTitle", content).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JobTitleModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);

            }
        }

        public JobTitleModel DeleteJobTitle(int id)
        {
            string result = string.Format("Setup/DeleteJobTitle?id={0}", id);
            response = client.PostAsync(result, null).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JobTitleModel>(_content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }

        public JobTitleModel RetireJobTitle(int id)
        {
            var form = new Dictionary<string, string>
            {

                {"Id", id.ToString()}
            };
            string result = string.Format("Setup/DeactivateJobTitles?id={0}", id);
            response = client.PostAsync(result, null).Result;

            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<JobTitleModel>(_content);
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

        public JobTitleModel GetJobUserRoleById(int Id)
        {
            response = client.GetAsync("Setup/GetJobUserRoleById?id=" + Id).Result;
            if (response.IsSuccessStatusCode)
            {
                var _content = response.Content.ReadAsStringAsync().Result;

                var val = JsonConvert.DeserializeObject<JobTitleModel>((JObject.Parse(_content)["resultData"]).ToString());

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