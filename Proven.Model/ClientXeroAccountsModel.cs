﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class ClientXeroAccountsModel
    {
        public int Id { get; set; }
        public string AccountID { get; set; }
        public int? AgencyId_ref { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Review { get; set; }
        public string Type { get; set; }
        public string TaxType { get; set; }
        public string Class { get; set; }
        public bool? EnablePaymentsToAccount { get; set; }
        public bool? ShowInExpenseClaims { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountType { get; set; }
        public string CurrencyCode { get; set; }
        public string ReportingCode { get; set; }
        public string ReportingCodeName { get; set; }
        public bool? HasAttachments { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string Link_token { get; set; }
        public string public_token { get; set; }
        public bool? PlaidConnectionStatus { get; set; }
    }

}
