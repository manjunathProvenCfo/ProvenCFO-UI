using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class BaseService
    {
        public HttpClient client = new HttpClient();
        //public HttpClient Licenseclient = new HttpClient();
        public BaseService()
        {
            //TODO:Make it dynamic from AppSettings
            //client.BaseAddress = new Uri("http://localhost:27754/Api/");
            client.BaseAddress = new Uri("https://provencfoapi.codewarriorsllc.com/api/");

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        #region HttpClientWrapperMethods
        public StringContent PreparePostContent<T>(T model)
        {
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        }
        private async Task<HttpResponseMessage> GetBase(string url)
        {
            return await client.GetAsync(url).ConfigureAwait(false);
        }

        public async Task<T> GetAsync<T>(string url, bool readResultDataProp = false)
        {
            var response = await GetBase(url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (readResultDataProp)
                    return JsonConvert.DeserializeObject<T>((JObject.Parse(content)["resultData"]).ToString());
                else
                    return JsonConvert.DeserializeObject<T>(content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }

        private async Task<HttpResponseMessage> PostBase(string url, HttpContent httpContent)
        {
            return await client.PostAsync(url, httpContent).ConfigureAwait(false);
        }
        public async Task<T> PostAsync<T>(string url)
        {
            return await PostAsync<T>(url, null);
        }

        public async Task<T> PostAsync<T, C>(string url, C contentToSerialize, bool readResultDataProp = false)
        {
            var httpContent = PreparePostContent(contentToSerialize);
            return await PostAsync<T>(url, httpContent, readResultDataProp);
        }
        public async Task<T> PostAsync<T>(string url, HttpContent httpContent = null, bool readResultDataProp = false)
        {
            var response = await PostBase(url, httpContent);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (readResultDataProp)
                    return JsonConvert.DeserializeObject<T>((JObject.Parse(content)["resultData"]).ToString());
                else
                    return JsonConvert.DeserializeObject<T>(content);
            }
            else
            {
                string msg = response.ReasonPhrase;
                throw new Exception(msg);
            }
        }
        #endregion
    }
}