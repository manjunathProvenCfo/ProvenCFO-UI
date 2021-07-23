using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class InvitedUserViewModel
    {
        public int id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int IsActive { get; set; }
        public Guid ActivationCode { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string SessionTimeout { get; set; }

        public int IsRegistered { get; set; }
        public string RoleId { get; set; }
    }
}