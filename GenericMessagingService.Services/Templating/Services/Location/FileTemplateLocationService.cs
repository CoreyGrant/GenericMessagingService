using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    public abstract class FileTemplateLocationService
    {
        protected readonly IFileTemplateLocationSettings settings;
        protected readonly Dictionary<string, Regex> regexCache;

        public FileTemplateLocationService(IFileTemplateLocationSettings settings)
        {
            this.settings = settings;
            regexCache = this.settings.Regex?.ToDictionary(x => x.Key, x => new Regex(x.Key, RegexOptions.Compiled)) ?? new Dictionary<string, Regex>();
        }

        protected bool TryMatchName(string templateName, out string location)
        {
            if (settings.NameAsPath.HasValue && settings.NameAsPath.Value)
            {
                location = templateName;
                return true;
            }
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

        protected List<string> GetTemplateNames(IEnumerable<string> fileNames)
        {
            var templateNames = new HashSet<string>();
            foreach (var (k, v) in settings.Fixed ?? new Dictionary<string, string>())
            {
                var matchedFile = fileNames.Any(f => f == v);
                if (matchedFile) { templateNames.Add(v); }
            }
            // Regex which matches any capture groups in a regex string
            var captureGroupRegex = new Regex("\\([A-Za-z0-9\\-_./\\\\\\[\\]\\s\\{\\}\\^\\$\\+]+\\)");
            foreach (var (regexStr, formatStr) in settings.Regex ?? new Dictionary<string, string>())
            {
                var regex = regexCache[regexStr];
                var regexStringCaptures = captureGroupRegex.Matches(regexStr);
                var regexStringCaptureGroups = regexStringCaptures.Select(x => x.Captures[0]);
                var replacedRegexString = regexStr;
                var i = 0;
                foreach (var capture in regexStringCaptureGroups)
                {
                    replacedRegexString = replacedRegexString.Replace(capture.Value, "{" + i + "}");
                    i++;
                }
                var replacedFormatString = string.Format(formatStr, regexStringCaptureGroups.Select(x => x.Value).ToArray());
                replacedFormatString = replacedFormatString.Replace("$", "").Replace("^", "");
                var newRegex = new Regex(replacedFormatString);
                foreach (var file in fileNames)
                {
                    // we have regex which matches template name, and a format string for the file path
                    // need to go from file path to the template name matched.
                    // replace each capture group regex into the format string, regex the result, match that on file paths, then format over the "regex" string.
                    var newRegexMatch = newRegex.Match(file);
                    if (!newRegexMatch.Success) { continue; }
                    var newCaptures = newRegexMatch.Groups.Cast<Group>().Skip(1).Select(x => x.Value).ToArray();
                    templateNames.Add(string.Format(replacedRegexString, newCaptures));
                }
            }
            return templateNames.ToList();
        }
    }
}
