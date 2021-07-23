using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class ClientUserAssociationModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string UserId { get; set; }
       
        public string ClientName { get; set; }
     
        public string UserName { get; set; }
    }
    public class ClientUserAssociatioMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<ClientUserAssociationModel> resultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }
    }
}
