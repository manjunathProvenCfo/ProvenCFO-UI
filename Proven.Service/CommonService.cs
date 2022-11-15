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
    public class CommonService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public UserPreferencesVM SetUserPreferences(UserPreferencesVM UserPref, string LoggedInUser)
        {
            var form = new Dictionary<string, string>
           {
               {"Id",Convert.ToString(UserPref.Id)},
               {"PreferenceCategory", UserPref.PreferenceCategory},
               {"Sub_Category",UserPref.Sub_Category},
               {"PreferanceValue", UserPref.PreferanceValue},
               {"UserID",UserPref.UserID},
               {"UserRole",UserPref.UserRole},
               {"CreatedBy",LoggedInUser}
           };
            //var content = new StringContent(JsonConvert.SerializeObject(form), Encoding.UTF8, "application/json");
            return PostAsync<UserPreferencesVM, Dictionary<string, string>>("Common/SetUserPreferences", form, true).Result;
            //response = client.PostAsync("Common/SetUserPreferences", content).Result;

            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;
            //    return JsonConvert.DeserializeObject<UserPreferencesVM>(_content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);

            //}
        }

        public List<UserPreferencesVM> GetUserPreferences(string UserID)
        {
            return GetAsync<List<UserPreferencesVM>>("Common/GetUserPreferences?UserID=" + UserID, true).Result;
            //response = client.GetAsync("Common/GetUserPreferences?UserID=" + UserID).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;

            //    var val = JsonConvert.DeserializeObject<List<UserPreferencesVM>>((JObject.Parse(_content)["resultData"]).ToString());

            //    return val;
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }

        public List<UserSecurityVM> GetUserSecurityModels(string UserEmail)
        {
            return GetAsync<List<UserSecurityVM>>("Common/GetUserSecurityModels?userEmail=" + UserEmail, true).Result;
            //response = client.GetAsync("Common/GetUserSecurityModels?userEmail=" + UserEmail).Result;
            //if (response.IsSuccessStatusCode)
            //{
            //    var _content = response.Content.ReadAsStringAsync().Result;

            //    var val = JsonConvert.DeserializeObject<List<UserSecurityVM>>((JObject.Parse(_content)["resultData"]).ToString());

            //    return val;
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


        public UserTypeVM FindUserByEmail(string email)
        {
            var res   = GetAsync<object>("Common/FindUserByEmail?email=" + email).Result;

            var re = JsonConvert.DeserializeObject<UserTypeVM>(res.ToString());
            return (UserTypeVM)re;

        }
    }
}
