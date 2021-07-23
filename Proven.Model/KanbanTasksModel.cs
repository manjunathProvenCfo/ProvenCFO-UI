using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class KanbanTasksModel
    {

        public int? Id { get; set; }
        public String TaskTitle { get; set; }
        public String TaskDescription { get; set; }       
        public int SegmentTypeID_Ref { get; set; }
        public string SegmentTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; }
        public string Reporter { get; set; }
        public string ReporterName { get; set; }
        public string Assignee { get; set; }
        public int AgencyId_Ref { get; set; }
        public string AgencyName { get; set; }
        public string Labels { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? EstimatedHours { get; set; }
        public string TaskType { get; set; }
        public int? NumberofAttachment { get; set; }
        public List<KanbanAttachmentsVM> KanbanAttachments { get; set; }
        public List<KanbanTaskAssigneeAsssociationShortVM> KanbanAssigneesList { get; set; }
        public List<KanbanTaskCommentsListModel> KanbanComments { get; set; }
    }

    public class KanbanTasksVM
    {
        public int Id { get; set; }
        public String TaskTitle { get; set; }
        public String TaskDescription { get; set; }
        public int SegmentTypeID_Ref { get; set; }
        public string SegmentTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; }
        public string Reporter { get; set; }
        public string ReporterName { get; set; }
        public string Assignee { get; set; }
        public int AgencyId_Ref { get; set; }
        public string AgencyName { get; set; }
        public string Labels { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int? EstimatedHours { get; set; }
        public string TaskType { get; set; }

        public List<KanbanAttachmentsVM> KanbanAttachments { get; set; }
    }
    public class KanbanTasksMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<KanbanTasksModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
