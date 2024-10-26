using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services
{
    public class ComboTemplateService : ITemplateService
    {
        private readonly ComboTemplateSettings settings;
        private readonly List<ITemplateService> templateServices;

        public ComboTemplateService(
            ComboTemplateSettings settings,
            ITemplateServiceFactory templateServiceFactory,
            string? templateStrategy)
        {
            this.settings = settings;
            this.templateServices = new List<ITemplateService>();
            var strategy = templateStrategy ?? settings.Strategy;
            var strategyParts = strategy.Split("|");
            foreach(var strategyPart in strategyParts)
            {
                var config = settings.Combo[strategyPart];
                this.templateServices.Add(templateServiceFactory.CreateTemplateService(config));
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

        public async Task<List<string>> GetTemplateNames()
        {
            var allTemplateNames = new List<string>();
            foreach (var templateService in templateServices)
            {
                allTemplateNames.AddRange(await templateService.GetTemplateNames());
            }
            return allTemplateNames;
        }
    }

    public interface IComboTemplateServiceFactory
    {
        ITemplateService CreateComboTemplateService(ComboTemplateSettings settings);
    }

    [InjectScoped(ServiceType.Template)]
    public class ComboTemplateServiceFactory : IComboTemplateServiceFactory
    {
        private readonly ITemplateServiceFactory templateServiceFactory;
        private readonly ITemplateStrategyService templateStrategyService;

        public ComboTemplateServiceFactory(ITemplateServiceFactory templateServiceFactory, ITemplateStrategyService templateStrategyService) 
        {
            this.templateServiceFactory = templateServiceFactory;
            this.templateStrategyService = templateStrategyService;
        }
        public ITemplateService CreateComboTemplateService(ComboTemplateSettings settings)
        {
            return new ComboTemplateService(settings, templateServiceFactory, templateStrategyService.TemplateStrategy);
        }
    }
}
