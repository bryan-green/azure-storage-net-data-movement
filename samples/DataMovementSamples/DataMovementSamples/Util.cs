//------------------------------------------------------------------------------
// <copyright file="Util.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation
// </copyright>
//------------------------------------------------------------------------------
namespace DataMovementSamples
{
    using System;
    using Microsoft.Azure.Storage;
    using Microsoft.Azure.Storage.Blob;
    using Microsoft.Azure.Storage.File;
    using System.Threading.Tasks;
    using System.IO;
    using Newtonsoft.Json.Linq;
    
    /// <summary>
    /// A helper class provides convenient operations against storage account configured in the App.config.
    /// </summary>
    public class Util
    {
        private static CloudStorageAccount storageAccount;
        private static CloudBlobClient blobClient;
        private static CloudFileClient fileClient;

        /// <summary>
        /// Get a CloudBlob instance with the specified name and type in the given container.
        /// </summary>
        /// <param name="containerName">Container name.</param>
        /// <param name="blobName">Blob name.</param>
        /// <param name="blobType">Type of blob.</param>
        /// <returns>A <see cref="Task{T}"/> object of type <see cref="CloudBlob"/> that represents the asynchronous operation.</returns>
        public static async Task<CloudBlob> GetCloudBlobAsync(string containerName, string blobName, BlobType blobType)
        {
            CloudBlobClient client = GetCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            CloudBlob cloudBlob;
            switch (blobType)
            {
                case BlobType.AppendBlob:
                    cloudBlob = container.GetAppendBlobReference(blobName);
                    break;
                case BlobType.BlockBlob:
                    cloudBlob = container.GetBlockBlobReference(blobName);
                    break;
                case BlobType.PageBlob:
                    cloudBlob = container.GetPageBlobReference(blobName);
                    break;
                case BlobType.Unspecified:
                default:
                    throw new ArgumentException(string.Format("Invalid blob type {0}", blobType.ToString()), "blobType");
            }

            return cloudBlob;
        }

        /// <summary>
        /// Get a CloudBlobDirectory instance with the specified name in the given container.
        /// </summary>
        /// <param name="containerName">Container name.</param>
        /// <param name="directoryName">Blob directory name.</param>
        /// <returns>A <see cref="Task{T}"/> object of type <see cref="CloudBlobDirectory"/> that represents the asynchronous operation.</returns>
        public static async Task<CloudBlobDirectory> GetCloudBlobDirectoryAsync(string containerName, string directoryName)
        {
            CloudBlobClient client = GetCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            return container.GetDirectoryReference(directoryName);
        }

        /// <summary>
        /// Get a CloudFile instance with the specified name in the given share.
        /// </summary>
        /// <param name="shareName">Share name.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>A <see cref="Task{T}"/> object of type <see cref="CloudFile"/> that represents the asynchronous operation.</returns>
        public static async Task<CloudFile> GetCloudFileAsync(string shareName, string fileName)
        {
            CloudFileClient client = GetCloudFileClient();
            CloudFileShare share = client.GetShareReference(shareName);
            await share.CreateIfNotExistsAsync();

            CloudFileDirectory rootDirectory = share.GetRootDirectoryReference();
            return rootDirectory.GetFileReference(fileName);
        }

        /// <summary>
        /// Delete the share with the specified name if it exists.
        /// </summary>
        /// <param name="shareName">Name of share to delete.</param>
        public static async Task DeleteShareAsync(string shareName)
        {
            CloudFileClient client = GetCloudFileClient();
            CloudFileShare share = client.GetShareReference(shareName);
            await share.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Delete the container with the specified name if it exists.
        /// </summary>
        /// <param name="containerName">Name of container to delete.</param>
        public static async Task DeleteContainerAsync(string containerName)
        {
            CloudBlobClient client = GetCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(containerName);
            await container.DeleteIfExistsAsync();
        }

        private static CloudBlobClient GetCloudBlobClient()
        {
            if (Util.blobClient == null)
            {
                Util.blobClient = GetStorageAccount().CreateCloudBlobClient();
            }

            return Util.blobClient;
        }

        private static CloudFileClient GetCloudFileClient()
        {
            if (Util.fileClient == null)
            {
                Util.fileClient = GetStorageAccount().CreateCloudFileClient();
            }
            
            return Util.fileClient;
        }

        private static string LoadConnectionStringFromConfigration()
        {
            // How to create a storage connection string: http://msdn.microsoft.com/en-us/library/azure/ee758697.aspx
#if DOTNET5_4
            //For .Net Core,  will get Storage Connection string from Config.json file
            return JObject.Parse(File.ReadAllText("Config.json"))["StorageConnectionString"].ToString(); 
#else
            //For .net, will get Storage Connection string from App.Config file
            return System.Configuration.ConfigurationManager.AppSettings["StorageConnectionString"];
#endif
        }

        private static CloudStorageAccount GetStorageAccount()
        {
            if (Util.storageAccount == null)
            {
                string connectionString = LoadConnectionStringFromConfigration();
                Util.storageAccount = CloudStorageAccount.Parse(connectionString);
            }

            return Util.storageAccount;
        }
    }
}
