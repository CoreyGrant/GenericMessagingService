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
    public class TemplateFormattingServiceResolverTests
    {
        private TemplateFormattingServiceResolver sut;
        
        public TemplateFormattingServiceResolverTests()
        {
            var re = Substitute.For<IRazorEngine>();
            var cacheManager = Substitute.For<ICacheManager>();
            var hashService = Substitute.For<IHashService>();
            sut = new TemplateFormattingServiceResolver(re, hashService, cacheManager);
        }

        [Fact]
        public async Task TestTemplateFormattingServiceResolverBasic()
        {
            var config = new TemplateSettings { Formatting = new TemplateFormattingSettings { Basic = new BasicTemplateFormattingSettings { } } };
            var service = sut.Resolve(config);
            Assert.True(service is BasicTemplateFormattingService);
        }

        [Fact]
        public async Task TestTemplateFormattingServiceResolverRazor()
        {
            var config = new TemplateSettings { Formatting = new TemplateFormattingSettings { Razor = new RazorTemplateFormattingSettings { } } };
            var service = sut.Resolve(config);
            Assert.True(service is RazorTemplateFormattingService);
        }
    }
}
