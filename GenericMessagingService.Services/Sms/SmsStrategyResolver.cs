using GenericMessagingService.Services.Sms.Services;
using GenericMessagingService.Services.Utils;
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
        private readonly IFileManager fileManager;
        private FolderSmsService folderSmsService;
        private TwilioService twilioService;

        public SmsStrategyResolver(SmsSettings smsSettings, IFileManager fileManager)
        {
            this.smsSettings = smsSettings;
            this.fileManager = fileManager;
        }

        public ISmsService Resolve()
        {
            if (smsSettings.Folder != null)
            {
                return folderSmsService ??= new FolderSmsService(smsSettings.Folder, fileManager);
            }
            else
            {
                return twilioService ??= new TwilioService(smsSettings.Twilio);
            }
        }
    }
}
