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
        private readonly RazorTemplateFormattingSettings settings;
        private readonly IRazorEngine engine;

        public RazorTemplateFormattingService(
            RazorTemplateFormattingSettings settings,
            IRazorEngine engine)
        {
            this.settings = settings;
            this.engine = engine;
        }

        public async Task<string> FormatTemplate(string template, IDictionary<string, string> data)
        {
            var compiledTemplate = await engine.CompileAsync(template);
            return await compiledTemplate.RunAsync(data);
        }
    }
}
