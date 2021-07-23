using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
   public class KanbanTaskCommentsModel
    {
        public int Id { get; set; }
        public String CommentText { get; set; }
        public string CommentType { get; set; }      
        public int TaskId_Ref { get; set; }        
        public int ParentCommentID_Ref { get; set; }
        public int LikeCount { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class KanbanTaskCommentsMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public KanbanTaskCommentsModel ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
