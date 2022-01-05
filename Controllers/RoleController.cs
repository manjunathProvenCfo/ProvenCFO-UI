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
    [Exception_Filters]
    public class RoleController : BaseController
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
                    var result = objResult.ResultData.Where(x => x.IsVisible == true).ToList();
                    return View(result);
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
                using (RoleService objRole = new RoleService())
                {
                    ProvenCfoUI.Models.RolesViewModel result = new ProvenCfoUI.Models.RolesViewModel();
                    result.UserType = 2;
                    result.MasterFeaturesList = objRole.GetMasterFeatures().ResultData.OrderBy(x => x.Id).ToList();
                    TempData["UserTypeList"] = objRole.GetUserTypes().ResultData;
                    return View(result);
                }
               
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
                    objvm.MasterFeaturesList = objRole.GetMasterFeatures().ResultData;
                    TempData["UserTypeList"] = objRole.GetUserTypes().ResultData;
                    var result = objRole.GetRoleById(id);
                    objvm.Id = result.Id;
                    objvm.Name = result.Name;
                    objvm.Status = result.Status.ToString().Trim();
                    objvm.CreatedBy = result.CreatedBy;
                    objvm.CreatedDate = result.CreatedDate;
                    objvm.ModifiedBy = result.ModifiedBy;
                    objvm.ModifiedDate = result.ModifiedDate;
                    objvm.DisplayRoleName = result.DisplayRoleName;
                    objvm.IsVisible = result.IsVisible;
                    objvm.UserType = result.UserType;
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
                    objvm.MasterFeaturesList = objRole.GetMasterFeatures().ResultData;
                    var LoginUserid = Session["UserId"].ToString();
                    var resultdata = objRole.GetRoleById(id);
                    var result = objRole.UpdateRoles(resultdata.Id, resultdata.Name, Status, LoginUserid, resultdata.DisplayRoleName, resultdata.UserType.Value);
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
        public ActionResult CreateRole(ProvenCfoUI.Models.RolesViewModel Role)
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

                        if (Role.Id == null)
                        {
                            TempData["UserTypeList"] = objRole.GetUserTypes().ResultData;
                            RoleVM.MasterFeaturesList = objRole.GetMasterFeatures().ResultData;
                            var Existresult = objRole.GetRoleByName(Role.Name);
                            if (Existresult != null)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                
                                return View("AddRole", RoleVM);
                            }

                            result = objRole.AddRoles(Role.Name, Role.Status.ToString().Trim(), LoginUserid, Role.DisplayRoleName,Role.UserType.Value);
                            ViewBag.ErrorMessage = "Created";
                        }
                        else
                        {
                            TempData["UserTypeList"] = objRole.GetUserTypes().ResultData;
                            RoleVM.MasterFeaturesList = objRole.GetMasterFeatures().ResultData;
                            var Existresult = objRole.GetRoleByName(Role.Name);
                            RoleVM.Id = Role.Id;
                            RoleVM.Name = Role.Name;
                            RoleVM.Status = Role.Status;
                            RoleVM.CreatedBy = Role.CreatedBy;
                            RoleVM.CreatedDate = Role.CreatedDate;
                            RoleVM.DisplayRoleName = Role.DisplayRoleName;
                            RoleVM.UserType = Role.UserType;
                            if (Existresult != null && Existresult.Id != Role.Id)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                return View("AddRole", RoleVM);
                            }
                            result = objRole.UpdateRoles(Role.Id, Role.Name, Role.Status.ToString().Trim(), LoginUserid, Role.DisplayRoleName, Role.UserType.Value);
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
        public ActionResult UpdateRole(ProvenCfoUI.Models.RolesViewModel Role)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (RoleService objRole = new RoleService())
                    {
                        var LoginUserid = Session["UserId"].ToString();
                        var result = objRole.UpdateRoles(Role.Id, Role.Name, Role.Status, LoginUserid, Role.DisplayRoleName,Role.UserType.Value);
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
        public string DeleteRole(string id)
        {
            try
            {
                using (RoleService objRole = new RoleService())
                {
                    var results = objRole.GetUserRoleById(id);
                    if (results != null)
                    {
                        return results.Status;
                    }
                    else
                    {
                        var result = objRole.DeleteRoles(id);
                        return result.Status;
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
        public JsonResult ExportToExcel()
        {
            try
            {
                using (RoleService objRole = new RoleService())
                {
                    Utltity obj = new Utltity();
                    var objResult = objRole.GetRoles().ResultData.Select(s => new
                    {
                        User_Role = s.Name,
                        Status = s.Status,
                        Created_By = s.CreatedByUser,
                        Created_Date = (s.CreatedDate.ToString("MM/dd/yyyy") == "01-01-0001" || s.CreatedDate.ToString("MM/dd/yyyy") == "01/01/0001") ? "" : s.CreatedDate.ToString("MM/dd/yyyy").Replace("-", "/"),
                        Modified_By = s.ModifiedByUser,
                        Modified_Date = (s.ModifiedDate.ToString("MM/dd/yyyy") == "01-01-0001" || s.ModifiedDate.ToString("MM/dd/yyyy") == "01/01/0001") ? "" : s.ModifiedDate.ToString("MM/dd/yyyy").Replace("-", "/"),
                        DisplayRoleName = s.DisplayRoleName
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
