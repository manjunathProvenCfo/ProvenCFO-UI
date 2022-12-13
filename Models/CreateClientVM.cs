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
        [MaxLength(100, ErrorMessage = "Maximum 100 characters exceeded")]
        public string ClientName { get; set; }
        //[Required(ErrorMessage = "Please enter Email Address.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,9}" +
        //                   @"\.[0-9]{1,9}\.[0-9]{1,9}\.)|(([a-zA-Z0-9\-]+\" +
        //                   @".)+))([a-zA-Z]{1,9}|[0-9]{1,9})(\]?)$",
        //                   ErrorMessage = "Please Enter a valid Email Address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string ContactPersonName { get; set; }
        public string Status { get; set; }
        public int CityId { get; set; }
        [Required(ErrorMessage = "City is a required field.")]
        [MaxLength(50, ErrorMessage = "Maximum 50 characters exceeded.")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "State is a required field.")]
        public int StateId { get; set; }
        [Required(ErrorMessage = "Team is a required field")]
        public int TeamId { get; set; }
        [Required(ErrorMessage = "Billable Entity  is a required field")]
        public int BillableEntityId { get; set; }
        //[Required(ErrorMessage = "Start Date is a required field.")]
        public DateTime? StartDate { get; set; }
        //[Required(ErrorMessage = "Xero ID is a required field")]
        public string XeroID { get; set; }
        [Required(ErrorMessage = "API Client ID is a required field")]
        public string APIClientID { get; set; }
        [Required(ErrorMessage = "API Client Secret is a required field")]
        public string APIClientSecret { get; set; }
        [Required(ErrorMessage = "API Scope is a required field")]
        public string[] XeroScopeArray { get; set; }

        public string[] DomoScopeArray { get; set; }
        //public string[] XeroScope { get; set; }
        //[Required(ErrorMessage = "Xero Scope is a required field.")]

        public string APIScope { get; set; }

        [Required(ErrorMessage = "Start Date is a required field.")]
        public string StartDateText { get; set; }
        public List<City> CityList { get; set; }
        public List<State> StateList { get; set; }
        public bool EnableAutomation { get; set; }
        public bool ReceiveQuarterlyReports { get; set; }
        public string XeroContactIDforProvenCfo { get; set; }
        public string AsanaId { get; set; }
        public string EverhourId { get; set; }
        public string CrmId { get; set; }
        [Range(0, 9999999, ErrorMessage = "Dashboard ID must be between 0 and 9999999")]
        public int? DashboardId { get; set; }

        [Range(0, 9999999999999999999, ErrorMessage = "QuickBook Company Id must be between 0 and 9999999999999999999")]
        public Int64? QuickBooksCompanyId { get; set; }

        public int? ThirdPartyAccountingApp_ref { get; set; }
        public string DashboardURLId { get; set; }
        public string ReportId { get; set; }

        public string XeroShortCode { get; set; }
        public int? XeroTokenInfoLink_ref { get; set; }

        public string ExcludedAccountNumbers { get; set; }
        public string IncludedAccountNumbers { get; set; }

        public int? CompanyId { get; set; }
        public List<Proven.Model.TeamsVM> TeamList { get; set; }
        public List<Proven.Model.BillableEntitiesVM> billableEntitiesList { get; set; }
        public List<Proven.Model.ClientXeroAccountsVM> clientXeroAccounts { get; set; }
        public bool Plaid_Enabled { get; set; }
        [Required(ErrorMessage = "Data Set ID is a required field")]
        public string DOMO_datasetId { get; set; }

        public DateTime DOMO_Last_batchrun_time { get; set; }

        public int DOMO_Batchrun_id {get;set;}

        public bool IsDomoEnabled { get; set; }
        public bool EnableDataSynTimeTrigge { get; set; }
    }
}