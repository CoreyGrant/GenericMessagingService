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
    public class RazorTemplateFormattingService : CompiledTemplateFormattingService<IRazorEngineCompiledTemplate>
    {
        private readonly IRazorEngine engine;

        public RazorTemplateFormattingService(
            TemplateSettings templateSettings,
            RazorTemplateFormattingSettings settings,
            IRazorEngine engine,
            ICacheManager cacheManager,
            IHashService hashService) : base(templateSettings, hashService, cacheManager, "razor-template")
        {
            this.engine = engine;
        }

        protected override async Task<IRazorEngineCompiledTemplate> CompileTemplate(string template)
        {
            return await engine.CompileAsync(template);
        }

        protected override async Task<string> Format(IRazorEngineCompiledTemplate template, IDictionary<string, string> data)
        {
            return await template.RunAsync(data);
        }
    }
}
