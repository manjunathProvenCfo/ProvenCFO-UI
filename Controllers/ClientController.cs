namespace ProvenCfoUI.Controllers
{
    using log4net;
    using Proven.Model;
    using Proven.Service;
    using ProvenCfoUI.Comman;
    using ProvenCfoUI.Helper;
    using ProvenCfoUI.Models;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Xero.NetStandard.OAuth2.Token;


    [CustomAuthenticationFilter]
    [Exception_Filters]
    public class ClientController : BaseController
    {
        /// <summary>
        /// Defines the log.
        /// </summary>
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [CheckSession]
        [CustomAuthorize("Staff User")]
        public ActionResult ClientList()
        {
            try
            {
                using (ClientService obj = new ClientService())
                {
                    var objResult = obj.GetClientList();
                    return View(objResult.ResultData);
                }
            }
            catch (Exception ex)
            {
                 ex.Data.Add("UserId",Session["UserId"].ToString()); log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

  
        [CheckSession]
        public ActionResult ClientUserAssociationList()
        {
            try
            {
                using (ClientService obj = new ClientService())
                {
                    var objResult = obj.GetClientUserAssociationList();
                    return View(objResult.resultData);
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw;
            }
        }

        [CheckSession]
        public JsonResult ExportToExcel()
        {
            try
            {
                using (ClientService objClientUser = new ClientService())
                {
                    Utltity obj = new Utltity();
                    var objResult = objClientUser.GetClientList().ResultData.Select(s => new
                    {
                        Client_Agency_ID = s.Id,
                        Client_Name = s.Name,
                        City = s.CityName,
                        Entity_Name = s.EntityName,
                        State = s.StateName,
                        Status = s.Status == true ? "Active" : "Inactive",
                        Start_Date = s.StartDate,
                        //Team = Convert.ToInt32(s.TeamId),
                        Team = s.TeamName.ToString(),
                        Xero_Contact_ID_For_ProvenCfo = s.XeroContactIDforProvenCfo,
                        XeroID = s.XeroID,
                        XeroClientID = s.APIClientID,
                        XeroClientSecret = s.APIClientSecret,
                        XeroScope = s.APIScope,
                        Asana_ID = s.AsanaId,
                        Everhour_ID = s.EverhourId,
                        CRM_ID = s.CrmId,
                        Xero_Short_Code = s.XeroShortCode,
                        Created_Date = s.CreatedDate.HasValue == false || (((DateTime)s.CreatedDate).ToString("MM/dd/yyyy") == "01-01-0001" || ((DateTime)s.CreatedDate).ToString("MM/dd/yyyy") == "01/01/0001") ? "" : ((DateTime)s.CreatedDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                        Created_By = s.CreatedByUser,
                        Modified_Date = s.ModifiedDate.HasValue == false || (((DateTime)s.ModifiedDate).ToString("MM/dd/yyyy") == "01-01-0001" || ((DateTime)s.ModifiedDate).ToString("MM/dd/yyyy") == "01/01/0001") ? "" : ((DateTime)s.ModifiedDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                        Modified_By = s.ModifiedByUser

                    }).ToList();
                    string filename = obj.ExportTOExcel("clientList", obj.ToDataTable(objResult));
                    return Json(filename, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult ExportToExcel1()
        {
            try
            {
                using (ClientService objClientUser = new ClientService())
                {
                    Utltity obj = new Utltity();
                    var objResult = objClientUser.GetClientUserAssociationList().resultData.Select(s => new
                    { ClientName = s.ClientName, UserName = s.UserName }).ToList();
                    string filename = obj.ExportTOExcel("ClientUserAssociationList", obj.ToDataTable(objResult));
                    return Json(filename, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }


        [CheckSession]
        public ActionResult Download(string fileName)
        {
            try
            {
                string fullPath = System.IO.Path.Combine(Server.MapPath("~/ExportFile/"), fileName);
                byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                return File(fileByteArray, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        [HttpPost]
        public ActionResult UpdateToken(TokenInfoVM Token)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        [HttpGet]
        public ActionResult CreateClient()
        {
            try
            {
                using (ClientService obj = new ClientService())
                {
                    using (TeamsService objTeams = new TeamsService())
                    {
                        using (BillableEntitiesService objEntities = new BillableEntitiesService())
                        {
                            using (AccountService accountService = new AccountService())
                            {
                                CreateClientVM Clientvm = new CreateClientVM();
                                Clientvm.TeamList = objTeams.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                Clientvm.StateList = obj.GetAllStates().ResultData.ToList();
                                Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.Where(x => x.Status == "Active").ToList();
                                Clientvm.ThirdPartyAccountingApp_ref = 1;
                                Clientvm.IsDomoEnabled = false;
                                Clientvm.EnableAutomation = true;
                                Clientvm.EnableDataSynTimeTrigge = true;
                                Clientvm.StaffList = accountService.GetRegisteredStaffUserList().resultData;
                                TempData["ThirdPartyAccountApp"] = obj.GetThirdPartyAccountingData().ResultData;

                                ViewBag.thirdPatyAPI = obj.GetThirdPatyAPIDetails().list;

                                return View(Clientvm);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }


        [CheckSession]
        [HttpGet]
        public ActionResult EditClient(int Id)
        {
            try
            {
                using (ClientService objClientService = new ClientService())
                {
                    using (TeamsService objTeamService = new TeamsService())
                    {
                        using (BillableEntitiesService objEntities = new BillableEntitiesService())
                        {
                            using (AccountService accountService = new AccountService())
                            {

                                CreateClientVM Clientvm = new CreateClientVM();

                                var client = objClientService.GetClientById(Id);
                                Clientvm.Id = client.Id;
                                Clientvm.PhoneNumber = client.PhoneNumber;
                                Clientvm.StateId = client.StateId;
                                Clientvm.Status = client.Status == true ? "Active" : "Inactive";
                                Clientvm.Email = client.Email;
                                Clientvm.ClientName = client.Name;
                                Clientvm.CityName = client.CityName;
                                Clientvm.CityId = client.CityId;
                                Clientvm.Address = client.Address;
                                Clientvm.StateId = client.State;
                                Clientvm.TeamId = Convert.ToInt32(client.TeamId);
                                Clientvm.StateList = objClientService.GetAllStates().ResultData.ToList();
                                Clientvm.TeamList = objTeamService.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();

                                Clientvm.DOMO_datasetId = client.DOMO_datasetId;
                                Clientvm.IsDomoEnabled = false;//(bool)(client.IsDomoEnabled==null?false: client.IsDomoEnabled);
                                Clientvm.EnableAutomation = (bool)(client.EnableAutomation == null ? false : client.EnableAutomation);

                                Clientvm.EnableDataSynTimeTrigge = client.EnableDataSynTimeTrigge == null ? false : (bool)client.EnableDataSynTimeTrigge;


                                Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                Clientvm.BillableEntityId = client.BillableEntityId;
                                Clientvm.ContactPersonName = client.ContactPersonName;
                                Clientvm.EnableAutomation = client.EnableAutomation.HasValue ? client.EnableAutomation.Value : false;
                                Clientvm.XeroTokenInfoLink_ref = client.XeroTokenInfoLink_ref;

                                Clientvm.StaffList = accountService.GetRegisteredStaffUserList().resultData.ToList();
                                var teamMembers = objTeamService.GetTeamsById((int)client.TeamId);

                                Clientvm.TeamMemberId1 = teamMembers.TeamMemberId1;
                                Clientvm.TeamMemberId2 = teamMembers.TeamMemberId2;
                                Clientvm.TeamMemberId3 = teamMembers.TeamMemberId3;
                                Clientvm.TeamMemberId4 = teamMembers.TeamMemberId4;


                                if (client.StartDate != null)
                                {
                                    Clientvm.StartDateText = client.StartDate.Value.ToString("MM/dd/yyyy");

                                }


                                ViewBag.thirdPatyAPI = objClientService.GetThirdPatyAPIDetails().list;
                                Clientvm.clientXeroAccounts = objClientService.GetClientXeroAcccountsByAgencyId(Id).ResultData;

                                TempData["ThirdPartyAccountApp"] = objClientService.GetThirdPartyAccountingData().ResultData;
                                Clientvm.XeroID = client.XeroID;
                                if (client.APIScope != null)
                                {
                                    if (client.ThirdPartyAccountingApp_ref == 1)
                                    {
                                        if (client.APIScope.Split(' ').Length < 2)
                                        {
                                            Clientvm.APIScope = client.APIScope;

                                        }
                                        else
                                        {

                                            Clientvm.APIScope = client.APIScope.Split(' ').Skip(1).ToArray().Aggregate((e1, e2) => { return e1 + " " + e2; });
                                        }
                                    }
                                    else
                                    {
                                        Clientvm.APIScope = client.APIScope;
                                    }
                                }
                                Clientvm.APIClientID = client.APIClientID;
                                Clientvm.APIClientSecret = client.APIClientSecret;
                                Clientvm.ReceiveQuarterlyReports = client.ReceiveQuarterlyReports;
                                Clientvm.XeroContactIDforProvenCfo = client.XeroContactIDforProvenCfo;
                                Clientvm.AsanaId = client.AsanaId;
                                Clientvm.EverhourId = client.EverhourId;
                                Clientvm.CrmId = client.CrmId;
                                Clientvm.XeroShortCode = client.XeroShortCode;
                                Clientvm.DashboardId = client.DashboardId;
                                Clientvm.DashboardURLId = client.DashboardURLId;
                                Clientvm.ReportId = client.ReportId;
                                if (client.ThirdPartyAccountingApp_ref == null)
                                {
                                    Clientvm.ThirdPartyAccountingApp_ref = 1;

                                }
                                else
                                {
                                    Clientvm.ThirdPartyAccountingApp_ref = client.ThirdPartyAccountingApp_ref;
                                }
                                Clientvm.QuickBooksCompanyId = client.QuickBooksCompanyId;
                                Clientvm.Plaid_Enabled = client.Plaid_Enabled.HasValue ? client.Plaid_Enabled.Value : false;
                                Clientvm.XeroScopeArray = Clientvm.APIScope != null ? Clientvm.APIScope.Split(' ') : new String[0];

                                return View("CreateClient", Clientvm);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

    
        [CheckSession]
        [HttpPost]
        public ActionResult CreateClient(CreateClientVM createClientVM)
        {

              if (ModelState.IsValid|| ModelState.IsValidField("DOMO_datasetId")==false &&createClientVM.IsDomoEnabled==false)
            {
                try
                {
                    using (ClientService obj = new ClientService())
                    {
                        using (TeamsService objTeams = new TeamsService())
                        {
                            using (BillableEntitiesService objEntities = new BillableEntitiesService())
                            {
                                using (AccountService accountService = new AccountService())
                                {
                                    TeamsVM teamsVM = new TeamsVM();
                                    CreateClientVM Clientvm = new CreateClientVM();

                                    Clientvm.XeroScopeArray = createClientVM.XeroScopeArray;

                                    var LoginUserid = Session["UserId"].ToString();
                                    Clientvm.StateList = obj.GetAllStates().ResultData.ToList();
                                    Clientvm.TeamList = objTeams.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                    Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                    Clientvm.ThirdPartyAccountingApp_ref = createClientVM.ThirdPartyAccountingApp_ref;

                                    Clientvm.DomoScopeArray = createClientVM.DomoScopeArray;
                                    Clientvm.DOMO_datasetId = createClientVM.DOMO_datasetId;
                                    Clientvm.StaffList = accountService.GetRegisteredStaffUserList().resultData.ToList();

                                    Clientvm.EnableDataSynTimeTrigge = createClientVM.EnableDataSynTimeTrigge;
                                    ViewBag.thirdPatyAPI = obj.GetThirdPatyAPIDetails().list.OrderBy(pkg => pkg.Id).ToList();


                                    if (!string.IsNullOrEmpty(createClientVM.StartDateText))
                                    {
                                        CultureInfo provider = CultureInfo.InvariantCulture;
                                        createClientVM.StartDate = DateTime.ParseExact(createClientVM.StartDateText, "MM/dd/yyyy", provider);
                                    }

                                    if (createClientVM.Id == 0)
                                    {
                                        TempData["ThirdPartyAccountApp"] = obj.GetThirdPartyAccountingData().ResultData;
                                        var ClientExist = obj.GetClientByName(createClientVM.ClientName);

                                        if (ClientExist != null)
                                        {
                                            ViewBag.ErrorMessage = "Exist";
                                            createClientVM.APIScope = string.Join(" ", createClientVM.XeroScopeArray);
                                            Clientvm.APIScope = createClientVM.APIScope;
                                            Clientvm.XeroScopeArray = createClientVM.XeroScopeArray;
                                            return View("CreateClient", Clientvm);
                                        }


                                        if (createClientVM.APIClientID != null && createClientVM.APIClientID != "")
                                        {
                                            createClientVM.APIClientID = createClientVM.APIClientID.Split('=')[0];
                                        }


                                        if (Clientvm.DomoScopeArray != null)
                                        {
                                            createClientVM.APIScope = string.Join(" ", createClientVM.DomoScopeArray);
                                        }
                                        else
                                        if (Clientvm.XeroScopeArray != null)
                                        {
                                            createClientVM.APIScope = string.Join(" ", createClientVM.XeroScopeArray);
                                        }

                                        if (createClientVM.ThirdPartyAccountingApp_ref == 1)
                                        {
                                            createClientVM.APIClientID = ViewBag.thirdPatyAPI[0].ClientId;

                                            createClientVM.APIClientSecret = ViewBag.thirdPatyAPI[0].ClientSecret;
                                            createClientVM.XeroScopeArray = ViewBag.thirdPatyAPI[0].APIScope.Split(' ');


                                        }

                                        if (createClientVM.ThirdPartyAccountingApp_ref == 2)
                                        {

                                            createClientVM.APIClientID = ViewBag.thirdPatyAPI[1].ClientId;

                                            createClientVM.APIClientSecret = ViewBag.thirdPatyAPI[1].ClientSecret;

                                            createClientVM.XeroScopeArray = ViewBag.thirdPatyAPI[1].APIScope.Split(' ');
                                        }
                                        var teamName = "Team - " + createClientVM.ClientName;
                                        teamsVM = objTeams.AddTeam(teamName, "Active", createClientVM.TeamMemberId1, createClientVM.TeamMemberId2, createClientVM.TeamMemberId3, createClientVM.TeamMemberId4, LoginUserid);
                                        teamsVM = objTeams.GetTeamsByName(teamName);

                                        var result = obj.CreateClient(createClientVM.ClientName, createClientVM.Email, createClientVM.PhoneNumber, createClientVM.Address, createClientVM.ContactPersonName, createClientVM.CityName, Convert.ToString(createClientVM.StateId), createClientVM.Status, LoginUserid, /*Convert.ToString(createClientVM.TeamId)*/ Convert.ToString(teamsVM.Id), Convert.ToString(createClientVM.BillableEntityId), createClientVM.StartDate ?? null, createClientVM.XeroID, createClientVM.APIScope, createClientVM.APIClientID, createClientVM.APIClientSecret, createClientVM.ReceiveQuarterlyReports, createClientVM.EnableAutomation, createClientVM.XeroContactIDforProvenCfo, createClientVM.AsanaId, createClientVM.EverhourId, createClientVM.CrmId, createClientVM.XeroShortCode, Convert.ToString(createClientVM.DashboardId), createClientVM.DashboardURLId, createClientVM.ReportId, Convert.ToInt32(createClientVM.ThirdPartyAccountingApp_ref), Convert.ToInt64(createClientVM.QuickBooksCompanyId), createClientVM.Plaid_Enabled, createClientVM.DOMO_datasetId, createClientVM.EnableDataSynTimeTrigge);
                                        if (result == null)
                                            ViewBag.ErrorMessage = "";
                                        ViewBag.ErrorMessage = "Created";
                                    }
                                    else
                                    {

                                        var clientId = createClientVM.Id;

                                         var clientRef = obj.GetClientById(clientId).ThirdPartyAccountingApp_ref;
                                        createClientVM.ThirdPartyAccountingApp_ref = clientRef;

                                        TempData["ThirdPartyAccountApp"] = obj.GetThirdPartyAccountingData().ResultData;
                                        var ClientExist = obj.GetClientByName(createClientVM.ClientName);
                                        createClientVM.StateList = obj.GetAllStates().ResultData.ToList();
                                        createClientVM.TeamList = objTeams.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                        createClientVM.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                        createClientVM.clientXeroAccounts = obj.GetClientXeroAcccountsByAgencyId(createClientVM.Id).ResultData;
                                        createClientVM.StaffList = accountService.GetRegisteredStaffUserList().resultData.ToList();

                                        var client = obj.GetClientById(createClientVM.Id);
                                        var team = objTeams.GetTeamsById((int)client.TeamId);

                                        if (createClientVM.ThirdPartyAccountingApp_ref == 1)
                                        {
                                            createClientVM.APIClientID = ViewBag.thirdPatyAPI[0].ClientId;

                                            createClientVM.APIClientSecret = ViewBag.thirdPatyAPI[0].ClientSecret;
                                            createClientVM.XeroScopeArray = ViewBag.thirdPatyAPI[0].APIScope.Split(' ');


                                        }

                                        if (createClientVM.ThirdPartyAccountingApp_ref == 2)
                                        {

                                            createClientVM.APIClientID = ViewBag.thirdPatyAPI[1].ClientId;

                                            createClientVM.APIClientSecret = ViewBag.thirdPatyAPI[1].ClientSecret;

                                            createClientVM.XeroScopeArray = ViewBag.thirdPatyAPI[1].APIScope.Split(' ');
                                        }


                                        if (ClientExist != null && ClientExist.Id != createClientVM.Id)
                                        {
                                            ViewBag.ErrorMessage = "Exist";
                                            return View("CreateClient", createClientVM);
                                        }
                                        if (Clientvm.XeroScopeArray != null)
                                        {
                                            createClientVM.APIScope = string.Join(" ", createClientVM.XeroScopeArray);
                                        }
                                        
                                        createClientVM.IsUpdate = true;
                                        var result = obj.UpdateClient(createClientVM.Id, createClientVM.ClientName, createClientVM.Email, createClientVM.PhoneNumber, createClientVM.Address, createClientVM.ContactPersonName, createClientVM.CityName, Convert.ToString(createClientVM.StateId), createClientVM.Status, LoginUserid, /*Convert.ToString(createClientVM.TeamId)*/ Convert.ToString(team.Id), createClientVM.BillableEntityId.ToString(), createClientVM.StartDate ?? null, createClientVM.XeroID, createClientVM.APIScope, createClientVM.APIClientID, createClientVM.APIClientSecret, createClientVM.ReceiveQuarterlyReports, createClientVM.EnableAutomation, createClientVM.XeroContactIDforProvenCfo, createClientVM.AsanaId, createClientVM.EverhourId, createClientVM.CrmId, createClientVM.XeroShortCode, Convert.ToString(createClientVM.DashboardId), createClientVM.DashboardURLId, createClientVM.ReportId, createClientVM.IncludedAccountNumbers, createClientVM.ExcludedAccountNumbers, 0, Convert.ToInt64(createClientVM.QuickBooksCompanyId), createClientVM.Plaid_Enabled, createClientVM.IsDomoEnabled, createClientVM.DOMO_datasetId, createClientVM.TeamMemberId1, createClientVM.TeamMemberId2, createClientVM.TeamMemberId3, createClientVM.TeamMemberId4, createClientVM.IsUpdate);

                                        ViewBag.ErrorMessage = "";
                                        ViewBag.ErrorMessage = "Updated";
                                        return View("CreateClient", createClientVM);
                                    }
                                    return View("CreateClient", Clientvm);

                                }
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                    return View();
                }
            }
            return View();
        }

        [CheckSession]
        public ActionResult DeactivateClient(int id, string Status)
        {
            try
            {
                using (ClientService objClientService = new ClientService())
                {
                    var LoginUserid = Session["UserId"].ToString();
                    var client = objClientService.GetClientById(id);
                    client.IsUpdate = false;

                    var EnableAutomation = client.EnableAutomation.HasValue ? client.EnableAutomation.Value : false;
                    var Plaid_Enabled = client.Plaid_Enabled.HasValue ? client.Plaid_Enabled.Value : false;
                    var result = objClientService.UpdateClient(client.Id, client.Name, client.Email, client.PhoneNumber, client.Address, client.ContactPersonName, client.CityName, client.State.ToString(), Status, LoginUserid, client.TeamId.ToString(), client.BillableEntityId.ToString(), Convert.ToDateTime(client.StartDate), client.XeroID, client.APIScope, client.APIClientSecret, client.APIClientSecret, client.ReceiveQuarterlyReports, EnableAutomation, client.XeroContactIDforProvenCfo, client.AsanaId, client.EverhourId, client.CrmId, client.XeroShortCode,
                        Convert.ToString(client.DashboardId), client.DashboardURLId, client.ReportId, string.Empty, string.Empty, Convert.ToInt32(client.ThirdPartyAccountingApp_ref), Convert.ToInt64(client.QuickBooksCompanyId), Plaid_Enabled, (bool)client.IsDomoEnabled, client.DOMO_datasetId, client.TeamMemberId1, client.TeamMemberId2, client.TeamMemberId3, client.TeamMemberId4, client.IsUpdate);
                    if (result == null)
                        ViewBag.ErrorMessage = "";
                    return RedirectToAction("ClientList");
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        public JsonResult DeleteClient(int id)
        {
            try
            {
                using (ClientService objClient = new ClientService())
                {
                    var results = objClient.IsClientInvitationAssociationByIdExists(id);

                    if (results == false)
                    {
                        var result = objClient.DeleteClient(id);
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "This user is associated with client.";
                        Utltity.Log4NetInfoLog(ViewBag.ErrorMessage);
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

        [CheckSession]
        [HttpGet]
        public ActionResult GetThirdPartyDetails(int Id = 0)
        {

            try
            {
                using (var clientServ = new ClientService())
                {


                    return Json(clientServ.GetThirdPatyAPIDetails().list);
                }

            }
            catch (Exception ex)
            {

                return Json(new { msg = ex.Message });
            }
        }

        [CheckSession]
        public ActionResult GetClientXeroAcccountsList()
        {
            try
            {
                using (ClientService obj = new ClientService())
                {
                    var objResult = obj.GetClientXeroAcccounts();
                    return View(objResult.ResultData);
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }

   
        [HttpGet]
        [CheckSession]
        public async Task<ActionResult> GetXeroRelatedInfo(XeroDetailsVM xeroInfo) //fetch xero details like xero ID and xero short code
        {
            string xeroId = "", xeroShortCode = "", xeroProvenCfoContactId = "";
            string errorMsg = "";
            var apiDetails = new ClientService().GetThirdPatyAPIDetails().list.OrderBy(PK=>PK.Id).ToList().FirstOrDefault();

            xeroInfo.clientId = apiDetails.ClientId;
            xeroInfo.clientSecret = apiDetails.ClientSecret;
            try
            {
                ClientModel client;
                using (var clientService = new ClientService())
                {
                    client = clientService.GetClientByName("ProvenCFO");
                }

                using (XeroService<IXeroToken, List<Xero.NetStandard.OAuth2.Models.Tenant>> xeroService = new XeroService<IXeroToken,
                    List<Xero.NetStandard.OAuth2.Models.Tenant>>(xeroInfo.clientId, xeroInfo.clientSecret,
                    AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName
                    ))
                {
                    List<Xero.NetStandard.OAuth2.Models.Tenant> result = await xeroService.GetXeroConnections(AccountingPackageInstance.Instance.XeroToken);

                    var orgi = (from org in result
                                where org.TenantName == xeroInfo.agencyName
                                select new
                                {
                                    Name = org.TenantName,
                                    xeroid = org.TenantId
                                }).FirstOrDefault();

                    if (orgi != null)
                    {
                        xeroId = Convert.ToString(orgi.xeroid);
                        using (var _xeroService = new XeroService<IXeroToken, Xero.NetStandard.OAuth2.Model.Accounting.Organisations>(
                         xeroInfo.clientId, xeroInfo.clientSecret,
                         AccountingPackageInstance.Instance.Scope,
                         AccountingPackageInstance.Instance.XeroAppName))
                        {
                            var organisations = await _xeroService.GetOrganisationsId(AccountingPackageInstance.Instance.XeroToken, xeroId);
                            var org = organisations._Organisations.Where(_org => _org.Name == xeroInfo.agencyName).FirstOrDefault();
                            if (org != null)
                            {
                                xeroId = org.OrganisationID.ToString();
                                xeroShortCode = org.ShortCode;

                                using (var _service = new XeroService<IXeroToken, Xero.NetStandard.OAuth2.Model.Accounting.Contacts>(
                                                            xeroInfo.clientId, xeroInfo.clientSecret,
                                                            AccountingPackageInstance.Instance.Scope,
                                                            AccountingPackageInstance.Instance.XeroAppName))
                                {
                                   var contacts = await _service.GetContacts(AccountingPackageInstance.Instance.XeroToken, xeroId);
                                    contacts._Contacts.ForEach(con =>
                                    {
                                        if (con.Name == "ProvenCFO")
                                        {
                                            xeroProvenCfoContactId = con.ContactID.ToString();
                                        }
                                    });
                                }
                            }
                            else
                            {
                                errorMsg = "Not a Valid Xero Client!";
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "Sorry, this organization cannot be found in Xero.\n" +
                                   "Please contact an admin to get this organization properly configured in Xero.";
                        return Json(new
                        {
                            XeroID = "",
                            XeroContectInfo ="",
                            XeroShortCod = "",
                            Status = "Error",
                            ErrorMessage = errorMsg,
                        }, JsonRequestBehavior.AllowGet);
                    }

                }
                return Json(new
                {
                    XeroID = xeroId,
                    XeroContectInfo = xeroProvenCfoContactId,
                    XeroShortCod = xeroShortCode,
                    Status = "Success",
                    ErrorMessage = "",
                }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));

                return Json(new { Status="Error",ErrorMessage=ex.Message}, JsonRequestBehavior.AllowGet);
            }
        }
    }
}
