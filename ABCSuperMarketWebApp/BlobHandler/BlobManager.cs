using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ABCSuperMarketWebApp.BlobHandler
{
    public class BlobManager
    {
        private CloudBlobContainer blobContainer;

        public BlobManager(string ContainerName)
        {
            //check if container name is null or empty

            if (string.IsNullOrEmpty(ContainerName))
            {
                throw new ArgumentNullException("Container", "Container name cannot be empty");

            }
            try
            {
                //Get azure blob storage connection string
                string ConnectionSting = "DefaultEndpointsProtocol=https;AccountName=abcsupermarketstorageac;AccountKey=Y0C3YTA1THH6XzJeLH6rCnvwwWSnT3PQpJb5DaoCzTV76uXa92lcM7ODXIJmPumPj6WajSrAYzo7cIEE3tRNlA==;EndpointSuffix=core.windows.net";
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConnectionSting);

                //create blob if it does no exist 
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();
                blobContainer = cloudBlobClient.GetContainerReference(ContainerName);

                //create container and set the permissions
                if (blobContainer.CreateIfNotExists())
                {
                    blobContainer.SetPermissions(
                        new BlobContainerPermissions
                        {
                            PublicAccess = BlobContainerPublicAccessType.Container
                        }
                        );
                }
            }
            catch(Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //upload item image/file

        public string UploadFile(HttpPostedFileBase FileToUpload)
        {
            string AbsoluteUri;
            //check if the posted file base is null or not if not upload
            if (FileToUpload == null || FileToUpload.ContentLength == 0)
                return null;
            try
            {
                string FileName = Path.GetFileName(FileToUpload.FileName);

                //create the block blob
                CloudBlockBlob blockBlob;
                blockBlob = blobContainer.GetBlockBlobReference(FileName);
                //set object content type
                blockBlob.Properties.ContentType = FileToUpload.ContentType;
                //upload blob
                blockBlob.UploadFromStream(FileToUpload.InputStream);

                //get the Uri or filepath
                AbsoluteUri = blockBlob.Uri.AbsoluteUri;
            }
            catch(Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
            return AbsoluteUri;
        }

        //Delete Blob
        public bool DeleteBlob(string AbsoluteUri)
        {
            try
            {
                Uri uriObj = new Uri(AbsoluteUri);
                string BlobName = Path.GetFileName(uriObj.LocalPath);

                //get blob reference
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(BlobName);
                //delete blob from container
                blockBlob.Delete();
                return true;
            }
            catch (Exception ExceptionObj)
            {
                throw ExceptionObj;
            }
        }

        //Retrieve Blob
        public List<string> BlobList()
        {
            List<string> _blobList = new List<string>();
            foreach (IListBlobItem item in blobContainer.ListBlobs())
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob _blobPage = (CloudBlockBlob)item;
                    _blobList.Add(_blobPage.Uri.AbsoluteUri.ToString());
                }
            }
            return _blobList;
        }

    }
}