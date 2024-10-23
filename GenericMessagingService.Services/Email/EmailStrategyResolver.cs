using GenericMessagingService.Services.Email.Services;
using GenericMessagingService.Services.StorageService;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Email
{
    internal interface IEmailStrategyResolver
    {
        IEmailService Resolve();
    }

    [InjectTransient(ServiceType.Email)]
    internal class EmailStrategyResolver : IEmailStrategyResolver
    {
        private readonly EmailSettings settings;
        private readonly ISendGridClient sendGridClient;
        private readonly IFileManager fileManager;
        private readonly IAzureBlobStorageServiceFactory azureBlobStorageServiceFactory;

        public EmailStrategyResolver(
            EmailSettings settings,
            ISendGridClient sendGridClient,
            IFileManager fileManager,
            IAzureBlobStorageServiceFactory azureBlobStorageServiceFactory) 
        {
            this.settings = settings;
            this.sendGridClient = sendGridClient;
            this.fileManager = fileManager;
            this.azureBlobStorageServiceFactory = azureBlobStorageServiceFactory;
        }

        public IEmailService Resolve() 
        {
            if(settings.Smtp != null)
            {

            } else if(settings.MailChimp != null)
            {
                
            } else if(settings.SendGrid != null)
            {
                return new SendGridEmailService(settings.SendGrid, sendGridClient);
            } else if(settings.Folder != null)
            {
                return new FileEmailService(settings.Folder, new FolderStorageService(fileManager));
            } else if(settings.AzureBlobStorage != null)
            {
                return new FileEmailService(settings.AzureBlobStorage, azureBlobStorageServiceFactory.Create(settings.AzureBlobStorage));
            }
            throw new Exception("Email client was not configured");
        }
    }
}
