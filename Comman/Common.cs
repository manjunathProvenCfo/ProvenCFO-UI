using Proven.Model;
using Proven.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
        public static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
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
        public static void ConnectXeroClient(ClientModel client)
        {
            if (client != null && !string.IsNullOrEmpty(client.XeroScope) && !string.IsNullOrEmpty(client.XeroClientID) && !string.IsNullOrEmpty(client.XeroClientSecret))
            {
                XeroInstance.Instance.XeroScope = client.XeroScope;//"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                XeroInstance.Instance.XeroClientID = client.XeroClientID;
                XeroInstance.Instance.XeroClientSecret = client.XeroClientSecret;
                XeroInstance.Instance.XeroAppName = "ProvenCfo_web";
                XeroInstance.Instance.XeroTenentID = client.XeroID;
                //XeroService Xero = new XeroService("8CED4A15FB7149198DB6260147780F6D", "MHr607yAVALE1EX6QrhwOYYeCrQePcrRAfofw056YTK6qWg8", scope);

                if (XeroInstance.Instance.XeroToken == null || XeroInstance.Instance.XeroConnectionStatus == false || (XeroInstance.Instance.XeroToken != null && XeroInstance.Instance.XeroToken.ExpiresAtUtc.Subtract(DateTime.UtcNow).TotalSeconds < 200))
                {
                    XeroService Xero = new XeroService(XeroInstance.Instance.XeroClientID, XeroInstance.Instance.XeroClientSecret, XeroInstance.Instance.XeroScope, XeroInstance.Instance.XeroAppName);

                    var XeroTokenInfoSaved = Xero.GetSavedXeroToken(client.Id).ResultData;
                    if (XeroTokenInfoSaved != null)
                    {

                        var SavedToken = Xero.getTokenFormat(XeroTokenInfoSaved);
                        XeroInstance.Instance.XeroService = Xero;
                        Task.Run(async () =>
                        {
                            try
                            {

                            //var newtoken = await Xero.LoginXeroAccess();



                            var token = await Xero.RefreshToken(SavedToken);

                                XeroTokenInfoSaved.access_token = token.AccessToken;
                                XeroTokenInfoSaved.id_token = token.IdToken;
                                XeroTokenInfoSaved.refresh_token = token.RefreshToken;
                                XeroTokenInfoSaved.expires_in = (DateTime.UtcNow - token.ExpiresAtUtc).Seconds;
                                XeroTokenInfoSaved.ModifiedDate = DateTime.Now;
                                XeroTokenInfoSaved.AppName = XeroInstance.Instance.XeroAppName;
                                Xero.UpdateXeroToken(XeroTokenInfoSaved);
                            //var url = Xero.BuildLoginUri();
                            //
                            //if (DateTime.UtcNow > token.ExpiresAtUtc)
                            //{
                            //    await Xero.RefreshToken(token);
                            //}
                            XeroInstance.Instance.XeroToken = token;
                                XeroInstance.Instance.XeroConnectionStatus = true;
                                XeroInstance.Instance.XeroConnectionMessage = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                XeroInstance.Instance.XeroScope = string.Empty; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                            XeroInstance.Instance.XeroClientID = string.Empty;
                                XeroInstance.Instance.XeroClientSecret = string.Empty;
                                XeroInstance.Instance.XeroToken = null;
                                XeroInstance.Instance.XeroConnectionStatus = false;
                                XeroInstance.Instance.XeroConnectionMessage = ex.Message;
                                throw;
                            }

                        });
                    }
                }
            }
            else
            {
                XeroInstance.Instance.XeroScope = string.Empty; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                XeroInstance.Instance.XeroClientID = string.Empty;
                XeroInstance.Instance.XeroClientSecret = string.Empty;
                XeroInstance.Instance.XeroToken = null;
                XeroInstance.Instance.XeroConnectionMessage = "Insufficient client information";
                XeroInstance.Instance.XeroConnectionStatus = false;
            }
        }
        public static string getXeroLoginUrl(ClientModel client)
        {
            if (client != null && !string.IsNullOrEmpty(client.XeroScope) && !string.IsNullOrEmpty(client.XeroClientID) && !string.IsNullOrEmpty(client.XeroClientSecret))
            {
                XeroInstance.Instance.XeroScope = client.XeroScope;//"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                XeroInstance.Instance.XeroClientID = client.XeroClientID;
                XeroInstance.Instance.XeroClientSecret = client.XeroClientSecret;
                XeroInstance.Instance.XeroAppName = "ProvenCfo_web";
                XeroInstance.Instance.XeroTenentID = client.XeroID;
                //XeroService Xero = new XeroService("8CED4A15FB7149198DB6260147780F6D", "MHr607yAVALE1EX6QrhwOYYeCrQePcrRAfofw056YTK6qWg8", scope);
                XeroService Xero = new XeroService(XeroInstance.Instance.XeroClientID, XeroInstance.Instance.XeroClientSecret, XeroInstance.Instance.XeroScope, XeroInstance.Instance.XeroAppName);
                XeroInstance.Instance.XeroService = Xero;

                try
                {
                    var url = Xero.BuildLoginUri();
                    return url;
                }
                catch (Exception ex)
                {
                    throw;
                }


            }
            return string.Empty;
        }

        public static void ConnectXeroOrganization(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                XeroService Xero = new XeroService(XeroInstance.Instance.XeroClientID, XeroInstance.Instance.XeroClientSecret, XeroInstance.Instance.XeroScope, XeroInstance.Instance.XeroAppName);
                XeroInstance.Instance.XeroService = Xero;
                Task.Run(async () =>
                {
                    try
                    {
                        var token = await Xero.LoginXeroAccesswithCode(code);
                        if (DateTime.UtcNow > token.ExpiresAtUtc)
                        {
                            await Xero.RefreshToken(token);
                        }
                        XeroInstance.Instance.XeroToken = token;
                        XeroInstance.Instance.XeroConnectionStatus = true;
                        XeroInstance.Instance.XeroConnectionMessage = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        XeroInstance.Instance.XeroScope = string.Empty; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                        XeroInstance.Instance.XeroClientID = string.Empty;
                        XeroInstance.Instance.XeroClientSecret = string.Empty;
                        XeroInstance.Instance.XeroToken = null;
                        XeroInstance.Instance.XeroConnectionStatus = false;
                        XeroInstance.Instance.XeroConnectionMessage = ex.Message;
                        throw;
                    }
                });
            }
            else
            {
                XeroInstance.Instance.XeroScope = string.Empty; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                XeroInstance.Instance.XeroClientID = string.Empty;
                XeroInstance.Instance.XeroClientSecret = string.Empty;
                XeroInstance.Instance.XeroToken = null;
                XeroInstance.Instance.XeroConnectionMessage = "Insufficient client information";
                XeroInstance.Instance.XeroConnectionStatus = false;
            }
        }

    }

}