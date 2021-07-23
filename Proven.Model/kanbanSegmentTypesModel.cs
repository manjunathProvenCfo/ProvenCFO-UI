using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class kanbanSegmentTypesModel
    {
        public string Id { get; set; }
        public string SegmentType { get; set; }
        public string SegmentDisplayName { get; set; }
        public string SegmentDesciption { get; set; }
        public string Status { get; set; }
        public string IsDeleted { get; set; }
        public List<KanbanTasksModel> KanbanTaskList { get; set; }
    }
    public class kanbanSegmentTypesMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<kanbanSegmentTypesModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
