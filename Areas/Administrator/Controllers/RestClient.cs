using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Areas.Administrator.Controllers
{
    internal class RestClient
    {
        private object ping;

        public RestClient(object ping)
        {
            this.ping = ping;
        }
    }
}