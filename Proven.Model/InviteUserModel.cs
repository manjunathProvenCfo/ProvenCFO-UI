using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Proven.Model
{
    [Serializable]
    public class InviteUserModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First name is a required field.")]
        [MaxLength(50, ErrorMessage = "First Name cannot be longer than 50 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name is a required field.")]
        [MaxLength(50, ErrorMessage = "Last Name cannot be longer than 50 characters")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email address is a required field.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-19]{1,19}" +
                            @"\.[0-9]{1,19}\.[0-19]{1,19}\.)|(([a-zA-Z0-9\-]+\" +
                            @".)+))([a-zA-Z]{2,19}|[0-19]{1,19})(\]?)$",
                            ErrorMessage = "Please Enter a valid Email Address")]
      
        public string Email { get; set; }
        public string IsActive { get; set; }
        public Guid ActivationCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string SessionTimeout { get; set; }
        public int? IsRegistered { get; set; }
        public string ModifiedBy { get; set; }
        public string CreatedBy { get; set; }
        [Required(ErrorMessage = "Role is a required field.")]
        public string RoleId { get; set; }
        public virtual RolesViewModel roleModel { get; set; }
        public DateTime? ExpiryTime { get; set; }

        public string UserId { get; set; }
        [Required(ErrorMessage = "Job title is a required field.")]
        public int? JobId { get; set; }

        public string JobTitle { get; set; }

        public string RoleName { get; set; }

        public string Register { get; set; }
        public List<RolesViewModel> Rolelist { get; set; }

        public List<JobTitleModel> JobTitlelist { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy h:mm tt}")]
        public DateTime? LastLogin { get; set; }
        public int? UserType { get; set; }

        public int? AgencyID { get; set; }

        public string ModifiedByUser { get; set; }
        public string CreatedByUser { get; set; }
        public string AgencyName { get; set; }
        public string DisplayName { get; set; }
        public string Status { get; set; }
        [RegularExpression(@"((http(s?)://)*([a-zA-Z0-9\-])*\.|[linkedin])[linkedin/~\-]+\.[a-zA-Z0-9/~\-_,&=\?\.;]+[^\.,\s<]",
                            ErrorMessage = "Please Enter a valid LinkedIn profile URL.")]
        public string LinkedInProfile { get; set; }
        public string ProfileImage { get; set; }
    }
    public class InviteUserMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public IEnumerable<InviteUserModel> ResultData { get; set; }
        public List<RolesViewModel> Rolelist { get; set; }
        public List<JobTitleModel> JobTitlelist { get; set; }
        public List<InviteUserModel> StaffList { get; set; }
        public string TeamId { get; set; }
        public string DisplayName { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
    [Serializable]
    public class ReturnModel
    {
        public bool status { get; set; }
        public int statusCode { get; set; }
        public bool resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }


    [Serializable]
    public class ReturnModelBulk
    {
        public bool status { get; set; }
        public int statusCode { get; set; }
        public int resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
    public class ReturnStringModel
    {
        public bool status { get; set; }
        public int statusCode { get; set; }
        public string[] resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
    public class ReturnNumberModel
    {
        public bool status { get; set; }
        public int statusCode { get; set; }
        public int resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }



    public class ResultAsyncData
    {
        public bool result { get; set; }
        public int id { get; set; }
        public object exception { get; set; }
        public int status { get; set; }
        public bool isCanceled { get; set; }
        public bool isCompleted { get; set; }
        public bool isCompletedSuccessfully { get; set; }
        public int creationOptions { get; set; }
        public object asyncState { get; set; }
        public bool isFaulted { get; set; }
    }

    public class ReturnAsyncModel
    {
        public bool status { get; set; }
        public int statusCode { get; set; }
        public ResultAsyncData resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }

}
