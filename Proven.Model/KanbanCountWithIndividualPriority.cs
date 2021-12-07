using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class KanbanCountWithIndividualPriority
    {
        public string KanbanTaskLabelName { get; set; }
        public int LabelNameCount { get; set; }
        public int TotalTasks { get; set; }
        public string AgencyId { get; set; }
    }

    public class KanbanCountWithIndividualPriorityMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<KanbanCountWithIndividualPriority> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }

    }
   
}
