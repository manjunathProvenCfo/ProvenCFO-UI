using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class ReconciliationCommentAttachments
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string ReconciliationId_ref { get; set; }
        public int ReconciliationCommentId_ref { get; set; }
        public Boolean IsDeleted { get; set; }
        public DateTime? CreatedDateUTC { get; set; }
        public string CreatedBy { get; set; }
    }
}
