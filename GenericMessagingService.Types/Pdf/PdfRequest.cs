using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Pdf
{
    public class PdfRequest
    {
        public string TemplateName { get; set; }
        public IDictionary<string, string> Data { get; set; }
        public string? Filename { get; set; }
    }
}
