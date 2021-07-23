using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class RegisterViewModel
    {
        public int id { get; set; }
        [Required(ErrorMessage = "FirstName is a required field.")]
        public string firstname { get; set; }
        [Required(ErrorMessage = "LastName is a required field.")]
        public string lastname { get; set; }
        [Required(ErrorMessage = "Email Address is a required field.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,9}" +
                           @"\.[0-9]{1,9}\.[0-9]{1,9}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{1,9}|[0-9]{1,9})(\]?)$",
                           ErrorMessage = "Please Enter a valid Email Address")]
        public string email { get; set; }
        [Required(ErrorMessage = "Please Enter Correct Password.")]
        [StringLength(30, ErrorMessage = "Your password must be between 8 and 30 characters", MinimumLength = 8)]
        [RegularExpression(@"(?=.*\d)(?=.*[A-Za-z]).{8,}", ErrorMessage = "Your password must be at least 8 characters long and contain at least 1 letter and 1 number")]
        [DataType(DataType.Password)]
        public string passwordhash { get; set; }
        [Required(ErrorMessage = "Please enter Confirm Password.")]
        //[Range(6, 50, ErrorMessage = "Password should contain atleast 6 characters.")]
        [Compare("passwordhash", ErrorMessage = "Password and Password Confirmation do not match.")]
        public string confirmpassword { get; set; }

        public int? UserType { get; set; }
        public int? AgencyID { get; set; }

    }
}