using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio.Annotations;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    public class FolderTemplateLocationService : FileTemplateLocationService, ITemplateLocationService
    {
        private readonly string folderPath;
        private readonly IFileManager fileManager;

        public FolderTemplateLocationService(
            FolderTemplateLocationSettings settings,
            IFileManager fileManager) : base(settings)
        {
            this.folderPath = settings.FolderPath;
            this.fileManager = fileManager;
        }

        public async Task<List<string>> GetTemplateNames()
        {
            // Walk path of baseFolder, get the filenames relative to baseFolder
            // find all templates which match the fixed and regex
            // figure out the template names which are assigned for them
            var directoryFiles = fileManager.WalkDirectory(folderPath);
            var allFiles = directoryFiles.Select(x =>
            {
                var name = string.Join(folderPath, x.Split(folderPath).Skip(1));
                if (name.StartsWith(fileManager.PathSeperator)) { name = name.Substring(1); }
                return name;
            });
            return GetTemplateNames(allFiles);
        }

        public async Task<(string?, string?)> LocateTemplateAsync(string templateName)
        {
            if (TryMatchName(templateName, out var path)) 
            {
                try
                {
                    var fileName = Path.Join(folderPath, path);
                    if (!fileManager.FileExists(fileName)) { return (null, null); }
                    var result = (await fileManager.GetFileAsync(fileName), (string)null);
                    return result;
                }
                catch 
                {
                    return (null, null);
                }
            }
            return (null, null);
        }
    }
}
