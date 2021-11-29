using System;
using System.Collections.Generic;
using System.Text;

namespace CFO.Model.ViewModels
{
   public class BulkActionParametersVM
    {
        public string[] Ids { get; set; }
        public int GLaccount { get; set; }
        public int TrackingCategory { get; set; }
        public int AditionalTrackingCategory { get; set; }
        public string BankRule { get; set; }
        public int AgencyID { get; set; }
        public bool IsAllSelected { get; set; }
        public string SelectedItems { get; set; }
        public string UnSelectedRecords { get; set; }
        public int? reconcilationStatus { get; set; }
    }
}
