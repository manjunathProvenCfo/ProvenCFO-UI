﻿using Proven.Service;
using ProvenCfoUI.Models;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ProvenCfoUI.Comman;
using System.Globalization;
using log4net;
using System.Threading.Tasks;
namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    [Exception_Filters]
    public class ClientController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Client
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                            CreateClientVM Clientvm = new CreateClientVM();
                            Clientvm.TeamList = objTeams.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                            Clientvm.StateList = obj.GetAllStates().ResultData.ToList();
                            Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.Where(x => x.Status == "Active").ToList();
                            Clientvm.ThirdPartyAccountingApp_ref = 1;
                            TempData["ThirdPartyAccountApp"] = obj.GetThirdPartyAccountingData().ResultData;
                            return View(Clientvm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                            Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                            Clientvm.BillableEntityId = client.BillableEntityId;
                            Clientvm.ContactPersonName = client.ContactPersonName;
                            Clientvm.EnableAutomation = client.EnableAutomation.HasValue ? client.EnableAutomation.Value : false;
                            if (client.StartDate != null)
                            {
                                Clientvm.StartDateText = client.StartDate.Value.ToString("MM/dd/yyyy");
                                //   // Clientvm.StartDateText = Clientvm.StartDateText == "01-01-0001" ? "" : Convert.ToString(Clientvm.StartDateText);
                                //    //Clientvm.StartDate = client.StartDate;
                            }
                            Clientvm.clientXeroAccounts = objClientService.GetClientXeroAcccountsByAgencyId(Id).ResultData;
                            //else
                            //{
                            //    Clientvm.StartDate = client.StartDate;
                            //   // Clientvm.StartDateText = client.StartDate.Value.ToString("MM/dd/yyyy");
                            //    //Clientvm.StartDateText = client.StartDate.Value.ToString("MM/dd/yyyy");
                            //    //Clientvm.StartDateText = Clientvm.StartDateText == "01-01-0001" ? "" : Convert.ToString(Clientvm.StartDateText);
                            //}
                            TempData["ThirdPartyAccountApp"] = objClientService.GetThirdPartyAccountingData().ResultData;
                            Clientvm.XeroID = client.XeroID;
                            Clientvm.APIScope = client.APIScope;
                            Clientvm.APIClientID = client.APIClientID;
                            Clientvm.APIClientSecret = client.APIClientSecret;
                            Clientvm.ReceiveQuarterlyReports = client.ReceiveQuarterlyReports;
                            Clientvm.XeroContactIDforProvenCfo = client.XeroContactIDforProvenCfo;
                            Clientvm.AsanaId = client.AsanaId;
                            Clientvm.EverhourId = client.EverhourId;
                            Clientvm.CrmId = client.CrmId;
                            Clientvm.XeroShortCode = client.XeroShortCode;
                            //Clientvm.DashboardId = Convert.ToInt32 (client.DashboardId);
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
                            Clientvm.Plaid_Enabled = client.Plaid_Enabled;
                            

                            return View("CreateClient", Clientvm);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        [HttpPost]
        public ActionResult CreateClient(CreateClientVM createClientVM)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    using (ClientService obj = new ClientService())
                    {
                        using (TeamsService objTeams = new TeamsService())
                        {
                            using (BillableEntitiesService objEntities = new BillableEntitiesService())
                            {
                                
                                CreateClientVM Clientvm = new CreateClientVM();
                                Clientvm.XeroScopeArray = createClientVM.XeroScopeArray;
                                var LoginUserid = Session["UserId"].ToString();
                                Clientvm.StateList = obj.GetAllStates().ResultData.ToList();
                                Clientvm.TeamList = objTeams.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                
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
                                        return View("CreateClient", Clientvm);
                                    }
                                    // createClientVM.XeroScope = createClientVM.XeroScopeArray.ToString();
                                    //string result = string.Join(".", array);
                                    //string.Join(",", Client);s
                                    if (Clientvm.XeroScopeArray != null)
                                    {
                                        createClientVM.APIScope = string.Join(" ", createClientVM.XeroScopeArray);
                                    }
                                    //(creteClientVM.StartDate == null ? null : Convert.ToDateTime(creteClientVM.StartDate))
                                    var result = obj.CreateClient(createClientVM.ClientName, createClientVM.Email, createClientVM.PhoneNumber, createClientVM.Address, createClientVM.ContactPersonName, createClientVM.CityName, Convert.ToString(createClientVM.StateId), createClientVM.Status, LoginUserid, Convert.ToString(createClientVM.TeamId), Convert.ToString(createClientVM.BillableEntityId), createClientVM.StartDate ?? null, createClientVM.XeroID, createClientVM.APIScope, createClientVM.APIClientID, createClientVM.APIClientSecret, createClientVM.ReceiveQuarterlyReports, createClientVM.EnableAutomation, createClientVM.XeroContactIDforProvenCfo, createClientVM.AsanaId, createClientVM.EverhourId, createClientVM.CrmId, createClientVM.XeroShortCode, Convert.ToString(createClientVM.DashboardId), createClientVM.DashboardURLId, createClientVM.ReportId, Convert.ToInt32(createClientVM.ThirdPartyAccountingApp_ref), Convert.ToInt64(createClientVM.QuickBooksCompanyId),createClientVM.Plaid_Enabled);
                                    if (result == null)
                                        ViewBag.ErrorMessage = "";
                                    ViewBag.ErrorMessage = "Created";
                                }
                                else
                                {
                                    //var exitclient = obj.GetClientById(createClientVM.Id);
                                    TempData["ThirdPartyAccountApp"] = obj.GetThirdPartyAccountingData().ResultData;
                                    var ClientExist = obj.GetClientByName(createClientVM.ClientName);
                                    createClientVM.StateList = obj.GetAllStates().ResultData.ToList();
                                    createClientVM.TeamList = objTeams.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                    createClientVM.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                    createClientVM.clientXeroAccounts = obj.GetClientXeroAcccountsByAgencyId(createClientVM.Id).ResultData;
                                    if (ClientExist != null && ClientExist.Id != createClientVM.Id)
                                    {
                                        ViewBag.ErrorMessage = "Exist";
                                        return View("CreateClient", createClientVM);
                                    }
                                    if (Clientvm.XeroScopeArray != null)
                                    {
                                        createClientVM.APIScope = string.Join(" ", createClientVM.XeroScopeArray);
                                    }
                                    
                                  var result = obj.UpdateClient(createClientVM.Id, createClientVM.ClientName, createClientVM.Email, createClientVM.PhoneNumber, createClientVM.Address, createClientVM.ContactPersonName, createClientVM.CityName, Convert.ToString(createClientVM.StateId), createClientVM.Status, LoginUserid,Convert.ToString(createClientVM.TeamId), createClientVM.BillableEntityId.ToString(), createClientVM.StartDate ?? null, createClientVM.XeroID, createClientVM.APIScope, createClientVM.APIClientID, createClientVM.APIClientSecret, createClientVM.ReceiveQuarterlyReports, createClientVM.EnableAutomation, createClientVM.XeroContactIDforProvenCfo, createClientVM.AsanaId, createClientVM.EverhourId, createClientVM.CrmId, createClientVM.XeroShortCode,Convert.ToString(createClientVM.DashboardId), createClientVM.DashboardURLId,createClientVM.ReportId, createClientVM.IncludedAccountNumbers, createClientVM.ExcludedAccountNumbers, 0 ,Convert.ToInt64(createClientVM.QuickBooksCompanyId),createClientVM.Plaid_Enabled);




                                    ViewBag.ErrorMessage = "";
                                    ViewBag.ErrorMessage = "Updated";
                                    return View("CreateClient", createClientVM);
                                }
                                return View("CreateClient", Clientvm);
                                //return RedirectToAction("ClientList");
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
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
                    var EnableAutomation = client.EnableAutomation.HasValue ? client.EnableAutomation.Value : false;
                    var result = objClientService.UpdateClient(client.Id, client.Name, client.Email, client.PhoneNumber, client.Address, client.ContactPersonName, client.CityName, client.State.ToString(), Status, LoginUserid, client.TeamId.ToString(), client.BillableEntityId.ToString(), Convert.ToDateTime(client.StartDate), client.XeroID, client.APIScope, client.APIClientSecret, client.APIClientSecret, client.ReceiveQuarterlyReports, EnableAutomation, client.XeroContactIDforProvenCfo, client.AsanaId, client.EverhourId, client.CrmId, client.XeroShortCode,
                        Convert.ToString(client.DashboardId),client.DashboardURLId,client.ReportId, string.Empty,string.Empty, Convert.ToInt32(client.ThirdPartyAccountingApp_ref),Convert.ToInt64(client.QuickBooksCompanyId),client.Plaid_Enabled);
                    if (result == null)
                        ViewBag.ErrorMessage = "";
                    return RedirectToAction("ClientList");
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
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
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

    }
}