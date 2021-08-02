using Newtonsoft.Json;
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
            client.BaseAddress = new Uri("http://provencfoapi.codewarriorsllc.com/api/");



            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public StringContent PreparePostContent<T>(T model)
        {
            return new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        }
    }
}
