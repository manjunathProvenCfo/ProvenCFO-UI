using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class KanbanTaskAssigneeAsssociationModel
    {
        public int Id { get; set; }
        public int? TaskId_Ref { get; set; }
        public string UserId_Ref { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class KanbanTaskAssigneeAsssociationShortVM
    {
        public string Id { get; set; }
        public int? TaskId_Ref { get; set; }
        public string UserId_Ref { get; set; }
        public string UserName { get; set; }
        public string ProfileImage { get; set; }

        

    }
    public class KanbanTaskAssigneeAsssociationMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<KanbanTaskAssigneeAsssociationModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
