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

        private FolderSmsService folderSmsService;
        private TwilioService twilioService;

        public SmsStrategyResolver(SmsSettings smsSettings)
        {
            this.smsSettings = smsSettings;
        }

        public ISmsService Resolve()
        {
            if (smsSettings.Folder != null)
            {
                return folderSmsService ??= new FolderSmsService(smsSettings.Folder);
            }
            else
            {
                return twilioService ??= new TwilioService(smsSettings.Twilio);
            }
        }
    }
}
