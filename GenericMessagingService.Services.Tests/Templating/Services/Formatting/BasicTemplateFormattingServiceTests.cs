using GenericMessagingService.Services.Templating.Services.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.Services.Tests.Templating.Services.Formatting
{
    public class BasicTemplateFormattingServiceTests
    {
        private BasicTemplateFormattingService sut;
        public BasicTemplateFormattingServiceTests() 
        {
            sut = new BasicTemplateFormattingService();
        }

        [Fact]
        public async Task TestBasicTemplating()
        {
            var templateString = @"{ValueOne}-{ValueTwo}-{ValueThree}";
            var data = new Dictionary<string, string>
            {
                ["ValueOne"] = "ONE",
                ["ValueTwo"] = "TWO",
                ["ValueThree"] = "THREE",
            };
            var result = await sut.FormatTemplate(templateString, data);
            Assert.Equal("ONE-TWO-THREE", result);
        }
    }
}
