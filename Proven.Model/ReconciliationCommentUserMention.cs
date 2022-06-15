using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class ReconciliationCommentUserMention
    {
        public int Id { get; set; }
        public string MentionedUserId { get; set; }
        public string MentionedByUserId { get; set; }
        public string MentionedUserName { get; set; }
        public string MentionedByUserName { get; set; }
        public int ReconciliationCommentId_ref { get; set; }
        public bool IsDeleted { get; set; }
    }
}
