using GenericMessagingService.Services.Templating.Database;
using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Attributes;
using GenericMessagingService.Types.Config;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services
{
    public interface ITemplateRunnerService
    {
        Task<(string Body, string? Subject)?> RunTemplate(string templateName, IDictionary<string, string> data);
        Task<List<string>> GetTemplateNames();
    }

    [InjectTransient(ServiceType.Template)]
    public class TemplateRunnerService : ITemplateRunnerService
    {
        private ITemplateService templateService;
        public TemplateRunnerService(
            AppSettings settings,
            ITemplateServiceFactory templateServiceFactory,
            IComboTemplateServiceFactory comboTemplateServiceFactory)
        {
            if (settings.Template != null)
            {
                templateService = templateServiceFactory.CreateTemplateService(settings.Template);
            } else if (settings.ComboTemplate != null)
            {
                templateService = comboTemplateServiceFactory.CreateComboTemplateService(settings.ComboTemplate);
            }
        }

        public async Task<(string Body, string? Subject)?> RunTemplate(string templateName, IDictionary<string, string> data)
        {
            return await templateService.GetTemplate(new Types.Template.TemplateRequest { Data = data, TemplateName = templateName });
        }

        public async Task<List<string>> GetTemplateNames()
        {
            return await templateService.GetTemplateNames();
        }
    }
}
