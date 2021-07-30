using System;
using System.Collections.Generic;
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
        public string amount { get; set; }
        public string company { get; set; }
        public string date { get; set; }
        public String description { get; set; }
        public string gl_account { get; set; }
        public string reconciled { get; set; }
        public String reference { get; set; }
        public string rule { get; set; }
        public string type { get; set; }
    }
    public class ReconciliationMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public reconciliationVM ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
