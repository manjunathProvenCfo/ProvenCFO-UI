using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class reconciliationVM
    {
        public string plenadata__id { get; set; }
        public string id { get; set; }
        public string account_name { get; set; }
        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal amount { get; set; }
        public string company { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime date { get; set; }
        public String description { get; set; }
        public string gl_account { get; set; }
        public string reconciled { get; set; }
        public String reference { get; set; }
        public string rule { get; set; }
        //public string type { get; set; }
        public String AgencyID { get; set; }
        public String IsDeleted { get; set; }
        public int? gl_account_ref { get; set; }
        public int? tracking_category_ref { get; set; }
    }
    public class ReconciliationMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<reconciliationVM> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
