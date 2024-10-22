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
                    [templateName] = "folder2\\folder3\\file.cshtml"
                }
            };
            

            var template = "template";
            var fileManager = Substitute.For<IFileManager>();
            var expectedPath = Path.Combine("folder", "folder2", "folder3", "file.cshtml");
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
                    ["templateName-([a-zA-Z0-9]+)$"] = "folder2\\folder3\\{0}.cshtml"
                }
            };
            
            var template = "template";
            var fileManager = Substitute.For<IFileManager>();
            var expectedPath = Path.Join("folder", "folder2", "folder3", "regexfile.cshtml");
            fileManager.GetFileAsync(expectedPath)
                .Returns(template);
            sut = new FolderTemplateLocationService(config, fileManager);
            var result = await sut.LocateTemplateAsync(templateName);
            Assert.Equal(template, result.Item1);
        }

        [Fact]
        public async Task TestGetTemplateNames()
        {
            var baseFolder = "/";
            var directoryNames = new[]
            {
                "/Folder-A/File-1",
                "/Folder-A/File-2",
                "/Folder-A/File-3",
                "/Folder-A/File-4",
                "/Folder-A/File-5",
                "/Folder-B/File-6",
                "/Folder-B/File-7",
                "/Folder-B/File-8",
                "/Folder-B/File-9",
                // shouldnt be found
                "/Folder-C/File-Q",
                "/Folder-C/File-T",
            };
            var expectedTemplateNames = new[]
            {
                "Folder-A-File-1",
                "Folder-A-File-2",
                "Folder-A-File-3",
                "Folder-A-File-4",
                "Folder-A-File-5",
                "Folder-B-File-6",
                "Folder-B-File-7",
                "Folder-B-File-8",
                "Folder-B-File-9",
            };
            var settings = new FolderTemplateLocationSettings
            {
                Regex = new Dictionary<string, string>
                {
                    ["Folder-([AB])-File-([1-9])"] = "/Folder-{0}/File-{1}"
                },
                BaseFolder = baseFolder
            };
            var fileManager = Substitute.For<IFileManager>();
            fileManager.WalkDirectory(baseFolder).Returns(directoryNames);
            sut = new FolderTemplateLocationService(settings, fileManager);
            var templateNames = await sut.GetTemplateNames();
            Assert.True(expectedTemplateNames.All(templateNames.Contains));
        }
    }
}
