using Proven.Model;
using Proven.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProvenCfoUI.Helper
{
    public class Log4NetDBAppender:BaseService
    {
        public async Task<bool> Log4NetDBAppend(string msg,string level,string date,string logger,string stackTrace,string userId="")
        {
            try
            {
                var model = new Log4NetModel();

                model.Date = date;
                model.Logger = logger;
                model.StackTrace = stackTrace;
                model.Message = msg;
                model.Level = level;
                model.UserId = userId;
                
                var url = "/api/Common/Logger4NetLog";


                var result = await this.PostAsync<object,Log4NetModel>(url,model);


            }
            catch (Exception ex) { 
            
               
            }

            return true;
        }

    }
}