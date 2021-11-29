using log4net;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProvenCfoUI.Comman
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public override void OnException(ExceptionContext filterContext)
        {
            filterContext.ExceptionHandled = true;

            log.Error(Utltity.Log4NetExceptionLog(filterContext.Exception));

            if (filterContext != null)
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    JsonResult jsonResult = new JsonResult
                    {
                        Data = new { success = false, serverError = "500", stackTrace = filterContext.Exception.InnerException },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    jsonResult.ExecuteResult(filterContext.Controller.ControllerContext);
                    //httpContext.Response.End();
                }
                else
                {
                    //Redirect or return a view, but not both.
                    filterContext.Result = new ViewResult
                    {
                        ViewName = "~/Views/ErrorHandler/Index.cshtml"
                    };
                }
            }
        }
    }
}