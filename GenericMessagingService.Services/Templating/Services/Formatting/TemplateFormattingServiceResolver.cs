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
        ITemplateFormattingService Resolve(TemplateFormattingSettings settings);
    }

    public class TemplateFormattingServiceResolver : ITemplateFormattingServiceResolver
    {
        private readonly IRazorEngine razorEngine;

        public TemplateFormattingServiceResolver(
            IRazorEngine razorEngine)
        {
            this.razorEngine = razorEngine;
        }

        public ITemplateFormattingService Resolve(TemplateFormattingSettings settings)
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
