using Proven.Model;
using Proven.Service;
using ProvenCfoUI.Comman;
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
        // GET: Communication
        [CheckSession]
        public ActionResult CommunicationMain()
        {
            return View();
        }

        [CheckSession]
        public ActionResult Chat()
        {
            ViewBag.UserId = Convert.ToString(Session["UserId"]);
            ViewBag.UserEmail = Convert.ToString(Session["LoginName"]);

            return View();
        }

        public async Task<JsonResult> ChatParticipants(string userId)
        {
            var chatParticipants = new List<ChatParticipants>();

            using (var communicationService = new CommunicationService())
            {
                chatParticipants = await communicationService.GetChatParticipants(userId);
            }
            return Json(chatParticipants, JsonRequestBehavior.AllowGet);
        }
    }
}