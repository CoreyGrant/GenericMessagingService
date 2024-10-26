using GenericMessagingService.Client;
using GenericMessagingService.Client.Web;
using GenericMessagingService.IntegrationTests.Assertions;
using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.WebApi;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.IntegrationTests.Tests.Full
{
    [TestFixture("Full")]
    public class FullTests : TestBase
    {
        [Test]
        public void TestFull()
        {
            var to = "test@test.com";
            var from = "test@test-sender.com";

            var emailTemplate1Name = "Test1.cshtml";
            var emailTemplate1Data = new Dictionary<string, string> { ["TestData"] = "TestData" };

            var email1Result = EmailClient.SendTemplateEmail(to, emailTemplate1Name, emailTemplate1Data, from).Result;
            Assert.True(email1Result);
        }

        [Test]
        public void TestEmail2()
        {
            var to = "test@test.com";
            var from = "test@test-sender.com";

            var emailTemplate2Name = "Test2.txt";
            var emailTemplate2Data = new Dictionary<string, string> { ["TestData"] = "TestData" };

            var email2Result = EmailClient.SendTemplateEmail(to, emailTemplate2Name, emailTemplate2Data, from).Result;
            Assert.True(email2Result);
        }

        [Test]
        public void TestSms()
        {
            var toNumber = "+7111111111";
            var fromNumber = "+7222222222";

            var smsTemplateName = "Test1.txt";
            var smsTemplateData = new Dictionary<string, string> { ["TestData"] = "TestData" };

            var smsResult = SmsClient.SendTemplateSms(toNumber, smsTemplateName, smsTemplateData, fromNumber).Result;
            Assert.True(smsResult);
        }

        [Test]
        public void TestPdf() 
        { 
            var pdfTemplateName = "Test1.cshtml";
            var pdfTemplateData = new Dictionary<string, string> { ["TestData"] = "TestData" };
            var pdfFilename = "TestPdf.pdf";

            var pdfResult = PdfClient.GetPdf(pdfTemplateName, pdfTemplateData, pdfFilename).Result;
            Assert.NotNull(pdfResult);
            CustomAssert.FileExists(this.gmsSettings.Config.Pdf!.Folder!.FolderPath, pdfFilename);
        }
    }
}
