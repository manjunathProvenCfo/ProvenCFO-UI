using log4net;
using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace ProvenCfoUI.Controllers
{
    public class CommunicationController : Controller
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Communication
        [CheckSession]
        public ActionResult CommunicationMain()
        {
            return View();
        }

        [CheckSession]
        public ActionResult Chat()
        {
            try
            {
                ViewBag.UserId = Convert.ToString(Session["UserId"]);
                ViewBag.UserEmail = Convert.ToString(Session["LoginName"]);
                return View();
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                throw ex;
            }
        }

        public async Task<JsonResult> ChatParticipants(string userId, string userEmail)
        {
            try
            {
                var chatParticipants = new List<ChatParticipants>();

                using (var communicationService = new CommunicationService())
                {
                    chatParticipants = await communicationService.GetChatParticipants(userId,userEmail);
                }
                return Json(chatParticipants, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                return Json("", JsonRequestBehavior.AllowGet);

                throw ex;
            }
        }
    }
}