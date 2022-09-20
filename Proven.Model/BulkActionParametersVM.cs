using System;
using System.Collections.Generic;
using System.Text;

namespace CFO.Model.ViewModels
{
    [Serializable]
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
        public int Action { get; set; }
        public string CommentText { get; set; }
        public string CreatedBy { get; set; }
    }
}
