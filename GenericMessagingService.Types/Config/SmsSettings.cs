using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class SmsSettings
    {
        public string? OverrideToNumber { get; set; }
        public string? DefaultFromNumber { get; set; }
        public TwilioSettings Twilio { get; set; }
    }

    public class TwilioSettings
    {
        public string AccountSid { get; set; }
        public string AuthToken { get; set; }
    }
}
