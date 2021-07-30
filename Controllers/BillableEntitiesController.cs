using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
    [CustomAuthenticationFilter]
    public class BillableEntitiesController : Controller
    {
        string errorMessage = string.Empty;
        string errorDescription = string.Empty;
        // GET: BillableEntities

        [CheckSession]
        [CustomAuthorize("Administrator", "Super Administrator", "Manager")]
        public ActionResult GetAllBillableEntitiesList()
        {
            using (BillableEntitiesService obj = new BillableEntitiesService())
            {
                var objResult = obj.GetAllBillableEntitiesList();

                return View(objResult.ResultData);
            }
        }

        [CheckSession]
        [HttpGet]
        public ActionResult AddBillableEntity()
        {
            BillableEntitiesVM result = new BillableEntitiesVM();
            return View(result);
        }

        [CheckSession]
        public ActionResult EditBillableEntity(int Id)
        {
            using (BillableEntitiesService obj = new BillableEntitiesService())
            {
                BillableEntitiesVM objvm = new BillableEntitiesVM();
                var result = obj.GetBillableEntitiesById(Id);
                objvm.Id = result.Id;
                objvm.EntityName = result.EntityName;
                objvm.ProvenCFOXeroContactID = result.ProvenCFOXeroContactID;
                objvm.Clients = result.Clients;
                objvm.Status = result.Status;
                objvm.CreatedBy = result.CreatedBy;
                objvm.CreatedDate = result.CreatedDate;
                objvm.ModifiedBy = result.ModifiedBy;
                objvm.ModifiedDate = result.ModifiedDate;
                return View("AddBillableEntity", objvm);
            }

        }

        [CheckSession]
        [HttpPost]
        public ActionResult CreateBillableEntity(BillableEntitiesVM bill)
        {
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
                            result = obj.AddBillableEntity(bill.EntityName, bill.ProvenCFOXeroContactID, bill.Clients, bill.Status.ToString().Trim(), LoginUserid);
                            //result.Id = 0;
                            ViewBag.ErrorMessage = "Created";
                        }
                        else
                        {
                            var Existresult = obj.GetBillableEntitiesByName(billsVM.EntityName);
                            billsVM.Id = bill.Id;
                            billsVM.EntityName = bill.EntityName;
                            billsVM.ProvenCFOXeroContactID = bill.ProvenCFOXeroContactID;
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
                            result = obj.UpdateBillableEntity(Convert.ToString(bill.Id), bill.EntityName, bill.ProvenCFOXeroContactID, bill.Clients, bill.Status.ToString().Trim(), Convert.ToString(bill.IsDeleted), LoginUserid);
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
                        var result = obj.UpdateBillableEntity(Convert.ToString(bill.Id), bill.EntityName, bill.ProvenCFOXeroContactID, bill.Clients, bill.Status, Convert.ToString(bill.IsDeleted), LoginUserid);
                        if (result == null)
                            ViewBag.ErrorMessage = "";
                        return RedirectToAction("GetAllBillableEntitiesList");
                    }

                }
                catch (Exception ex)
                {
                    return View();
                }
            }
            return JavaScript("AlertBillableEntity");

        }


        [CheckSession]
        public string DeleteBillableEntity(int Id)
        {
            using (BillableEntitiesService objBills = new BillableEntitiesService())
            {
                var result = objBills.DeleteBillableEntity(Id);
                return result.Status;
                if (result == null)
                    ViewBag.ErrorMessage = "Can't Delete";
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
                ViewBag.ErrorMessage = "";
                return View();
            }
        }


    }
}