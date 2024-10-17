using GenericMessagingService.Types.Config;
using GenericMessagingService.WebApi.Setup;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Sdk;

namespace GenericMessagingService.WebApi.Tests.Setup
{
    public class ConfigValidatorTests
    {
        private readonly ConfigValidator sut;
        private readonly ILogger<ConfigValidator> logger;

        public ConfigValidatorTests()
        {
            logger = Substitute.For<ILogger<ConfigValidator>>();
            sut = new ConfigValidator(logger);
        }

        [Fact]
        public async Task TestNullAppSettingsThrows()
        {
            AppSettings? appSettings = null;
            Assert.Throws<ConfigValidationException>(() => sut.Validate(appSettings));
        }

        [Theory]
        [InlineData("TestNoFeatures")] // Whole
        [InlineData("TestNoEmailSections")] // Email
        [InlineData("TestMultipleEmailSections")]
        [InlineData("TestEmailEmptySendGridApiKey")]
        [InlineData("TestEmailEmptyFolderPath")]
        [InlineData("TestSmsNoTwilio")] // Sms
        [InlineData("TestSmsEmptyTwilioAuthToken")] 
        [InlineData("TestSmsEmptyTwilioAccountSid")]
        [InlineData("TestTemplateNoLocation")] // Template
        [InlineData("TestTemplateNoFormatting")]
        [InlineData("TestTemplateEmptyLocation")]
        public async Task TestFailuresThrow(string jsonPath)
        {
            var settings = ReadJson(jsonPath);
            Assert.Throws<ConfigValidationException>(() => sut.Validate(settings));
        }

        [Theory]
        [InlineData("Email/TestValidFolder")]
        [InlineData("Email/TestValidSendGrid")]
        [InlineData("Sms/TestValidFolder")]
        [InlineData("Sms/TestValidTwilio")]
        [InlineData("Template/TestValidBasic")]
        [InlineData("Template/TestValidRazor")]
        [InlineData("Full/TestFullConfigOne")]
        public async Task TestSuccess(string jsonPath)
        {
            var settings = ReadJson(Path.Combine("Valid", jsonPath));
            sut.Validate(settings);
        }

        private AppSettings ReadJson(string fileName)
        {
            var assemblyPath = AppDomain.CurrentDomain.BaseDirectory;

            var path = Path.Combine(assemblyPath, "Data", fileName + ".json");
            return JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(path))!;
        }
    }
}
