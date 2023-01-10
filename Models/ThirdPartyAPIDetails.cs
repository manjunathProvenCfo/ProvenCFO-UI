using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class ThirdPartyAPIDetailsVW
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ThirdParty { get; set; }
        public int Priority { get; set; }
        public string RedirectUrl { get; set; }
        public string APIScope { get; set; } 

        public bool Status { get; set; }

        public bool IsDeleted { get; set; }
    }
}