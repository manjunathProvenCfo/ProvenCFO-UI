using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class InviteUserVMId
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string JobTitle { get; set; }
        public string RoleName { get; set; }
        public int? JobId { get; set; }
        public string RoleId { get; set; }
        public int? IsActive { get; set; }       
        public string AgencyId { get; set; }
        public Guid? ActivationCode { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string SessionTimeout { get; set; }
        public int? IsRegistered { get; set; }        
        public DateTime? ExpiryTime { get; set; }

        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? LastLogin { get; set; }
        public int? UserType { get; set; }
        public List<RolesViewModel> Rolelist { get; set; }

        public List<JobTitleModel> JobTitlelist { get; set; }
        public List<InviteUserModel> StaffList { get; set; }
        public string AgencyName { get; set; }

        public string DisplayName { get; set; }
        public string StaffId { get; set; }

    }
    public class InviteUserVMMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<InviteUserVMId> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }

}
