using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class RoleController : Controller
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);        
        // GET: Role
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        [CheckSession]
        public ActionResult Role()
        {
            try
            {
                using (RoleService objRole = new RoleService())
                {
                    var objResult = objRole.GetRoles();
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
        public ActionResult AddRole()
        {
            try
            {
                Models.RolesViewModel result = new Models.RolesViewModel();
                return View(result);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }
        //[HttpGet]
        [CheckSession]
        public ActionResult EditRole(string id)
        {
            try
            {
                using (RoleService objRole = new RoleService())
                {
                    ProvenCfoUI.Models.RolesViewModel objvm = new ProvenCfoUI.Models.RolesViewModel();
                    var result = objRole.GetRoleById(id);
                    objvm.id = result.id;
                    objvm.name = result.name;
                    objvm.Status = result.Status.ToString().Trim();
                    objvm.CreatedBy = result.CreatedBy;
                    objvm.CreatedDate = result.CreatedDate;
                    objvm.ModifiedBy = result.ModifiedBy;
                    objvm.ModifiedDate = result.ModifiedDate;
                    return View("AddRole", objvm);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public ActionResult DeactivateRole(string id, string Status)
        {
            try
            {
                using (RoleService objRole = new RoleService())
                {
                    ProvenCfoUI.Models.RolesViewModel objvm = new ProvenCfoUI.Models.RolesViewModel();
                    var LoginUserid = Session["UserId"].ToString();
                    var resultdata = objRole.GetRoleById(id);
                    var result = objRole.UpdateRoles(resultdata.id, resultdata.name, Status, LoginUserid);
                    if (result == null)
                        ViewBag.ErrorMessage = "";
                    return RedirectToAction("Role");
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
        public ActionResult CreateRole(Proven.Model.RolesViewModel Role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RoleService objRole = new RoleService())
                    {
                        Models.RolesViewModel RoleVM = new Models.RolesViewModel();
                        var LoginUserid = Session["UserId"].ToString();
                        var result = new Proven.Model.RolesViewModel();

                        if (Role.id == null)
                        {
                            var Existresult = objRole.GetRoleByName(Role.name);
                            if (Existresult != null)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                return View("AddRole", RoleVM);
                            }

                            result = objRole.AddRoles(Role.name, Role.Status.ToString().Trim(), LoginUserid);
                            ViewBag.ErrorMessage = "Created";
                        }
                        else
                        {
                            var Existresult = objRole.GetRoleByName(Role.name);
                            RoleVM.id = Role.id;
                            RoleVM.name = Role.name;
                            RoleVM.Status = Role.Status;
                            RoleVM.CreatedBy = Role.CreatedBy;
                            RoleVM.CreatedDate = Role.CreatedDate;
                            if (Existresult != null && Existresult.id != Role.id)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                return View("AddRole", RoleVM);
                            }
                            result = objRole.UpdateRoles(Role.id, Role.name, Role.Status.ToString().Trim(), LoginUserid);
                            ViewBag.ErrorMessage = "Updated";
                            return View("AddRole", RoleVM);
                        }
                        if (result == null)
                            ViewBag.ErrorMessage = "";
                        //return RedirectToAction("Role");
                        return View("AddRole", RoleVM);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    //return RedirectToAction("Role");
                }

            }
            return View("AddRole", Role);

        }

        [CheckSession]
        public ActionResult UpdateRole(Proven.Model.RolesViewModel Role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RoleService objRole = new RoleService())
                    {
                        var LoginUserid = Session["UserId"].ToString();
                        var result = objRole.UpdateRoles(Role.id, Role.name, Role.Status, LoginUserid);
                        if (result == null)
                            ViewBag.ErrorMessage = "";
                        return RedirectToAction("Role");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    return View();
                }

            }
            // return View();
            return JavaScript("AlertRole");
        }
        //[HttpPut]
        [CheckSession]
        public ActionResult DeleteRole(string id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RoleService objRole = new RoleService())
                    {
                        var results = objRole.GetUserRoleById(id);
                        if (results != null)
                        {
                            return RedirectToAction("Role");
                        }
                        else
                        {
                            var result = objRole.DeleteRoles(id);
                            if (result == null)
                                ViewBag.ErrorMessage = "";
                            return RedirectToAction("Role");
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
        public JsonResult ExportToExcel()
        {
            try
            {
                using (RoleService objRole = new RoleService())
                {
                    Utltity obj = new Utltity();
                    var objResult = objRole.GetRoles().ResultData.Select(s => new
                    {
                        User_Role = s.name,
                        Status = s.Status,
                        Created_By = s.CreatedByUser,
                        Created_Date = (s.CreatedDate.ToString("MM/dd/yyyy") == "01-01-0001" || s.CreatedDate.ToString("MM/dd/yyyy") == "01/01/0001") ? "" : s.CreatedDate.ToString("MM/dd/yyyy").Replace("-", "/"),
                        Modified_By = s.ModifiedByUser,
                        Modified_Date = (s.ModifiedDate.ToString("MM/dd/yyyy") == "01-01-0001" || s.ModifiedDate.ToString("MM/dd/yyyy") == "01/01/0001") ? "" : s.ModifiedDate.ToString("MM/dd/yyyy").Replace("-", "/")
                    }).ToList();
                    string filename = obj.ExportTOExcel("RolesList", obj.ToDataTable(objResult));
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
        protected bool CheckDate(String date)
        {
            try
            {
                DateTime Temp;
                if (DateTime.TryParse(date, out Temp) == true)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
            
        }


    }







}
