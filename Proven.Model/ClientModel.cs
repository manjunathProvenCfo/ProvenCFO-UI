using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{


    public class ClientModel
    {
        public int Id { get; set; }
        public string ClientName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public int? ContactMobile { get; set; }
        public bool? Status { get; set; }
        public int CityId { get; set; }
        public int StateId { get; set; }
        public int State { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? CreatedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string XeroID { get; set; }
        public string XeroClientID { get; set; }
        public string XeroClientSecret { get; set; }
        public string XeroContactIDforProvenCfo { get; set; }
        public string AsanaId { get; set; }
        public string EverhourId { get; set; }
        public string CrmId { get; set; }
        public string XeroShortCode { get; set; }

        // public string[] XeroScope { get; set; }
        public string[] XeroScopeArray { get; set; }
        public string XeroScopeString { get; set; }
         public string XeroScope { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public int BillableEntityId { get; set; }
        public string EntityName { get; set; }
        public string CreatedByUser { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedByUser { get; set; }
        public string DisplayName { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public List<City> CityList { get; set; }
        public List<State> StateList { get; set; }
        public List<BillableEntitiesVM> billableEntitiesList { get; set; }
        public List<TeamsVM> TeamList { get; set; }
        public string TeamName { get; set; }
        public string TeamMemberId1 { get; set; }
        public string TeamMemberId2 { get; set; }
        public string TeamMemberId3 { get; set; }
        public string JobName { get; set; }
        public string JobtitleId1 { get; set; }
        public string JobtitleId2 { get; set; }
        public string JobtitleId3 { get; set; }
        public string ProfileImage { get; set; }
        public string ProfileImage1 { get; set; }
        public string ProfileImage2 { get; set; }
        public string ProfileImage3 { get; set; }
        public int? TeamId { get; set; }
        public bool ReceiveQuarterlyReports { get; set; }

        public bool? EnableAutomation { get; set; }

        //For Summary Page
        public string Summary { get; set; }
        public string SummaryCreatedBy { get; set; }
        public string SummaryCreatedByFullName { get; set; }
        public DateTime? SummaryCreatedDate { get; set; }
        public string SummaryModifiedBy { get; set; }
        public string SummaryModifiedByFullName { get; set; }
        public DateTime? SummaryModifiedDate { get; set; }        
    }

    public class ClientMainModel
    {
        public bool Status { get; set; }
        public int statusCode { get; set; }
        public List<ClientModel> ResultData { get; set; }
        public string message { get; set; }
        public object resourceType { get; set; }
        public object metaData { get; set; }


    }



}
