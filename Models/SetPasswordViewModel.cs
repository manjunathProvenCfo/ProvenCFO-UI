using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Password field cannot be blank.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Password field cannot be blank.")]
        [StringLength(30, ErrorMessage = "Your password must be between 8 and 30 characters", MinimumLength = 8)]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{8,}", ErrorMessage = "Your password must be at least 8 characters long and contain at least 1 letter and 1 number")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter Confirm Password.")]
        
        [Compare("NewPassword")/*,ErrorMessage = "Password did'nt matched")*/]
        public string ConfirmPassword { get; set; }

        public string ActivationCode { get; set; }
    }
}