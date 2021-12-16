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
        }

        public async Task<bool> UpdateTwilioUserId(string UserId, string TwilioUserId)
        {
            string queryString = $"UserId={UserId}&TwilioUserId={TwilioUserId}";
            return await PostAsync<bool>($"Twilio/UpdateTwilioUserId?{queryString}");
        }

        public async Task InsertUpdateTwilioConversation(TwilioConversations twilioConversations)
        {
            await client.PostAsync($"Twilio/InsertUpdateTwilioConversation", PreparePostContent(twilioConversations));
        }
        public void GenereateAllReconciliationTwilioConversationAndAddParticipants(int clientId, string userId, string userEmail)
        {
            Task.Run(async () => await client.PostAsync($"Twilio/GenereateAllReconciliationTwilioConversationAndAddParticipants?clientId={clientId}&userId={userId}&userEmail={userEmail}", null).ConfigureAwait(false)).ConfigureAwait(false);
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
