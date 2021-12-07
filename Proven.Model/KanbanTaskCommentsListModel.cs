using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class KanbanTaskCommentsListModel
    {
        public int Id { get; set; }
        public String CommentText { get; set; }
        public string CommentType { get; set; }
        public int TaskId_Ref { get; set; }
        public int ParentCommentID_Ref { get; set; }
        public int LikeCount { get; set; }
        public string Status { get; set; }
        public string CommentDuration { get; set; }
        public string UserProfilePic { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string IsDeleted { get; set; }
    }
    public class KanbanTaskCommentsListMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<KanbanTaskCommentsListModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
    public class KanbanTaskCommentsVMMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public KanbanTaskCommentsListModel ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
