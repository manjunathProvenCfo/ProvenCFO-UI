using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class XeroTrackingCategoriesVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }       
    }

    public class XeroTrackingMainCategoriesVM
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }             
        public List<XeroTrackingCategoriesVM> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }
}
