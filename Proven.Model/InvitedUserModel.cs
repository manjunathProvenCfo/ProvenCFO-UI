using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class InvitedUserModel
    {
        public int id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public int IsActive { get; set; }
        public Guid ActivationCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string SessionTimeout { get; set; }

        public int IsRegistered { get; set; }
        public string RoleId { get; set; }
    }
    public class InvitedUserMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<InvitedUserModel> resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
}
