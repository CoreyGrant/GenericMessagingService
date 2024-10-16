using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services
{
    internal class ComboTemplateService : ITemplateService
    {
        private readonly ComboTemplateSettings settings;
        private readonly List<ITemplateService> templateServices;

        public ComboTemplateService(
            ComboTemplateSettings settings,
            ITemplateFormattingServiceResolver templateFormattingServiceResolver,
            ITemplateLocationServiceResolver templateLocationServiceResolver)
        {
            this.settings = settings;
            this.templateServices = new List<ITemplateService>();
            var strategyParts = settings.Strategy.Split("|");
            foreach(var strategyPart in strategyParts)
            {
                var config = settings.Combo[strategyPart];
                this.templateServices.Add(new TemplateService(
                    config,
                    templateFormattingServiceResolver,
                    templateLocationServiceResolver));
            }
        }

        public async Task<(string Body, string Subject)?> GetTemplate(TemplateRequest request)
        {
            foreach (var templateService in templateServices)
            {
                var tr = await templateService.GetTemplate(request);
                if(tr != null)
                {
                    return tr;
                }
            }
            throw new Exception("Failed to get template from combo config");
        }
    }
}
