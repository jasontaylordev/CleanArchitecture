using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Cloud.AzureStorage
{
    public class AzureCloudBlob
    {
        #region Singleton

        private const string ConnectionStringName = "ConnectionStrings:azurestorage";

        public string BaseUri { get; set; }
        private static volatile AzureCloudBlob instance;
        private static readonly object syncRoot = new object();

        public static string CONTAINER_FILE_PUBLIC = "publicfiles";
        public static string CONTAINER_IMAGE_PUBLIC = "publicimages";
        public static string CONTAINER_IMAGE_PRIVATE = "privateimages";
        private readonly CloudStorageAccount storageAccount;


        public static AzureCloudBlob GetInstance(IConfiguration configuration)
        {
            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                    {
                        instance = new AzureCloudBlob(configuration);
                    }
                }
            }

            return instance;
        }

        private AzureCloudBlob(IConfiguration configuration)
        {
            var setting = configuration[ConnectionStringName];
            storageAccount = CloudStorageAccount.Parse(setting);
            BaseUri = BlobClient.BaseUri.ToString();
        }

        #endregion Singleton

        private CloudBlobClient BlobClient
        {
            get
            {
                // always create new client to handle new request so that if any crashes happen to this instance it will affect other request
                return storageAccount.CreateCloudBlobClient();
            }
        }

        #region Blob

        public async Task<string> CreateFileBlobAsync(string fileName, Stream dataStream, string containerName = null)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                containerName = CONTAINER_FILE_PUBLIC;
            }

            var container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            // configure container for public access
            var permissions = await container.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            await container.SetPermissionsAsync(permissions);

            var fileExtension = Path.GetExtension(fileName);

            var blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileExtension);
            await blob.UploadFromStreamAsync(dataStream);

            return blob.Uri.ToString();
        }

        public async Task<string> CreateFileBlobBAsync(string fileName, byte[] dataStream, string containerName = null)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                containerName = CONTAINER_FILE_PUBLIC;
            }

            var container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            // configure container for public access
            var permissions = await container.GetPermissionsAsync();
            permissions.PublicAccess = BlobContainerPublicAccessType.Container;
            await container.SetPermissionsAsync(permissions);

            var fileExtension = Path.GetExtension(fileName);

            var blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileExtension);
            await blob.UploadFromByteArrayAsync(dataStream, 0, dataStream.Length);

            return blob.Uri.ToString();
        }


        public async Task<(string fileUrl, string sasToken)> CreateFileBlobWithSASAsync(string containerName, string fileName, Stream dataStream, bool isParent = true, int validityHour = 12, int? validityMinute = 20)
        {
            var sasName = Path.GetFileNameWithoutExtension(fileName);
            string sasContainerToken = "";

            var container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            if (isParent)
            {
                // Create a new shared access policy and define its constraints.
                //the access policy provides create, write, read, list, and delete permissions.
                SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = validityMinute == null ? DateTime.UtcNow.AddHours(validityHour) : DateTime.UtcNow.AddHours(validityHour).AddMinutes(validityMinute.Value),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
                };

                // configure container for public access
                var permissions = await container.GetPermissionsAsync();

                try
                {
                    // Clear any existing access policies on container if any
                    BlobContainerPermissions existingPermission = await container.GetPermissionsAsync();
                    if (existingPermission.SharedAccessPolicies.Count >= 0)
                    {
                        existingPermission.SharedAccessPolicies.Remove(sasName);
                    }

                    permissions.SharedAccessPolicies.Add(sasName, adHocPolicy);
                    await container.SetPermissionsAsync(permissions);
                }
                catch (Exception)
                {
                }

                sasContainerToken = container.GetSharedAccessSignature(adHocPolicy, null);
            }

            var fileExtension = Path.GetExtension(fileName);

            var blob = container.GetBlockBlobReference(fileName);
            blob.Properties.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileExtension);
            await blob.UploadFromStreamAsync(dataStream);
            return (blob.Uri.ToString(), sasContainerToken);
        }

        public async Task<(List<string> fileUrls, string sasToken)> CreateMultiFileBlobWithSASAsync(string containerName, string sasName, List<string> fileNames, List<Stream> dataStreams, bool isParent = true)
        {
            string sasContainerToken = "";

            var container = BlobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();

            if (isParent)
            {
                // Create a new shared access policy and define its constraints.
                //the access policy provides create, write, read, list, and delete permissions.
                SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
                {
                    // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                    // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                    SharedAccessExpiryTime = DateTime.UtcNow.AddHours(12),
                    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
                };

                // configure container for public access
                var permissions = await container.GetPermissionsAsync();

                try
                {
                    // Clear any existing access policies on container if any
                    BlobContainerPermissions existingPermission = await container.GetPermissionsAsync();
                    if (existingPermission.SharedAccessPolicies.Count >= 0)
                    {
                        existingPermission.SharedAccessPolicies.Remove(sasName);
                    }

                    permissions.SharedAccessPolicies.Add(sasName, adHocPolicy);
                    await container.SetPermissionsAsync(permissions);
                }
                catch (Exception)
                {
                }

                sasContainerToken = container.GetSharedAccessSignature(adHocPolicy, null);
            }

            List<string> blobUris = new List<string>();

            for (int i = 0, total = dataStreams.Count; i < total; i++)
            {
                var fileName = fileNames[i];
                var dataStream = dataStreams[i];
                var fileExtension = Path.GetExtension(fileName);

                var blob = container.GetBlockBlobReference(fileName);
                blob.Properties.ContentType = MimeTypes.MimeTypeMap.GetMimeType(fileExtension);
                await blob.UploadFromStreamAsync(dataStream);
                string blobUri = blob.Uri.ToString();
                blobUris.Add(blobUri);
            }

            return (blobUris, sasContainerToken);
        }

        //internal Task<string> CreateFileBlobAsync(string containerName, string fileName, Stream pdfStream)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<string> RenewBlobSASPolicyAsync(string containerName, string fileName)
        {
            var sasName = Path.GetFileNameWithoutExtension(fileName);

            var container = BlobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);

            // Clear any existing access policies on container.
            BlobContainerPermissions permissions = await container.GetPermissionsAsync();
            permissions.SharedAccessPolicies.Remove(sasName);
            await container.SetPermissionsAsync(permissions);

            // The access policy provides create, write, read, list, and delete permissions.
            SharedAccessBlobPolicy adHocPolicy = new SharedAccessBlobPolicy()
            {
                // When the start time for the SAS is omitted, the start time is assumed to be the time when the storage service receives the request.
                // Omitting the start time for a SAS that is effective immediately helps to avoid clock skew.
                SharedAccessExpiryTime = DateTime.UtcNow.AddHours(12),
                Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
            };

            permissions.SharedAccessPolicies.Add(sasName, adHocPolicy);
            await container.SetPermissionsAsync(permissions);
            return container.GetSharedAccessSignature(adHocPolicy, null);
        }

        public async Task<bool> DeleteBlobIfExistAsync(string blobUri)
        {
            if (blobUri.Contains(BaseUri))
            {
                var uri = new Uri(blobUri);
                string containerName = uri.Segments[1].Replace("/", "");// remove last '/'
                string blobName = uri.AbsolutePath.Replace(uri.Segments[0] + uri.Segments[1], "");

                var blobCointainer = BlobClient.GetContainerReference(containerName);
                var blob = blobCointainer.GetBlockBlobReference(blobName);
                var result = await blob.DeleteIfExistsAsync();
                return result;
            }

            return false;
        }

        public async Task<Stream> GetBlobAsStreamAsync(string containerName, string fileName)
        {
            var container = BlobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);

            Stream fileStream = new MemoryStream();
            await blob.DownloadToStreamAsync(fileStream);
            return fileStream;
        }

        #endregion Blob
    }
}
