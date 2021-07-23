//using ProvenCfoUI.Areas.Model;
using Proven.Service;
using ProvenCfoUI.Areas.Model;
using ProvenCfoUI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Areas.Administrator.Controllers
{
    public class RolesController : Controller
    {
        
        HttpClient clientnew;

        public RolesController()
        {
            clientnew = new HttpClient();
            //clientnew.BaseAddress = baseAddress;
        }

        // GET: Administrator/Roles
        public ActionResult Index()
        {
            RoleService obj = new RoleService();
            var objResult = obj.GetRoles();


            return View(objResult.ResultData);
        }

        // GET: Administrator/Roles/Details/5
        //    public ActionResult Details(int id)
        //    {
        //        return View();
        //    }

        //    // GET: Administrator/Roles/Create
        //    public ActionResult Create()
        //    {
        //        return View();
        //    }

        //    // POST: Administrator/Roles/Create
        //    [HttpPost]
        //    public ActionResult Create(FormCollection collection)
        //    {
        //        try
        //        {
        //            // TODO: Add insert logic here

        //            return RedirectToAction("Index");
        //        }
        //        catch
        //        {
        //            return View();
        //        }
        //    }

        //    // GET: Administrator/Roles/Edit/5
        //    public ActionResult Edit(int id)
        //    {
        //        return View();
        //    }

        //    // POST: Administrator/Roles/Edit/5
        //    [HttpPost]
        //    public ActionResult Edit(int id, FormCollection collection)
        //    {
        //        try
        //        {
        //            // TODO: Add update logic here

        //            return RedirectToAction("Index");
        //        }
        //        catch
        //        {
        //            return View();
        //        }
        //    }

        //    // GET: Administrator/Roles/Delete/5
        //    public ActionResult Delete(int id)
        //    {
        //        return View();
        //    }

        //    // POST: Administrator/Roles/Delete/5
        //    [HttpPost]
        //    public ActionResult Delete(int id, FormCollection collection)
        //    {
        //        try
        //        {
        //            // TODO: Add delete logic here

        //            return RedirectToAction("Index");
        //        }
        //        catch
        //        {
        //            return View();
        //        }
        //    }
        //}
    }
}
