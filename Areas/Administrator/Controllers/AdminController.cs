using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Areas.Model;
using ProvenCfoUI.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
            //IEnumerable<ManageRolesViewModel> manageRolesVM = null;

            //using (var staff = new HttpClient())
            //{
            //    staff.BaseAddress = new Uri(URL.BaseApi);
            //    //HTTP GET
            //    var responseTask = staff.GetAsync("api/account/SendInvitaion");
            //    responseTask.Wait();

            //    var result = responseTask.Result;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<IList<ManageRolesViewModel>>();
            //        readTask.Wait();

            //        manageRolesVM = readTask.Result;
            //    }
            //    else //web api sent error response 
            //    {
            //        //log response status here..

            //        manageRolesVM = Enumerable.Empty<ManageRolesViewModel>();

            //        ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
            //    }
            //}
            //return View(manageRolesVM);

            return View();
        }
        //[HttpPost]
        //public async Task<ActionResult> InviteStaff(InviteStaffViewModel inviteStaffVM)
        //{
        //    IEnumerable<ManageRolesViewModel> manageRolesVM = null;
        //    string url = "/api/account/sendinvitaion";

        //    InviteStaffViewModel dd = new InviteStaffViewModel();
        //        dd.Email = "john@gmail.com";

        //    var json = Newtonsoft.Json.JsonConvert.SerializeObject(dd);

        //    var kuchv = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

        //    using (var staff = new HttpClient())
        //    {
        //        staff.BaseAddress = new Uri(URL.BaseApi);

        //        //HTTP GET
        //        var result = await staff.PostAsync(url, kuchv);

        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<IList<ManageRolesViewModel>>();
        //            readTask.Wait();

        //            manageRolesVM = readTask.Result;
        //        }
        //        else //web api sent error response 
        //        {
        //            //log response status here..

        //            manageRolesVM = Enumerable.Empty<ManageRolesViewModel>();

        //            ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");

        //        }
        //    }
        //    return View(manageRolesVM);

        //}

        [HttpPost]
        public ActionResult InviteStaff(InviteStaffViewModel inviteStaffVM)
        {

            try
            {
                var LoginUserid = Session["UserId"].ToString();
                InvitationServices obj = new InvitationServices();
                var session = ConfigurationManager.AppSettings["userinvitationsessionouttime"].ToString();
                var result = obj.AddInvitation(inviteStaffVM.FirstName, inviteStaffVM.LastName, inviteStaffVM.Email, inviteStaffVM.roleid,inviteStaffVM.jobid,inviteStaffVM.SessionTimeout, LoginUserid);
                if (result == null)
                    ViewBag.ErrorMessage = "Email or Password not correct.Please check and try again.";

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "The Email Id doesn't Exist.";
                //Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                return View();
            }
        }


    }
}