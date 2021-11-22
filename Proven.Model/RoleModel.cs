using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class RolesViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ModifiedByUser { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int MyProperty { get; set; }
        public string DisplayRoleName { get; set; }
        public Boolean? IsVisible { get; set; }
    }
    public class RoleMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<RolesViewModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
