using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Template
{
    public class TemplateRequest
    {
        public string TemplateName { get; set; }
        public Dictionary<string, string> Data { get; set; }
    }
}
