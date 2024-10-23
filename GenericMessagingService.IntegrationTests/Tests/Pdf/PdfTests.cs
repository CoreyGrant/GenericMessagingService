using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.IntegrationTests.Tests.Pdf
{
    [Test("Pdf")]
    internal class PdfTests : TestBase
    {
        public PdfTests(ILogger logger, WebClientSettings clientSettings) : base(logger, clientSettings)
        {
        }

        [TestName("Test pdf")]
        public void TestPdf()
        {
            var templateName = "Test1";
            var data = new Dictionary<string, string> { ["TestData"] = "PdfTest"};
            var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var filename = $"TestOutput-{timestamp}.pdf";
            var result = PdfClient.GetPdf(templateName, data, filename).Result;
            Assert.NotNull(result);
        }
    }
}
