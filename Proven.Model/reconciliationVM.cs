using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class reconciliationVM
    {
        public string plenadata__id { get; set; }
        public string id { get; set; }
        public string account_name { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}")]
        [DataType(DataType.Currency)]
        public decimal amount { get; set; }
        public string company { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime date { get; set; }
        public String description { get; set; }
        public String description_display { get { return description != null && description.Length > 30 ? description.Substring(0, 30) + "..." : description; } }
        public string gl_account { get; set; }
        public string reconciled { get; set; }
        public String reference { get; set; }
        public string reference_display { get { return reference != null && reference.Length > 30 ? reference.Substring(0, 30) + "..." : reference; } }
        public string rule { get; set; }
        //public string type { get; set; }
        public String AgencyID { get; set; }
        public String IsDeleted { get; set; }
        public int? gl_account_ref { get; set; }
        public int? tracking_category_ref { get; set; }
        public int? additional_tracking_category_ref { get; set; }
        public bool Selected { get; set; }
        public bool IsChatExist { get; set; }
        public string TwilioConversationId { get; set; }
        public bool has_twilio_conversation { get; set; }
        public bool Iscurrent_user_mentioned { get; set; }
        public int? ref_ReconciliationAction { get; set; }

        public DateTime? ActionModifiedDateUTC { get; set; }
        public DateTime? GlAccountModifiedDateUTC { get; set; }

        public DateTime? ActionModifiedDate { get; set; }
        public DateTime? GlAccountModifiedDate { get; set; }
        public string GlAccountModifiedBy { get; set; }
        public string ActionModifiedBy { get; set; }
		public string type { get; set; }
        public bool? RuleNew { get; set; }
    }
    public class ReconcilationVMPagination
    {
        public List<reconciliationVM> company_ReconciliationVMs { get; set; }
        public int totalcount { get; set; }
    }
    public class ReconciliationMainModelPaging
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public ReconcilationVMPagination ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
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
    [Serializable]
    public class ReconciliationInputModel
    {
        public String HtmlorCsvString { get; set; }
        public string CompanyName { get; set; }
        public string AccountName { get; set; }
        public int? CompanyId { get; set; }
        public string Type { get; set; }
    }
    [Serializable]
    public class ReconciliationOutputModel
    {
        public bool Status { get; set; }
        public string ValidationStatus { get; set; }
        public string ValidationMessage { get; set; }
    }
    public class ReconciliationOutputModelMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public ReconciliationOutputModel ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }

    }
}
