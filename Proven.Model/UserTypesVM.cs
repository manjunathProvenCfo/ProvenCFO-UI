using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{
    public class UserTypesVM
    {
        public int Id { get; set; }
        public int? UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        public Boolean? Status { get; set; }
    }
    public class UserTypeModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<UserTypesVM> ResultData { get; set; }
        public string Message { get; set; }
        public object ResourceType { get; set; }
        public object MetaData { get; set; }
    }
}
