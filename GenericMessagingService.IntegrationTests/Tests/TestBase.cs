using GenericMessagingService.Client;
using GenericMessagingService.Client.Interfaces;
using GenericMessagingService.Client.Utils;
using GenericMessagingService.Client.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Tests
{
    public abstract class TestBase
    {
        protected readonly ILogger logger;
        protected readonly WebClientSettings clientSettings;
        private readonly ClassToDictionaryConverter classToDictConverter;

        public TestBase(ILogger logger, WebClientSettings clientSettings)
        {
            this.logger = logger;
            this.clientSettings = clientSettings;
            this.classToDictConverter = new ClassToDictionaryConverter();
        }

        protected IEmailClient EmailClient => new EmailClient(clientSettings, classToDictConverter);
        protected ISmsClient SmsClient => new SmsClient(clientSettings, classToDictConverter);
        protected ITemplateClient TemplateClient => new TemplateClient(clientSettings, classToDictConverter);
    }
}
