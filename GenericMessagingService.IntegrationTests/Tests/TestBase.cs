using GenericMessagingService.Client;
using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Client.Web;
using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Tests
{
    public abstract class TestBase
    {
        public ILogger logger;
        public WebClientSettings clientSettings;
        public GenericMessagingServiceSettings gmsSettings;
        private readonly ClassToDictionaryConverter classToDictConverter;

        public TestBase()
        {
            this.classToDictConverter = new ClassToDictionaryConverter();
        }

        protected IEmailClient EmailClient => new EmailClient(clientSettings, classToDictConverter);
        protected ISmsClient SmsClient => new SmsClient(clientSettings, classToDictConverter);

        protected IPdfClient PdfClient => new PdfClient(clientSettings, classToDictConverter);
        protected ITemplateClient TemplateClient => new TemplateClient(clientSettings, classToDictConverter);
    }
}
