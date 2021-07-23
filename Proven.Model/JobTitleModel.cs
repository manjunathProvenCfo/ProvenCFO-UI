using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class JobTitleModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Job Title is a required field.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "Please Enter a valid Job Title")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string Title { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "Please Enter a valid Job Code")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string JobCode { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByUser { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public int MyProperty { get; set; }
    }

    public class JobTitleMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<JobTitleModel> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }

}
