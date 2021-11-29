using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class TeamUserAssociationVM
    {
        public int Id { get; set; }
        public string TeamId { get; set; }
        public string UserId { get; set; }
        public string TeamName { get; set; }
        public string Username { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByUser { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string LinkedInProfile { get; set; }
        public string Jobtitle { get; set; }
        public string Profileimage { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

    }

    public class TeamUserAssociationVMMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<TeamUserAssociationVM> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
}
