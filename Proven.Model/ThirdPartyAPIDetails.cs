using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{

    [Serializable]
    public class ThirdPartyAPIDetailsVM
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string ThirdParty { get; set; }

        public int Priority { get; set; }

        public bool Status { get; set; }

        public bool IsDeleted { get; set; }
    }




    [Serializable]

    public  class  ThirdPartyAPIDetails
    {

        public List<ThirdPartyAPIDetailsVM> list { get; set; }
   
    }
}