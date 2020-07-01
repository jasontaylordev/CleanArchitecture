using System.Drawing;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IQRGenerator
    {
        Bitmap GenerateQRCodeBmp(string content, bool customSize = true);
        string GenerateQRCodeBase64(string content, bool customSize = true);
        byte[] GenerateQRCodeBytes(string content, bool customSize = true);
    }
}
