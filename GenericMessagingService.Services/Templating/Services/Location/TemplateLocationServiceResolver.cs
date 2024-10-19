using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    public interface ITemplateLocationServiceResolver
    {
        ITemplateLocationService Resolve(TemplateLocationSettings settings);
    }

    public class TemplateLocationServiceResolver : ITemplateLocationServiceResolver
    {
        private readonly IDatabaseStrategyResolver databaseStrategyResolver;
        private readonly IFileManager fileManager;

        public TemplateLocationServiceResolver(
            IDatabaseStrategyResolver databaseStrategyResolver,
            IFileManager fileManager)
        {
            this.databaseStrategyResolver = databaseStrategyResolver;
            this.fileManager = fileManager;
        }

        public ITemplateLocationService Resolve(TemplateLocationSettings settings)
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
                        services.Add(new FolderTemplateLocationService(folder, fileManager));
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
                    return new FolderTemplateLocationService(folder, fileManager);
                }
            }
            
            throw new Exception("No template location option specified");
        }
    }

    public class ComboTemplateLocationService : ITemplateLocationService
    {
        private readonly IEnumerable<ITemplateLocationService> services;

        public ComboTemplateLocationService(IEnumerable<ITemplateLocationService> services)
        {
            this.services = services;
        }

        public async Task<(string, string)> LocateTemplateAsync(string templateName)
        {
            foreach (var service in services)
            {
                var (body, subject) = await service.LocateTemplateAsync(templateName);
                if (body != null)
                {
                    return (body, subject);
                }
            }
            return (null, null);
        }
    }


}
