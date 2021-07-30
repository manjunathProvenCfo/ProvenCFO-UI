using Proven.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;

namespace ProvenCfoUI.Comman
{
    public class Common : IDisposable
    {
        protected static List<UserSecurityVM> _roleList;
        private bool isDisposed = false;
        public static string RegxPasswordMatch = @"(?=.*\d)(?=.*[A-Za-z]).{8,}";
        public static bool IsFeatureAccessable(string FeatureCode)
        {

            _roleList = (List<UserSecurityVM>)HttpContext.Current.Session["LoggedInUserUserSecurityModels"];

            if (_roleList != null && _roleList.Count > 0)
            {
                if (FeatureCode.Length == 3)
                {
                    if (_roleList.Where(x => x.FeatureCode == FeatureCode).ToList().Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool IsModuleAccessable(string ModuleCode)
        {
            _roleList = (List<UserSecurityVM>)HttpContext.Current.Session["LoggedInUserUserSecurityModels"];

            if (_roleList != null && _roleList.Count > 0)
            {
                if (ModuleCode.Length == 3)
                {
                    if (_roleList.Where(x => x.ModuleCode == ModuleCode).ToList().Count > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static void SaveStreamAsFile(string filePath, Stream inputStream, string fileName)
        {

            DirectoryInfo info = new DirectoryInfo(filePath);
            if (!info.Exists)
            {
                info.Create();

            }

            string path = Path.Combine(filePath, fileName);


            using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            {
                inputStream.CopyTo(outputFileStream);
            }
            //FileStream outputFileStream = new FileStream(path, FileMode.Create);
            //inputStream.CopyTo(outputFileStream);
            //using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
            //{
            //    inputStream.CopyTo(outputFileStream);
            //}
        }
        public static bool DeleteFile(string fileFullPath)
        {



            if (File.Exists(fileFullPath))
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(fileFullPath));

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();                   
                }
                if (Directory.Exists(Path.GetDirectoryName(fileFullPath)))
                {
                    Directory.Delete(Path.GetDirectoryName(fileFullPath));
                }
              
                return true;
            }
            else
            {
                return false;
            }

        }

        public static DataTable ConvertXSLXtoDataTable(string strFilePath, string connString)
        {
            OleDbConnection oledbConn = new OleDbConnection(connString);
            DataTable dt = new DataTable();
            try
            {

                oledbConn.Open();
                using (OleDbCommand cmd = new OleDbCommand("SELECT * FROM [Sheet1$]", oledbConn))
                {
                    OleDbDataAdapter oleda = new OleDbDataAdapter();
                    oleda.SelectCommand = cmd;
                    DataSet ds = new DataSet();
                    oleda.Fill(ds);

                    dt = ds.Tables[0];
                }
            }
            catch
            {
            }
            finally
            {

                oledbConn.Close();
            }

            return dt;

        }
        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            DataTable dt = new DataTable();
            using (StreamReader sr = new StreamReader(strFilePath))
            {
                string[] headers = sr.ReadLine().Split(',');
                foreach (string header in headers)
                {
                    dt.Columns.Add(header);
                }

                while (!sr.EndOfStream)
                {
                    string[] rows = sr.ReadLine().Split(',');
                    if (rows.Length > 1)
                    {
                        DataRow dr = dt.NewRow();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            dr[i] = rows[i].Trim();
                        }
                        dt.Rows.Add(dr);
                    }
                }

            }


            return dt;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;

            if (disposing)
            {
                if (_roleList != null)
                    _roleList = null;
                // free managed resources               
            }
            isDisposed = true;
        }
    }
}