using Azure.Storage.Sas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ProvenCfoUI.Comman
{
    public interface IStorageAccess: IDisposable
    {
        bool UploadFile(string StorageContainerName, string dirName, string fileName, HttpPostedFileBase inputfile);
        Uri GetFileSasUri(string StorageContainerName, string filePath, DateTime expiration, ShareFileSasPermissions permissions);
        bool DeleteAzureFiles(string StorageContainerName, string dirName, string[] fileNames);
        string RenameAzureFiles(string StorageContainerName, string FilePath, string FileName, string FileNewName);

        string GetAzureFilePath(string StorageContainerName, string dirName, string fileNames);

        System.IO.Stream GetFileStream(string StorageContainerName, string dirName, string fileNames);
    }
}
