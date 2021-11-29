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
    public class BillableEntitiesController : BaseController
    {
        string errorMessage = string.Empty;
        string errorDescription = string.Empty;
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: BillableEntities

        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        public ActionResult GetAllBillableEntitiesList()
        {
            try
            {
                using (BillableEntitiesService obj = new BillableEntitiesService())
                {
                    var objResult = obj.GetAllBillableEntitiesList();

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
        public ActionResult AddBillableEntity()
        {
            try
            {
                BillableEntitiesVM result = new BillableEntitiesVM();
                return View(result);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        [CheckSession]
        public ActionResult EditBillableEntity(int Id)
        {
            try
            {
                using (BillableEntitiesService obj = new BillableEntitiesService())
                {
                    BillableEntitiesVM objvm = new BillableEntitiesVM();
                    var result = obj.GetBillableEntitiesById(Id);
                    objvm.Id = result.Id;
                    objvm.EntityName = result.EntityName;
                    objvm.XeroId = result.XeroId;
                    objvm.ProvenCFOXeroContactID = result.ProvenCFOXeroContactID;
                    objvm.CrmId = result.CrmId;
                    objvm.XeroShortCode = result.XeroShortCode;
                    objvm.Status = result.Status;
                    objvm.CreatedBy = result.CreatedBy;
                    objvm.CreatedDate = result.CreatedDate;
                    objvm.ModifiedBy = result.ModifiedBy;
                    objvm.ModifiedDate = result.ModifiedDate;
                    return View("AddBillableEntity", objvm);
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
        public ActionResult CreateBillableEntity(BillableEntitiesVM bill)
        {
            ModelState.Remove("Clients");
            if (ModelState.IsValid)
            {
                try
                {
                    using (BillableEntitiesService obj = new BillableEntitiesService())
                    {
                        BillableEntitiesVM billsVM = new BillableEntitiesVM();
                        var LoginUserid = Session["UserId"].ToString();
                        var result = new BillableEntitiesVM();
                        if (bill.Id == 0)
                        {
                            var Existresult = obj.GetBillableEntitiesByName(bill.EntityName);
                            if (Existresult != null)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                return View("AddBillableEntity", billsVM);
                            }
                            result = obj.AddBillableEntity(bill.EntityName, bill.XeroId, bill.ProvenCFOXeroContactID, bill.CrmId, bill.XeroShortCode, bill.Clients, bill.Status.ToString().Trim(), LoginUserid); ;
                            //result.Id = 0;
                            ViewBag.ErrorMessage = "Created";
                        }
                        else
                        {
                            var Existresult = obj.GetBillableEntitiesByName(bill.EntityName);
                            billsVM.Id = bill.Id;
                            billsVM.EntityName = bill.EntityName;
                            billsVM.XeroId = bill.XeroId;
                            billsVM.ProvenCFOXeroContactID = bill.ProvenCFOXeroContactID;
                            billsVM.CrmId = bill.CrmId;
                            billsVM.XeroShortCode = bill.XeroShortCode;
                            billsVM.Clients = bill.Clients;
                            billsVM.Status = bill.Status;
                            billsVM.CreatedBy = bill.CreatedBy;
                            billsVM.CreatedDate = bill.CreatedDate;
                            billsVM.IsDeleted = bill.IsDeleted;

                            if (Existresult != null && Existresult.Id != bill.Id)
                            {
                                ViewBag.ErrorMessage = "Exist";
                                return View("AddBillableEntity", billsVM);
                            }
                            result = obj.UpdateBillableEntity(Convert.ToString(bill.Id), bill.EntityName,bill.XeroId, bill.ProvenCFOXeroContactID, bill.CrmId, bill.XeroShortCode, bill.Clients, bill.Status.ToString().Trim(), Convert.ToString(bill.IsDeleted), LoginUserid);
                            //result.Id = 0;
                            ViewBag.ErrorMessage = "Updated";
                            return View("AddBillableEntity", billsVM);
                        }
                        if (result == null)
                            ViewBag.ErrorMessage = "";

                        return View("AddBillableEntity", billsVM);
                    }
                }

                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    return View();
                }
            }
            return View("AddBillableEntity", bill);
        }

        [CheckSession]
        public ActionResult UpdateBillableEntity(BillableEntitiesVM bill)
        {
            BillableEntitiesVM billsVM = new BillableEntitiesVM();
            if (ModelState.IsValid)
            {
                try
                {
                    var LoginUserid = Session["UserId"].ToString();
                    using (BillableEntitiesService obj = new BillableEntitiesService())
                    {
                        var result = obj.UpdateBillableEntity(Convert.ToString(bill.Id), bill.EntityName, bill.XeroId, bill.ProvenCFOXeroContactID, bill.CrmId, bill.XeroShortCode, bill.Clients, bill.Status, Convert.ToString(bill.IsDeleted), LoginUserid);
                        if (result == null)
                            ViewBag.ErrorMessage = "";
                        return RedirectToAction("GetAllBillableEntitiesList");
                    }

                }
                catch (Exception ex)
                {
                    log.Error(Utltity.Log4NetExceptionLog(ex));
                    return View();
                }
            }
            return RedirectToAction("GetAllBillableEntitiesList");
            //return JavaScript("AlertBillableEntity");

        }


        [CheckSession]
        public string DeleteBillableEntity(int Id)
        {
            try
            {
                using (BillableEntitiesService objBills = new BillableEntitiesService())
                {
                    var results = objBills.GetClientBillsAssociationById(Id);
                    if (results != null)
                    {
                        return results.Status;
                    }
                    else
                    {
                        var result = objBills.DeleteBillableEntity(Id);
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
        public ActionResult DeactivateBillableEntities(int Id)
        {
            try
            {
                using (BillableEntitiesService obj = new BillableEntitiesService())
                {
                    var result = obj.RetireBillableEntity(Id);
                    if (result == null)
                        ViewBag.ErrorMessage = "Can't deacactivte";
                    return RedirectToAction("GetAllBillableEntitiesList");
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
                using (BillableEntitiesService objBills = new BillableEntitiesService())
                {
                    Utltity obj = new Utltity();
                    var objResult = objBills.GetAllBillableEntitiesList().ResultData.Select(b => new
                    {
                        Billable_Entity_Id = b.Id,
                        Entity_Name = b.EntityName,
                        Xero_ID = b.XeroId,
                        ProvenCFO_Xero_Contact_ID = b.ProvenCFOXeroContactID,
                        CRM_ID = b.CrmId,
                        Xero_Short_Code = b.XeroShortCode,
                        Status = b.Status == "Active" ? "Active" : "Inactive",
                        Created_By = b.CreatedByUser,
                        Created_Date = b.CreatedDate.ToString() == "Inactive" || (((DateTime)b.CreatedDate).ToString("MM/dd/yyyy") == "01-01-0001" || ((DateTime)b.CreatedDate).ToString("MM/dd/yyyy") == "01/01/0001") ? "" : ((DateTime)b.CreatedDate).ToString("MM/dd/yyyy").Replace("-", "/"),
                        Modified_By = b.ModifiedByUser,
                        Modified_Date = b.ModifiedDate.ToString() == "Inactive" || (((DateTime)b.ModifiedDate).ToString("MM/dd/yyyy") == "01-01-0001" || ((DateTime)b.ModifiedDate).ToString("MM/dd/yyyy") == "01/01/0001") ? "" : ((DateTime)b.ModifiedDate).ToString("MM/dd/yyyy").Replace("-", "/")

                    }).ToList();
                    string filename = obj.ExportTOExcel("BillableEntitiesList", obj.ToDataTable(objResult));
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