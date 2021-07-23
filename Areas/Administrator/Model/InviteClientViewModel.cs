using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Areas.Administrator.Model
{
    public class InviteClientViewModel
    {
        [Required(ErrorMessage = "Please Enter Name.This Field cannot be blank.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter Email Address.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Please Enter a valid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Select Role.")]
        public string Role { get; set; }
        [Required(ErrorMessage = "Please Select SessionTimeout.")]
        public string SessionTimeout
        {
            get; set;
        }
    }
}