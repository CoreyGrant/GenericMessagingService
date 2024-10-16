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
        private Dictionary<string, Regex> regexCache;

        public FolderTemplateLocationService(FolderTemplateLocationSettings settings) 
        {
            this.settings = settings;
            regexCache = this.settings.Regex.ToDictionary(x => x.Key, x => new Regex(x.Key));
        }

        public async Task<(string?, string?)> LocateTemplateAsync(string templateName)
        {
            if (TryMatchName(templateName, out var path)) 
            {
                var fileName = Path.Combine(settings.BaseFolder, path);
                return (await File.ReadAllTextAsync(fileName), null);
            }
            return (null, null);
        }

        private bool TryMatchName(string templateName, out string location)
        {
            if (settings.Fixed.ContainsKey(templateName))
            {
                location = settings.Fixed[templateName];
                return true;
            }
            foreach (var (k, v) in settings.Regex)
            {
                var regex = regexCache[k];
                var regexResult = regex.Match(templateName);
                if (regexResult.Success)
                {
                    var matches = regexResult.Captures.Select(x => x.Value);
                    location = string.Format(v, matches);
                    return true;
                }
            }
            location = null;
            return false;
        }
    }
}
