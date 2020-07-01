using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Models
{
    public class AttachmentModel
    {
        public string Name { get; set; }
        public string ContentType { get; set; }
        public byte[] File { get; set; }

        public string Url { get; set; } //for one pdf usage
    }
}
