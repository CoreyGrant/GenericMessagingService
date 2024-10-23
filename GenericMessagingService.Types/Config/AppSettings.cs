using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class AppSettings
    {
        public EmailSettings? Email { get; set; }
        public SmsSettings? Sms { get; set; }
        public TemplateSettings? Template { get; set; }
        public ComboTemplateSettings? ComboTemplate { get; set; }
        public PdfSettings Pdf { get; set; }
    }
}
