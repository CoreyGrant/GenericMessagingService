using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace GenericMessagingService.Services.Tests.Templating.Services.Location
{
    public class TemplateLocationServiceResolverTests
    {
        private readonly IDatabaseStrategyResolver databaseStrategyResolver;
        private readonly IFileManager fileManager;
        private readonly TemplateLocationServiceResolver sut;

        public TemplateLocationServiceResolverTests() 
        {
            databaseStrategyResolver = Substitute.For<IDatabaseStrategyResolver>();
            fileManager = Substitute.For<IFileManager>();
            sut = new TemplateLocationServiceResolver(databaseStrategyResolver, fileManager);
        }

        [Fact]
        public async Task TestBasicResolveDatabase()
        {
            var config = new TemplateLocationSettings
            {
                Database = new DatabaseTemplateLocationSettings()
            };
            var result = sut.Resolve(config);
            Assert.True(result is DatabaseTemplateLocationService);
        }

        [Fact]
        public async Task TestBasicResolveFolder()
        {
            var config = new TemplateLocationSettings
            {
                Folder = new FolderTemplateLocationSettings()
            };
            var result = sut.Resolve(config);
            Assert.True(result is FolderTemplateLocationService);
        }

        [Fact]
        public async Task TestComboResolve()
        {
            var strategy = "database|folder";
            var config = new TemplateLocationSettings
            {
                Folder = new FolderTemplateLocationSettings(),
                Database = new DatabaseTemplateLocationSettings(),
                Strategy = strategy
            };
            var result = sut.Resolve(config);
            Assert.True(result is ComboTemplateLocationService);
        }
    }
}
