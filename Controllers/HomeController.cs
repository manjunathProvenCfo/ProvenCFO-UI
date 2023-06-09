﻿using log4net;
using Newtonsoft.Json;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using ProvenCfoUI.Models;
using ProvenCfoUI.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace ProvenCfoUI.Controllers
{
    //[Authorize]
    [Exception_Filters]
    public class HomeController : BaseController
    {
        string errorMessage = string.Empty;
        string errorDescription = string.Empty;
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public HomeController()
        {

        }


        public ActionResult Index()
        {
            if (Session["UserId"] != null)
            {
               // return View();
               return RedirectToAction("AgencyHome", "AgencyService");
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        [CheckSession]
        public ActionResult About()
        {


            return PartialView("_Error");
        }

        [OutputCache(Duration = 60, VaryByParam = "none", Location = System.Web.UI.OutputCacheLocation.Server)]
        public ActionResult Login()
        {
            Session["AppVersion"] = "v" + ConfigurationManager.AppSettings["webpages:Version"].ToString();
            return View();
        }


        public ActionResult LoginSessionExpired()
        {

            return View();
        }

        public ActionResult UnAuthorized()
        {
            return PartialView("_UnAuthorized");
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    using (AccountService obj = new AccountService())
                    {
                        CommonService commSrv;
                        var result = obj.LoginAccess(loginVM.UserName, loginVM.PasswordHash);
                        if (result.resultData != null && !string.IsNullOrEmpty(result.resultData.Id) && result.status == true)
                        {
                            commSrv = new CommonService();
                            string userData = $"{result.resultData.Id},{result.resultData.FirstName},{loginVM.UserName},{result.resultData.FirstName + " " + result.resultData.LastName},{result.resultData.UserType}";
                            FormsAuthentication.SetAuthCookie(userData, false);
                            ViewBag.Sucess = "Login Sucessfully";

                            var objUserPref = commSrv.GetUserPreferences(result.resultData.Id.ToString());
                            Session["LoggedInUserPreferences"] = objUserPref;

                            //List<UserPreferencesVM> UserPref = (List<UserPreferencesVM>)Session["LoggedInUserPreferences"];

                            var selectedAgency = objUserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                            var selectedAgencyId = Convert.ToInt32(selectedAgency.PreferanceValue);

                            Session["LoggedinUserEmail"] = loginVM.UserName.ToString();
                            var objUserRoleSec = commSrv.GetUserSecurityModels(loginVM.UserName.ToString(), selectedAgencyId);
                            Session["LoggedInUserUserSecurityModels"] = objUserRoleSec;
  
                            Session["LogedinUserName"] = result.resultData.FirstName + " " + result.resultData.LastName;

                            using (ClientService objClient = new ClientService())
                            {
                                var client = objClient.GetClientById(Convert.ToInt32(objUserPref.FirstOrDefault().PreferanceValue));
                                AccountingPackageInstance.Instance.ClientModel = client;
                            }

                            return RedirectToAction("AgencyHome", "AgencyService");
                        }
                        else
                        {
                            switch (result.message)
                            {
                                case "001":
                                    ViewBag.ErrorMessage = " \n The username or password you have entered is invalid.";
                                    break;
                                case "002":
                                    ViewBag.ErrorMessage = " \n The username or password you have entered is invalid.";
                                    break;
                                case "003":
                                    ViewBag.ErrorMessage = "  \n The username or password you have entered is invalid.";
                                    break;
                                case "004":
                                    ViewBag.ErrorMessage = "  \n The username or password you have entered is invalid.";
                                    break;
                                default:
                                    ViewBag.ErrorMessage = " \n The username or password you have entered is invalid.";
                                    break;
                            }
                            Utltity.Log4NetInfoLog(ViewBag.ErrorMessage + " , User Name: " + loginVM.UserName);
                            return View("Login");
                        }
                    }

                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Email or Password not correct.";
                    ViewBag.Sucess = null;
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                }
            }
            return View();
        }

        public ActionResult Register(int id, string ActiveCode)
        {
            try
            {
                using (InvitationServices obj = new InvitationServices())
                {

                    ViewBag.Sucess = false;
                    ViewBag.ErrorMessage = string.Empty;
                    var invitation = obj.GetInvitationForVlidationStaff(id, Guid.Parse(ActiveCode));
                    if (invitation == null)
                    {
                        TempData["InviteType"] = "InvalidInvite";
                        return PartialView("InvalidInvite");
                    }
                    else if (invitation != null && invitation.IsActive == 0)
                    {
                        ViewBag.ErrorMessage = "Inactive invitation Link";
                        return PartialView("_UserAcceptAgencyInvite");
                    }
                    else if (invitation != null && DateTime.Now > invitation.ExpiryTime)
                    {
                        //ViewBag.ErrorMessage = "The user invitation link has been expired!";
                        ViewBag.ErrorMessage = "Sorry, the Invitation in this link is expired " +
                                    " Please contact the administrators of the ProvenCFO Management Application to recive a new application";
                        return PartialView("_UserAcceptAgencyInvite");
                    }
                    else if (invitation != null && invitation.IsActive == 1 && invitation.IsRegistered == 1)
                    {
                        ViewBag.ShowLoginButton = true;
                        ViewBag.ErrorMessage = "User already accepted the invitation.";
                        return PartialView("_UserAcceptAgencyInvite");
                    }

                    using (AccountService objStaff = new AccountService())
                    {
                        RegisterViewModel model = new RegisterViewModel();
                        if (id > 0)
                        {
                            var IsValied = objStaff.IsInviteValied(id).resultData;
                            if (!IsValied)
                            {
                                TempData["InviteType"] = "InvalidInvite";
                                return PartialView("InvalidInvite");
                            }

                            var userdetail = objStaff.GetUserById(id).resultData;
                            model.firstname = userdetail.FirstName;
                            model.email = userdetail.Email;
                            model.lastname = userdetail.LastName;
                            model.UserType = 1;
                        }
                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
            }
            return View();
        }

        private bool CheckUserRegistered(string email)
        {
            bool status = false;
            using (InvitationServices obj = new InvitationServices())
            {

                var AgencyUserInvitation = obj.GetAgencyUserByEmailInvitations(email);
                if (AgencyUserInvitation != null)
                {
                    var registerd = (from invite in AgencyUserInvitation
                                     where invite.IsRegistered == 1
                                     select invite).ToList();
                    if (registerd != null)
                    {
                        if (registerd.Count() > 0)
                        {
                            status = true;
                        }
                    }
                }
            }
            return status;
        }
        [HttpGet]
        public ActionResult RegisterAgencyUser(int id, string ActiveCode, int AgencyId)
        {
            try
            {
                using (InvitationServices obj = new InvitationServices())
                {
                    ViewBag.Sucess = false;
                    ViewBag.ShowLoginButton = false;
                    ViewBag.ErrorMessage = string.Empty;
                    var invitation = obj.GetInvitationForVlidation(id, AgencyId, Guid.Parse(ActiveCode));
                    if (invitation == null)
                    {
                        TempData["InviteType"] = "InvalidInvite";
                        return PartialView("InvalidInvite");
                    }
                    else if (invitation != null && invitation.IsActive == 0)
                    {
                        ViewBag.ErrorMessage = "Inactive invitation Link";
                        return PartialView("_UserAcceptAgencyInvite");
                    }
                    else if (invitation == null && DateTime.Now > invitation.ExpiryTime)
                    {
                        ViewBag.ErrorMessage = "Sorry, the Invitation in this link is expired " +
                                     "Please contact the administrators of the ProvenCFO Management Application to recive a new application";
                        return PartialView("_UserAcceptAgencyInvite");
                    }
                    else if (invitation != null && invitation.IsActive == 1 && invitation.IsRegistered == 1)
                    {
                        ViewBag.ShowLoginButton = true;
                        ViewBag.ErrorMessage = "User already accepted the invitation.";
                        return PartialView("_UserAcceptAgencyInvite");
                    }

                    using (AccountService objAccount = new AccountService())
                    {
                        RegisterViewModel model = new RegisterViewModel();
                        if (id > 0)
                        {
                            var IsValied = objAccount.IsInviteValied(id).resultData;
                            if (!IsValied)
                            {
                                TempData["InviteType"] = "InvalidInvite";
                                return PartialView("InvalidInvite");
                            }

                            var userdetail = objAccount.GetUserById(id).resultData;

                       

                            model.firstname = userdetail.FirstName;
                            model.email = userdetail.Email;
                            model.lastname = userdetail.LastName;
                            model.UserType = 2;
                            model.AgencyID = AgencyId;



                            if (CheckUserRegistered(userdetail.Email))
                            {
                                var result = obj.ExistingAgencyUserInvitation(Convert.ToString(invitation.Id), Convert.ToString(AgencyId), Convert.ToString(ActiveCode));

                                if (result.resultData == true)
                                {
                                    ViewBag.Sucess = "User added to new agency successfully. Please click on the below link to log in";

                                    return View("_Userregisteredsuccessfully");
                                }
                            }

                        }

                        return View("Register", model);
                    }
                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
            }
            return View();
        }


        public ActionResult InvalidInvite()
        {
            return View();
        }


        public ActionResult Userregisteredsuccessfully()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterViewModel registerVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    AccountService obj = new AccountService();



                    var result = obj.Register(registerVM.email, registerVM.passwordhash, registerVM.confirmpassword, registerVM.firstname, registerVM.lastname, registerVM.UserType, registerVM.AgencyID);
                    if (result == null)
                        ViewBag.ErrorMessage = "";
                    ViewBag.Sucess = "User Registered Successfully";
                    return PartialView("_Userregisteredsuccessfully");
                }
                catch (Exception ex)
                {
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                    return View();
                }
            }
            return View();

        }


        public ActionResult ForgotPassword()
        {
            return View();
        }



        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (AccountService obj = new AccountService())
                    {
                        var result = obj.ForgotPassword(forgotPasswordVM.email);
                        if (result == null)
                        {
                            ViewBag.ErrorMessage = "Email or Password not correct.Please check and try again.";
                            return View();
                        }

                        ViewBag.Sucess = "The link has been Sent in you Email";
                        return View("ConfirmMail");
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "The Email Id doesn't Exist.";
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                    //Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                    return View();
                }
            }
            return View();
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
            try
            {
                if (ModelState.IsValid)
                {
                    using (AccountService obj = new AccountService())
                    {
                        var result = obj.SetPassword(setPasswordVM.Id, setPasswordVM.NewPassword, setPasswordVM.ConfirmPassword, setPasswordVM.ActivationCode);
                        if (result == null)
                        {
                            ViewBag.ErrorMessage = "";
                            return View();
                        }
                    }

                    ViewBag.Sucess = "Reset Password Successfully";
                    return PartialView("_SetPasswordSuccessfully");
                    //return RedirectToAction("Login");
                }
                return View();

            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "The Email Id doesn't Exist.";
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                return View();
            }
        }


        public ActionResult Logout()
        {
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
            AccountingPackageInstance.Instance.XeroToken = null;
            AccountingPackageInstance.Instance.ConnectionStatus = false;
        }


        public ActionResult ChangePassword()
        {
            return View();
        }

        [CheckSession]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel changePasswordVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (AccountService obj = new AccountService())
                    {
                        var id = Session["UserId"].ToString();
                        changePasswordVM.id = id;

                        var result = obj.ChangePassword(changePasswordVM.OldPassword, changePasswordVM.NewPassword, changePasswordVM.ConfirmPassword, changePasswordVM.id);
                        if (result.resultData == null)
                        {
                            ViewBag.ErrorMessage = result.message;
                            return View("ChangePassword");
                        }
                        ViewBag.Success = "Success";
                        return View();
                    }
                }

                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "";
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                    throw ex;
                    //Response.Write("<script>alert('Please enter your valid Email and Password.')</script>");
                    return View();
                }
            }
            return View();

        }
        [CheckSession]
        [HttpPost]
        public JsonResult ChangePasswordFromProfile(ChangePasswordViewModel changePasswordVM)
        {
            try
            {
                using (AccountService obj = new AccountService())
                {
                    var id = Session["UserId"].ToString();
                    changePasswordVM.id = id;

                    var IsValidOldpassword = obj.LoginAccess(Session["LoginName"].ToString(), changePasswordVM.OldPassword);
                    if (IsValidOldpassword.resultData != null && !string.IsNullOrEmpty(IsValidOldpassword.resultData.Id))
                    {
                        Regex rg = new Regex(Common.RegxPasswordMatch);
                        var IsValidnewpass = rg.IsMatch(changePasswordVM.NewPassword);
                        var IsValidConfirmpass = rg.IsMatch(changePasswordVM.ConfirmPassword);
                        if (IsValidnewpass == true && IsValidConfirmpass == true)
                        {
                            var result = obj.ChangePassword(changePasswordVM.OldPassword, changePasswordVM.NewPassword, changePasswordVM.ConfirmPassword, changePasswordVM.id);
                            return Json(new { result = result, sucess = true, ErrorMsg = "" }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            if (IsValidnewpass == false && IsValidConfirmpass == false)
                            {
                                return Json(new { sucess = false, ErrorMsg = "Your password must be at least 8 characters long and contain at least 1 letter and 1 number", Target = "BOTH" }, JsonRequestBehavior.AllowGet);
                            }
                            else if (IsValidnewpass == true && IsValidConfirmpass == false)
                            {
                                return Json(new { sucess = false, ErrorMsg = "Your password must be at least 8 characters long and contain at least 1 letter and 1 number", Target = "ConfirmPassword" }, JsonRequestBehavior.AllowGet);
                            }
                            else if (IsValidnewpass == false && IsValidConfirmpass == true)
                            {
                                return Json(new { sucess = false, ErrorMsg = "Your password must be at least 8 characters long and contain at least 1 letter and 1 number", Target = "NewPassword" }, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        return Json(new { sucess = false, ErrorMsg = "Old Password is not valid.", Target = "OldPassword" }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new { result = "", sucess = false, ErrorMsg = "" }, JsonRequestBehavior.AllowGet);
                    //Content("Sucess");


                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }


        [CheckSession]
        public ActionResult ProfileSetup()
        {
            using (AccountService obj = new AccountService())
            {
                UserModel model = new UserModel();
                try
                {


                    if (/*Session["UserId"] != null && */!string.IsNullOrEmpty(Session["UserId"].ToString()))
                    {
                        string userid = Session["UserId"].ToString();
                        model = obj.GetUserDetail(userid).resultData;
                        ViewBag.ProfileUrl = model.ProfileImage;
                        ViewBag.UserId = userid;
                    }
                }
                catch (Exception ex)
                {
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                    throw ex;
                }
                return View(model);
            }
        }

        //[CheckSession]
        //public ActionResult CallApi()
        //{
        //    using (AccountService obj = new AccountService())
        //    {
        //        try
        //        {


        //            UserModel model = new UserModel();
        //            string path = Server.MapPath("~/UploadedFiles/export_CW-ReconLines_2021-07-21_21-54-16.csv");
        //            string connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
        //            DataTable dt = Common.ConvertCSVtoDataTable(path);


        //            for (int i = 0; i < dt.Rows.Count; i++)
        //            {
        //                string id = Convert.ToString(dt.Rows[i][17]);
        //                string account_name = Convert.ToString(dt.Rows[i][0]);
        //                string amount = Convert.ToString(dt.Rows[i][1]);
        //                string company = Convert.ToString(dt.Rows[i][2]);
        //                string date = Convert.ToString(dt.Rows[i][3]).Replace(".", ",");
        //                string description = Convert.ToString(dt.Rows[i][4]);
        //                string gl_account = Convert.ToString(dt.Rows[i][5]);
        //                string reconciled = Convert.ToString(dt.Rows[i][7]);
        //                string reference = Convert.ToString(dt.Rows[i][8]);
        //                string rule = Convert.ToString(dt.Rows[i][9]);
        //                string type = Convert.ToString(dt.Rows[i][12]);
        //                var result = obj.CreateReconciliation(id, account_name, amount, company, date, description, gl_account, reconciled, reference, rule, type).ResultData;
        //                if (result != null)
        //                {
        //                    string msg = result.ToString();
        //                }
        //                else
        //                {
        //                    string error = result.ToString();
        //                }

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //             log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
        //            throw ex;
        //        }
        //        return View();

        //    }
        //}

        [CheckSession]
        public ActionResult TopNavigation()
        {
            using (AccountService obj = new AccountService())
            {
                try
                {

                    UserModel model = new UserModel();

                    if (Session["UserId"] != null && !string.IsNullOrEmpty(Session["UserId"].ToString()))
                    {
                        string userid = Session["UserId"].ToString();
                        model = obj.GetUserDetail(userid).resultData;

                        if (!string.IsNullOrEmpty(model.ProfileImage))
                        {
                            ViewBag.ProfileUrl = "" + model.ProfileImage.Substring(2);
                        }
                        else
                        {
                            ViewBag.ProfileUrl = model.ProfileImage;
                        }
                        
                        ViewBag.UserId = userid;
                    }
                    else
                    {
                        ViewBag.ProfileUrl = string.Empty;
                        ViewBag.UserId = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                    throw ex;
                }
                return PartialView("_TopNav");
            }
        }


        [CheckSession]
        [HttpPost]
        public ActionResult Upload()
        {
            string replacefile = string.Empty;
            string fileName = string.Empty, path = string.Empty;
            try
            {


                if (Request.Files.Count != 0)
                {
                    for (int i = 0; i < Request.Files.Count; i++)
                    {

                        var uploadFile = Request["FIleUpload"].ToString();

                        var file = Request.Files[i];
                        fileName = Path.GetFileName(file.FileName);

                        String ret = Regex.Replace(fileName.Trim(), "[^A-Za-z0-9_. ]+", "");


                        replacefile = ret.Replace(" ", String.Empty);


                        if (uploadFile == "Profile")
                            path = Server.MapPath("~/Upload/Profile/");
                        else
                            path = Server.MapPath("~/Upload/CoverProfile/");


                        if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
                        string PathfileName = Path.Combine(path, replacefile);
                        //file.SaveAs(PathfileName);

                        byte[] image = new byte[file.ContentLength];
                        file.InputStream.Read(image, 0, image.Length);
                        Common.Compressimage(PathfileName, "", image);
                        using (AccountService obj = new AccountService())
                        {
                            string userid = Session["UserId"].ToString();
                            var user = obj.GetUserDetail(userid).resultData;
                            if (user != null)
                            {
                                if (uploadFile == "Profile")
                                {
                                    user.ProfileImage = "../Upload/Profile/" + replacefile;
                                }
                                else
                                {
                                    user.CoverImage = "../Upload/CoverProfile/" + replacefile;
                                }
                                obj.UpdateUserDetail(user);

                            }

                        }
                    }

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
            return Json(replacefile, JsonRequestBehavior.AllowGet);

        }
        
        [CheckSession]
        public JsonResult UpdateProfile(UserModel model)
        {
            using (AccountService service = new AccountService())
            {
                try
                {
                    var result = service.UpdateUserDetail(model);
                    if (result.resultData != null)
                    {
                        Session["UserId"] = result.resultData.Id.ToString();
                        Session["UserName"] = result.resultData.FirstName;
                        Session["LoginName"] = result.resultData.FirstName.ToString();
                        Session["UserFullName"] = result.resultData.FirstName + " " + result.resultData.LastName;
                    }

                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {
                     log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                    throw ex;
                }
            }


        }

        [CheckSession]
        public ActionResult ChangePasswordFromProfile()
        {
            try
            {
                UserModel objuser = new UserModel();
                return PartialView("_ChangePasswordFromProfile", objuser);
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }
        }
        public ActionResult SessionTimeoutExtend(string UserName, string password)
        {
            using (AccountService obj = new AccountService())
            {
                var result = obj.LoginAccess(UserName, password);
                if (result.resultData != null && !string.IsNullOrEmpty(result.resultData.Id) && result.status == true)
                {
                    CommonService commSrv = new CommonService();
                    string userData = $"{result.resultData.Id},{result.resultData.FirstName},{UserName},{result.resultData.FirstName + " " + result.resultData.LastName},{result.resultData.UserType}";
                    FormsAuthentication.SetAuthCookie(userData, false);
                    var objUserPref = commSrv.GetUserPreferences(result.resultData.Id.ToString());
                    Session["LoggedInUserPreferences"] = objUserPref;

                    var selectedAgency = objUserPref.Where(x => x.PreferenceCategory == "Agency" && x.Sub_Category == "ID").FirstOrDefault();
                    var selectedAgencyId = Convert.ToInt32(selectedAgency.PreferanceValue);

                    var objUserRoleSec = commSrv.GetUserSecurityModels(UserName.ToString(), selectedAgencyId);
                    Session["LoggedInUserUserSecurityModels"] = objUserRoleSec;
                }
                else
                {
                    return Json("Invalid", JsonRequestBehavior.AllowGet);
                }
            }

            //Session.Timeout = Session.Timeout + 20;
            return Json("success", JsonRequestBehavior.AllowGet);
        }

        [CheckSession]
        [CustomAuthorize("Staff User")]
        public ActionResult GetRegisterdStaffUserList()
        {
            try
            {
                using (AccountService obj = new AccountService())
                {
                    var objResult = obj.GetRegisteredStaffUserList();
                    //objResult.StaffList = obj.GetRegisterdStaffUserList().ResultData;
                    return View(objResult.resultData);

                }
            }
            catch (Exception ex)
            {
                 log.Error(Utltity.Log4NetExceptionLog(ex,Convert.ToString(Session["UserId"])));
                throw ex;
            }

        }
    }
}