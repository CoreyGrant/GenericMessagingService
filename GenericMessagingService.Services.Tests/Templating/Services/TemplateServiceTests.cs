using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Config;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.Services.Tests.Templating.Services
{
    public class TemplateServiceTests
    {
        private readonly ITemplateFormattingServiceResolver templateFormattingServiceResolver;
        private readonly ITemplateLocationServiceResolver templateLocationServiceResolver;
        private readonly ICacheManager cacheManager;
        private TemplateService sut;
        public TemplateServiceTests() 
        {
            templateFormattingServiceResolver = Substitute.For<ITemplateFormattingServiceResolver>();
            templateLocationServiceResolver = Substitute.For<ITemplateLocationServiceResolver>();
            cacheManager = Substitute.For<ICacheManager>();
        }

        [Fact]
        public async Task TestGetTemplateSuccess()
        {
            var templateName = "templateName";
            var data = new Dictionary<string, string>();
            var body = "body";
            var formattedBody = "formattedBody";
            var formattedSubject = "formattedSubject";
            var subject = "subject";
            
            var location = new TemplateLocationSettings();
            var formatting = new TemplateFormattingSettings();
            var config = new TemplateSettings
            {
                Location = location,
                Formatting = formatting
            };
            var templateLocationService = Substitute.For<ITemplateLocationService>();
            var templateFormattingService = Substitute.For<ITemplateFormattingService>();
            templateFormattingServiceResolver.Resolve(config).Returns(templateFormattingService);
            templateLocationServiceResolver.Resolve(location).Returns(templateLocationService);
            templateLocationService.LocateTemplateAsync(templateName)
                .Returns(Task.FromResult<(string, string)>((body, subject)));
            templateFormattingService.FormatTemplate(body, data)
                .Returns(Task.FromResult(formattedBody));
            templateFormattingService.FormatTemplate(subject, data)
                .Returns(Task.FromResult(formattedSubject));
            sut = new TemplateService(config, templateFormattingServiceResolver, templateLocationServiceResolver, cacheManager);
            var result = await sut.GetTemplate(new Types.Template.TemplateRequest { Data = data, TemplateName = templateName });
            Assert.Equal(formattedBody, result.Value.Body);
            Assert.Equal(formattedSubject, result.Value.Subject);
        }

        [Fact]
        public async Task TestGetTemplateCantLocate()
        {
            var templateName = "templateName";
            var data = new Dictionary<string, string>();
            string body = null;
            var subject = "subject";
            var location = new TemplateLocationSettings();
            var formatting = new TemplateFormattingSettings();
            var config = new TemplateSettings
            {
                Location = location,
                Formatting = formatting
            };
            
            var templateLocationService = Substitute.For<ITemplateLocationService>();
            var templateFormattingService = Substitute.For<ITemplateFormattingService>();
            templateFormattingServiceResolver.Resolve(config).Returns(templateFormattingService);
            templateLocationServiceResolver.Resolve(location).Returns(templateLocationService);
            templateLocationService.LocateTemplateAsync(templateName)
                .Returns(Task.FromResult<(string, string)>((body, subject)));
            sut = new TemplateService(config, templateFormattingServiceResolver, templateLocationServiceResolver, cacheManager);
            var result = await sut.GetTemplate(new Types.Template.TemplateRequest { Data = data, TemplateName = templateName });
            Assert.Null(result);
        }
    }
}
