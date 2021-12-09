using Proven.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Areas.Model
{
    public class InviteStaffViewModel
    {
        public int? id { get; set; }
        [Required(ErrorMessage = "First name is a required field.")]
        [MaxLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }
        [MaxLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        [Required(ErrorMessage = "Last name is a required field.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email address is a required field.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                            @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
                            ErrorMessage = "Please Enter a valid Email Address")]
        public string Email { get; set; }

        public string Role { get; set; }
        public string IsActive { get; set; }      
        public string JobTitle { get; set; }
        [Required(ErrorMessage = "Job title is a required field.")]
        public string jobid { get; set; }
        [Required(ErrorMessage = "Role is a required field.")]
        public string roleid { get; set; }

        public string SessionTimeout { get; set; }
      
        public List<RolesViewModel> Rolelist { get; set; }

        public List<JobTitleModel> JobTitlelist { get; set; }
        public int? AgencyID { get; set; }

        public String AgencyName { get; set; }

        public string DisplayName { get; set; }
    }
    public class InviteAgencyUserViewModel
    {
        public int? id { get; set; }
        [Required(ErrorMessage = "Please Enter First Name.This Field cannot be blank.")]
        [MaxLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }
        [MaxLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        [Required(ErrorMessage = "Please Enter Last Name.This Field cannot be blank.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter Email Address.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,9}" +
                            @"\.[0-9]{1,9}\.[0-9]{1,9}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,9}|[0-9]{1,9})(\]?)$",
                            ErrorMessage = "Please Enter a valid Email Address")]
        //[RegularExpression(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$", ErrorMessage = "Please Enter a valid Email Address")]
        public string Email { get; set; }

        public string Role { get; set; }
        public string IsActive { get; set; }
        public string JobTitle { get; set; }
     
        public string jobid { get; set; }

        public string roleid { get; set; }

        public string SessionTimeout { get; set; }

        public List<RolesViewModel> Rolelist { get; set; }

        public List<JobTitleModel> JobTitlelist { get; set; }
        public int? AgencyID { get; set; }

        public String AgencyName { get; set; }
    }
}