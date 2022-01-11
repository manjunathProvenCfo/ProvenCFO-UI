using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    [Exception_Filters]
    public class TeamsController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Teams
        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        public ActionResult TeamsList()
        {
            try
            {
                using (TeamsService obj = new TeamsService())
                {
                    var objResult = obj.GetTeamsList();
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
        [HttpGet]
        public ActionResult AddTeams()
        {
            try
            {
                TeamsVM result = new TeamsVM();
                using (AccountService obj = new AccountService())
                {
                    result.StaffList = obj.GetRegisteredStaffUserList().resultData;
                }
                return View(result);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public ActionResult EditTeam(int Id)
        {
            try
            {
                using (TeamsService obj = new TeamsService())
                {
                    using (AccountService objAcc = new AccountService())
                    {
                        TeamsVM objvm = new TeamsVM();
                        objvm.StaffList = objAcc.GetRegisteredStaffUserList().resultData;
                        objvm.StaffList = objvm.StaffList;
                        var result = obj.GetTeamsById(Id);
                        objvm.Id = result.Id;
                        objvm.TeamName = result.TeamName;
                        objvm.TeamMemberId1 = result.TeamMemberId1;
                        objvm.TeamMembersList = result.TeamMembersList;
                        objvm.TeamMemberId2 = result.TeamMemberId2;
                        objvm.TeamMemberId3 = result.TeamMemberId3;
                        objvm.Status = result.Status/*.ToString().Trim()*/;
                        objvm.CreatedBy = result.CreatedBy;
                        objvm.CreatedDate = result.CreatedDate;
                        objvm.ModifiedBy = result.ModifiedBy;
                        objvm.ModifiedDate = result.ModifiedDate;
                        return View("AddTeams", objvm);
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
        public ActionResult CreateTeams(TeamsVM team)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (TeamsService obj = new TeamsService())
                    {
                        using (AccountService objAcc = new AccountService())
                        {
                            TeamsVM teamsVM = new TeamsVM();
                            var LoginUserid = Session["UserId"].ToString();
                            var result = new TeamsVM();
                            team.StaffList = objAcc.GetRegisteredStaffUserList().resultData;
                            teamsVM.StaffList = team.StaffList;
                            if (team.Id == 0)
                            {
                                var Existresult = obj.GetTeamsByName(team.TeamName);
                                if (Existresult != null)
                                {
                                    ViewBag.ErrorMessage = "Exist";
                                    return View("AddTeams", teamsVM);
                                }
                                result = obj.AddTeam(team.TeamName, team.Status.ToString().Trim(), team.TeamMemberId1, team.TeamMemberId2, team.TeamMemberId3, LoginUserid);
                                result.StaffList = objAcc.GetRegisteredStaffUserList().resultData;
                                result.Id = 0;

                                ViewBag.ErrorMessage = "Created";
                            }
                            else
                            {
                                var Existresult = obj.GetTeamsByName(team.TeamName);
                                teamsVM.Id = team.Id;
                                teamsVM.TeamName = team.TeamName;
                                teamsVM.Status = team.Status;
                                teamsVM.CreatedBy = team.CreatedBy;
                                teamsVM.CreatedDate = team.CreatedDate;
                                teamsVM.IsDeleted = team.IsDeleted;

                                if (Existresult != null && Existresult.Id != team.Id)
                                {
                                    ViewBag.ErrorMessage = "Exist";
                                    return View("AddTeams", teamsVM);
                                }
                                result = obj.UpdateTeam(Convert.ToString(team.Id), team.TeamName, team.TeamMemberId1, team.TeamMemberId2, team.TeamMemberId3, team.Status.ToString().Trim(), Convert.ToString(team.IsDeleted), LoginUserid);
                                result.StaffList = objAcc.GetRegisteredStaffUserList().resultData;
                                result.Id = 0;
                                ViewBag.ErrorMessage = "Updated";
                                return View("AddTeams", teamsVM);
                            }
                            if (result == null)
                                ViewBag.ErrorMessage = "";

                            return View("AddTeams", teamsVM);
                        }

                    }
                }

                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    return View();
                }
            }
            return View("AddTeams", team);
        }

        [CheckSession]
        public ActionResult UpdateTeam(TeamsVM team)
        {
            try
            {
                using (AccountService objAcc = new AccountService())
                {
                    TeamsVM results = new TeamsVM();
                    team.StaffList = objAcc.GetRegisteredStaffUserList().resultData;
                    results.StaffList = team.StaffList;
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            var LoginUserid = Session["UserId"].ToString();
                            using (TeamsService obj = new TeamsService())
                            {
                                var result = obj.UpdateTeam(Convert.ToString(team.Id), team.TeamName, team.Status, team.TeamMemberId1, team.TeamMemberId2, team.TeamMemberId3, Convert.ToString(team.IsDeleted), LoginUserid);
                                if (result == null)
                                    ViewBag.ErrorMessage = "";
                                return RedirectToAction("TeamsList");
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error(Utltity.Log4NetExceptionLog(ex));
                            return View();
                        }
                    }
                    return JavaScript("AlertTeam");
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        [CheckSession]
        public string DeleteTeam(int Id)
        {
            try
            {
                using (TeamsService objTeams = new TeamsService())
                {
                    var results = objTeams.GetTeamClientById(Id);
                    if (results != null)
                    {
                        return results.Status;
                    }
                    else
                    {
                        var result = objTeams.DeleteTeams(Id);
                        return result.Status;
                        //if (result == null)
                        //    ViewBag.ErrorMessage = "Can't Delete"; 
                    }
                    return results.Status;
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }            
        }

        [CheckSession]
        public ActionResult DeleteTeamUserAssociation(int Id)
        {
            try
            {
                using (TeamsService obj = new TeamsService())
                {
                    var result = obj.DeleteTeamUserAssociation(Id);
                    if (result == null)
                        ViewBag.ErrorMessage = "Can't Delete";

                    return RedirectToAction("TeamsList");
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                ViewBag.ErrorMessage = "";
                return View();
            }
        }

        [CheckSession]
        public ActionResult DeactivateTeams(int Id)
        {
            try
            {
                using (TeamsService obj = new TeamsService())
                {
                    var result = obj.RetireTeams(Id);
                    if (result == null)
                        ViewBag.ErrorMessage = "Can't deacactivte";
                    return RedirectToAction("TeamsList");
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                ViewBag.ErrorMessage = "";
                return View();
            }
        }

        [CheckSession]
        public JsonResult ExportToExcel()
        {
            try
            {
                using (TeamsService objsetup = new TeamsService())
                {
                    Utltity obj = new Utltity();
                    var objResult = objsetup.GetTeamsList().ResultData.Select(s => new
                    {
                        Team_Id = s.Id,
                        Team_Name = s.TeamName,
                        Status = s.Status,
                        Created_By = s.CreatedByUser,
                        Created_Date = s.CreatedDate.HasValue == false || (((DateTime)s.CreatedDate).ToString("MM/dd/yyyy") == "01-01-0001" || ((DateTime)s.CreatedDate).ToString("MM/dd/yyyy") == "01/01/0001") ? "" : ((DateTime)s.CreatedDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                        Modified_By = s.ModifiedByUser,
                        Modified_Date = s.ModifiedDate.HasValue == false || (((DateTime)s.ModifiedDate).ToString("MM/dd/yyyy") == "01-01-0001" || ((DateTime)s.ModifiedDate).ToString("MM/dd/yyyy") == "01/01/0001") ? "" : ((DateTime)s.ModifiedDate).ToString("MM/dd/yyyy").Replace("-", "/")
                    }).ToList();
                    string filename = obj.ExportTOExcel("TeamsList", obj.ToDataTable(objResult));
                    return Json(filename, JsonRequestBehavior.AllowGet);
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