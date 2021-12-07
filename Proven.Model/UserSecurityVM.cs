using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class UserSecurityVM
    {
        public int Id { get; set; }
        public string FeatureCode { get; set; }
        public string FeatureDescription { get; set; }
        public string ModuleDescription { get; set; }
        public string ModuleCode { get; set; }
        public string ModuleDisplayName { get; set; }
        public string FeatureDisplayName { get; set; }
        public bool? ModuleIsLeftNavBarItem { get; set; }
        public bool? FeatureIsLeftNavBarItem { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
    }
    public class UserSecurityModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<UserSecurityVM> resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
}
