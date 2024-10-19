using GenericMessagingService.Services.Cache;
using GenericMessagingService.Types.Config;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Formatting
{
    public class RazorTemplateFormattingService : ITemplateFormattingService
    {
        private readonly TemplateSettings templateSettings;
        private readonly RazorTemplateFormattingSettings settings;
        private readonly IRazorEngine engine;
        private readonly ICacheManager cacheManager;
        private readonly IHashService hashService;

        public RazorTemplateFormattingService(
            TemplateSettings templateSettings,
            RazorTemplateFormattingSettings settings,
            IRazorEngine engine,
            ICacheManager cacheManager,
            IHashService hashService)
        {
            this.templateSettings = templateSettings;
            this.settings = settings;
            this.engine = engine;
            this.cacheManager = cacheManager;
            this.hashService = hashService;
        }

        public async Task<string> FormatTemplate(string template, IDictionary<string, string> data)
        {
            var compiledTemplate = await GetCompiledTemplate(template);
            return await compiledTemplate.RunAsync(data);
        }

        private async Task<IRazorEngineCompiledTemplate> GetCompiledTemplate(string template)
        {
            if (templateSettings.Cache)
            {
                var cacheKey = hashService.GetHash(template);
                var cached = await cacheManager.Get<IRazorEngineCompiledTemplate>("razor-template", cacheKey);
                if(cached != null) { return cached; }
                var compiledTemplate = await engine.CompileAsync(template);

                await cacheManager.Set("razor-template", cacheKey, compiledTemplate);
                return compiledTemplate;
            }
            return await engine.CompileAsync(template);
        }
    }
}
