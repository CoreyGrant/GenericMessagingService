using GenericMessagingService.Services.Cache;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Formatting
{
    public interface ICompiledTemplateFormattingService : ITemplateFormattingService
    {
        Task Precompile(IEnumerable<string> templates);
    }

    public abstract class CompiledTemplateFormattingService<TCompiledTemplate> : ITemplateFormattingService where TCompiledTemplate : class
    {
        private readonly TemplateSettings settings;
        private readonly IHashService hashService;
        private readonly ICacheManager cacheManager;
        private readonly string cacheKey;

        public CompiledTemplateFormattingService(
            TemplateSettings settings,
            IHashService hashService,
            ICacheManager cacheManager,
            string cacheKey)
        {
            this.settings = settings;
            this.hashService = hashService;
            this.cacheManager = cacheManager;
            this.cacheKey = cacheKey;
        }

        public async Task<string> FormatTemplate(string template, IDictionary<string, string> data)
        {
            
            return this.Format(template, data);
        }

        protected abstract Task<string> Format(TCompiledTemplate template, IDictionary<string, string> data);
        protected abstract Task<TCompiledTemplate> CompileTemplate(string template);

        private async Task<TCompiledTemplate> GetCompiled(string template)
        {
            if (settings.Cache)
            {
                var cacheItemName = hashService.GetHash(template);
                var cached = await cacheManager.Get<TCompiledTemplate>(cacheKey, cacheItemName);
                if (cached != null) { return cached; }
                var compiledTemplate = await CompileTemplate(template);
                await cacheManager.Set(cacheKey, cacheItemName, compiledTemplate);
                return compiledTemplate;
            }
            return await CompileTemplate(template);
        }

    }
}
