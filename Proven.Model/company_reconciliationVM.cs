using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class company_reconciliationVM
    {
        public string plenadata__id { get; set; }
        public string id { get; set; }
        public string account_name { get; set; }
        public decimal? amount { get; set; }
        public string company { get; set; }
        public DateTime? date { get; set; }
        public String description { get; set; }
        public string gl_account { get; set; }
        public Int16? reconciled { get; set; }
        public String reference { get; set; }
        public string rule { get; set; }
        public string type { get; set; }
        public int? AgencyID { get; set; }
        public int? IsDeleted { get; set; }
        public int? gl_account_ref { get; set; }
        public int? tracking_category_ref { get; set; }
        public int? additional_tracking_category_ref { get; set; }
        public string gl_account_name { get; set; }
        public string tracking_category { get; set; }
        public string tracking_category_name { get; set; }
        public DateTime? RunAtDateUTC { get; set; }
        public bool? IsStatusUpdatedManually { get; set; }
        public bool IsChatExist { get; set; }
        public bool IsParticipantExist { get; set; }
        public string TwilioConversationId { get; set; }
        public bool has_twilio_conversation { get; set; }
        public bool Iscurrent_user_mentioned { get; set; }
        public DateTime? LastcommentedDate { get; set; }
        public int? ref_reconciliationAction { get; set; }
        public string GlAccountModifiedBy { get; set; }
        public DateTime? GlAccountModifiedDateUTC { get; set; }
        public string ActionModifiedBy { get; set; }
        public DateTime? ActionModifiedDateUTC { get; set; }
        public DateTime? ActionModifiedDate { get; set; }
        public DateTime? GlAccountModifiedDate { get; set; }
    }
}
