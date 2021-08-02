using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using ProvenCfoUI.Models;

namespace ProvenCfoUI.Models
{
    public class CreateClientVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Client Agency name is a required field.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded")]
        public string ClientName { get; set; }
        //[Required(ErrorMessage = "Please enter Email Address.")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,9}" +
                           @"\.[0-9]{1,9}\.[0-9]{1,9}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{1,9}|[0-9]{1,9})(\]?)$",
                           ErrorMessage = "Please Enter a valid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public string Status { get; set; }
        public int CityId { get; set; }
        [Required(ErrorMessage = "City is required field.")]
        [StringLength(50, ErrorMessage = "Maximum 50 characters exceeded.")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "State is a required field.")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "Team is a required field")]
        public int TeamId { get; set; }
        public int EntityId { get; set; }
        public DateTime? StartDate { get; set; }
        public List<City> CityList { get; set; }
        public List<State> StateList { get; set; }
        public List<Proven.Model.TeamsVM> TeamList { get; set; }
        public List<Proven.Model.BillableEntitiesVM> billableEntitiesList { get; set; }
    }
}