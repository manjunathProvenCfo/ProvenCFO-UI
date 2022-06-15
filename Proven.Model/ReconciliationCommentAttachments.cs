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
    [Serializable]
    public class ReconciliationCommentAttachmentsVM
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileType { get; set; }
        public string ReconciliationId_ref { get; set; }
        public int? ReconciliationCommentId_ref { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? CreatedDateUTC { get; set; }
        public string CreatedBy { get; set; }
    }

    public class ReconciliationCommentAttachmentsMain
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public ReconciliationCommentAttachmentsVM ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
