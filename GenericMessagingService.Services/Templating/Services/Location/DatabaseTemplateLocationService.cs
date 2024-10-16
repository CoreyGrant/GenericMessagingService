using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Types.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services.Location
{
    internal class DatabaseTemplateLocationService : ITemplateLocationService
    {
        private readonly DatabaseTemplateLocationSettings settings;
        private readonly IDatabaseService databaseService;

        public DatabaseTemplateLocationService(
            DatabaseTemplateLocationSettings settings,
            IDatabaseStrategyResolver databaseStrategyResolver) 
        {
            this.settings = settings;
            this.databaseService = databaseStrategyResolver.Resolve(settings.Type);
        }

        public async Task<(string?, string?)> LocateTemplateAsync(string templateName)
        {
            var result = await databaseService.GetTemplate(templateName);
            return result.HasValue
                ? result.Value
                : (null, null);
        }
    }
}
