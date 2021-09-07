using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class ReportsController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Reports
        [CheckSession]
        public ActionResult ReportsList()
        {
            return View();
        }

        [HttpPost]
        public JsonResult UploadReportAndSave(HttpPostedFileBase file, int agentId, int year, string periodType)
        {
            try
            {

                if (file.ContentLength > 0)
                {
                    var LoginUserid = User.UserId;
                    var guid = Guid.NewGuid().ToString();

                    string fileName = Path.GetFileName(file.FileName);
                    string folderPath = $"~/UploadedFiles/Reports/{year}/{periodType}/";
                    
                    string filePath = folderPath + $"{guid}_{fileName}";

                    ReportsVM reportsVM = new ReportsVM();
                    reportsVM.AgencyId_Ref = agentId;
                    reportsVM.FileName = fileName;
                    reportsVM.FileGuid = guid;
                    reportsVM.FilePath = filePath;
                    reportsVM.FileExtention = Path.GetExtension(fileName);
                    reportsVM.Year = year;
                    reportsVM.PeriodType = periodType;
                    reportsVM.Status = "Active";
                    reportsVM.CreatedBy = LoginUserid;
                    reportsVM.CreatedDate = DateTime.Now;
                    reportsVM.ModifiedDate = null;

                    using (ReportsService reportsService = new ReportsService())
                    {
                        var report = reportsService.SaveReport(reportsVM);
                        Common.CreateDirectory(Server.MapPath(folderPath));
                        file.SaveAs(Server.MapPath(report.FilePath));
                        return Json(new { File = report.FilePath, Status = "Success", Message = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                ViewBag.ErrorMessage = "Error";
                return Json(new
                {
                    File = "",
                    Status = ViewBag.ErrorMessage,
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { File = "", Status = ViewBag.ErrorMessage, Message = "File upload failed!!" }, JsonRequestBehavior.AllowGet);
        }
    }
}