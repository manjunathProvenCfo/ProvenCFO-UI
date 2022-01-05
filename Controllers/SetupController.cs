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
    //abcd
    [CustomAuthenticationFilter]
    [Exception_Filters]
    public class SetupController : BaseController
    {
        // GET: Setup
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        [CheckSession]
        public ActionResult JobTitle()
        {
            try
            {
                using (SetupService obj = new SetupService())
                {
                    var objResult = obj.GetJobTitleList();
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
        [HttpPost]
        public ActionResult CreateJobTitle(JobTitleModel jobTitleModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SetupService obj = new SetupService())
                    {
                        JobTitleModel JobTitleVM = new JobTitleModel();
                        var LoginUserid = Session["UserId"].ToString();
                        var result = new JobTitleModel();

                        if (jobTitleModel.Id == 0)
                        {
                            var Existresult = obj.GetJobTitleByTitle(jobTitleModel.Title);
                            if (Existresult != null)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                return View("AddJobTitle", JobTitleVM);
                            }

                            result = obj.AddJobTitle(jobTitleModel.Title, jobTitleModel.JobCode, jobTitleModel.Status.ToString().Trim(), LoginUserid);
                            ViewBag.ErrorMessage = "Created";
                        }

                        else
                        {
                            var Existresult = obj.GetJobTitleByTitle(jobTitleModel.Title);
                            JobTitleVM.Id = jobTitleModel.Id;
                            JobTitleVM.Title = jobTitleModel.Title;
                            JobTitleVM.JobCode = jobTitleModel.JobCode;
                            JobTitleVM.Status = jobTitleModel.Status;
                            JobTitleVM.CreatedBy = jobTitleModel.CreatedBy;
                            JobTitleVM.CreatedDate = jobTitleModel.CreatedDate;
                            if (Existresult != null && Existresult.Id != jobTitleModel.Id)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                return View("AddJobTitle", JobTitleVM);
                            }
                            result = obj.UpdateJobTitle(Convert.ToString(jobTitleModel.Id), jobTitleModel.Title, jobTitleModel.JobCode, jobTitleModel.Status.ToString().Trim(), Convert.ToString(jobTitleModel.IsDeleted), LoginUserid);
                            ViewBag.ErrorMessage = "Updated";
                            return View("AddJobTitle", JobTitleVM);
                        }

                        if (result == null)
                            ViewBag.ErrorMessage = "";
                        //return RedirectToAction("Role");

                        return View("AddJobTitle", JobTitleVM);
                    }

                }
                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    //return RedirectToAction("Role");
                }
            }
            return View("AddJobTitle", jobTitleModel);
        }

        [CheckSession]
        public ActionResult UpdateJobTitle(JobTitleModel jobTitleModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SetupService obj = new SetupService())
                    {
                        var LoginUserid = Session["UserId"].ToString();
                        var result = obj.UpdateJobTitle(Convert.ToString(jobTitleModel.Id), jobTitleModel.Title, jobTitleModel.JobCode, Convert.ToString(jobTitleModel.Status), Convert.ToString(jobTitleModel.IsDeleted), LoginUserid);
                        if (result == null)
                            ViewBag.ErrorMessage = "";
                        return RedirectToAction("jobTitleModel");
                    }
                }
                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    return View();
                }
            }
            // return View();
            return JavaScript("AlertJobTitle");
        }

        [CheckSession]
        public ActionResult AddJobTitle()
        {
            try
            {
                JobTitleModel result = new JobTitleModel();
                return View(result);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                return View();
            }
        }

        [CheckSession]
        public ActionResult EditJobTitle(int? id)
        {
            try
            {
                using (SetupService service = new SetupService())
                {
                    JobTitleModel result = new JobTitleModel();

                    if (id > 0)
                    {
                        result = service.GetJobTitleList().ResultData.Where(w => w.Id == id).FirstOrDefault();
                    }
                    return View("AddJobTitle", result);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public ActionResult SaveJobTilte(JobTitleModel model)
        {
            try
            {
                using (SetupService service = new SetupService())
                {
                    var result = service.SaveJobTitle(model);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }            
        }

        [CheckSession]
        public string DeleteJobTitle(int id)
        {
            try
            {
                using (SetupService objJobTitle = new SetupService())
                {
                    var results = objJobTitle.GetJobUserRoleById(id);
                    if (results != null)
                    {
                        return results.Status;
                    }
                    else
                    {
                        var result = objJobTitle.DeleteJobTitle(id);
                        return (result.Status);
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
        public ActionResult DeactivateJobTitle(int Id)
        {
            try
            {
                using (SetupService obj = new SetupService())
                {
                    var result = obj.RetireJobTitle(Id);
                    if (result == null)
                        ViewBag.ErrorMessage = "Can't deacactivte";
                    return RedirectToAction("JobTitle");
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                ViewBag.ErrorMessage = "";
                //Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                return View();
            }
        }

        [CheckSession]
        public JsonResult ExportToExcel()
        {
            try
            {
                using (SetupService objsetup = new SetupService())
                {

                    Utltity obj = new Utltity();
                    var objResult = objsetup.GetJobTitleList().ResultData.Select(s => new
                    {
                        Job_Title_ID = s.Id,
                        Job_Title = s.Title,
                        Job_Code = s.JobCode,
                        Status = s.Status == "0" ? "Inactive" : "Active",
                        Created_By = s.CreatedByUser,
                        Created_Date = (s.CreatedDate.ToString("MM/dd/yyyy") == "01-01-0001" || s.CreatedDate.ToString("MM/dd/yyyy") == "01/01/0001") ? "" : s.CreatedDate.ToString("MM/dd/yyyy").Replace("-", "/"),
                        Modified_By = s.ModifiedByUser,
                        Modified_Date = (s.ModifiedDate.ToString("MM/dd/yyyy") == "01-01-0001" || s.ModifiedDate.ToString("MM/dd/yyyy") == "01/01/0001") ? "" : s.ModifiedDate.ToString("MM/dd/yyyy").Replace("-", "/")
                    }).ToList();
                    string filename = obj.ExportTOExcel("JobTitlesList", obj.ToDataTable(objResult));
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