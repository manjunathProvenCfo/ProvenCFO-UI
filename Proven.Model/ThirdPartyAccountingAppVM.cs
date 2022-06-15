using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class ThirdPartyAccountingAppVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class ThirdPartyAccountingAppVMModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<ThirdPartyAccountingAppVM> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
