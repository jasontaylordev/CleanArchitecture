using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Models
{
    public class PDFFooterSetting
    {
        public string Right { get; set; }
        public string Left { get; set; }
        public string Center { get; set; }
        public string PageNoPosition { get; set; } // none, right, center, left
        public bool Line { get; set; }
        public int FontSize { get; set; }
    }

    public class PDFHeaderSetting
    {
        public string Right { get; set; }
        public string Left { get; set; }
        public string Center { get; set; }
        public int FontSize { get; set; }
    }
}
