using GenericMessagingService.Services.Templating.Services.Formatting;
using GenericMessagingService.Services.Templating.Services.Location;
using GenericMessagingService.Types.Config;
using GenericMessagingService.Types.Template;
using System.Threading.Tasks;

namespace GenericMessagingService.Services.Templating.Services
{
    public interface ITemplateService
    {
        Task<(string Body, string? Subject)?> GetTemplate(TemplateRequest request);
    }

    internal class TemplateService : ITemplateService
    {
        private readonly TemplateSettings templateSettings;
        private readonly ITemplateFormattingService templateFormattingService;
        private readonly ITemplateLocationService templateLocationService;

        public TemplateService(
            TemplateSettings templateSettings,
            ITemplateFormattingServiceResolver templateFormattingServiceResolver,
            ITemplateLocationServiceResolver templateLocationServiceResolver)
        {
            this.templateSettings = templateSettings;
            this.templateFormattingService = templateFormattingServiceResolver.Resolve();
            this.templateLocationService = templateLocationServiceResolver.Resolve();
        }

        public async Task<(string Body, string? Subject)?> GetTemplate(TemplateRequest request)
        {
            var (template, subject) = await templateLocationService.LocateTemplateAsync(request.TemplateName);
            if(template == null)
            {
                return null;
            }
            var body = await templateFormattingService.FormatTemplate(template, request.Data);
            return (body, subject);
        }
    }
}
