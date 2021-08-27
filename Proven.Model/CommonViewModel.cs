using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public  class CommonViewModel
    {
        public List< XeroGlAccountVM> xeroGlAccount { get; set; }
        public List <XeroTrackingCategoriesVM> xeroTracking { get; set; }
    }
}
