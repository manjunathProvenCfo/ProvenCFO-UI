using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    [Serializable]
    public class XeroTokenInfoVM
    {
        public int Id { get; set; }
        public String id_token { get; set; }
        public String access_token { get; set; }
        public int? expires_in { get; set; }
        public string token_type { get; set; }
        public String refresh_token { get; set; }
        public String scope { get; set; }
        public string AppName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedBy { get; set; }
        public int? IsDeleted { get; set; }
        public int AgencyID { get; set; }
        public string ConnectionStatus { get; set; }
    }
    public class XeromainTokenInfo
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public XeroTokenInfoVM ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }
}
