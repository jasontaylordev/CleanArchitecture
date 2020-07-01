using CleanArchitecture.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IPDFConverter
    {
        byte[] ConvertToPDF(string html, bool pageNumber = false, string paperSize = "A4", bool noMargin = false);
        byte[] ConvertToPDFCustomSize(string html, bool defaultPaper = true, double marginLeft = 30, double marginRight = 30, string paperWidth = "768", string paperHeight = "0", bool pageNumber = false);

    }
}
