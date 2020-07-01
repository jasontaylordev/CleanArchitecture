using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace CleanArchitecture.Application.Common.Helpers.ExcelHelper
{
    public class ExcelWriter : IDisposable
    {
        private readonly StreamWriter streamWriter;
        public ExcelWriter(StreamWriter streamWriter)
        {
            this.streamWriter = streamWriter;
        }

        public void Dispose()
        {
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }

        public void WriteSingleSheetRecords<T>(string sheetName, IEnumerable<T> records, bool includeIndex = false)
        {
            WriteRecords(new List<ExcelWriterModel<T>> { new ExcelWriterModel<T> { Models = records, SheetName = sheetName } }, includeIndex);
        }

        public void WriteRecords<T>(IEnumerable<ExcelWriterModel<T>> records, bool includeIndex = false)
        {
            using var p = new ExcelPackage();
            foreach (var item in records)
            {
                p.Workbook.Worksheets.Add(item.SheetName);
                ExcelWorksheet ws = p.Workbook.Worksheets[item.SheetName];
                ws.PrinterSettings.FitToPage = true;
                ws.Cells.Style.Font.Size = 11;
                ws.Cells.Style.Font.Name = "Calibri";
                //set header
                PropertyInfo[] properties = typeof(T).GetProperties();
                int colIndex = 1;

                if (includeIndex)
                {
                    ws.Cells[1, colIndex].Value = "#";
                    colIndex++;
                }

                foreach (PropertyInfo property in properties)
                {
                    ExcelIgnore ignoreAttribute = (ExcelIgnore)property.GetCustomAttribute(typeof(ExcelIgnore));
                    if (ignoreAttribute is null)
                    {
                        ws.Cells[1, colIndex].Value = property.Name;
                        colIndex++;
                    }
                }

                int rowIndex = 2; 
                foreach (var v in item.Models)
                {
                    colIndex = 1;

                    if (includeIndex)
                    {
                        ws.Cells[rowIndex, colIndex].Value = rowIndex - 1;
                        colIndex++;
                    }

                    foreach (PropertyInfo property in properties)
                    {
                        try
                        {
                            var propertyInfo = v.GetType().GetProperty(property.Name);
                            ExcelIgnore ignoreAttribute = (ExcelIgnore)property.GetCustomAttribute(typeof(ExcelIgnore));
                            if (ignoreAttribute is null)
                            {
                                var value = propertyInfo.GetValue(v, null)?.ToString();
                                ws.Cells[rowIndex, colIndex].Value = value;
                                colIndex++;
                            }
                        }
                        catch //(Exception ex)
                        {
                        }
                    }

                    rowIndex++;
                }
            }

            var data = p.GetAsByteArray();
            streamWriter.BaseStream.Write(data, 0, data.Length);
        }
    }
}
