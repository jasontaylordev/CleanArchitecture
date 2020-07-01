using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Infrastructure.Cloud.AzureStorage;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Cloud
{
    public class CloudUploader : ICloudUploader
    {
        private readonly IConfiguration configuration;
        public CloudUploader(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        
        public Task<string> UploadFileAsync(string containerName, string fileName, Stream dataStream)
        {
            return AzureCloudBlob.GetInstance(configuration).CreateFileBlobAsync(fileName, dataStream, containerName);
        }

        public Task<string> UploadFileAsync(string containerName, string fileName, byte[] dataStream)
        {
            return AzureCloudBlob.GetInstance(configuration).CreateFileBlobBAsync(fileName, dataStream, containerName);
        }
    }
}
