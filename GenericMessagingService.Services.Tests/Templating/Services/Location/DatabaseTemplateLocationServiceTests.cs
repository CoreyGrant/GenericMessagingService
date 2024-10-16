using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Config;
using NSubstitute;
using Org.BouncyCastle.Crypto.Prng;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.Services.Tests.Templating.Services.Location
{
    public class DatabaseTemplateLocationServiceTests
    {
        private DatabaseTemplateLocationService sut;
        public DatabaseTemplateLocationServiceTests() 
        {
            
        }

        [Fact]
        public async Task TestLocateTemplate()
        {
            var templateName = "templateName";
            var databaseStrategyResolver = Substitute.For<IDatabaseStrategyResolver>();
            var sqlServerService = Substitute.For<IDatabaseService>();
            var databaseTemplateLocationSettings = new DatabaseTemplateLocationSettings
            {
                ConnectionString = "connectionString",
                LookupColumn = "id",
                Table = "templates",
                SubjectColumn = "subject",
                TemplateColumn = "body"
            };
            var sqlServerOutput = ("templateReturn", "subjectReturn");
            databaseStrategyResolver.Resolve(null).Returns(sqlServerService);
            sqlServerService.GetTemplate(templateName).Returns(sqlServerOutput);
            sut = new DatabaseTemplateLocationService(databaseTemplateLocationSettings, databaseStrategyResolver);

            var result = await sut.LocateTemplateAsync(templateName);
            Assert.NotNull(result.Item1);
            Assert.NotNull(result.Item2);
            Assert.Equal("templateReturn", result.Item1);
            Assert.Equal("subjectReturn", result.Item2);
        }
    }
}
