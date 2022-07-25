using Azure;
using Azure.Storage;
using Azure.Storage.Files.Shares;
using Azure.Storage.Sas;
using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProvenCfoUI.Comman
{
    public class AzureStorageAccess : IStorageAccess
    {
        private bool isDisposed = false;
        private string _storageAccountKey;
        private string _storageAccountName;
        private string _storageConnection;
        public AzureStorageAccess(string StorageAccountName, string StorageAccountKey, string StorageConnection)
        {
            _storageAccountKey = StorageAccountKey;
            _storageAccountName = StorageAccountName;
            _storageConnection = StorageConnection;
        }
        public Uri GetFileSasUri(string StorageContainerName, string filePath, DateTime expiration, ShareFileSasPermissions permissions)
        {
            // Get the account details from app settings
            //string accountName = ConfigurationManager.AppSettings["StorageAccountName"];
            //string accountKey = ConfigurationManager.AppSettings["StorageAccountKey"];

            ShareSasBuilder fileSAS = new ShareSasBuilder()
            {
                ShareName = StorageContainerName,
                FilePath = filePath,
                // Specify an Azure file resource
                Resource = "f",
                // Expires in 24 hours
                ExpiresOn = expiration
            };

            // Set the permissions for the SAS
            fileSAS.SetPermissions(permissions);
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey);

            // Create a SharedKeyCredential that we can use to sign the SAS token
            //StorageSharedKeyCredential credential = new StorageSharedKeyCredential(accountName, accountKey);

            // Build a SAS URI
            UriBuilder fileSasUri = new UriBuilder($"https://{_storageAccountName}.file.core.windows.net/{fileSAS.ShareName}/{fileSAS.FilePath}");
            fileSasUri.Query = fileSAS.ToSasQueryParameters(credential).ToString();


            // Return the URI
            return fileSasUri.Uri;
        }

        public bool UploadFile(string StorageContainerName, string dirName, string fileName, HttpPostedFileBase inputfile)
        {
            try
            {
                //string connectionString = ConfigurationManager.ConnectionStrings["StorageConnection"].ToString();

                // Get a reference to a share and then create it
                ShareClient share = new ShareClient(_storageConnection, StorageContainerName);
                share.CreateIfNotExistsAsync();


                // Get a reference to a directory and create it


                var delimiter = new char[] { '/' };
                var nestedFolderArray = dirName.Split(delimiter);
                var concatFolder = nestedFolderArray[0];
                ShareDirectoryClient directory;
                for (var i = 0; i < nestedFolderArray.Length; i++)
                {
                    if (i > 0) concatFolder = concatFolder != "" ? concatFolder + "/" + Convert.ToString(nestedFolderArray[i]) : concatFolder;
                    directory = share.GetDirectoryClient(concatFolder);
                    if (directory.Exists().Value == false) directory.CreateIfNotExists();

                }
                directory = share.GetDirectoryClient(concatFolder);
                // Get a reference to a file and upload it
                ShareFileClient file = directory.GetFileClient(fileName);

                
                // Max. 4MB (4194304 Bytes in binary) allowed
                const int uploadLimit = 4194304;

                // Upload the file
                using (System.IO.Stream stream = inputfile.InputStream)
                {
                    file.Create(stream.Length);
                    if (stream.Length <= uploadLimit)
                    {
                        file.UploadRange(
                        new HttpRange(0, stream.Length),
                        stream);
                    }
                    else
                    {
                        int bytesRead;
                        long index = 0;
                        byte[] buffer = new byte[uploadLimit];
                        // Stream is larger than the limit so we need to upload in chunks
                        while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            // Create a memory stream for the buffer to upload
                            MemoryStream ms = new MemoryStream(buffer, 0, bytesRead);
                            file.UploadRange(new HttpRange(index, ms.Length), ms);
                            index += ms.Length; // increment the index to the account for bytes already written
                        }
                    }
                }



                //using (FileStream stream = File.OpenRead(localFilePath))
                //{
                //    await file.CreateAsync(stream.Length);
                //    await file.UploadRangeAsync(
                //        new HttpRange(0, stream.Length),
                //        stream);
                //}
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteAzureFiles(string StorageContainerName, string dirName, string[] fileNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(StorageContainerName))
                {
                    ShareClient share = new ShareClient(_storageConnection, StorageContainerName);
                    if (!string.IsNullOrEmpty(dirName))
                    {
                        var delimiter = new char[] { '/' };
                        var nestedFolderArray = dirName.Split(delimiter);
                        var concatFolder = nestedFolderArray[0];
                        ShareDirectoryClient directory;
                        for (var i = 0; i < nestedFolderArray.Length - 1; i++)
                        {
                            if (i > 0) concatFolder = concatFolder != "" ? concatFolder + "/" + Convert.ToString(nestedFolderArray[i]) : concatFolder;
                            directory = share.GetDirectoryClient(concatFolder);
                        }
                        directory = share.GetDirectoryClient(concatFolder);
                        if (fileNames != null && fileNames.Length > 0)
                        {
                            foreach (var item in fileNames)
                            {
                                ShareFileClient file = directory.GetFileClient(item);

                                var result = file.DeleteIfExists();
                                return result.Value;

                            }
                        }
                        else
                        {
                            ShareFileClient file = directory.GetFileClient(nestedFolderArray[nestedFolderArray.Count() - 1]);
                            var result = file.DeleteIfExists();
                            return result.Value;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
                throw;
            }

        }
        public string RenameAzureFiles(string StorageContainerName, string FilePath, string FileName, string FileNewName)
        {
            try
            {
                if (!string.IsNullOrEmpty(StorageContainerName))
                {
                    ShareClient share = new ShareClient(_storageConnection, StorageContainerName);
                    if (!string.IsNullOrEmpty(FilePath))
                    {
                        var delimiter = new char[] { '/' };
                        var nestedFolderArray = FilePath.Split(delimiter);
                        var concatFolder = nestedFolderArray[0];
                        ShareDirectoryClient directory;
                        for (var i = 0; i < nestedFolderArray.Length - 1; i++)
                        {
                            if (i > 0) concatFolder = concatFolder != "" ? concatFolder + "/" + Convert.ToString(nestedFolderArray[i]) : concatFolder;
                            directory = share.GetDirectoryClient(concatFolder);
                        }
                        directory = share.GetDirectoryClient(concatFolder);

                        ShareFileClient file = directory.GetFileClient(FileNewName);

                        var ssauri = GetFileSasUri(StorageContainerName, FilePath, DateTime.UtcNow.AddHours(24), ShareFileSasPermissions.Read);
                        var fileRename = file.StartCopy(ssauri);
                        DeleteAzureFiles(StorageContainerName, FilePath, new string[] { FileName });
                        return fileRename.Value.CopyStatus.ToString();
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
                throw;
            }

        }
        public string GetAzureFilePath(string StorageContainerName, string dirName, string fileNames)
        {
            try
            {
                if (!string.IsNullOrEmpty(StorageContainerName))
                {
                    ShareClient share = new ShareClient(_storageConnection, StorageContainerName);
                    if (!string.IsNullOrEmpty(dirName))
                    {
                        var delimiter = new char[] { '/' };
                        var nestedFolderArray = dirName.Split(delimiter);
                        var concatFolder = nestedFolderArray[0];
                        ShareDirectoryClient directory;
                        for (var i = 0; i < nestedFolderArray.Length - 1; i++)
                        {
                            if (i > 0) concatFolder = concatFolder != "" ? concatFolder + "/" + Convert.ToString(nestedFolderArray[i]) : concatFolder;
                            directory = share.GetDirectoryClient(concatFolder);
                        }
                        directory = share.GetDirectoryClient(concatFolder);
                        if (fileNames != null && fileNames.Length > 0)
                        {
                            foreach (var item in fileNames)
                            {
                                ShareFileClient file = directory.GetFileClient(fileNames);

                                var result = file.DeleteIfExists();
                                return string.Empty;

                            }
                        }
                        else
                        {
                            ShareFileClient file = directory.GetFileClient(nestedFolderArray[nestedFolderArray.Count() - 1]);
                            var result = file.DeleteIfExists();
                            return string.Empty;
                        }
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Empty;
                throw;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }
            this.isDisposed = true;
        }
    }
}