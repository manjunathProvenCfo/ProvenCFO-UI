using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
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
            IsReadOnlyUser();
            return View();
        }

        [CheckSession]
        [HttpPost]
        public JsonResult UploadReportAndSave(HttpPostedFileBase file, int agencyId, int year, string periodType)
        {
            try
            {
                periodType = periodType.Trim();
                periodType = char.ToUpper(periodType[0]) + periodType.Substring(1);

                if (file.ContentLength > 0)
                {
                    var LoginUserid = User.UserId;
                    var guid = Guid.NewGuid().ToString();

                    string fileName = Path.GetFileName(file.FileName);
                    string folderPath = $"~/UploadedFiles/Reports/{year}/{periodType}/";

                    string filePath = folderPath + $"{guid}_{fileName}";

                    ReportsVM reportsVM = new ReportsVM();
                    reportsVM.AgencyId_Ref = agencyId;
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
                        return Json(new { File = report, Status = "Success", Message = "" }, JsonRequestBehavior.AllowGet);
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
            return Json(new { File = "", Status = ViewBag.ErrorMessage, Message = "Report upload failed!!" }, JsonRequestBehavior.AllowGet);
        }
        [CheckSession]
        public JsonResult GetReports(int agencyId, int year, string periodType)
        {
            try
            {
                using (ReportsService reportsService = new ReportsService())
                {
                    var reports = reportsService.GetReports(agencyId, year, periodType);
                    return Json(new { Data = reports, Status = "Success", Message = "" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [CheckSession]
        public FileContentResult DownloadAll(int agencyId, int year, string periodType)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var ziparchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    using (ReportsService reportsService = new ReportsService())
                    {
                        var reports = reportsService.GetReports(agencyId, year, periodType);

                        foreach (var report in reports)
                        {
                            ziparchive.CreateEntryFromFile(Server.MapPath(report.FilePath), report.FileName);
                        }
                    }
                }
                return File(memoryStream.ToArray(), "application/zip", $"Reports_{periodType}_{year}.zip");
            }

        }

        [CheckSession]
        [HttpPost]
        public JsonResult SoftDeleteFile(int Id, string PeriodType)
        {
            try
            {
                using (ReportsService reportsService = new ReportsService())
                {
                    var apiResult = reportsService.SoftDeleteFile(Id);
                    if (apiResult.Status)
                        return Json(new { Status = "Success", PeriodType = PeriodType }, JsonRequestBehavior.AllowGet);
                    else
                        return Json(new { Status = "Error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSession]
        [HttpPost]
        public JsonResult UpdatePositions(int[] Ids, int[] Positions)
        {
            try
            {
                using (ReportsService reportsService = new ReportsService())
                {
                    var result = reportsService.UpdatePositions(Ids, Positions);

                    if (result != null)
                    {
                        return Json(new { notes = result, message = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { notes = result, message = "error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
            //return Json(new { Message = "Error" }, JsonRequestBehavior.AllowGet);
        }
    }
}