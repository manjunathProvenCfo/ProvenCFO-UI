using Newtonsoft.Json;
using ProvenCfoUI.Models;
using ProvenCfoUI.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Controllers
{
   //[Authorize]
    public class AccountsController : Controller
    {
        HttpClient staff;

        public AccountsController()
        {
            staff = new HttpClient();
        }

        public ActionResult Index(string token)
        {
            if (token == null)
            {
                return RedirectToAction("Login");

            }
            else
            {
                return View();
            }
      
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginVM)
        {
            try
            {
                WebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL.BaseApi + "/token");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                string body = "username=" + loginVM.username + "&password=" + loginVM.password + "&grant_type=password";
                byte[] byteArray = Encoding.UTF8.GetBytes(body);
                webRequest.ContentLength = byteArray.Length;
                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = webRequest.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                Login login = new Login();
                login = JsonConvert.DeserializeObject<Login>(responseFromServer);
                Session["token"] = login.access_token;
               
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Email or Password not found or matched";
                //Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                return View();
            }
            return View("Index");

        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel registerVM)
        {
            
                WebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL.BaseApi + "/api/account/register");
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                string body = "email=" + registerVM.email + "&password=" + registerVM.password + "&confirmpassword=" + registerVM.confirmpassword + "&firstname=" + registerVM.firstname + "&lastname=" + registerVM.lastname;
                byte[] byteArray = Encoding.UTF8.GetBytes(body);
                webRequest.ContentLength = byteArray.Length;
                Stream dataStream = webRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
                WebResponse response = webRequest.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseFromServer = reader.ReadToEnd();
                RegisterViewModel registerViewModel = new RegisterViewModel();
                registerVM = JsonConvert.DeserializeObject<RegisterViewModel>(responseFromServer);
                return View("Login");

        }
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
        {

            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL.BaseApi + "/api/account/ForgotPassword");

            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            //webRequest.Headers.Add("Authorization", "Bearer " + Session["token"]);
            string body = "email=" + forgotPasswordVM.email;
            byte[] byteArray = Encoding.UTF8.GetBytes(body);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = webRequest.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            if (responseFromServer == "")
            {
                return View("ConfirmMail");
            }
            else
            {
                
                ViewBag.ErrorMessage = "Unable to send the password reset link. Please try again.";

                return View();
            }

        }

       

        public ActionResult ConfirmMail()
        {
            return View();
        }

        public ActionResult SetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SetPassword(SetPasswordViewModel setPasswordVM)
        {
            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL.BaseApi + "/api/account/SetPassword");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Headers.Add("Authorization", "Bearer " + Session["token"]);
            string body = "NewPassword=" + setPasswordVM.NewPassword +  "&ConfirmPassword=" + setPasswordVM.ConfirmPassword;
            byte[] byteArray = Encoding.UTF8.GetBytes(body);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = webRequest.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            if (responseFromServer == "")
            {
                return View("Login");
            }
            else
            {
                ViewBag.ErrorMessage = "Your Password is not changed please try again.";
                //Response.Write("<script>alert('Your Password is not changed please try again.')</script>");
                return View();
            }
        }
        public ActionResult Logout()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]        
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordVM)
        {
            
            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(URL.BaseApi + "/api/account/ChangePassword");
            webRequest.Method = "POST";
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Headers.Add("Authorization", "Bearer " + Session["token"]);
            string body = "OldPassword=" + changePasswordVM.OldPassword + "&NewPassword=" + changePasswordVM.NewPassword + "&ConfirmPassword=" + changePasswordVM.ConfirmPassword;
            byte[] byteArray = Encoding.UTF8.GetBytes(body);
            webRequest.ContentLength = byteArray.Length;
            Stream dataStream = webRequest.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = webRequest.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            if (responseFromServer == "")
            {
                return View("Index");
            }
            else
            {
                ViewBag.ErrorMessage = "Your Password is not changed please try again.";
                //Response.Write("<script>alert('Your Password is not changed please try again.')</script>");
                return View();
            }
            
        }

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