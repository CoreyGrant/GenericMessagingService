using GenericMessagingService.Types.Config;
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

        public EmailStrategyResolver(EmailSettings settings) 
        {
            this.settings = settings;
        }

        public IEmailService Resolve() 
        {
            if(settings.Smtp != null)
            {

            } else if(settings.MailChimp != null)
            {
                
            } else if(settings.SendGrid != null)
            {
                return new SendGridEmailService(settings.SendGrid);
            } else if(settings.Folder != null)
            {
                return new FolderEmailService(settings.Folder);
            }
            throw new Exception("Email client was not configured");
        }
    }
}
