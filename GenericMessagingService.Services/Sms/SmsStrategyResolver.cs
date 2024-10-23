using GenericMessagingService.Services.Sms.Services;
using GenericMessagingService.Services.StorageService;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Attributes;
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

    [InjectTransient(ServiceType.Sms)]
    internal class SmsStrategyResolver : ISmsStrategyResolver
    {
        private readonly SmsSettings smsSettings;
        private readonly IFileManager fileManager;
        private readonly IAzureBlobStorageServiceFactory azureBlobStorageServiceFactory;

        public SmsStrategyResolver(
            SmsSettings smsSettings,
            IFileManager fileManager,
            IAzureBlobStorageServiceFactory azureBlobStorageServiceFactory)
        {
            this.smsSettings = smsSettings;
            this.fileManager = fileManager;
            this.azureBlobStorageServiceFactory = azureBlobStorageServiceFactory;
        }

        public ISmsService Resolve()
        {
            if (smsSettings.Folder != null)
            {
                return new FileSmsService(smsSettings.Folder, new FolderStorageService(fileManager));
            }
            else if(smsSettings.AzureBlobStorage != null)
            {
                return new FileSmsService(
                    smsSettings.AzureBlobStorage,
                    azureBlobStorageServiceFactory.Create(smsSettings.AzureBlobStorage));
            }
            else if(smsSettings.Twilio != null)
            {
                return new TwilioService(smsSettings.Twilio);
            }
            throw new Exception("No Sms service configured");
        }
    }
}
