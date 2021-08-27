using ProvenCfoUI.Comman;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Proven.Service;
using Proven.Model;

namespace ProvenCfoUI.Controllers
{
    public class TwilioController : BaseController
    {
        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> Token(string Identity)
        {
            string token = "";
            using (var twilioService = new TwilioService())
            {
                token = await twilioService.GetToken(Identity);
            }
            return Json(token);
        }

        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> UpdateTwilioUserId(string UserId, string TwilioUserId)
        {
            bool IsSuccessful = false;
            using (var twilioService = new TwilioService())
            {
                IsSuccessful = await twilioService.UpdateTwilioUserId(UserId, TwilioUserId);
            }
            return Json(IsSuccessful);
        }

        [HttpPost]
        [CheckSession]
        public async Task<JsonResult> InsertUpdateTwilioConversation(TwilioConversations twilioConversations)
        {
            using (var twilioService = new TwilioService())
            {
                twilioConversations.CreatedModifiedBy = Convert.ToString(Session["LoginName"]);
                await twilioService.InsertUpdateTwilioConversation(twilioConversations);
            }
            return Json("");
        }

    }
}