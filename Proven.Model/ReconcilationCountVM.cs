using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class ReconcilationCountVM
    {
        public string type { get; set; }
        public int totalCount { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [DataType(DataType.Currency)]
        public decimal? amount { get; set; }
        public int Count { get; set; }
        public decimal? amountPositive { get; set; }
        public decimal? amountNegative { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}")]
        public double percentage { get; set; }

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
