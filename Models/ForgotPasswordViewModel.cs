using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "This field cannot be blank.")]

        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,9}" +
                           @"\.[0-9]{1,9}\.[0-9]{1,9}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{1,9}|[0-9]{1,9})(\]?)$",
                           ErrorMessage = "Please Enter a valid Email Address")]
        public string email { get; set; }
    }
}