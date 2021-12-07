using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class MasterFeaturesVM
    {
        public int Id { get; set; }
        public string FeaturName { get; set; }
        public string FeatureCode { get; set; }
        public string DisplayName { get; set; }
        public int? ParentId { get; set; }
        public bool? IsLeftNavBarItem { get; set; }
        public string ElementName { get; set; }
        public string ActionName { get; set; }

        public string ControlerName { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool? IsDeleted { get; set; }

        public bool IsChecked { get; set; }
    }
    public class MasterFeaturesMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<MasterFeaturesVM> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }
}
