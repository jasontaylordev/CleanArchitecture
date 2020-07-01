using CleanArchitecture.Application.Common.Interfaces;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.QrCode
{
    public class QRGenerator : IQRGenerator
    {
        public Bitmap GenerateQRCodeBmp(string content, bool customSize = true)
        {
            int width = 200, height = 200;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            if (customSize)
            {
                Bitmap qrcodebitmap = new Bitmap(width, height);
                //RectangleF rectSerial = new RectangleF(0, 0, width, height);

                Graphics g = Graphics.FromImage(qrcodebitmap);
                g.DrawImage(qrCodeImage, 0, 0, width, height);
                return qrcodebitmap;
            }
            else
            {
                return qrCodeImage;
            }
        }

        public string GenerateQRCodeBase64(string content, bool customSize = true)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                Base64QRCode qrCode = new Base64QRCode(qrCodeData);
                return qrCode.GetGraphic(20);
            }
            catch
            {
                return "";
            }
        }

        public byte[] GenerateQRCodeBytes(string content, bool customSize = true)
        {
            int width = 200, height = 200;

            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);

            if (customSize)
            {
                Bitmap qrcodebitmap = new Bitmap(width, height);
                //RectangleF rectSerial = new RectangleF(0, 0, width, height);

                Graphics g = Graphics.FromImage(qrcodebitmap);
                g.DrawImage(qrCodeImage, 0, 0, width, height);
                using var mem = new MemoryStream();
                qrcodebitmap.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = mem.ToArray();
                return byteImage;
            }
            else
            {
                using var mem = new MemoryStream();
                qrCodeImage.Save(mem, System.Drawing.Imaging.ImageFormat.Png);
                byte[] byteImage = mem.ToArray();
                return byteImage;
            }
        }
    }
}
