using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class ReconcilationActionOptionVM
    {
        public int Id { get; set; }
        public string ActionName { get; set; }
        public string Status { get; set; }
    }

    public class ReconcilationmainActionOptionVM
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<ReconcilationActionOptionVM> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
}
