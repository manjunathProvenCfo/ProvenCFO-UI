using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class NotesSummaryVM
    {
        public int Id { get; set; }
        public string SummaryData { get; set; }
        public string Status { get; set; }
        public bool? IsDeleted { get; set; }
        public string SummaryType { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
       

    }
    public class NotesSummarymainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
       
        public IEnumerable<NotesSummaryVM> ResultData { get; set; }
      

        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }
}
