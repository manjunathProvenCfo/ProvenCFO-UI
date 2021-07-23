using Proven.Model;
using System;
using System.Collections.Generic;
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