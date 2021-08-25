using Proven.Service;
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

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class ClientController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Client
        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
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
                        XeroID = s.XeroID,
                        XeroClientID = s.XeroClientID,
                        XeroClientSecret = s.XeroClientSecret,
                        XeroScope = s.XeroScope,
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
                            Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList();
                            Clientvm.BillableEntityId = client.BillableEntityId;
                            Clientvm.ContactPersonName = client.ContactPersonName;
                            if (client.StartDate != null)
                            {
                                Clientvm.StartDateText = client.StartDate.Value.ToString("MM/dd/yyyy");
                               // Clientvm.StartDateText = Clientvm.StartDateText == "01-01-0001" ? "" : Convert.ToString(Clientvm.StartDateText);
                                //Clientvm.StartDate = client.StartDate;
                            }
                            //else
                            //{
                            //    Clientvm.StartDate = client.StartDate;
                            //   // Clientvm.StartDateText = client.StartDate.Value.ToString("MM/dd/yyyy");
                            //    //Clientvm.StartDateText = client.StartDate.Value.ToString("MM/dd/yyyy");
                            //    //Clientvm.StartDateText = Clientvm.StartDateText == "01-01-0001" ? "" : Convert.ToString(Clientvm.StartDateText);
                            //}

                            Clientvm.XeroID = client.XeroID;
                            Clientvm.XeroScope = client.XeroScope;
                            Clientvm.XeroClientID = client.XeroClientID;
                            Clientvm.XeroClientSecret = client.XeroClientSecret;

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
                                Clientvm.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList();
                                if (!string.IsNullOrEmpty(createClientVM.StartDateText))
                                {
                                    CultureInfo provider = CultureInfo.InvariantCulture;
                                    createClientVM.StartDate = DateTime.ParseExact(createClientVM.StartDateText, "MM/dd/yyyy", provider);
                                }

                                if (createClientVM.Id == 0)
                                {
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
                                        createClientVM.XeroScope = string.Join(" ", createClientVM.XeroScopeArray);
                                    }
                                    //(creteClientVM.StartDate == null ? null : Convert.ToDateTime(creteClientVM.StartDate))
                                    var result = obj.CreateClient(createClientVM.ClientName, createClientVM.Email, createClientVM.PhoneNumber, createClientVM.Address, createClientVM.ContactPersonName, createClientVM.CityName, createClientVM.StateId.ToString(), createClientVM.Status, LoginUserid, createClientVM.TeamId.ToString(), createClientVM.BillableEntityId.ToString(), Convert.ToDateTime(createClientVM.StartDate), createClientVM.XeroID, createClientVM.XeroScope, createClientVM.XeroClientID, createClientVM.XeroClientSecret);
                                    if (result == null)
                                        ViewBag.ErrorMessage = "";
                                    ViewBag.ErrorMessage = "Created";
                                }
                                else
                                {
                                    var ClientExist = obj.GetClientByName(createClientVM.ClientName);
                                    createClientVM.StateList = obj.GetAllStates().ResultData.ToList();
                                    createClientVM.TeamList = objTeams.GetTeamsList().ResultData.ToList().Where(x => x.Status == "Active").ToList();
                                    createClientVM.billableEntitiesList = objEntities.GetAllBillableEntitiesList().ResultData.ToList();
                                    if (ClientExist != null && ClientExist.Id != createClientVM.Id)
                                    {
                                        ViewBag.ErrorMessage = "Exist";
                                        return View("CreateClient", createClientVM);
                                    }
                                    if (Clientvm.XeroScopeArray != null)
                                    {
                                        createClientVM.XeroScope = string.Join(" ", createClientVM.XeroScopeArray);
                                    }
                                    var result = obj.UpdateClient(createClientVM.Id, createClientVM.ClientName, createClientVM.Email, createClientVM.PhoneNumber, createClientVM.Address, createClientVM.ContactPersonName, createClientVM.CityName, createClientVM.StateId.ToString(), createClientVM.Status, LoginUserid, createClientVM.TeamId.ToString(), createClientVM.BillableEntityId.ToString(), createClientVM.StartDate ?? null, createClientVM.XeroID, createClientVM.XeroScope, createClientVM.XeroClientID, createClientVM.XeroClientSecret);



                                    ViewBag.ErrorMessage = "";
                                    ViewBag.ErrorMessage = "Updated";
                                    return View("CreateClient", createClientVM);
                                }
                                return View("CreateClient", Clientvm);
                                return RedirectToAction("ClientList");
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

                    var result = objClientService.UpdateClient(client.Id, client.Name, client.Email, client.PhoneNumber, client.Address, client.ContactPersonName, client.CityName, client.State.ToString(), Status, LoginUserid, client.TeamId.ToString(), client.BillableEntityId.ToString(), Convert.ToDateTime(client.StartDate), client.XeroID, client.XeroScope, client.XeroClientSecret, client.XeroClientSecret);
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


    }
}