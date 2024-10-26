using GenericMessagingService.Services.Cache;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Formatting
{
    public interface ITemplateFormattingServiceResolver
    {
        ITemplateFormattingService Resolve(TemplateSettings settings);
    }

    [InjectScoped(ServiceType.Template)]
    public class TemplateFormattingServiceResolver : ITemplateFormattingServiceResolver
    {
        private readonly IRazorEngine razorEngine;
        private readonly IHashService hashService;
        private readonly ICacheManager cacheManager;

        public TemplateFormattingServiceResolver(
            IRazorEngine razorEngine,
            IHashService hashService,
            ICacheManager cacheManager)
        {
            this.razorEngine = razorEngine;
            this.hashService = hashService;
            this.cacheManager = cacheManager;
        }

        public ITemplateFormattingService Resolve(TemplateSettings tSettings)
        {
            var settings = tSettings.Formatting;
            var razor = settings.Razor;
            var basic = settings.Basic;
            if(razor != null)
            {
                return new RazorTemplateFormattingService(tSettings, razor, razorEngine, cacheManager, hashService);
            } else if (basic != null)
            {
                return new BasicTemplateFormattingService();
            }
            throw new Exception("No template formatting option specified");
        }
    }
}
