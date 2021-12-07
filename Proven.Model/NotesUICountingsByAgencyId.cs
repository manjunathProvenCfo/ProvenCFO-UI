using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class NotesUICountingsByAgencyId
    {
        public int TotalNotes { get; set; }
        public string AgencyId { get; set; }

    }

    public class NotesCountModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<NotesUICountingsByAgencyId> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }

    }
}
