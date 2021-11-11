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
    public class CommunicationController : BaseController
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        // GET: Communication
        [CheckSession]
        public ActionResult CommunicationMain()
        {
            return View();
        }

        [CheckSession]
        [HttpGet]
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
        public async Task<JsonResult> ChatParticipants(string userId, string userEmail, int clientId)
        {
            try
            {
                var ChatChannels = new List<ChatChannel>();

                using (var communicationService = new CommunicationService())
                {
                    ChatChannels = await communicationService.GetChatParticipants(userId, userEmail, clientId);
                }
                return Json(ChatChannels, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> GetPublicChat(string userId, string userEmail, TwilioConversationsTypeEnum type, string channelUniqueNameGuid, int clientId)
        {
            try
            {
                List<ChatChannel> chatChannels = new List<ChatChannel>();

                using (var communicationService = new CommunicationService())
                {
                    chatChannels = await communicationService.GetPublicChat(userId, userEmail, type, channelUniqueNameGuid, clientId);
                }
                return Json(chatChannels, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<JsonResult> FilterMentionUsers(string searchUser)
        {
            try
            {
                using (var communicationService = new CommunicationService())
                {
                    var ret = await communicationService.FilterMentionUsers(searchUser);
                    return Json(await communicationService.FilterMentionUsers(searchUser), JsonRequestBehavior.AllowGet);
                }
                
            }
            catch (Exception ex)
            {
                log.Error(Utltity.Log4NetExceptionLog(ex));
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }
    }
}