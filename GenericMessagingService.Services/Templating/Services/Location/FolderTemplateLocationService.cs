using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    public class FolderTemplateLocationService : ITemplateLocationService
    {
        private readonly FolderTemplateLocationSettings settings;
        private readonly IFileManager fileManager;
        private Dictionary<string, Regex> regexCache;

        public FolderTemplateLocationService(
            FolderTemplateLocationSettings settings,
            IFileManager fileManager) 
        {
            this.settings = settings;
            this.fileManager = fileManager;
            regexCache = this.settings.Regex?.ToDictionary(x => x.Key, x => new Regex(x.Key, RegexOptions.Compiled)) ?? new Dictionary<string, Regex>();
        }

        public async Task<(string?, string?)> LocateTemplateAsync(string templateName)
        {
            if (TryMatchName(templateName, out var path)) 
            {
                var fileName = Path.Combine(settings.BaseFolder, path);
                var result = (await fileManager.GetFileAsync(fileName), (string)null);
                return result;
            }
            return (null, null);
        }

        private bool TryMatchName(string templateName, out string location)
        {
            if (settings.Fixed?.ContainsKey(templateName) ?? false)
            {
                location = settings.Fixed[templateName];
                return true;
            }
            foreach (var (k, v) in settings.Regex ?? new Dictionary<string, string>())
            {
                var regex = regexCache[k];
                var regexResult = regex.Match(templateName);
                if (regexResult.Success)
                {
                    var matches = regexResult.Groups.Cast<Group>().Skip(1).Select(x => x.Value).ToArray();
                    location = string.Format(v, matches);
                    return true;
                }
            }
            location = null;
            return false;
        }
    }
}
