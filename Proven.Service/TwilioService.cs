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
    public class TwilioService : BaseService, IDisposable
    {
        private bool isDisposed = false;
        private HttpResponseMessage response;
        private StringContent content;
        public async Task<string> GetToken(string identity)
        {
            return await PostAsync<string>("Twilio/Token?identity=" + identity);
            //response = await client.PostAsync("Twilio/Token?identity=" + identity, null).ConfigureAwait(false);
            //if (response.IsSuccessStatusCode)
            //{
            //    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            //    return JsonConvert.DeserializeObject<string>(content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }

        public async Task<bool> UpdateTwilioUserId(string UserId, string TwilioUserId)
        {
            string queryString = $"UserId={UserId}&TwilioUserId={TwilioUserId}";
            return await PostAsync<bool>($"Twilio/UpdateTwilioUserId?{queryString}");
            //response = await client.PostAsync($"Twilio/UpdateTwilioUserdId?{queryString}", null);
            //if (response.IsSuccessStatusCode)
            //{
            //    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            //    return JsonConvert.DeserializeObject<bool>(content);
            //}
            //else
            //{
            //    string msg = response.ReasonPhrase;
            //    throw new Exception(msg);
            //}
        }

        public async Task InsertUpdateTwilioConversation(TwilioConversations twilioConversations)
        {
            await client.PostAsync($"Twilio/InsertUpdateTwilioConversation", PreparePostContent(twilioConversations));
            //response = await client.PostAsync($"Twilio/InsertUpdateTwilioConversation", PreparePostContent(twilioConversations));
            //if (!response.IsSuccessStatusCode)
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
    }
}
