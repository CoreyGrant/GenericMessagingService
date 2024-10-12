using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Email;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating
{
    internal class RazorTemplateService : ITemplateService
    {
        private readonly RazorTemplateSettings settings;
        private readonly IRazorEngine engine;
        private Dictionary<string, Regex> _regexCache;
        private Dictionary<string, IRazorEngineCompiledTemplate> _compiledTemplates;

        public RazorTemplateService(RazorTemplateSettings settings, IRazorEngine engine)
        {
            this.settings = settings;
            _regexCache = this.settings.Regex.ToDictionary(x => x.Key, x => new Regex(x.Key));
            this.engine = engine;
            // build compiled templates
            _compiledTemplates = LoadCompiledTemplates();
        }

        public async Task<TemplateResponse?> GetTemplate(TemplateRequest request)
        {
            if (TryMatchName(request.TemplateName, out var location))
            {
                var template = _compiledTemplates.ContainsKey(location) ? _compiledTemplates[location] : null;
                if (template != null)
                {
                    var completedTemplate = await template.RunAsync(request.Data);
                    return new TemplateResponse { Body = completedTemplate };
                }
                return null;
            }
            else
            {
                return null;
            }
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
                var regex = _regexCache[k];
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

        private Dictionary<string, IRazorEngineCompiledTemplate> LoadCompiledTemplates()
        {
            var output = new Dictionary<string, IRazorEngineCompiledTemplate>();

            var files = WalkDirectory(settings.BaseFolder);

            foreach (var file in files)
            {
                var filePathAfterBase = file.Split(settings.BaseFolder)[0];
                if (filePathAfterBase.StartsWith("\\"))
                {
                    filePathAfterBase = filePathAfterBase.Substring(1);
                }
                output[filePathAfterBase] = engine.Compile(File.ReadAllText(file));
            }

            return output;
        }

        private IList<string> WalkDirectory(string directory)
        {
            var output = new List<string>();

            var files = Directory.GetFiles(directory).Where(x => x.EndsWith(".razor"));
            var directories = Directory.GetDirectories(directory);

            output.AddRange(files);

            foreach (var dir in directories)
            {
                output.AddRange(WalkDirectory(dir));
            }

            return output;
        }
    }
}
