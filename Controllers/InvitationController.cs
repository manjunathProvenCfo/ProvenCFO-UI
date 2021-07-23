using OfficeOpenXml;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Areas.Model;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class InvitationController : Controller
    {
        // GET: 

        string errorMessage = string.Empty;
        string errorDescription = string.Empty;

        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        public ActionResult InviteStaffList()
        {
            using (InvitationServices obj = new InvitationServices())
            {

                using (RoleService objrole = new RoleService())
                {
                    using (SetupService objJob = new SetupService())
                    {
                        var objResult = obj.GetInvitation();
                        objResult.Rolelist = objrole.GetAllRoleInvitation().ResultData;
                        objResult.JobTitlelist = objJob.GetJobTitleListInvitation().ResultData;
                        return View(objResult.ResultData);
                    }

                }

            }

        }

        [CheckSession]
        public ActionResult InviteStaff()
        {
            using (RoleService objrole = new RoleService())
            {
                using (SetupService objJob = new SetupService())
                {
                    InviteStaffViewModel emptyobj = new InviteStaffViewModel();
                    emptyobj.Rolelist = objrole.GetAllRoleInvitation().ResultData;
                    emptyobj.JobTitlelist = objJob.GetJobTitleListInvitation().ResultData;
                    return View(emptyobj);
                }
            }

        }


        [CheckSession]
        [HttpPost]
        public ActionResult InviteStaff(InviteStaffViewModel inviteStaffVM)
        {

            try
            {
                using (InvitationServices obj = new InvitationServices())
                {
                    using (RoleService objrole = new RoleService())
                    {
                        using (SetupService objJob = new SetupService())
                        {
                            InviteStaffViewModel InviteVM = new InviteStaffViewModel();
                            var result = new InviteUserModel();
                            var LoginUserid = Session["UserId"].ToString();
                            var session = ConfigurationManager.AppSettings["userinvitationsessionouttime"].ToString();
                            inviteStaffVM.Rolelist = objrole.GetAllRoleInvitation().ResultData;
                            inviteStaffVM.JobTitlelist = objJob.GetJobTitleListInvitation().ResultData;
                            InviteVM.Rolelist = inviteStaffVM.Rolelist;
                            InviteVM.JobTitlelist = inviteStaffVM.JobTitlelist;
                            if (inviteStaffVM.id == null || inviteStaffVM.id == 0)
                            {
                                var Existresult = obj.GetInvitationByEmail(inviteStaffVM.Email);
                                if (Existresult != null)
                                {
                                    ViewBag.ErrorMessage = "Exist";
                                    return View("InviteStaff", InviteVM);
                                }
                                result = obj.AddInvitation(inviteStaffVM.FirstName, inviteStaffVM.LastName, inviteStaffVM.Email, inviteStaffVM.roleid, inviteStaffVM.jobid, inviteStaffVM.SessionTimeout, LoginUserid);
                                ViewBag.ErrorMessage = "Sent";
                            }

                            if (result == null)
                                ViewBag.ErrorMessage = "";
                            ViewBag.Message = "Invitation Send Successfully";
                            return RedirectToAction("InviteStaffList");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to Send Invitation.";
                Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                return View();
            }

        }

        [CheckSession]
        public ActionResult InviteStaffByAgency(InviteUserModel InviteUser)
        {
            using (InvitationServices obj = new InvitationServices())
            {
                if (InviteUser.Id > 0)
                {
                    var data = obj.GetInvitation().ResultData.Where(x => x.Id == InviteUser.Id).FirstOrDefault();
                    return View(data);
                }
                return View();
            }

        }

        [CheckSession]
        public ActionResult NavigateToInviteStaffByAgency()
        {
            InviteUserModel objInvite = new InviteUserModel();
            List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
            if (UserPref != null && UserPref.Count() > 0)
            {
                var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                objInvite.AgencyID = Convert.ToInt32(selectedAgency.PreferanceValue);
                using (ClientService objClient = new ClientService())
                {
                    var objResult = objClient.GetClientById(Convert.ToInt32(objInvite.AgencyID));
                    objInvite.AgencyName = objResult.Name;
                }
            }

            return View("InviteStaffByAgency", objInvite);
        }

        [CheckSession]
        public ActionResult EditToInviteStaffByAgency(int Id)
        {
            InviteUserModel objInvite = new InviteUserModel();

            using (InvitationServices obj = new InvitationServices())
            {
                var result = obj.GetInvitationById(Id);
                objInvite.Id = result.Id.Value;
                objInvite.FirstName = result.FirstName;
                objInvite.LastName = result.LastName;
                objInvite.Email = result.Email;
                objInvite.AgencyID = Convert.ToInt32(result.AgencyId);
                objInvite.IsActive = Convert.ToString(result.IsActive);
                using (ClientService objClient = new ClientService())
                {
                    var objResult = objClient.GetClientById(Convert.ToInt32(objInvite.AgencyID));
                    objInvite.AgencyName = objResult.Name;
                }

            }
            return View("InviteStaffByAgency", objInvite);
        }

        [CheckSession]
        [HttpPost]
        public ActionResult InviteStaffByAgency(InviteAgencyUserViewModel inviteStaffVM)
        {
            try
            {
                InviteUserModel InviteUser = new InviteUserModel();
                if (ModelState.IsValid)
                {
                    try
                    {
                        using (InvitationServices obj = new InvitationServices())
                        {
                            var LoginUserid = Session["UserId"].ToString();
                            InviteUser.Id = Convert.ToInt32(inviteStaffVM.id == null ? 0 : inviteStaffVM.id);
                            InviteUser.FirstName = inviteStaffVM.FirstName;
                            InviteUser.LastName = inviteStaffVM.LastName;
                            InviteUser.Email = inviteStaffVM.Email;
                            InviteUser.SessionTimeout = inviteStaffVM.SessionTimeout;
                            InviteUser.AgencyID = inviteStaffVM.AgencyID;
                            InviteUser.AgencyName = inviteStaffVM.AgencyName;
                            if (inviteStaffVM.id == null || inviteStaffVM.id == 0)
                            {
                                inviteStaffVM.id = 0;

                                using (AccountService objAccount = new AccountService())
                                {
                                    var AllAgenctyInvitation = objAccount.RegisteredUserListbyAgency(Convert.ToString(inviteStaffVM.AgencyID)).ResultData;
                                    var IsExistAgencyUserInvitation = AllAgenctyInvitation.Where(x => x.Email == inviteStaffVM.Email).ToList();
                                    if (IsExistAgencyUserInvitation != null && IsExistAgencyUserInvitation.Where(x => x.IsActive == "1").Count() > 0 && IsExistAgencyUserInvitation.Where(x => x.IsRegistered == 0).Count() > 0)
                                    {
                                        ViewBag.ErrorMessage = "InvitationSend";
                                        return View("InviteStaffByAgency", InviteUser);
                                    }
                                    else if (IsExistAgencyUserInvitation != null && IsExistAgencyUserInvitation.Where(x => x.IsActive == "1").Count() > 0 && IsExistAgencyUserInvitation.Where(x => x.IsRegistered == 1).Count() > 0)
                                    {
                                        ViewBag.ErrorMessage = "UserAlreadyLinked";
                                        return View("InviteStaffByAgency", InviteUser);
                                    }

                                }

                                var result = obj.AddInvitationAgency(inviteStaffVM.FirstName, inviteStaffVM.LastName, inviteStaffVM.Email, inviteStaffVM.SessionTimeout, Convert.ToString(inviteStaffVM.AgencyID), LoginUserid);
                                if (result == null)
                                    ViewBag.Message = "Created";
                                ViewBag.ErrorMessage = "Created";

                            }
                            else
                            {

                                var objInviteUser = obj.UpdateInvitationAgency(inviteStaffVM.FirstName, inviteStaffVM.LastName, inviteStaffVM.Email, inviteStaffVM.IsActive, LoginUserid, Convert.ToString(inviteStaffVM.AgencyID));
                                if (objInviteUser == null)
                                    ViewBag.Message = "Error";
                                //ViewBag.Message = "Invitation Send Successfully";
                                ViewBag.ErrorMessage = "Updated";
                                return View("InviteStaffByAgency", InviteUser);

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
                return View("InviteStaffByAgency", InviteUser);
            }


            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to Send Invitation.";
                //Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                return View();
            }
        }

        [CheckSession]
        [HttpPost]
        public ActionResult UpdateInviteStaffByAgency(InviteAgencyUserViewModel inviteStaffVM)
        {
            InviteUserModel user = new InviteUserModel();
            try
            {
                if (ModelState.IsValid)
                {
                    using (InvitationServices obj = new InvitationServices())
                    {
                        var LoginUserid = Session["UserId"].ToString();
                        user = obj.UpdateInvitationAgency(inviteStaffVM.FirstName, inviteStaffVM.LastName, inviteStaffVM.Email, inviteStaffVM.IsActive, LoginUserid, Convert.ToString(inviteStaffVM.AgencyID));
                        if (user == null)
                            ViewBag.Message = "Error";
                        //ViewBag.Message = "Invitation Send Successfully";
                        ViewBag.ErrorMessage = "Updated";
                        return View("InviteStaffByAgency", user);
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Unable to Send Invitation.";
                //Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                return View();
            }
            return View("InviteStaffByAgency", user);
        }



        [CheckSession]
        public ActionResult DeactivateInvite(int id, string Status)
        {
            try
            {
                using (InvitationServices obj = new InvitationServices())
                {
                    var result = obj.DeactivateInvitation(id);
                    if (result == null)
                        ViewBag.ErrorMessage = "Cant deacactivte";

                    return RedirectToAction("InviteStaffList");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "";
                return View();
            }
        }

        [CheckSession]
        public ActionResult DeactivateAgencyUserInvite(int id, string Status)
        {
            try
            {
                using (InvitationServices obj = new InvitationServices())
                {
                    var result = obj.DeactivateInvitation(id);
                    if (result == null)
                        ViewBag.ErrorMessage = "Cant deacactivte";

                    return RedirectToAction("RegisteredUserList");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "";
                return View();
            }
        }

        [CheckSession]
        public ActionResult RegisteredUserList()
        {

            using (AccountService obj = new AccountService())
            {
                List<InviteUserModel> user = new List<InviteUserModel>();
                List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                if (UserPref != null && UserPref.Count > 0)
                {
                    var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    if (selectedAgency != null && selectedAgency.Id != 0)
                    {
                        var result = obj.RegisteredUserListbyAgency(Convert.ToString(selectedAgency.PreferanceValue));
                        return View(result.ResultData);
                    }
                    return View(user);
                }
                else
                {

                }
                return View(user);
            }
        }

        [CheckSession]
        public ActionResult RefreshRegisteredUserList(string AgencyID)
        {
            using (AccountService obj = new AccountService())
            {
                UserModel user = new UserModel();
                var result = obj.RegisteredUserListbyAgency(Convert.ToString(AgencyID));
                return View("RegisteredUserList", result.ResultData);
            }
        }


        [CheckSession]
        public ActionResult EditInvitation()
        {

            using (InvitationServices obj = new InvitationServices())
            {
                using (RoleService objrole = new RoleService())
                {
                    using (SetupService objJob = new SetupService())
                    {
                        InviteUserModel jobj1 = new InviteUserModel();
                        var result = obj.GetInvitationById(Convert.ToInt32(TempData["SelectediD"]));
                        jobj1.Rolelist = objrole.GetAllRoleInvitation().ResultData;
                        jobj1.JobTitlelist = objJob.GetJobTitleListInvitation().ResultData;
                        jobj1.Id = Convert.ToInt32(result.Id);
                        jobj1.FirstName = result.FirstName;
                        jobj1.LastName = result.LastName;
                        jobj1.JobTitle = result.JobTitle;
                        jobj1.RoleName = result.RoleName;
                        jobj1.IsActive = result.IsActive.ToString();
                        jobj1.RoleId = result.RoleId;
                        jobj1.JobId = Convert.ToInt32(result.JobId);
                        jobj1.Email = result.Email;


                        return View("EditInvitation", jobj1);
                    }

                }

            }

        }


        [AllowAnonymous]
        public ActionResult UserAcceptAgencyInvite(int InvitationId, int AgencyId, Guid ActiveCode)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    InvitationServices obj = new InvitationServices();
                    ViewBag.Sucess = false;
                    ViewBag.ShowLoginButton = false;
                    ViewBag.ErrorMessage = string.Empty;
                    var invitation = obj.GetInvitationForVlidation(InvitationId, AgencyId, ActiveCode);
                    if (invitation == null)
                    {
                        ViewBag.ErrorMessage = "Invalid invitation Link";
                        return PartialView("_UserAcceptAgencyInvite");
                    }
                    else if (invitation != null && invitation.IsActive == 0)
                    {
                        ViewBag.ErrorMessage = "Inactive invitation Link";
                        return PartialView("_UserAcceptAgencyInvite");
                    }
                    else if (invitation == null && DateTime.Now > invitation.ExpiryTime)
                    {
                        // ViewBag.ErrorMessage = "The agency user invitation link has been expired!";
                        ViewBag.ErrorMessage = "Sorry, the Invitation in this link is expired" +
                             "Please contact the administrators of the ProvenCFO Management Application to recive a new application";
                        return PartialView("_UserAcceptAgencyInvite");
                    }
                    else if (invitation != null && invitation.IsActive == 1 && invitation.IsRegistered == 1)
                    {
                        ViewBag.ShowLoginButton = true;
                        ViewBag.ErrorMessage = "User already accepted the invitation.";
                        return PartialView("_UserAcceptAgencyInvite");
                    }

                    var result = obj.ExistingAgencyUserInvitation(Convert.ToString(InvitationId), Convert.ToString(AgencyId), Convert.ToString(ActiveCode));

                    if (result.resultData == true)
                    {
                        ViewBag.Sucess = true;
                        ViewBag.ErrorMessage = "User added to new agency successfully. Please click on the below link to log in.";
                    }


                    return PartialView("_UserAcceptAgencyInvite");
                }
                catch (Exception ex)
                {

                    return View();
                }
            }
            return View();
        }



        [CheckSession]
        public ActionResult NavigateToEdit(int id)
        {

            if (id > 0)
            {

                TempData["SelectediD"] = id.ToString();
            }
            return RedirectToAction("EditInvitation");
        }

        [CheckSession]
        [HttpPost]
        public ActionResult EditInvitation(InviteUserModel Invite)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (InvitationServices obj = new InvitationServices())
                    {
                        using (RoleService objrole = new RoleService())
                        {
                            using (SetupService objJob = new SetupService())
                            {
                                var LoginUserid = Session["UserId"].ToString();

                                var result = obj.UpdateInvite(Convert.ToString(Invite.Id), Invite.FirstName, Invite.LastName, Invite.RoleId, Convert.ToString(Invite.JobId), Convert.ToString(Invite.IsActive), LoginUserid);
                                result.Rolelist = objrole.GetAllRoleInvitation().ResultData;
                                result.JobTitlelist = objJob.GetJobTitleListInvitation().ResultData;
                                if (result == null)
                                    ViewBag.ErrorMessage = "";
                                ViewBag.Message = "Successfull";
                                return RedirectToAction("InviteStaffList");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            return View();
        }


        [CheckSession]
        public JsonResult ExportToExcel()
        {
            using (InvitationServices objInviteUser = new InvitationServices())
            {

                Utltity obj = new Utltity();
                var objResult = objInviteUser.GetInvitation().ResultData;
                foreach (var item in objResult)
                {
                    if (item.IsRegistered == 0 && Convert.ToString(item.IsActive) == "1")
                    {
                        item.Status = "Invited";
                    }
                    else if (Convert.ToString(item.IsActive) == "1" && item.IsRegistered == 1)
                    {
                        item.Status = "Active";
                    }
                    else if (Convert.ToString(item.IsActive) == "0" && item.IsRegistered == 0)
                    {
                        item.Status = "Inactive";
                    }
                    else if (Convert.ToString(item.IsActive) == "0" && item.IsRegistered == 1)
                    {
                        item.Status = "Inactive";
                    }
                }
                var obj1 = objResult.Select(s => new
                { FirstName = s.FirstName, LastName = s.LastName, JobTitle = s.JobTitle, UserRole = s.RoleName, Email = s.Email, Status = s.Status, CreatedDate = s.CreatedDate, ModifiedDate = s.ModifiedDate, CreatedBy = s.CreatedByUser, ModifiedBy = s.ModifiedByUser }).ToList();
                string filename = obj.ExportTOExcel("Staff Users", obj.ToDataTable(obj1));
                return Json(filename, JsonRequestBehavior.AllowGet);


            }

        }

        [CheckSession]
        public JsonResult ExportToExcelAgencyUsers()
        {
            string filename = string.Empty;
            using (AccountService objInviteUser = new AccountService())
            {
                List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];
                if (UserPref != null && UserPref.Count > 0)
                {
                    var selectedAgency = UserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    if (selectedAgency != null && selectedAgency.Id != 0)
                    {
                        Utltity obj = new Utltity();
                        var objResult = objInviteUser.RegisteredUserListbyAgency(Convert.ToString(selectedAgency.PreferanceValue)).ResultData;
                        foreach (var s in objResult)
                        {
                            if (s.IsRegistered == 0 && Convert.ToString(s.IsActive) == "1")
                            {
                                s.Status = "Invited";
                            }
                            else if (Convert.ToString(s.IsActive) == "1" && s.IsRegistered == 1)
                            {
                                s.Status = "Active";
                            }
                            else if (Convert.ToString(s.IsActive) == "0" && s.IsRegistered == 0)
                            {
                                s.Status = "Inactive";
                            }
                            else if (Convert.ToString(s.IsActive) == "0" && s.IsRegistered == 1)
                            {
                                s.Status = "Inactive";
                            }
                        }
                        var obj1 = objResult.Select(s => new
                        { AgencyName = s.AgencyName, FirstName = s.FirstName, LastName = s.LastName, Email = s.Email, Status = s.Status, CreatedDate = s.CreatedDate, CreatedBy = s.CreatedByUser, ModifiedDate = s.ModifiedDate, ModifiedBy = s.ModifiedByUser }).ToList();
                        filename = obj.ExportTOExcel("Agency Users List", obj.ToDataTable(obj1));
                        return Json(filename, JsonRequestBehavior.AllowGet);
                    }

                }

            }
            return Json(filename, JsonRequestBehavior.AllowGet);

        }

        [CheckSession]
        public JsonResult DeleteUser(string id)
        {
            using (InvitationServices objInvite = new InvitationServices())
            {
                var result = objInvite.DeleteUser(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        [CheckSession]
        public JsonResult DeleteInvite(int id)
        {
            using (InvitationServices objInvite = new InvitationServices())
            {
                var result = objInvite.DeleteInvite(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }

        [CheckSession]
        public JsonResult IsInviteValied(int id)
        {
            using (InvitationServices objInvite = new InvitationServices())
            {
                var result = objInvite.IsInviteValied(id);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

        }


        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        public ActionResult GetRegisterdStaffUserList()
        {
            using (InvitationServices obj = new InvitationServices())
            {
                var objResult = obj.GetRegisteredStaffUserList();
                //objResult.StaffList = obj.GetRegisterdStaffUserList().ResultData;
                return View(objResult.ResultData);

            }

        }


    }
}
