using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Sms
{
    public class SmsRequest
    {
        public string? Template { get; set; }
        public string? TemplateName { get; set; }
        public IList<string> To { get; set; }
        public string? From { get; set; }
        public IDictionary<string, string>? Data { get; set; }
        public IDictionary<string, IDictionary<string, string>>? ToData { get; set; }
    }
}
