﻿using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using Microsoft.CodeAnalysis.FlowAnalysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twilio.Annotations;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    public class FolderTemplateLocationService : ITemplateLocationService
    {
        private readonly string baseFolder;
        private readonly FolderTemplateLocationSettings settings;
        private readonly IFileManager fileManager;
        private Dictionary<string, Regex> regexCache;

        public FolderTemplateLocationService(
            FolderTemplateLocationSettings settings,
            IFileManager fileManager) 
        {
            var bf = settings.BaseFolder;
            this.baseFolder = bf;
            this.settings = settings;
            this.fileManager = fileManager;
            regexCache = this.settings.Regex?.ToDictionary(x => x.Key, x => new Regex(x.Key, RegexOptions.Compiled)) ?? new Dictionary<string, Regex>();
        }

        public async Task<List<string>> GetTemplateNames()
        {
            // Walk path of baseFolder, get the filenames relative to baseFolder
            // find all templates which match the fixed and regex
            // figure out the template names which are assigned for them
            var directoryFiles = fileManager.WalkDirectory(baseFolder);
            var allFiles = directoryFiles.Select(x =>
            {
                var name = string.Join(baseFolder, x.Split(baseFolder).Skip(1));
                if (name.StartsWith(fileManager.PathSeperator)) { name = name.Substring(1); }
                return name;
            });
            var templateNames = new HashSet<string>();
            foreach (var (k, v) in settings.Fixed ?? new Dictionary<string, string>())
            {
                var matchedFile = allFiles.Any(f => f == v);
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
                foreach (var file in allFiles)
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

        public async Task<(string?, string?)> LocateTemplateAsync(string templateName)
        {
            if (TryMatchName(templateName, out var path)) 
            {
                var fileName = Path.Join(baseFolder, path);
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
