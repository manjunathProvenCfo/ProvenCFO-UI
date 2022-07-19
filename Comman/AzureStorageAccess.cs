﻿using Azure;
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
        public AzureStorageAccess(string StorageAccountName,string StorageAccountKey,string StorageConnection)
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
            return  fileSasUri.Uri;
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
                    if(directory.Exists().Value == false)  directory.CreateIfNotExists();
                    
                }
                directory = share.GetDirectoryClient(concatFolder);
                // Get a reference to a file and upload it
                ShareFileClient file = directory.GetFileClient(fileName);
                using (System.IO.Stream stream = inputfile.InputStream)  
                {
                    file.Create(inputfile.InputStream.Length);
                    file.UploadRange(
                        new HttpRange(0, inputfile.InputStream.Length),
                        stream);

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