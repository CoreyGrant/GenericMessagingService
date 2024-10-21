using GenericMessagingService.Services.Cache;
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
        private ICacheManager cacheManager;
        private IHashService hashService;

        public RazorTemplateFormattingServiceTests() 
        {
            razorEngine = new RazorEngine();
            hashService = Substitute.For<IHashService>();
            cacheManager = Substitute.For<ICacheManager>();
            var config = new RazorTemplateFormattingSettings { };
            var templateSettings = new TemplateSettings { };
            sut = new RazorTemplateFormattingService(templateSettings, config, razorEngine, cacheManager, hashService);
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
