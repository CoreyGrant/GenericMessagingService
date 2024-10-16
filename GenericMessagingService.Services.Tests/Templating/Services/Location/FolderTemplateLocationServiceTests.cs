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
    public class FolderTemplateLocationServiceTests
    {
        private FolderTemplateLocationService sut;

        public FolderTemplateLocationServiceTests() 
        {

        }

        [Fact]
        public async Task TestLocationTemplateBasic()
        {
            var templateName = "templateName";
            var baseFolder = "folder\\";
            var config = new FolderTemplateLocationSettings
            {
                BaseFolder = baseFolder,
                Fixed = new Dictionary<string, string>
                {
                    [templateName] = "folder2\\folder3\\file"
                }
            };
            

            var template = "template";
            var fileManager = Substitute.For<IFileManager>();
            var expectedPath = Path.Combine("folder", "folder2", "folder3", "file.razor");
            fileManager.GetFileAsync(expectedPath)
                .Returns(template);
            sut = new FolderTemplateLocationService(config, fileManager);
            var result = await sut.LocateTemplateAsync(templateName);
            Assert.Equal(template, result.Item1);
        }

        [Fact]
        public async Task TestLocationTemplateRegex()
        {
            var templateName = "templateName-regexfile";
            var baseFolder = "folder\\";
            var config = new FolderTemplateLocationSettings
            {
                BaseFolder = baseFolder,
                Regex = new Dictionary<string, string>
                {
                    ["templateName-([a-zA-Z0-9]+)$"] = "folder2\\folder3\\{0}"
                }
            };
            
            var template = "template";
            var fileManager = Substitute.For<IFileManager>();
            var expectedPath = Path.Combine("folder", "folder2", "folder3", "regexfile.razor");
            fileManager.GetFileAsync(expectedPath)
                .Returns(template);
            sut = new FolderTemplateLocationService(config, fileManager);
            var result = await sut.LocateTemplateAsync(templateName);
            Assert.Equal(template, result.Item1);
        }
    }
}
