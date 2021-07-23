using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class DeactivateInvitationViewModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int IsActive { get; set; }
        public Guid ActivationCode { get; set; }
        public string RoleId { get; set; }
        public string SessionTimeout { get; set; }
        public int IsRegistered { get; set; }
        public string link { get; set; }
        public DateTime? CreatedDate { get; set; }
        public Guid token { get; set; }
    }
}