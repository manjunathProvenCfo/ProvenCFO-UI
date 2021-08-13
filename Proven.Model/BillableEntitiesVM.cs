using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class BillableEntitiesVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Entity Name is a required field.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "Please Enter a valid Entity Name")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string EntityName { get; set; }
        public string  ProvenCFOXeroContactID { get; set; }

        [Required(ErrorMessage = "Clients is a required field.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string Clients { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByUser { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string UserId { get; set; }
        public int MyProperty { get; set; }
    }
    public class BillableEntitiesMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<BillableEntitiesVM> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}

