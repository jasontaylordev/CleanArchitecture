using CleanArchitecture.Application.Common.Interfaces;
using DinkToPdf;
using DinkToPdf.Contracts;

namespace CleanArchitecture.Infrastructure.PDF
{
    public class PDFConverterHelper : IPDFConverter
    {
        private readonly IConverter _synchronizedConverter;

        public PDFConverterHelper(IConverter synchronizedConverter)
        {
            _synchronizedConverter = synchronizedConverter;
        }

        public byte[] ConvertToPDF(string html, bool pageNumber = false, string paperSize = "A4", bool noMargin = false)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = paperSize.Equals("A4") ? PaperKind.A4 : PaperKind.A5,
                    //Margins = new MarginSettings() { Top = 10 },
                },
                Objects =
                    {
                        new ObjectSettings()
                        {
                            HtmlContent = html,
                            PagesCount = true,
                            WebSettings = { DefaultEncoding = "utf-8" },
                            FooterSettings = pageNumber ? new FooterSettings { FontName = "Arial", FontSize = 7, Right = "Page [page] of [toPage]" } : new FooterSettings(),
                            //Page = uri, if want to use by url
                        },
                    }
            };
            doc.GlobalSettings.Margins = noMargin ? new MarginSettings { Bottom = 0, Left = 0, Right = 0, Top = 0, Unit = Unit.Centimeters } : doc.GlobalSettings.Margins;
            var result = _synchronizedConverter.Convert(doc);
            return result;
        }

        public byte[] ConvertToPDFCustomSize(string html, bool defaultPaper = true, double marginLeft = 30, double marginRight = 30, string paperWidth = "768", string paperHeight = "0", bool pageNumber = false)
        {
            var doc = new HtmlToPdfDocument
            {
                GlobalSettings =
                    {
                        ColorMode = ColorMode.Color,
                        Orientation = Orientation.Portrait,
                        PaperSize = defaultPaper ? PaperKind.A4 : new PechkinPaperSize(width: paperWidth, height: paperHeight),
                        Margins = new MarginSettings { Left = marginLeft, Right = marginRight},
                    },
                Objects =
                    {
                        new ObjectSettings
                        {
                            HtmlContent = html,
                            PagesCount = true,
                            FooterSettings = pageNumber ? new FooterSettings { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true } : new FooterSettings(),
                            WebSettings = { DefaultEncoding = "utf-8" }
                        }
                    }
            };

            return _synchronizedConverter.Convert(doc);
        }
    }
}
