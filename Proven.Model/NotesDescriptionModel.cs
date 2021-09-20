using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class NotesDescriptionModel
    {
        public int? Id { get; set; }
        public string NoteCatId { get; set; }
        public string AgencyId { get; set; }
        public string IsPublished { get; set; }
        public int Position { get; set; }
        [Required(ErrorMessage = "This field cannot be blank.")]
        [StringLength(250, ErrorMessage = "Maximum 250 characters exceeded")]
        public string Title { get; set; }
        [Required(ErrorMessage = "This field cannot be blank.")]
        public string Description { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsResolved { get; set; }
        public int UserType { get; set; }
        public string[] Ids { get; set; }
        public int[] Positions { get; set; }
        public int TotalNotes { get; set; }

    }
    public class NotesDescriptionMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<NotesDescriptionModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
    public class NotesDescriptionMainModel1
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public NotesDescriptionModel ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
