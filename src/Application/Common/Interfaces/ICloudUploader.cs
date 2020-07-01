using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICloudUploader
    {
        Task<string> UploadFileAsync(string containerName, string fileName, Stream dataStream);
        Task<string> UploadFileAsync(string containerName, string fileName, byte[] dataStream);
    }
}
