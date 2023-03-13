using Azure.Storage.Sas;
using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    [Exception_Filters]
    
    public class ReportsController : BaseController
    {
        private string StorageAccountName = Convert.ToString(ConfigurationManager.AppSettings["StorageAccountName"]);
        private string StorageAccountKey = Convert.ToString(ConfigurationManager.AppSettings["StorageAccountKey"]);
        private string StorageConnection = Convert.ToString(ConfigurationManager.AppSettings["StorageConnection"]);
        private string StorageContainerName = Convert.ToString(ConfigurationManager.AppSettings["StorageContainerName"]);
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Reports
        [CheckSession]
        public ActionResult ReportsList()
        {
            List<UserSecurityVM> _roleList = (List<UserSecurityVM>)Session["LoggedInUserUserSecurityModels"];
            if (_roleList.Where(x => x.FeatureCode != "RPT").ToList().Count == _roleList.Count())
            {
                return RedirectToAction("AgencyHome", "AgencyService");
            }

            IsReadOnlyUser();
            var userType = Convert.ToString(Session["UserType"]);
            if (userType == "1")
            {
                ViewBag.IsStaffUser = true;
            }
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

                    string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                    string fileExtension = Path.GetExtension(file.FileName);
                    string folderPath = $"reports/{year}/{periodType}/"; //$"~/UploadedFiles/Reports/{year}/{periodType}/"

                    string filePath = folderPath + $"{guid}_{fileName}{fileExtension}";

                    ReportsVM reportsVM = new ReportsVM();
                    reportsVM.AgencyId_Ref = agencyId;
                    reportsVM.FileName = fileName;
                    reportsVM.FileGuid = guid;
                    reportsVM.FilePath = filePath;
                    reportsVM.FileExtention = fileExtension;
                    reportsVM.Year = year;
                    reportsVM.PeriodType = periodType;
                    reportsVM.Status = "Active";
                    reportsVM.CreatedBy = LoginUserid;
                    reportsVM.CreatedDate = DateTime.Now;
                    reportsVM.ModifiedDate = null;

                    var StorageAccountName = Convert.ToString(ConfigurationManager.AppSettings["StorageAccountName"]);
                    var StorageAccountKey = Convert.ToString(ConfigurationManager.AppSettings["StorageAccountKey"]);
                    var StorageConnection = Convert.ToString(ConfigurationManager.AppSettings["StorageConnection"]);
                    var StorageContainerName = Convert.ToString(ConfigurationManager.AppSettings["StorageContainerName"]);
                    using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
                    {
                        var result = storage.UploadFile(StorageContainerName, folderPath, $"{guid}_{fileName}{fileExtension}", file);
                        if (result == true)
                        {
                            using (ReportsService reportsService = new ReportsService())
                            {
                                var report = reportsService.SaveReport(reportsVM);
                                return Json(new { File = report, Status = "Success", Message = "" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    return Json(new { File = "", Status = "Failed", Message = "File Not created" }, JsonRequestBehavior.AllowGet);

                    //using (ReportsService reportsService = new ReportsService())
                    //{
                    //    var report = reportsService.SaveReport(reportsVM);
                    //    Common.CreateDirectory(Server.MapPath(folderPath));
                    //    file.SaveAs(Server.MapPath(report.FilePath));
                    //    if (System.IO.File.Exists(Server.MapPath(report.FilePath)))
                    //    {
                    //        return Json(new { File = report, Status = "Success", Message = "" }, JsonRequestBehavior.AllowGet);
                    //    }
                    //    return Json(new { File = report, Status = "Failed", Message = "File Not created" }, JsonRequestBehavior.AllowGet);
                    //}
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
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
        [HttpGet]
        [Route("Reports/GetReportResource/{type}/{year}/{month}/{azure_fileName}/{fileName}")]
        public void GetReportResource(string type, string year, string month, string azure_fileName, string fileName)
     
        {
            azure_fileName = azure_fileName.Replace("|and|", "&");
            fileName = fileName.Replace("|and|", "&");
            var path = $"{type}/{year}/{month}/{azure_fileName}";
            using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
            {
                long readedByteChunk = 0;
                int byteChunkRemain = 0;

                Stream azureStream = storage.GetFileStream(StorageContainerName, path, fileName);
                if (azureStream != null)
                {
                    Response.ContentType = System.Web.MimeMapping.GetMimeMapping(path);

                    byte[] bytes = new byte[azureStream.Length + 1];

                    azureStream.Seek(0, SeekOrigin.Begin);
                    byteChunkRemain = (int)azureStream.Length;

                //for handling large files
                readChunk:
                    readedByteChunk += azureStream.Read(bytes, (int)readedByteChunk, byteChunkRemain);
                    if (0 < (azureStream.Length - readedByteChunk))
                    {
                        byteChunkRemain = (int)(azureStream.Length - readedByteChunk);
                        goto readChunk;
                    }
                    azureStream.Close();
                    Response.BinaryWrite(bytes);
                }
                else
                {
                    Response.StatusCode = 404;
                }
            }
        }

        [CheckSession]
        [HttpGet]

        public FileContentResult DownloadReportResource(string path, string fileName, string fileExtention)
        {


            using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
            {
                long readedByteChunk = 0;
                int byteChunkRemain = 0;

                Stream azureStream = storage.GetFileStream(StorageContainerName, path, fileName);
                Response.ContentType = System.Web.MimeMapping.GetMimeMapping(path);

                byte[] bytes = new byte[azureStream.Length + 1];

                azureStream.Seek(0, SeekOrigin.Begin);
                byteChunkRemain = (int)azureStream.Length;

            //for handling large files
            readChunk:
                readedByteChunk += azureStream.Read(bytes, (int)readedByteChunk, byteChunkRemain);
                if (0 < (azureStream.Length - readedByteChunk))
                {
                    byteChunkRemain = (int)(azureStream.Length - readedByteChunk);
                    goto readChunk;
                }
                azureStream.Close();


               // Response.BinaryWrite(bytes);

                return File(bytes, Response.ContentType, fileName + fileExtention);

            }
        }
        [CheckSession]
        public JsonResult GetReports(int agencyId, int year, string periodType)
        {
            try
            {
                using (ReportsService reportsService = new ReportsService())
                {

                    using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
                    {
                        var reports = reportsService.GetReports(agencyId, year, periodType);

                        foreach (var item in reports)
                        {
                            item.FileName = item.FileName; //item.FileName.Split('_').Length>1? item.FileName.Split('_')[1]:item.FileName;
                            item.DownloadFileLink = $@"/Reports/GetReportResource/{item.FilePath}/{item.FileName}";
                        }
                        return Json(new { Data = reports, Status = "Success", Message = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [CheckSession]
        public JsonResult GetIsReceiveQuarterlyReportEnable(int agencyId)
        {
            try
            {
                var IsReceiveQuarterlyReports = false;
                using (ClientService objClientService = new ClientService())
                {
                    CreateClientVM Clientvm = new CreateClientVM();
                    var client = objClientService.GetClientById(agencyId);
                    IsReceiveQuarterlyReports = client.ReceiveQuarterlyReports;
                }
                return Json(new { Data = IsReceiveQuarterlyReports, Status = "Success" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSession]
        public async Task<JsonResult> GetDashboardReports(int agencyId)
        {
            try
            {
                using (ReportsService reportsService = new ReportsService())
                {
                    using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
                    {
                        var reports = await reportsService.GetDashboardReports(agencyId);
                        foreach (var rpt in reports)
                        {
                            rpt.FileName = rpt.FileName.Replace(".", "_"); //Split('_').Length > 1 ? rpt.FileName.Split('_')[1] : rpt.FileName;
                            rpt.DownloadFileLink = $@"/Reports/GetReportResource/{rpt.FilePath}/{rpt.FileName}";

                        }


                        return Json(new { DataMonthly = reports.Where(x => x.PeriodType != "YearEnd"), DataYearly = reports.Where(x => x.PeriodType == "YearEnd"), Status = "Success", Message = "" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
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
                            using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
                            {
                                var filepath = storage.GetFileSasUri(StorageContainerName, report.FilePath, DateTime.UtcNow.AddMinutes(1), ShareFileSasPermissions.Read).AbsoluteUri;
                               var stream = storage.GetFileStream(StorageContainerName, report.FilePath, report.FileName);
                                var path = Server.MapPath("~/assets/tempReports/")+report.AgencyId_Ref+"\\"+report.FileName+report.FileExtention;
                                if(!Directory.Exists(Server.MapPath("~/assets/tempReports/") + report.AgencyId_Ref + "\\"))
                                {
                                    Directory.CreateDirectory(Server.MapPath("~/assets/tempReports/") + report.AgencyId_Ref + "\\");

                                }
                                var tempStream = System.IO.File.Create(path);
                                stream.Seek(0,SeekOrigin.Begin);
                                stream.CopyTo(tempStream);
                                stream.Close();
                                tempStream.Close();
                                ziparchive.CreateEntryFromFile(path,report.FileName+report.FileExtention);      
                            }

                            DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/assets/tempReports/")+report.AgencyId_Ref+"\\"); 
                            FileInfo[] files = di.GetFiles();
                            foreach (FileInfo file in files)
                            {
                                file.Delete();
                            }
                            di.Delete();
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
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
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
                        return Json(new { resultData = true, message = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSession]
        [HttpPost]
        public JsonResult Delete(int[] Ids)
        {
            try
            {

                using ( ReportsService reportsService = new ReportsService())
                {
                    var rptlist = reportsService.GetReportByIds(Ids).ResultData;
                    if (rptlist != null)
                    {
                        using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
                        {
                            foreach (var rpt in rptlist)
                            {
                                var IsDelted = storage.DeleteAzureFiles(StorageContainerName, rpt.FilePath, null);
                            }
                            var result = reportsService.Delete(Ids).ResultData;
                            if (result == true)
                            {
                                return Json(new { resultData = true, message = "success" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                    }
                    return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSession]
        [HttpPost]
        public JsonResult MakeItMonthlySummary(int Id, int Year, string PeriodType, int AgencyId)
        {
            try
            {
                using (ReportsService reportsService = new ReportsService())
                {
                    var result = reportsService.MakeItMonthlySummary(Id, Year, PeriodType, AgencyId);

                    if (result != null)
                    {
                        return Json(new { resultData = true, message = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        [CheckSession]
        [HttpPost]
        [Obsolete]
        public JsonResult Rename(int Id, string FileName)
        {
            try
            {
                int[] Ids = new int[] { Id };
                var guid = Guid.NewGuid().ToString();
                string NewfilePath = $"{guid}_{FileName}";
                using (ReportsService reportsService = new ReportsService())
                {
                    using (IStorageAccess storage = new AzureStorageAccess(StorageAccountName, StorageAccountKey, StorageConnection))
                    {
                        var rptlist = reportsService.GetReportByIds(Ids).ResultData;
                        if (rptlist != null && rptlist.Count() > 0)
                        {
                            var dbf_File = System.IO.Path.GetFileName(rptlist[0].FilePath);
                            NewfilePath += rptlist[0].FileExtention;
                            storage.RenameAzureFiles(StorageContainerName, rptlist[0].FilePath, dbf_File, NewfilePath);
                        }
                    }
                    var result = reportsService.Rename(Id, HttpUtility.UrlEncodeUnicode(FileName), HttpUtility.UrlEncodeUnicode(NewfilePath));

                    if (result != null)
                    {
                        return Json(new { resultData = true, message = "success" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new { resultData = false, message = "error" }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult EmailSend(string ClientName, string ClientId, string url, string sentdate)
        {
            try
            {
                using (AccountService obj = new AccountService())
                {
                    List<InviteUserModel> user = new List<InviteUserModel>();
                    var usersListwithRecPermission = obj.GetRegisteredUsersByAgencyWithReqPermission(ClientId, "RPT");
                    var Userslist = usersListwithRecPermission.ResultData;
                    var Recipientssdata = Userslist.Where(x => x.IsRegistered == 1 && x.IsActive == 1.ToString()).Select(x => x.Email);

                    XmlDocument doc = new XmlDocument();
                    doc.Load(Server.MapPath("~/assets/files/ReportEmailTemplate.xml"));

                    string xml = System.IO.File.ReadAllText(Server.MapPath("~/assets/files/ReportEmailTemplate.xml"));
                    var subject = doc.SelectNodes("EmailContent/subject")[0].InnerText;
                    var body = doc.SelectNodes("EmailContent/body")[0].InnerText;
                    var footer = doc.SelectNodes("EmailContent/footer")[0].InnerText;
                    subject = subject.Replace("{CompanyName}", ClientName);
                    subject = subject.Replace("{TodaysDate}", DateTime.Now.ToString("dd MMMM, yyyy", new System.Globalization.CultureInfo("en-US")));
                    var notes = "https://" + url + "/Notes/GetNotesPage";
                    var dashboard = "https://" + url + "/Dashboard/Dashboard";
                    var report = "https://" + url + "/Reports/ReportsList";
                    var login = "https://" + url + "/Home/Login";
                    var reconcilation = "https://" + url + "/Reconciliation/ReconciliationMain";
                    var chat = "https://" + url + "/Communication/Chat";
                    body = body.Replace("{notes}", notes);
                    body = body.Replace("{dashboard}", dashboard);
                    body = body.Replace("{report}", report);
                    body = body.Replace("{login}", login);
                    body = body.Replace("{reconcilation}", reconcilation);
                    body = body.Replace("{chat}", chat);
                    footer = sentdate != "null" ? footer.Replace("{LastSent}", sentdate) : "";
                    return Json(new { Subject = subject, Body = body, Recipients = Recipientssdata, Status = "Success", LastSent = footer }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {

                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return Json(new
                {
                    File = "",
                    Status = "Error",
                    Message = ex.Message
                }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}