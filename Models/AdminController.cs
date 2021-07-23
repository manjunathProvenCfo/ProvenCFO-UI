using ProvenCfoUI.Areas.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Areas.Administrator.Controllers
{
    public class AdminController : Controller
    {
        HttpClient staff;

        public AdminController()
        {
            staff = new HttpClient();
        }

        // GET: Administrator/Admin
        public ActionResult InviteStaff()
        {
            IEnumerable<ManageRolesViewModel> manageRolesVM = null;
           
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44341/api/");
                //HTTP GET
                var responseTask = client.GetAsync("roles");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<ManageRolesViewModel>>();
                    readTask.Wait();

                    manageRolesVM = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    manageRolesVM = Enumerable.Empty<ManageRolesViewModel>();

                    ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(manageRolesVM);
        }
        [HttpPost]
        public ActionResult InviteStaff(InviteStaffViewModel inviteStaffVM)
        {
            return View();
        }
    }
}