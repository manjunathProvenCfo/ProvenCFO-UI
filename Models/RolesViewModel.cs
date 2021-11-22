using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class RolesViewModel
    {
        public string id { get; set; }
        [Required(ErrorMessage = "Role Name is a required field.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "Please Enter a valid Name")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string name { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int MyProperty { get; set; }
        [Required(ErrorMessage = "Role Name is a required field.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "Please Enter a valid Name")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string DisplayRoleName { get; set; }
        public Boolean? IsVisible { get; set; }
    }
}