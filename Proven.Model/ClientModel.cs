using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proven.Model
{

    [Serializable]
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
        public string APIClientID { get; set; }
        public string APIClientSecret { get; set; }
        public string XeroContactIDforProvenCfo { get; set; }
        public string AsanaId { get; set; }
        public string EverhourId { get; set; }
        public string CrmId { get; set; }
        public string XeroShortCode { get; set; }

        // public string[] XeroScope { get; set; }
        public string[] XeroScopeArray { get; set; }
        public string XeroScopeString { get; set; }
        public string APIScope { get; set; }
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
        public List<ClientXeroAccountsModel> clientXeroAccounts { get; set; }
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
        public int?  DashboardId { get; set; }
        public string DashboardURLId { get; set; }
        public string ReportId { get; set; }
        public bool?   EnableAutomation { get; set; }
        public int? ThirdPartyAccountingApp_ref { get; set; }
        public int? XeroTokenInfoLink_ref { get; set; }
        public int? CompanyId { get; set; }



        //For Summary Page
        public string Summary { get; set; }
        public string SummaryCreatedBy { get; set; }
        public string SummaryCreatedByFullName { get; set; }
        public DateTime? SummaryCreatedDate { get; set; }
        public string SummaryModifiedBy { get; set; }
        public string SummaryModifiedByFullName { get; set; }
        public DateTime? SummaryModifiedDate { get; set; }
        public int? Summaryid_ref { get; set; }
        public string SummaryData { get; set; }
        public bool? IsMailSend { get; set; }
        public string MailSendDate { get; set; }
        public string MailSendDateNotes { get; set; }
        public string MailSendDateNeeds { get; set; }

        public  Int64? QuickBooksCompanyId { get; set; }

        public bool? Plaid_Enabled { get; set; }
        public string AppTokenStatus { get; set; }
        public string AccountingPackage { get; set; }
        public string DOMO_datasetId { get; set; }
        public DateTime? DOMO_Last_batchrun_time { get; set; }
        public int? DOMO_Batchrun_id { get; set; }
        public bool? IsDomoEnabled { get; set; }
        public bool? EnableDataSynTimeTrigge { get; set; }
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
