using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class UserPreferences
    {
        public int Id { get; set; }
        public string PreferenceCategory { get; set; }
        public string Sub_Category { get; set; }
        public string PreferanceValue { get; set; }
        public string UserID { get; set; }
        public string UserRole { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByUser { get; set; }
        public DateTime? ModdifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
