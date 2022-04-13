using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Proven.Service
{
    public class BaseService
    {
        public HttpClient client = new HttpClient();
        public HttpClient Prodclient = new HttpClient();
        //public HttpClient Licenseclient = new HttpClient();
        public BaseService()
        {
            //TODO:Make it dynamic from AppSettings
            client.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["provencfoapi"]));            
            Prodclient.BaseAddress = new Uri(Convert.ToString(ConfigurationManager.AppSettings["provenCfoTokenapi"]));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
        }
        #region HttpClientWrapperMethods
        public StringContent PreparePostContent<T>(T model)
        {
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        }
        private HttpResponseMessage GetBase(string url)
        {
            return client.GetAsync(url).Result;
        }

        public async Task<T> GetAsync<T>(string url, bool readResultDataProp = false)
        {
            var response = GetBase(url);
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

        private HttpResponseMessage PostBase(string url, HttpContent httpContent)
        {
            return client.PostAsync(url, httpContent).Result;
        }
        private HttpResponseMessage PostBaseWithCancellationToken(string url, HttpContent httpContent, CancellationToken? cancellationToken=null)
        {
            if (cancellationToken.HasValue)
            {
                return client.PostAsync(url, httpContent, cancellationToken.Value).Result;
            }
            else
            {
                return client.PostAsync(url, httpContent).Result;
            }
            
        }
        private HttpResponseMessage ProdPostBase(string url, HttpContent httpContent)
        {
            return Prodclient.PostAsync(url, httpContent).Result;
        }
        //public async Task<T> PostAsync<T>(string url)
        //{
        //    return await PostAsync<T>(url, null);
        //}
        //public async Task<T> PostAsyncWithCancellationToken<T>(string url,)
        //{
        //    return await PostAsync<T>(url, null);
        //}
        

        public async Task<T> PostAsync<T, C>(string url, C contentToSerialize, bool readResultDataProp = false)
        {
            var httpContent = PreparePostContent(contentToSerialize);
            return await PostAsync<T>(url, httpContent, readResultDataProp);
        }
        public async Task<T> PostAsyncWithCancellationToken<T, C>(string url, C contentToSerialize, CancellationToken? cancellationToken=null, bool readResultDataProp = false)
        {
            var httpContent = PreparePostContent(contentToSerialize);
            return await PostAsyncWithCancellationToken<T>(url, httpContent, cancellationToken, readResultDataProp);
        }
        public async Task<T> ProdPostAsync<T, C>(string url, C contentToSerialize, bool readResultDataProp = false)
        {
            var httpContent = PreparePostContent(contentToSerialize);
            return await ProdPostAsync<T>(url, httpContent, readResultDataProp);
        }
        public async Task<T> PostAsync<T>(string url, HttpContent httpContent = null, bool readResultDataProp = false)
        {
            var response = PostBase(url, httpContent);
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
        public async Task<T> PostAsyncWithCancellationToken<T>(string url, HttpContent httpContent = null, CancellationToken? cancellationToken = null, bool readResultDataProp = false)
        {
            var response = PostBaseWithCancellationToken(url, httpContent, cancellationToken);
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
        public async Task<T> ProdPostAsync<T>(string url, HttpContent httpContent = null, bool readResultDataProp = false)
        {
            var response = ProdPostBase(url, httpContent);
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