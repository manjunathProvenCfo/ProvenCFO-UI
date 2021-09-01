using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class XeroGlAccountVM
    {
        public int? Id { get; set; }
        public string AccountId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string TaxType { get; set; }
        public string Class { get; set; }
        public string EnablePaymentsToAccount { get; set; }
        public string ShowInExpenseClaims { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountType { get; set; }
        public string CurrencyCode { get; set; }
        public string ReportingCode { get; set; }
        public string HasAttachments { get; set; }
        public DateTime? UpdatedDateUTC { get; set; }
        public string AddToWatchlist { get; set; }
        public int? AgencyId { get; set; }
    }

    public class XeromainGlAccountVM
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<XeroGlAccountVM> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }
}
