using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class ReconcilationCountVM
    {
        public string type { get; set; }
        public int totalCount { get; set; }      
        public decimal? amount { get; set; }
        public int Count { get; set; }

    }
    public class ReconciliationCountModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<ReconcilationCountVM> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }

    }
}
