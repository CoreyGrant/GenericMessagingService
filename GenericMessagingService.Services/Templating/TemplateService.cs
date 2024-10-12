using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Email;
using RazorEngineCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating
{
    public interface ITemplateService
    {
        Task<TemplateResponse?> GetTemplate(TemplateRequest request);
    }

    internal class TemplateService : ITemplateService
    {
        private ITemplateService templateService;

        public TemplateService(
            EmailSettings emailSettings,
            ITemplateStrategyResolver templateStrategyResolver)
        {
            templateService = templateStrategyResolver.Resolve(emailSettings.TemplateStrategy);
        }

        public Task<TemplateResponse?> GetTemplate(TemplateRequest request)
        {
            // determine the type of email template
            return templateService.GetTemplate(request);
        }
    }
}
