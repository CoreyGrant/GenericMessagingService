using GenericMessagingService.Services.Templating.Services;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
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
    public class TemplateRunnerServiceTests
    {
        private readonly ITemplateServiceFactory templateServiceFactory;
        private readonly IComboTemplateServiceFactory comboTemplateServiceFactory;
        private TemplateRunnerService sut;

        public TemplateRunnerServiceTests() 
        {
            templateServiceFactory = Substitute.For<ITemplateServiceFactory>();
            comboTemplateServiceFactory = Substitute.For<IComboTemplateServiceFactory>();
        }

        [Fact]
        public async Task TestRunTemplateSingle()
        {
            var templateName = "templateName";
            var data = new Dictionary<string, string>();
            var body = "body";
            var subject = "subject";
            var templateSettings = new TemplateSettings
            {

            };
            var appSettings = new AppSettings { Template = templateSettings };
            var templateService = Substitute.For<ITemplateService>();
            templateServiceFactory.CreateTemplateService(templateSettings)
                .Returns(templateService);
            templateService.GetTemplate(Arg.Is((TemplateRequest tr) => tr.TemplateName == templateName && tr.Data == data))
                .Returns(Task.FromResult<(string, string?)?>((body, subject)));
            sut = new TemplateRunnerService(appSettings, templateServiceFactory, comboTemplateServiceFactory);
            var result = await sut.RunTemplate(templateName, data);
            Assert.Equal(body, result.Value.Body);
            Assert.Equal(subject, result.Value.Subject);
        }

        [Fact]
        public async Task TestRunTemplateCombo()
        {
            var templateName = "templateName";
            var data = new Dictionary<string, string>();
            var body = "body";
            var subject = "subject";
            var comboTemplateSettings = new ComboTemplateSettings
            {

            };
            var appSettings = new AppSettings { ComboTemplate = comboTemplateSettings };
            var templateService = Substitute.For<ITemplateService>();
            comboTemplateServiceFactory.CreateComboTemplateService(comboTemplateSettings)
                .Returns(templateService);
            templateService.GetTemplate(Arg.Is((TemplateRequest tr) => tr.TemplateName == templateName && tr.Data == data))
                .Returns(Task.FromResult<(string, string?)?>((body, subject)));
            sut = new TemplateRunnerService(appSettings, templateServiceFactory, comboTemplateServiceFactory);
            var result = await sut.RunTemplate(templateName, data);
            Assert.Equal(body, result.Value.Body);
            Assert.Equal(subject, result.Value.Subject);
        }
    }
}
