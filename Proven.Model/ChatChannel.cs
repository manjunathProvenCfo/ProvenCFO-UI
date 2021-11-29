using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class ChatChannel
    {
        public string ChannelId { get; set; }
        public string ChannelName { get; set; }
        public string ChannelUniqueName { get; set; }
        public string ChannelImage { get; set; }
        public bool IsPrivate { get; set; }
        public List<ChatParticipants> ChatParticipants { get; set; }
        #region reconciliation
        public string AccountName { get; set; }
        public string Company { get; set; }
        public string ReconciliationDescription { get; set; }
        public DateTime ReconciliationDate { get; set; }
        public decimal? ReconciliationAmount { get; set; }
        #endregion
    }
}
