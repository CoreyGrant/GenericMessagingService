using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Types.Config
{
    public class EmailSettings
    {
        public string? TemplateStrategy { get; set; }
        public string? OverrideToAddress { get; set; }
        public string? DefaultFromAddress { get; set; }
        /// <summary>
        /// Pipe seperated string
        /// Will try to use the strategies in order, if the first fails to find will use the second, and so on.
        /// </summary>
        public SMTPEmailSettings? Smtp { get; set; }
        public MailChimpSettings? MailChimp { get; set; }
        public SendGridSettings? SendGrid { get; set; }
        public FolderSettings? Folder { get; set; }
        public AzureBlobStorageSettings? AzureBlobStorage { get; set; }
        
    }

    public class SMTPEmailSettings
    {

    }

    public class MailChimpSettings
    {

    }

    public class SendGridSettings
    {
        public string? ApiKey { get; set; }
    }
}
