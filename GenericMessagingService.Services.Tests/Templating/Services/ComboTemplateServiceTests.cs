using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.Services.Tests.Templating.Services
{
    public class ComboTemplateServiceTests
    {
        private ComboTemplateService sut;
        private ITemplateServiceFactory templateServiceFactory;
        public ComboTemplateServiceTests() 
        {
            templateServiceFactory = Substitute.For<ITemplateServiceFactory>();
        }

        [Fact]
        public async Task TestGetTemplateResolvedFirst()
        {
            var template1Settings = new TemplateSettings();
            var template2Settings = new TemplateSettings();
            var comboSettings = new ComboTemplateSettings
            {
                Strategy = "a|b",
                Combo = new Dictionary<string, TemplateSettings>
                {
                    ["a"] = template1Settings,
                    ["b"] = template2Settings
                }
            };
            var service1 = Substitute.For<ITemplateService>();
            var service2 = Substitute.For<ITemplateService>();
            templateServiceFactory.CreateTemplateService(template1Settings).Returns(service1);
            templateServiceFactory.CreateTemplateService(template2Settings).Returns(service2);
            service1.GetTemplate(Arg.Any<TemplateRequest>()).Returns(Task.FromResult<(string, string)?>(("body", "subject")));
            sut = new ComboTemplateService(comboSettings, templateServiceFactory);
            var result = await sut.GetTemplate(new TemplateRequest());
            Assert.Equal("body", result.Value.Body);
            Assert.Equal("subject", result.Value.Subject);
        }

        [Fact]
        public async Task TestGetTemplateResolvedSecond()
        {
            var template1Settings = new TemplateSettings();
            var template2Settings = new TemplateSettings();
            var comboSettings = new ComboTemplateSettings
            {
                Strategy = "a|b",
                Combo = new Dictionary<string, TemplateSettings>
                {
                    ["a"] = template1Settings,
                    ["b"] = template2Settings
                }
            };

            var service1 = Substitute.For<ITemplateService>();
            var service2 = Substitute.For<ITemplateService>();
            templateServiceFactory.CreateTemplateService(template1Settings).Returns(service1);
            templateServiceFactory.CreateTemplateService(template2Settings).Returns(service2);
            (string Body, string Subject)? nullTuple = null;
            service1.GetTemplate(Arg.Any<TemplateRequest>()).Returns(Task.FromResult<(string, string)?>(null));
            service2.GetTemplate(Arg.Any<TemplateRequest>()).Returns(Task.FromResult<(string, string)?>(("body", "subject")));
            sut = new ComboTemplateService(comboSettings, templateServiceFactory);
            var result = await sut.GetTemplate(new TemplateRequest());
            Assert.Equal("body", result.Value.Body);
            Assert.Equal("subject", result.Value.Subject);
        }

        [Fact]
        public async Task TestGetTemplateNoResolve()
        {
            var template1Settings = new TemplateSettings();
            var template2Settings = new TemplateSettings();
            var comboSettings = new ComboTemplateSettings
            {
                Strategy = "a|b",
                Combo = new Dictionary<string, TemplateSettings>
                {
                    ["a"] = template1Settings,
                    ["b"] = template2Settings
                }
            };
            sut = new ComboTemplateService(comboSettings, templateServiceFactory);
            var service1 = Substitute.For<ITemplateService>();
            var service2 = Substitute.For<ITemplateService>();
            templateServiceFactory.CreateTemplateService(template1Settings).Returns(service1);
            templateServiceFactory.CreateTemplateService(template2Settings).Returns(service2);
            (string Body, string Subject)? nullTuple = null;
            service1.GetTemplate(Arg.Any<TemplateRequest>()).Returns(Task.FromResult<(string, string)?>(null));
            service2.GetTemplate(Arg.Any<TemplateRequest>()).Returns(Task.FromResult<(string, string)?>(null));
            sut = new ComboTemplateService(comboSettings, templateServiceFactory);
            await Assert.ThrowsAsync<Exception>(async () => await sut.GetTemplate(new TemplateRequest()));
        }
    }
}
