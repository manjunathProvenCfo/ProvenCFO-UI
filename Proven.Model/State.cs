using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    [Serializable]
    public class State
    {
        public string Id { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
    public class StateMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<State> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }
}