using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Helpers
{
    public class HtmlHelper
    {

        public static string ConvertEmailHtmlToString(string emailHtmlRelativePath)
        {
            string actualPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, emailHtmlRelativePath);
            return File.ReadAllText(actualPath, Encoding.UTF8);
        }

        public static byte[] GetFileBytes(string pathToFile)
        {
            string actualPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pathToFile);
            return File.ReadAllBytes(actualPath);
        }
    }
}
