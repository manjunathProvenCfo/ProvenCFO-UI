using log4net;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace ProvenCfoUI.Helper
{
    public class Utltity
    {
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly Log4NetDBAppender log4dbAppender =  new Log4NetDBAppender();
        public string ExportTOExcel(string fileName, DataTable dtt)
        {

            string path = System.Web.HttpContext.Current.Server.MapPath("~/ExportFile/");
            if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }

            fileName += DateTime.Now.ToString("_dd_MM_yyyy_hh_mm_ss_tt") + ".xls";
            string fullPath = Path.Combine(path, fileName);
            //string dtt = fileName;
            // string WorksheetName = dtt.TableName ?? "sheet";

            Stream stream = System.IO.File.Create(fullPath);

            using (ExcelPackage pack = new ExcelPackage())
            {

                DataTable dt = dtt;


                string WorksheetName = dt.TableName ?? "sheet";
                WorksheetName = fileName;
                //string WorksheetName1 = "staffuser";
                ExcelWorksheet ws = pack.Workbook.Worksheets.Add(WorksheetName);
                ws.Cells["A1"].LoadFromDataTable(dt, true);

                pack.SaveAs(stream);
                stream.Close();
            }
            return fileName;
        }
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties by using reflection   
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names  

                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        public static int GetWeekNumber()
        {
            CultureInfo ciCurr = CultureInfo.CurrentCulture;
            int weekNum = ciCurr.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;


        }
        public static void WriteMsg(Exception ex)
        {
            int WeekNum = GetWeekNumber();
            int Year = DateTime.Now.Year;
            string AppendStr = WeekNum.ToString() + "_" + Year.ToString() + "_";

            string appath = System.AppDomain.CurrentDomain.BaseDirectory + "Error\\";

            if (!Directory.Exists(appath))
            {
                Directory.CreateDirectory(appath);
            }

            string filePath = appath + AppendStr + "Error.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        public async static Task<string> Log4NetExceptionLog(Exception Exception,string userId=null)
        {

            var msg = string.Empty;
            var errorMessage = "Message= " + Exception.Message.ToString() + ". Method= " + Exception.TargetSite.Name.ToString();
            var errorDescription = " StackTrace : " + Exception.StackTrace.ToString() + " Source = " + Exception.Source.ToString();
            msg = errorMessage + " " + errorMessage;

           await  log4dbAppender.Log4NetDBAppend(errorMessage,"error",DateTime.Now.ToString("dd/MM/yy HH:mm:ss"),"ProvenCFO.Helper.Utility.Log4NetExceptionLog",errorDescription,userId);
            return msg;
        }
        public static void Log4NetInfoLog(string Info)
        {
            log.Info(Info);
        }

        public static void WriteExceptionLog(Exception Exception)
        {

            var msg = string.Empty;
            var errorMessage = "Message= " + Exception.Message.ToString() + ". Method= " + Exception.TargetSite.Name.ToString();
            var errorDescription = " StackTrace : " + Exception.StackTrace.ToString() + " Source = " + Exception.Source.ToString();
            msg = errorMessage + " " + errorMessage;
            int WeekNum = GetWeekNumber();
            int Year = DateTime.Now.Year;
            string AppendStr = WeekNum.ToString() + "_" + Year.ToString() + "_";

            string appath = System.AppDomain.CurrentDomain.BaseDirectory + "Error\\";
            if (!Directory.Exists(appath))
            {
                Directory.CreateDirectory(appath);
            }

            string filePath = appath + AppendStr + "Error.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + msg + "<br/>" + Environment.NewLine +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }


        public static void WriteMsg(string msg)
        {
            int WeekNum = GetWeekNumber();
            int Year = DateTime.Now.Year;
            string AppendStr = WeekNum.ToString() + "_" + Year.ToString() + "_";

            string appath = System.AppDomain.CurrentDomain.BaseDirectory + "Error\\";
            if (!Directory.Exists(appath))
            {
                Directory.CreateDirectory(appath);
            }

            string filePath = appath + AppendStr + "Error.txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + msg + "<br/>" + Environment.NewLine +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

        public static void WriteMsg(string msg, string FileName)
        {
            int WeekNum = GetWeekNumber();
            int Year = DateTime.Now.Year;
            string AppendStr = WeekNum.ToString() + "_" + Year.ToString() + "_";

            string appath = System.AppDomain.CurrentDomain.BaseDirectory + "Error\\";
            if (!Directory.Exists(appath))
            {
                Directory.CreateDirectory(appath);
            }
            string filePath = appath + AppendStr + FileName + ".txt";
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + msg + "<br/>" + Environment.NewLine +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

    }
}