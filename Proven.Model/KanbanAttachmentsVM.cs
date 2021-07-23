using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class KanbanAttachmentsVM
    {
        public int Id { get; set; }
        public String AttachedFileName { get; set; }
        public String FilePath { get; set; }
        public string FileType { get; set; }
        public string FileExtention { get; set; }
        public bool? IsTaskAttachement { get; set; }
        public bool? IsCommentAttachment { get; set; }
        public int TaskId_Ref { get; set; }
        public int? CommentId_Ref { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string CreatedDateForDisplay { get; set; }
        // public System.IO.Stream InputStream { get; set; }
    }
    public class KanbanAttachmentsStream
    {        
        public String AttachedFileName { get; set; }
        
        public System.IO.Stream InputStream { get; set; }
    }
    public class KanbanAttachmentsMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public KanbanAttachmentsVM ResultData { get; set; }
        
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
    public class KanbanAttachmentsListMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }        
        public List<KanbanAttachmentsVM> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
