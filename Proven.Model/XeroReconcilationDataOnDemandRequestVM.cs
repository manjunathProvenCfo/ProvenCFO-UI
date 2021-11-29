using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class XeroReconcilationDataOnDemandRequestVM
    {
        public int Id { get; set; }
        public string RequestType { get; set; }
        public DateTime? RequestedAtUTC { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? RequestCompletedAtUTC { get; set; }
        public string Remark { get; set; }
        public int? AgencyId { get; set; }
        public string AgencyName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }


    public class XeroReconcilationDataOnDemandRequestMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public XeroReconcilationDataOnDemandRequestVM ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }

    }
}
