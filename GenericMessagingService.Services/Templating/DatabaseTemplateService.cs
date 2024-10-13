using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating
{
    internal class DatabaseTemplateService : ITemplateService
    {
        private readonly IDatabaseService databaseService;

        public DatabaseTemplateService(
            DatabaseTemplateSettings settings,
            IDatabaseStrategyResolver databaseStrategyResolver)
        {
            this.databaseService = databaseStrategyResolver.Resolve(settings?.Type);
        }

        public async Task<TemplateResponse?> GetTemplate(TemplateRequest request)
        {
            // get the template from the database
            // may also contain the subject
            var result = await this.databaseService.GetTemplate(request.TemplateName);
            if(!result.HasValue) { return null; }
            // need to perform replacements on the database template and subject
            var subject = Replace(result.Value.Subject, request.Data);
            var body = Replace(result.Value.Template, request.Data);
            return new TemplateResponse { Subject = subject, Body = body };
        }

        private string Replace(string str, Dictionary<string, string> replacements)
        {
            foreach (var (k, v) in replacements)
            {
                str = str.Replace("{" + k + "}", v);
            }
            return str;
        }
    }
}
