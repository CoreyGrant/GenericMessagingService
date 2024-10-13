using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating
{
    internal interface ITemplateStrategyResolver
    {
        ITemplateService Resolve(string templateStrategy);
    }

    internal class TemplateStrategyResolver : ITemplateStrategyResolver
    {
        private readonly TemplateSettings settings;
        private readonly IRazorEngine razorEngine;
        private readonly IDatabaseStrategyResolver databaseStrategyResolver;

        public TemplateStrategyResolver(
            TemplateSettings settings,
            IRazorEngine razorEngine,
            IDatabaseStrategyResolver databaseStrategyResolver)
        {
            this.settings = settings;
            this.razorEngine = razorEngine;
            this.databaseStrategyResolver = databaseStrategyResolver;
        }

        public ITemplateService Resolve(string templateStrategy)
        {
            var strategyParts = templateStrategy.Split(new char[] { '|' });
            List<ITemplateService> services = strategyParts.Select(x =>
            {
                ITemplateService service;
                if (x == "razor")
                {
                    service = new RazorTemplateService(
                        settings?.RazorTemplates,
                        razorEngine);
                }
                else if (x == "database")
                {
                    service = new DatabaseTemplateService(
                        settings?.DatabaseTemplates,
                        databaseStrategyResolver
                        );
                }
                else
                {
                    throw new Exception($"Template strategy {x} not recognised");
                }
                return service;
            }).ToList();
            return new ComboTemplateService(services);
        }

        private class ComboTemplateService : ITemplateService
        {
            private readonly List<ITemplateService> services;

            public ComboTemplateService(List<ITemplateService> services)
            {
                this.services = services;
            }

            public Task<TemplateResponse?> GetTemplate(TemplateRequest request)
            {
                foreach (var service in services)
                {
                    var result = service.GetTemplate(request);
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
        }
    }
}
