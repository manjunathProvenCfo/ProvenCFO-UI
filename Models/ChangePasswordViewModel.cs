using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class ChangePasswordViewModel
    {
        public string id { get; set; }
        [Required(ErrorMessage = "Please enter Old Password.")]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Please enter New Password.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter Confirm Password.")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}