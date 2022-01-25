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
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime date { get; set; }
        public String description { get; set; }
        public String description_display { get { return description != null && description.Length > 30 ? description.Substring(0, 30) + "..." : description; } }
        public string gl_account { get; set; }
        public string reconciled { get; set; }
        public String reference { get; set; }
        public string reference_display { get { return reference != null && reference.Length > 30 ? reference.Substring(0, 30) + "...": reference; } }
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
