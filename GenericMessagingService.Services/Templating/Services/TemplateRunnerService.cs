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

    [InjectScoped(ServiceType.Template)]
    public class TemplateRunnerService : ITemplateRunnerService
    {
        private readonly AppSettings settings;
        private readonly ITemplateServiceFactory templateServiceFactory;
        private readonly IComboTemplateServiceFactory comboTemplateServiceFactory;
        private ITemplateService templateService;
        private ITemplateService TemplateService 
        {
            get
            {
                if(templateService != null) { return templateService; }
                if (settings.Template != null)
                {
                    return templateService = templateServiceFactory.CreateTemplateService(settings.Template);
                }
                else 
                {
                    return templateService = comboTemplateServiceFactory.CreateComboTemplateService(settings.ComboTemplate);
                }
            } 
        }
        public TemplateRunnerService(
            AppSettings settings,
            ITemplateServiceFactory templateServiceFactory,
            IComboTemplateServiceFactory comboTemplateServiceFactory)
        {
            this.settings = settings;
            this.templateServiceFactory = templateServiceFactory;
            this.comboTemplateServiceFactory = comboTemplateServiceFactory;
        }

        public async Task<(string Body, string? Subject)?> RunTemplate(string templateName, IDictionary<string, string> data)
        {
            return await TemplateService.GetTemplate(new Types.Template.TemplateRequest { Data = data, TemplateName = templateName });
        }

        public async Task<List<string>> GetTemplateNames()
        {
            return await TemplateService.GetTemplateNames();
        }
    }
}
