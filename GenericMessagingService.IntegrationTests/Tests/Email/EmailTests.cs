using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.WebApi;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.IntegrationTests.Tests.Email
{
    [TestFixture("Email")]
    internal class EmailTests : TestBase
    {
        [Test]
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
