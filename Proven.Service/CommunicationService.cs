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
    public class CommunicationService : BaseService, IDisposable
    {
        private bool isDisposed = false;

        public async Task<List<ChatChannel>> GetChatParticipants(string UserID, string userEmail)
        {
            return await GetAsync<List<ChatChannel>>($"Communication/GetChatParticipants?userId={UserID}&userEmail={userEmail}");
        }
        public async Task<List<ChatChannel>> GetPublicChat(string userId, string userEmail, TwilioConversationsTypeEnum type, string channelUniqueNameGuid)
        {
            return await GetAsync<List<ChatChannel>>($"Communication/GetPublicChat?userId={userId}&userEmail={userEmail}&type={(byte)type}&channelUniqueNameGuid={channelUniqueNameGuid}");
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
                // free managed resources               
            }
            isDisposed = true;
        }
    }
}
