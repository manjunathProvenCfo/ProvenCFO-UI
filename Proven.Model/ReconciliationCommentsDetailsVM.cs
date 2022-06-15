using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class ReconciliationCommentsDetailsVM
    {
        public reconciliationVM reconciliationdata { get; set; }
        public List<ReconciliationComments> ReconciliationComments { get; set; }
    }
}
