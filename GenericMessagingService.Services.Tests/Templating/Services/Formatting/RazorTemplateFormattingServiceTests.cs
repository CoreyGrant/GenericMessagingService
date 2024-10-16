using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Types.Config;
using NSubstitute;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.Services.Tests.Templating.Services.Formatting
{
    public class RazorTemplateFormattingServiceTests
    {
        private RazorTemplateFormattingService sut;
        private IRazorEngine razorEngine;

        public RazorTemplateFormattingServiceTests() 
        {
            razorEngine = new RazorEngine();
            var config = new RazorTemplateFormattingSettings { };
            sut = new RazorTemplateFormattingService(config, razorEngine);
        }

        [Fact]
        public async Task TestRazorTemplateFormatting()
        {
            var template = $@"@Model[""ValueOne""]-@Model[""ValueTwo""]-@Model[""ValueThree""]";
            var data = new Dictionary<string, string>
            {
                ["ValueOne"] = "ONE",
                ["ValueTwo"] = "TWO",
                ["ValueThree"] = "THREE"
            };
            var result = await sut.FormatTemplate(template, data);
            Assert.Equal("ONE-TWO-THREE", result);
        }

    }
}
