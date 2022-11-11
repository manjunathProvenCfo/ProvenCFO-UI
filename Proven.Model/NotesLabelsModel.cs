using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class NotesLabelsModel
    {
        public int? Id { get; set; }
        public string NoteLabel { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool? IsDeleted { get; set; }
        public string colorCode { get; set; }
        public string className { get; set; }
    }
    public class NotesLabelsMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<NotesLabelsModel> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
