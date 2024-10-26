using GenericMessagingService.Services.Cache;
using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Services.Utils;
using GenericMessagingService.Types.Attributes;
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

    [InjectScoped(ServiceType.Template)]
    public class TemplateLocationServiceResolver : ITemplateLocationServiceResolver
    {
        private readonly IDatabaseStrategyResolver databaseStrategyResolver;
        private readonly IFileManager fileManager;
        private readonly ITemplateStrategyService templateStrategyService;
        private readonly IAzureBlobStorageManager azureBlobStorageManager;

        public TemplateLocationServiceResolver(
            IDatabaseStrategyResolver databaseStrategyResolver,
            IFileManager fileManager,
            ITemplateStrategyService templateStrategyService,
            IAzureBlobStorageManager azureBlobStorageManager)
        {
            this.databaseStrategyResolver = databaseStrategyResolver;
            this.fileManager = fileManager;
            this.templateStrategyService = templateStrategyService;
            this.azureBlobStorageManager = azureBlobStorageManager;
        }

        public ITemplateLocationService Resolve(TemplateLocationSettings settings)
        {
            var database = settings.Database;
            var folder = settings.Folder;
            var azureBlobStorage = settings.AzureBlobStorage;
            var strategy = settings.Strategy;
            if (!string.IsNullOrEmpty(strategy))
            {
                var strategyParts = strategy.Split("|");
                var services = new List<ITemplateLocationService>(); 
                foreach(var strategyPart in strategyParts)
                {
                    if(strategyPart == "database")
                    {
                        services.Add(new DatabaseTemplateLocationService(database, databaseStrategyResolver));
                    } else if(strategyPart == "folder")
                    {
                        services.Add(new FolderTemplateLocationService(folder, fileManager));
                    } else if(strategyPart == "azureBlobStorage")
                    {
                        services.Add(new AzureBlobStorageTemplateLocationService(azureBlobStorage, azureBlobStorageManager));
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
                } else if(azureBlobStorage != null)
                {
                    return new AzureBlobStorageTemplateLocationService(azureBlobStorage, azureBlobStorageManager);
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

        public async Task<List<string>> GetTemplateNames()
        {
            var allTemplateNames = new List<string>();
            foreach (var service in services)
            {
                allTemplateNames.AddRange(await service.GetTemplateNames());
            }
            return allTemplateNames;
        }
    }


}
