using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class KanbanTasksViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "TaskTitle has a required field.")]        
        [StringLength(250, ErrorMessage = "Maximum 250 characters exceeded")]
        public String TaskTitle { get; set; }
        public String TaskDescription { get; set; }
        public int SegmentTypeID_Ref { get; set; }
        public string SegmentTypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DueDate { get; set; }
        public string dpStartDate { get; set; }
        public string dpEndDate { get; set; }
        public string dpDueDate { get; set; }
        public string Priority { get; set; }
        public string Reporter { get; set; }
        public string ReporterName { get; set; }
        public string Assignee { get; set; }
        public int AgencyId_Ref { get; set; }
        public string Labels { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        
        public int? EstimatedHours { get; set; }
        public string TaskType { get; set; }
    }
}