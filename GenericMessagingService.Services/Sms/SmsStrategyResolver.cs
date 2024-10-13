using GenericMessagingService.Services.Sms.Services;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Sms
{
    public interface ISmsStrategyResolver
    {
        ISmsService Resolve();
    }

    internal class SmsStrategyResolver : ISmsStrategyResolver
    {
        private readonly SmsSettings smsSettings;

        public SmsStrategyResolver(SmsSettings smsSettings)
        {
            this.smsSettings = smsSettings;
        }

        public ISmsService Resolve()
        {
            if (smsSettings.Folder != null)
            {
                return new FolderSmsService(smsSettings.Folder);
            }
            else
            {
                return new TwilioService(smsSettings.Twilio);
            }
        }
    }
}
