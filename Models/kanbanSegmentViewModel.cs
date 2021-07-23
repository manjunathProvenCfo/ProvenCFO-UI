using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class kanbanSegmentViewModel
    {
        public string Id { get; set; }
        public string SegmentType { get; set; }
        public string SegmentDisplayName { get; set; }
        public string SegmentDesciption { get; set; }
        public string Status { get; set; }
        public string IsDeleted { get; set; }
        public List<KanbanTasksViewModel> KanbanTasksList { get; set; }
    }
}