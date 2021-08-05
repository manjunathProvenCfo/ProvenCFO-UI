using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Proven.Model
{

    public class UserModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int? AccessFailedCount { get; set; }
        public string UserName { get; set; }
        [Required(ErrorMessage = "Please Enter FirstName")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please Enter LastName")]
        public string LastName { get; set; }
        public string UserId { get; set; }
        public string JobTitle { get; set; }
        public string UserRole { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByUser { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }

        public string AboutUs { get; set; }

        public string CoverImage { get; set; }

        public string ProfileImage { get; set; }

        public string OldPassword { get; set; }
        [Required(ErrorMessage = "Please enter New Password.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Please enter Confirm Password.")]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
        public string RoleId { get; set; }
        public virtual RolesViewModel roleModel { get; set; }
        public int? JobId { get; set; }
        public string RoleName { get; set; }
        public List<RolesViewModel> Rolelist { get; set; }

        public List<JobTitleModel> JobTitlelist { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        [RegularExpression(@"((http(s?)://)*([a-zA-Z0-9\-])*\.|[linkedin])[linkedin/~\-]+\.[a-zA-Z0-9/~\-_,&=\?\.;]+[^\.,\s<]",
                           ErrorMessage = "Please Enter a valid LinkedIn profile URL.")]
        public string LinkedInProfile { get; set; }

        public string UserType { get; set; }

    }
    public class UserMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<UserModel> resultData { get; set; }
        public List<RolesViewModel> Rolelist { get; set; }
        public List<JobTitleModel> JobTitlelist { get; set; }
        public List<UserModel> StaffList { get; set; }
        public string TeamId { get; set; }
        public string DisplayName { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }


    public class UserDetailMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }

        public  UserModel  resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
}
