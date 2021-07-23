using Proven.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Models
{
    public class AgencyClient 
    {
        public int Id { get; set; }     
        public string ClientName { get; set; }
        //[Required(ErrorMessage = "Please enter Email Address.")]        
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public string Status { get; set; }
        public int CityId { get; set; }       
        public int StateId { get; set; }
        public int CityName { get; set; }
        public int StateName { get; set; }

        public int SelectedClientNameID { get; set; }
        public List<ClientModel> ClientList { get; set; }

        public List<State> StateList { get; set; }
    }
}