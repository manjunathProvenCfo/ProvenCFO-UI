using log4net;
using Proven.Model;
using Proven.Service;
using Proven.Service.AccountingPackage;
using ProvenCfoUI.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xero.NetStandard.OAuth2.Token;

namespace ProvenCfoUI.Comman
{
    public class Common : IDisposable
    {
        protected static List<UserSecurityVM> _roleList;
        private bool isDisposed = false;
        public static string RegxPasswordMatch = @"(?=.*\d)(?=.*[A-Za-z]).{8,}";
        private static Object XeroLock = new Object();
        private static readonly ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public enum ChartOptions
        {
            Option_0 = 0,
            Option_1 = 1,
            Option_2 = 2,
            Option_3 = 3,
            Option_4 = 4,
            Option_5 = 5,
            Option_6 = 6,
            Option_7 = 7
        }
        public enum ChartType
        {
            Revenue = 0,
            NetIncome = 1
        }
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

            if (client != null && !string.IsNullOrEmpty(client.APIScope) && !string.IsNullOrEmpty(client.APIClientID) && !string.IsNullOrEmpty(client.APIClientSecret))
            {
                AccountingPackageInstance.Instance.Scope = client.APIScope;//"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                AccountingPackageInstance.Instance.ClientID = client.APIClientID;
                AccountingPackageInstance.Instance.ClientSecret = client.APIClientSecret;
                AccountingPackageInstance.Instance.XeroAppName = "ProvenCfo_web";
                AccountingPackageInstance.Instance.XeroTenentID = client.XeroID;
                if (!string.IsNullOrEmpty(client.XeroContactIDforProvenCfo) && Guid.TryParse(client.XeroContactIDforProvenCfo, out Guid result))
                {
                    AccountingPackageInstance.Instance.XeroContactIDofProvenCfo = new Guid(client.XeroContactIDforProvenCfo);
                }
                //XeroService Xero = new XeroService("8CED4A15FB7149198DB6260147780F6D", "MHr607yAVALE1EX6QrhwOYYeCrQePcrRAfofw056YTK6qWg8", scope);

                AccountingAppFatory<IXeroToken, TokenInfoVM> AppPackage = null;
                switch (client.ThirdPartyAccountingApp_ref.Value)
                {
                    case 1:
                        AppPackage = new XeroService<IXeroToken, TokenInfoVM>(AccountingPackageInstance.Instance.ClientID, AccountingPackageInstance.Instance.ClientSecret, AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName);
                        break;
                    case 2:
                        AppPackage = new QuickbooksLocalService<IXeroToken, TokenInfoVM>(AccountingPackageInstance.Instance.ClientID, AccountingPackageInstance.Instance.ClientSecret, AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName);
                        break;
                    default:
                        break;
                }
               
                //XeroService<IXeroToken, TokenInfoVM> Xero = new XeroService<IXeroToken, TokenInfoVM>(AccountingPackageInstance.Instance.ClientID, AccountingPackageInstance.Instance.ClientSecret, AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName)


                lock (XeroLock)
                {
                    Task.Run(async () =>
                    {
                        TokenInfoVM TokenInfoSaved = AppPackage.GetSavedToken(client.Id).ResultData;
                        if (TokenInfoSaved != null)
                        {
                            try
                            {
                                var SavedToken = AppPackage.getTokenFormat(TokenInfoSaved);
                                //AccountingPackageInstance.Instance.XeroService = AppPackage;
                                var token = await AppPackage.RefreshToken(SavedToken);
                                //CleanToken();
                                TokenInfoSaved.access_token = token.AccessToken;
                                TokenInfoSaved.id_token = token.IdToken;
                                TokenInfoSaved.refresh_token = token.RefreshToken;
                                TokenInfoSaved.expires_in = (DateTime.UtcNow - token.ExpiresAtUtc).Seconds;
                                TokenInfoSaved.ModifiedDate = DateTime.Now;
                                TokenInfoSaved.AppName = AccountingPackageInstance.Instance.XeroAppName;
                                TokenInfoSaved.ConnectionStatus = "SUCCESS";
                                TokenInfoSaved.ThirdPartyAccountingAppId_ref = client.ThirdPartyAccountingApp_ref;
                                AppPackage.UpdateToken(TokenInfoSaved);
                                AccountingPackageInstance.Instance.XeroToken = token;
                                AccountingPackageInstance.Instance.ConnectionStatus = true;
                                AccountingPackageInstance.Instance.ConnectionMessage = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                log.Error(Utltity.Log4NetExceptionLog(ex));
                                TokenInfoSaved.ConnectionStatus = "ERROR";
                                AppPackage.UpdateToken(TokenInfoSaved);
                                CleanToken();
                                AccountingPackageInstance.Instance.ConnectionMessage = ex.Message;
                                throw;
                            }
                        }
                        else
                        {
                            Utltity.Log4NetInfoLog("Insufficient client information");
                            CleanToken();
                            AccountingPackageInstance.Instance.ConnectionMessage = "Insufficient client information";
                        }
                    });
                }

            }
            else
            {
                Utltity.Log4NetInfoLog("Insufficient client information");
                CleanToken();
                AccountingPackageInstance.Instance.ConnectionMessage = "Insufficient client information";
            }
        }
        private static void CleanToken()
        {
            AccountingPackageInstance.Instance.Scope = string.Empty; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
            AccountingPackageInstance.Instance.ClientID = string.Empty;
            AccountingPackageInstance.Instance.ClientSecret = string.Empty;
            AccountingPackageInstance.Instance.XeroToken = null;
            AccountingPackageInstance.Instance.ConnectionMessage = "";
            AccountingPackageInstance.Instance.ConnectionStatus = false;
        }
        public static string getXeroLoginUrl(ClientModel client)
        {
            if (client != null && !string.IsNullOrEmpty(client.APIScope) && !string.IsNullOrEmpty(client.APIClientID) && !string.IsNullOrEmpty(client.APIClientSecret))
            {
                AccountingPackageInstance.Instance.Scope = client.APIScope;//"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                AccountingPackageInstance.Instance.ClientID = client.APIClientID;
                AccountingPackageInstance.Instance.ClientSecret = client.APIClientSecret;
                AccountingPackageInstance.Instance.XeroAppName = "ProvenCfo_web";
                AccountingPackageInstance.Instance.XeroTenentID = client.XeroID;
                //XeroService Xero = new XeroService("8CED4A15FB7149198DB6260147780F6D", "MHr607yAVALE1EX6QrhwOYYeCrQePcrRAfofw056YTK6qWg8", scope);
                using (XeroService<IXeroToken, TokenInfoVM> Xero = new XeroService<IXeroToken, TokenInfoVM>(AccountingPackageInstance.Instance.ClientID, AccountingPackageInstance.Instance.ClientSecret, AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName))
                {

                    //AccountingPackageInstance.Instance.XeroService = Xero;

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
            }
            return string.Empty;
        }
        public static ChartOptionsResultModel getChartOptionValues(ChartOptions Option)
        {
            ChartOptionsResultModel result = new ChartOptionsResultModel();
            DateTime datetime = DateTime.Now;
            switch (Option)
            {
                case ChartOptions.Option_0:
                    int currQuarter = (datetime.Month - 1) / 3 + 1;
                    result.StartDate = null; //new DateTime(datetime.Year, 3 * currQuarter - 2, 1);
                    result.EndDate = null; //datetime.AddDays(1 - datetime.Day).AddMonths(3 - (datetime.Month - 1) % 3).AddDays(-1);
                    result.timeframe = "MONTH";
                    result.periods = 11;
                    break;
                case ChartOptions.Option_1:
                    //int PreviousQuarter = (datetime.Month - 1) / 3 ;
                    result.StartDate = null;//new DateTime(datetime.Year, 3 * PreviousQuarter - 2, 1);
                    result.EndDate = null;// datetime.AddDays(1 - datetime.Day).AddMonths((datetime.Month - 1) % 3).AddDays(-1);
                    result.timeframe = "MONTH";
                    result.periods = 5;
                    break;
                case ChartOptions.Option_2:
                    result.StartDate = null;// new DateTime(datetime.Year, 1, 1);
                    result.EndDate = null;// new DateTime(datetime.Year, 12, 31);
                    result.timeframe = "MONTH";
                    result.periods = 2;
                    break;
                case ChartOptions.Option_3:
                    result.StartDate = new DateTime(datetime.Year, 1, 1);
                    result.EndDate = new DateTime(datetime.Year, 12, 31);
                    result.timeframe = "MONTH";
                    result.periods = 11;
                    break;
                case ChartOptions.Option_4:
                    result.StartDate = new DateTime(datetime.AddYears(-1).Year, 1, 1);
                    result.EndDate = new DateTime(datetime.AddYears(-1).Year, 12, 31);
                    result.timeframe = "QUARTER";
                    result.periods = 3;
                    break;
                case ChartOptions.Option_5:
                    result.StartDate = new DateTime(datetime.AddYears(-1).Year, 1, 1);
                    result.EndDate = new DateTime(datetime.AddYears(-1).Year, 12, 31);
                    result.timeframe = "MONTH";
                    result.periods = 11;
                    break;
                case ChartOptions.Option_6:
                    result.StartDate = new DateTime(datetime.AddYears(-2).Year, 1, 1);
                    result.EndDate = new DateTime(datetime.Year, 12, 31);
                    result.timeframe = "MONTH";
                    result.periods = 11;
                    break;
                case ChartOptions.Option_7:
                    result.StartDate = new DateTime(datetime.AddYears(-2).Year, 1, 1);
                    result.EndDate = new DateTime(datetime.Year, 12, 31);
                    result.timeframe = "QUARTER";
                    result.periods = 12;
                    break;
                default:
                    break;
            }
            return result;
        }
        public static void ConnectXeroOrganization(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                XeroService<IXeroToken, TokenInfoVM> Xero = new XeroService<IXeroToken, TokenInfoVM>(AccountingPackageInstance.Instance.ClientID, AccountingPackageInstance.Instance.ClientSecret, AccountingPackageInstance.Instance.Scope, AccountingPackageInstance.Instance.XeroAppName);
                AccountingPackageInstance.Instance.XeroService = (IAccouningPackage<IXeroToken, TokenInfoVM>)Xero;
                Task.Run(async () =>
                {
                    try
                    {
                        var token = await Xero.LoginXeroAccesswithCode(code);
                        if (DateTime.UtcNow > token.ExpiresAtUtc)
                        {
                            token = await Xero.RefreshToken(token);
                        }
                        AccountingPackageInstance.Instance.XeroToken = token;
                        AccountingPackageInstance.Instance.ConnectionStatus = true;
                        AccountingPackageInstance.Instance.ConnectionMessage = string.Empty;
                    }
                    catch (Exception ex)
                    {
                        AccountingPackageInstance.Instance.Scope = string.Empty; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                        AccountingPackageInstance.Instance.ClientID = string.Empty;
                        AccountingPackageInstance.Instance.ClientSecret = string.Empty;
                        AccountingPackageInstance.Instance.XeroToken = null;
                        AccountingPackageInstance.Instance.ConnectionStatus = false;
                        AccountingPackageInstance.Instance.ConnectionMessage = ex.Message;
                        throw;
                    }
                });
            }
            else
            {
                AccountingPackageInstance.Instance.Scope = string.Empty; //"accounting.transactions payroll.payruns payroll.settings accounting.contacts projects accounting.settings payroll.employees files";
                AccountingPackageInstance.Instance.ClientID = string.Empty;
                AccountingPackageInstance.Instance.ClientSecret = string.Empty;
                AccountingPackageInstance.Instance.XeroToken = null;
                AccountingPackageInstance.Instance.ConnectionMessage = "Insufficient client information";
                AccountingPackageInstance.Instance.ConnectionStatus = false;
            }
        }


    }

}