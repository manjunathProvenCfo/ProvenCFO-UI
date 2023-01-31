using System;
using System.Collections.Generic;
using System.Text;

namespace CFO.Model.ViewModels
{
    public enum reconcilationType
    {
        ShowAllUnreconciled ,
        NotReadytoReconcile,
        ShowReconciledOnly,
        ReadytobeReconciledOnly
    }
    [Serializable]
    public class ReconciliationfilterModel
    {
        public string accounts { get; set; }
        public DateTime? dateRangeFrom { get; set; }
        public DateTime? dateRangeTo { get; set; }
        public decimal? amountMin { get; set; }
        public decimal? amountMax { get; set; }
        public string Bankrule { get; set; }
        public string TrackingCategory1 { get; set; }
        public string TrackingCategory2  { get; set; }
        public string ref_reconciliationAction { get; set; }

        public reconcilationType? FilterType { get; set; }
        public int? AgencyID { get; set; }

        public string UserID { get; set; }
        public string Type { get; set; }
        public bool? RuleNew { get; set; }
    }
}