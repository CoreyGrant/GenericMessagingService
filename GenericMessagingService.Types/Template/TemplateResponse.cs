using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Template
{
    public class TemplateResponse
    {
        public string Body { get; set; }

        /// <summary>
        /// Some email templates contain the subject as well as the template body
        /// </summary>
        public string? Subject { get; set; }
    }
}
