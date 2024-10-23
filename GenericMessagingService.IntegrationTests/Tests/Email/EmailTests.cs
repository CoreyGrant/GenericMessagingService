using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Tests.Email
{
    [Test("Email")]
    internal class EmailTests : TestBase
    {
        public EmailTests(ILogger logger, WebClientSettings clientSettings) : base(logger, clientSettings)
        {
        }

        [TestName("Test email")]
        public void TestEmail()
        {
            var toEmailAddress = "test@test.com";
            var fromEmailAddress = "test@testsender.com";
            var templateName = "Test1";
            var data = new Dictionary<string, string> { ["TestData"] = "EmailTest"};
            EmailClient.SendTemplateEmail(toEmailAddress, templateName, data, fromEmailAddress).Wait();
        }
    }
}
