using GenericMessagingService.Client;
using GenericMessagingService.IntegrationTests.Attributes;
using GenericMessagingService.WebApi;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.IntegrationTests.Tests.Pdf
{
    [TestFixture("Pdf")]
    internal class PdfTests : TestBase
    {
        [Test]
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
