using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Helpers.ExcelHelper
{
    public class ExcelWriterModel<T>
    {
        public IEnumerable<T> Models { get; set; }
        public string SheetName { get; set; }
    }
}
