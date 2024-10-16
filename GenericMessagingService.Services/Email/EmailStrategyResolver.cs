using GenericMessagingService.Services.Email.Services;
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

    internal class EmailStrategyResolver : IEmailStrategyResolver
    {
        private readonly EmailSettings settings;
        private readonly ISendGridClient sendGridClient;

        private SendGridEmailService sendGridEmailService;
        private FolderEmailService folderEmailService;

        public EmailStrategyResolver(
            EmailSettings settings,
            ISendGridClient sendGridClient) 
        {
            this.settings = settings;
            this.sendGridClient = sendGridClient;
        }

        public IEmailService Resolve() 
        {
            if(settings.Smtp != null)
            {

            } else if(settings.MailChimp != null)
            {
                
            } else if(settings.SendGrid != null)
            {
                return sendGridEmailService ??= new SendGridEmailService(settings.SendGrid, sendGridClient);
            } else if(settings.Folder != null)
            {
                return folderEmailService ??= new FolderEmailService(settings.Folder);
            }
            throw new Exception("Email client was not configured");
        }
    }
}
