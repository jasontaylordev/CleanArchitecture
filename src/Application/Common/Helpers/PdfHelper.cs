using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;

namespace CleanArchitecture.Application.Common.Helpers
{
    public class PdfHelper
    {

        public static byte[] CombineMultiplePDFs(List<byte[]> pdfS)
        {
            // step 1: creation of a document-object
            Document document = new Document();
            //create newFileStream object which will be disposed at the end
            using var ms = new MemoryStream();
            // step 2: we create a writer that listens to the document
            PdfCopy writer = new PdfCopy(document, ms);
            if (writer == null)
            {
                return null;
            }

            // step 3: we open the document
            document.Open();

            foreach (var pdf in pdfS)
            {
                if (pdf != null && pdf.Length > 0)
                {
                    // we create a reader for a certain document
                    PdfReader reader = new PdfReader(pdf);
                    reader.ConsolidateNamedDestinations();

                    // step 4: we add content
                    for (int i = 1; i <= reader.NumberOfPages; i++)
                    {
                        PdfImportedPage page = writer.GetImportedPage(reader, i);
                        writer.AddPage(page);
                    }

                    PrAcroForm form = reader.AcroForm;
                    if (form != null)
                    {
                        writer.CopyAcroForm(reader);
                    }

                    reader.Close();
                }
            }

            // step 5: we close the document and writer
            writer.Close();
            document.Close();
            return ms.ToArray();
            //disposes the newFileStream object
        }
    }
}
