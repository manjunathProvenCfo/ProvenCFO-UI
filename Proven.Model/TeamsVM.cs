using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class TeamsVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Team Name is a required field.")]
        [RegularExpression(@"^[a-zA-Z0-9_ ]*$", ErrorMessage = "Please Enter a valid Team Name")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string TeamName { get; set; }
        [Required(ErrorMessage = "Team Member 1 is a required field.")]
        public string TeamMemberId1 { get; set; }
        //[Compare("TeamMemberId1", ErrorMessage = "Team members on a team must be unique.")]
        [Required(ErrorMessage = "Team Member 2 is a required field.")]
        public string TeamMemberId2 { get; set; }
        [Required(ErrorMessage = "Team Member 3 is a required field.")]
        public string TeamMemberId3 { get; set; }
        public string JobTitleId1 { get; set; }
        public string JobTitleId2 { get; set; }
        public string JobTitleId3 { get; set; }
        public string JobId { get; set; }
        public string UserId { get; set; }
        public string TeamId { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CreatedDate { get; set; }
        public string ModifiedByUser { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public List<TeamUserAssociationVM> TeamMembersList { get; set; }
        public List<UserModel> StaffList { get; set; }
        //public string StaffId { get; set; }


    }

    public class TeamsMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<TeamsVM> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
}
