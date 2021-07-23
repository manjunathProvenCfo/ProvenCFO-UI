using System;
using System.Collections.Generic;

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class InviteStaffViewModel
    {

        [Required(ErrorMessage = "FirstName is a required field.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "LastName is a required field.")]
        public string Lastname { get; set; }
        [Required(ErrorMessage = "Email Address is a required field.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,9}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,9}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,9}|[0-9]{1,9})(\]?)$",
                            ErrorMessage = "Please Enter a valid Email Address")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Select Role.")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Please Select Job Title.")]
        public string JobTitle { get; set; }
        public string SessionTimeout { get; set; }
        public string MyProperty { get; set; }

    }
}