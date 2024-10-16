using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    internal interface ITemplateLocationServiceResolver
    {
        ITemplateLocationService Resolve();
    }

    internal class TemplateLocationServiceResolver : ITemplateLocationServiceResolver
    {
        private readonly TemplateLocationSettings settings;
        private readonly IDatabaseStrategyResolver databaseStrategyResolver;

        public TemplateLocationServiceResolver(
            TemplateLocationSettings settings,
            IDatabaseStrategyResolver databaseStrategyResolver)
        {
            this.settings = settings;
            this.databaseStrategyResolver = databaseStrategyResolver;
        }

        public ITemplateLocationService Resolve()
        {
            var database = settings.Database;
            var folder = settings.Folder;
            if (!string.IsNullOrEmpty(settings.Strategy))
            {
                var strategyParts = settings.Strategy.Split("|");
                var services = new List<ITemplateLocationService>(); 
                foreach(var strategyPart in strategyParts)
                {
                    if(strategyPart == "database")
                    {
                        services.Add(new DatabaseTemplateLocationService(database, databaseStrategyResolver));
                    } else if(strategyPart == "folder")
                    {
                        services.Add(new FolderTemplateLocationService(folder));
                    }
                }
                return new ComboTemplateLocationService(services);
            } else
            {
                if (database != null)
                {
                    return new DatabaseTemplateLocationService(database, databaseStrategyResolver);
                }
                else if (folder != null)
                {
                    return new FolderTemplateLocationService(folder);
                }
            }
            
            throw new Exception("No template location option specified");
        }

        private class ComboTemplateLocationService : ITemplateLocationService
        {
            private readonly IEnumerable<ITemplateLocationService> services;

            public ComboTemplateLocationService(IEnumerable<ITemplateLocationService> services)
            {
                this.services = services;
            }

            public async Task<(string, string)> LocateTemplateAsync(string templateName)
            {
                foreach(var service in services)
                {
                    var (body, subject) = await service.LocateTemplateAsync(templateName);
                    if(body != null)
                    {
                        return (body, subject);
                    }
                }
                return (null, null);
            }
        }
    }

    
}
