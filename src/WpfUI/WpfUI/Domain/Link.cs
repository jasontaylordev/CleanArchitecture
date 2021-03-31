using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace WpfUI.Domain
{
    public static class Link
    {
        public static void OpenInBrowser(string url)
        {
            var psi = new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
