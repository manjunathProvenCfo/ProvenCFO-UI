using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
	[Serializable]
	public class ReportsVM
    {
		public int Id { get; set; }
		public int AgencyId_Ref { get; set; }
		public string FileName { get; set; }
		public string FileGuid { get; set; }
		public string FilePath { get; set; }
		public string FileExtention { get; set; }
		public int Year { get; set; }
		public string PeriodType { get; set; }
		public string Status { get; set; }
		public string CreatedBy { get; set; }
		public DateTime CreatedDate { get; set; }
		public string ModifiedBy { get; set; }
		public DateTime? ModifiedDate { get; set; }
		public bool IsDeleted { get; set; }
        public int Position { get; set; }
        public bool IsMonthlySummary { get; set; }
    }
}
