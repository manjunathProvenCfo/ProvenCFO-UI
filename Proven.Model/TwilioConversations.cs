using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class TwilioConversations
    {
        public string ConversationId { get; set; }
        public string ConversationUniqueName { get; set; }
        public bool IsPrivate { get; set; }
        public string Status { get; set; }
        public string CreatedModifiedBy { get; set; }
    }
}