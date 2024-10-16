using GenericMessagingService.Types.Config;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Formatting
{
    internal interface ITemplateFormattingServiceResolver
    {
        ITemplateFormattingService Resolve();
    }

    internal class TemplateFormattingServiceResolver : ITemplateFormattingServiceResolver
    {
        private readonly TemplateFormattingSettings settings;
        private readonly IRazorEngine razorEngine;

        public TemplateFormattingServiceResolver(
            TemplateFormattingSettings settings,
            IRazorEngine razorEngine)
        {
            this.settings = settings;
            this.razorEngine = razorEngine;
        }

        public ITemplateFormattingService Resolve()
        {
            var razor = settings.Razor;
            var basic = settings.Basic;
            if(razor != null)
            {
                return new RazorTemplateFormattingService(razor, razorEngine);
            } else if (basic != null)
            {
                return new BasicTemplateFormattingService();
            }
            throw new Exception("No template formatting option specified");
        }
    }
}
